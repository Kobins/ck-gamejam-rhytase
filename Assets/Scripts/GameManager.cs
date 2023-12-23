using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public enum GameState {
        WAITING,
        START_COUNT,
        PLAYING,
        END,
    }

    public BackgroundHandler m_background;
    public RectTransform m_startPanel;
    public RectTransform m_endPanel;
    public Text m_endPanelText;
    public RectTransform m_countPanel;
    public Text m_countText;

    public RectTransform m_pausePanel;
    public Button m_pauseResumeButton;
    public Button m_pauseOffsetButton;
    public Button m_pauseGotoIntroButton;
    public Slider m_pauseVolumeSlider;
    private bool m_paused = false;
    public bool m_autoPlay = false;
    
    public TouchPredictManager m_predictManager;

    public Camera m_camera;
    public float m_cameraMoveSpeed = 10f;
    public GameState m_state;
    public Player m_playerObject;
    public Stage m_stage;
    public float m_playerMusicOffset = 0f;

    public AudioSource m_audioSource;
    public int m_currentTileIndex;
    public Tile m_startTile;

    
    public float MusicTime => m_audioSource.time - m_playerMusicOffset;

    private Vector3 m_cameraOffset;
    private int m_tooFastCount = 0;
    private bool m_cleared = false;
    private void Awake() {
        m_stage.m_gameManager = this;
        var volume = PlayerPrefs.GetFloat("volume", 0.5f);
        m_audioSource.volume = volume;
        m_playerMusicOffset = PlayerPrefs.GetFloat("offset", 0f);
        m_paused = false;
        m_pauseResumeButton.onClick.AddListener(Pause);
        m_pauseOffsetButton.onClick.AddListener(() => SceneManager.LoadScene("Sync"));
        m_pauseGotoIntroButton.onClick.AddListener(() => SceneManager.LoadScene("Intro"));
        m_pauseVolumeSlider.value = volume;
        m_pauseVolumeSlider.onValueChanged.AddListener((v) => {
            m_audioSource.volume = v;
            PlayerPrefs.SetFloat("volume", v);
        });
        m_pausePanel.gameObject.SetActive(false);
    }
    private void Start() {
        m_startPanel.gameObject.SetActive(true);
        m_endPanel.gameObject.SetActive(false);
        m_cameraOffset = m_camera.transform.position - m_playerObject.transform.position;
        m_state = GameState.WAITING;
        m_audioSource.Stop();
        if (m_startTile) {
            int index = m_stage.m_tiles.IndexOf(m_startTile);
            if (index > 0) m_currentTileIndex = index;
        }
        var primaryTile = m_stage.m_tiles[m_currentTileIndex];
        m_playerObject.transform.position = primaryTile.transform.position;
        
    }

    // Update is called once per frame
    void Update() {
        var escapeDown = Input.GetKeyDown(KeyCode.Escape); 
        if (escapeDown) {
            Pause();
        }
        switch (m_state) {
            case GameState.WAITING:
                if (!escapeDown && Input.anyKeyDown) {
                    InitStage();
                }
                break;
            case GameState.START_COUNT:
                UpdatePlayCount();
                break;
            case GameState.PLAYING:
                UpdatePlaying();
                break;
            case GameState.END:
                UpdateEnding();
                break;
        }

        var cameraTransform = m_camera.transform;
        var to = m_playerObject.transform.position + m_cameraOffset;
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, to, Time.smoothDeltaTime * m_cameraMoveSpeed);
    }

    void Pause() {
        m_paused = !m_paused;
        if (m_paused) {
            Time.timeScale = 0f;
            m_audioSource.Pause();
            m_pausePanel.gameObject.SetActive(true);
        }
        else {
            Time.timeScale = 1f;
            m_audioSource.UnPause();
            m_pausePanel.gameObject.SetActive(false);
        }
    }

    void InitStage() {
        m_startPanel.gameObject.SetActive(false);
        m_state = GameState.START_COUNT;
        m_camera.transform.position = m_playerObject.transform.position + m_cameraOffset;
        
        m_audioSource.clip = m_stage.m_music;
        if (m_currentTileIndex > 0) {
            m_audioSource.time = m_stage.m_tiles[m_currentTileIndex-1].m_expectedTime;
        }

        m_singlePlayCount = 1 / (m_stage.m_startCountBPM * m_stage.m_startCountBPMMultiply / 60);
        m_leftPlayCount = m_singlePlayCount * 3;
        m_countText.text = "3";
        m_countPanel.gameObject.SetActive(true);
    }

    private float m_singlePlayCount = 0f;
    private float m_leftPlayCount = 0f;
    void UpdatePlayCount() {
        if (m_leftPlayCount <= 0) {
            m_audioSource.Play();
            m_state = GameState.PLAYING;
            m_countPanel.gameObject.SetActive(false);
            return;
        }

        var count = Mathf.FloorToInt(m_leftPlayCount / m_singlePlayCount) + 1;
        switch (count) {
            case 3:
                m_countText.text = "3";
                break;
            case 2:
                m_countText.text = "2";
                break;
            case 1:
                m_countText.text = "1";
                break;
        }
        
        m_leftPlayCount -= Time.deltaTime;
    }

    void UpdatePlaying() {
        var time = MusicTime;

        var currentTile = m_stage.m_tiles[m_currentTileIndex];
        var nextTile = m_stage.m_tiles[m_currentTileIndex + 1];
        // 양수 -> 느림 / 음수 -> 빠름
        var overed = time - nextTile.m_expectedTime;
        var timeRange = nextTile.m_expectedTime - currentTile.m_expectedTime;
        var validRange = (timeRange);
        
        // 너무 느림!
        if (overed > validRange) {
            EndGame(false);
            return;
        }
        // 아무튼 입력을 했는데 ...
        if (Input.anyKeyDown || m_autoPlay && overed >= 0) {

            var prediction = Mathf.Abs(overed);
            TouchPredictType predictType;
            if (prediction <= timeRange * 0.1) {
                predictType = TouchPredictType.Perfect;
            }
            else if (overed > 0 && overed <= timeRange * 0.3) {
                predictType = TouchPredictType.LittleSlow;
            }
            else if (overed < 0 && overed >= -timeRange * 0.3) {
                predictType = TouchPredictType.LittleFast;
            }
            else if (overed > 0 && overed <= timeRange * 0.5) {
                predictType = TouchPredictType.Slow;
            }
            else if (overed < 0 && overed >= -timeRange * 0.5) {
                predictType = TouchPredictType.Fast;
            }
            else if (overed > 0) {
                predictType = TouchPredictType.TooSlow;
            }
            else if (overed < 0) {
                predictType = TouchPredictType.TooFast;
                m_tooFastCount++;
            }
            else {
                predictType = TouchPredictType.None;
            }

            if (m_tooFastCount >= 3) {
                EndGame(false);
                return;
            }
            m_predictManager.Create(predictType, m_playerObject.transform.position + Vector3.up * 3);
            
            // 범위 내 입력이면 -> 다음 타일
            if (predictType != TouchPredictType.TooFast && Mathf.Abs(overed) <= validRange) {
                if (m_tooFastCount > 0) {
                    m_tooFastCount--;
                }
                NextTile(predictType);
            }
        }
        UpdatePlayerPosition();
    }

    void UpdatePlayerPosition() {
        var stage = m_stage;
        var currentTile = stage.m_tiles[m_currentTileIndex];
        if (m_currentTileIndex >= stage.m_tiles.Count - 1) {
            m_playerObject.transform.position = currentTile.transform.position;
            return;
        }
        var time = MusicTime;
        var nextTile = stage.m_tiles[m_currentTileIndex + 1];
        var percentage = Mathf.InverseLerp(currentTile.m_expectedTime, nextTile.m_expectedTime, time);

        currentTile.UpdatePlayer(m_playerObject, nextTile, percentage);

    }

    void NextTile(TouchPredictType type) {
        var stage = m_stage;
        var prevIndex = m_currentTileIndex;
        var nextIndex = m_currentTileIndex + 1;
        var prevTile = stage.m_tiles[prevIndex];
        var nextTile = stage.m_tiles[nextIndex];
        nextTile.Touch(m_playerObject, prevTile, type);
        m_currentTileIndex = nextIndex;
        if (m_currentTileIndex + 1 >= stage.m_tiles.Count) {
            EndGame(true);
            return;
        }
        prevTile.TouchNextTile(m_playerObject, nextTile);
    }

    void EndGame(bool cleared) {
        m_cleared = cleared;
        m_state = GameState.END;
        if (cleared) {
            m_endPanelText.text = "클리어!";
            m_playerObject.SetAnimation(PlayerAnimationType.CLEAR);
        }
        else {
            m_endPanelText.text = $"실패 ... {Mathf.FloorToInt((MusicTime / m_stage.m_endTime)*100)}%";
            m_playerObject.SetAnimation(PlayerAnimationType.FAIL);
        }
        m_endPanel.gameObject.SetActive(true);
        if (m_currentTileIndex > 0) {
            var currentTile = m_stage.m_tiles[m_currentTileIndex - 1];
            var nextTile = m_stage.m_tiles[m_currentTileIndex];
            m_endAnimationPeriod = nextTile.m_expectedTime - currentTile.m_expectedTime;
            m_endAnimationLeftTime = m_endAnimationPeriod * 
                (cleared ? m_playerObject.m_clearAnimation : m_playerObject.m_failAnimation).m_sprites.Count;
            m_endAnimationIndex = 0;
        }
        UpdatePlayerPosition();
    }

    private float m_endAnimationPeriod = 0f;
    private float m_endAnimationLeftTime = 0f;
    private int m_endAnimationIndex = 0;
    void UpdateEnding() {
        if (m_endAnimationLeftTime >= 0f) {
            var count = (m_cleared ? m_playerObject.m_clearAnimation : m_playerObject.m_failAnimation).m_sprites.Count;
            var index = count - Mathf.FloorToInt(m_endAnimationLeftTime / m_endAnimationPeriod);
            if (m_endAnimationIndex != index) {
                m_endAnimationIndex = index;
                m_playerObject.NextSprite();
            }

            m_endAnimationLeftTime -= Time.deltaTime;
            return;
        }

        if (Input.anyKeyDown) {
            Restart();
        }
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Exit() {
        SceneManager.LoadScene("Intro");
    }
}
