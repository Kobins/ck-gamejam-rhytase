using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class OffsetAdjuster : MonoBehaviour {
    public float m_offset;
    public float m_bpm;
    public MeshRenderer m_pulseObject;
    public Text m_offsetText;
    public Button m_decreaseOffsetButton;
    public Button m_increaseOffsetButton;
    
    private AudioSource m_audioSource;
    private float m_period;
    private float m_elapsed = 0f;

    private void Awake() {
        Time.timeScale = 1f;
        m_offset = PlayerPrefs.GetFloat("offset", 0f);
        UpdateOffset();
        m_decreaseOffsetButton.onClick.AddListener(Decrease);
        m_increaseOffsetButton.onClick.AddListener(Increase);
        m_audioSource = GetComponent<AudioSource>();
        m_audioSource.playOnAwake = false;
        m_audioSource.Stop();
        m_audioSource.loop = false;
        m_audioSource.volume = PlayerPrefs.GetFloat("volume", 0.5f);
        m_period = 1 / (m_bpm / 60);
    }

    private void Start() {
        InitializeMusic();
    }

    private int m_count = 0;
    private void Update() {
        if (!m_audioSource.isPlaying) {
            InitializeMusic();
            return;
        }

        var time = m_audioSource.time - m_offset;
        var currentCount = Mathf.CeilToInt(time / m_period);
        if (m_count != currentCount) {
            Pulse();
            m_count = currentCount;
        }
        m_elapsed += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("Intro");
        }else if (Input.GetKey(KeyCode.LeftArrow)) {
            Decrease();
        }else if (Input.GetKey(KeyCode.RightArrow)) {
            Increase();
        }
    }

    void InitializeMusic() {
        m_audioSource.Play();
        m_elapsed = 0f;
        m_count = 0;
    }

    void Pulse() {
        m_pulseObject.material.color = Color.white;
    }

    void Decrease() {
        m_offset -= 0.01f;
        UpdateOffset();
    }
    void Increase() {
        m_offset += 0.01f;
        UpdateOffset();
    }

    void UpdateOffset() {
        PlayerPrefs.SetFloat("offset", m_offset);
        m_offsetText.text = m_offset.ToString("F3");
    }
}
