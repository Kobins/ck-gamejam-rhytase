using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOption : MonoBehaviour {
    public Button m_closeButton;
    public Slider m_volumeSlider;
    public Button m_syncButton;

    public float Volume {
        get => m_volumeSlider.value;
        set => m_volumeSlider.value = value;
    }

    private void Awake() {
        m_closeButton.onClick.AddListener(() => {
            gameObject.SetActive(false);
        });

        var volume = PlayerPrefs.GetFloat("volume", 0.5f);
        m_volumeSlider.value = volume;
        m_volumeSlider.onValueChanged.AddListener((v) => Volume = v);
    }

    public void RegisterAudioSource(AudioSource audioSource) {
        m_volumeSlider.onValueChanged.AddListener((v) => audioSource.volume = v);
    }
}
