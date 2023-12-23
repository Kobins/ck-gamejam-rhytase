using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum IntroState {
    Title,
    SelectStage,
}

[RequireComponent(typeof(AudioSource))]
public class IntroManager : MonoBehaviour {
    public IntroState m_state;
    [Header("Title")]
    public RectTransform m_titlePanel;
    public Button m_startButton;
    public Button m_optionsButton;
    public Button m_exitButton;

    [Header("Select Stage")]
    public RectTransform m_selectStagePanel;
    public RectTransform m_stageObjectParent;
    public RectTransform m_prevStagePosition;
    public RectTransform m_nextStagePosition;
    public List<IntroStageData> m_stageDataList;
    public IntroStage m_stageObjectPrefab;
    [HideInInspector] public List<IntroStage> m_stages;
    public Button m_nextStageButton;
    public Button m_prevStageButton;
    public Button m_backToTitleButton;
    public Button m_optionsOpenButtonAtStagePanel;
    public Button m_muteButton;


    [Header("Options")] public UIOption m_uiOption;

    private AudioSource m_audioSource;

    private int m_stageIndex;

    private void Awake() {
        Time.timeScale = 1f;
        m_state = IntroState.Title;

        m_startButton.onClick.AddListener(() => {
            m_state = IntroState.SelectStage;
            m_titlePanel.gameObject.SetActive(false);
            m_selectStagePanel.gameObject.SetActive(true);
        });
        m_optionsButton.onClick.AddListener(OpenOptions);
        m_exitButton.onClick.AddListener(() => {Application.Quit();});

        m_nextStageButton.onClick.AddListener(() => StageUpdate(1));
        m_prevStageButton.onClick.AddListener(() => StageUpdate(-1));
        m_backToTitleButton.onClick.AddListener(() => {
            m_state = IntroState.Title;
            m_titlePanel.gameObject.SetActive(true);
            m_selectStagePanel.gameObject.SetActive(false);
        });
        m_optionsOpenButtonAtStagePanel.onClick.AddListener(OpenOptions);
        m_uiOption.m_syncButton.onClick.AddListener(() => SceneManager.LoadScene("Sync"));
        m_muteButton.onClick.AddListener(ToggleMute);

        m_titlePanel.gameObject.SetActive(true);
        m_selectStagePanel.gameObject.SetActive(false);
        m_uiOption.gameObject.SetActive(false);

        m_stages = new List<IntroStage>(m_stageDataList.Count);
        foreach (var stageData in m_stageDataList) {
            var instance = Instantiate(m_stageObjectPrefab, m_stageObjectParent);
            instance.m_data = stageData;
            instance.gameObject.SetActive(false);
            instance.Initialize();
            
            // 하드코딩
            if (instance.m_stageName.sprite != null) {
                instance.m_stageSelectButton.onClick.AddListener(StartGame);
            }
            else {
                instance.m_stageSelectButton.interactable = false;
            }
            
            m_stages.Add(instance);
        }

        m_stageIndex = 0;
        StageUpdate(0);
    }

    private void Start() {
        m_audioSource = GetComponent<AudioSource>();
        m_audioSource.volume = m_uiOption.Volume;
        m_audioSource.loop = true;
        m_uiOption.m_volumeSlider.onValueChanged.AddListener((v) => m_audioSource.volume = v);
    }

    void StartGame() {
        SceneManager.LoadScene("Game");
    }

    void OpenOptions() {
        m_uiOption.gameObject.SetActive(true);
    }

    private static readonly Vector3 smaller = new Vector3(0.75f, 0.75f, 1f);
    void StageUpdate(int move) {
        m_stageIndex = Mathf.Clamp(m_stageIndex + move, 0, m_stages.Count - 1);

        for (var i = 0; i < m_stages.Count; i++) {
            if (i < m_stageIndex - 1 || i > m_stageIndex + 1) {
                m_stages[i].gameObject.SetActive(false);
                continue;
            }

            var stage = m_stages[i];
            stage.gameObject.SetActive(true);
            stage.rectTransform.localScale = i == m_stageIndex ? Vector3.one : smaller;
            if (i == m_stageIndex + 1) {
                stage.rectTransform.anchoredPosition = m_nextStagePosition.anchoredPosition;
            }else if (i == m_stageIndex - 1) {
                stage.rectTransform.anchoredPosition = m_prevStagePosition.anchoredPosition;
            }else {
                stage.rectTransform.anchoredPosition = Vector2.zero;
            }
        }
    }

    void SetVolume(float volume) {
        PlayerPrefs.SetFloat("volume", volume);
        m_audioSource.volume = volume;
    }

    void ToggleMute() {
        var prev = PlayerPrefs.GetInt("mute", 0);
        var mute = prev == 0;
        PlayerPrefs.SetInt("mute", mute ? 1 : 0);
        var volume = PlayerPrefs.GetFloat("volume");
        m_audioSource.volume = mute ? volume : 0f;
        if(mute) {
            // TODO ��Ʈ ��ư ���� ����
        }
    }
}
