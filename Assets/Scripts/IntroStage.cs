using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
[System.Serializable]
public class IntroStageData {
    public Sprite m_cover;
    public Sprite m_stageName;
    public Sprite m_musicName;
}

public class IntroStage : MonoBehaviour {
    [HideInInspector] public RectTransform rectTransform;
    public IntroStageData m_data;
    public Image m_cover;
    public Image m_stageName;
    public Image m_musicName;
    public Button m_stageSelectButton;

    private static readonly Color transparent = new Color(1f, 1f, 1f, 0f); 
    public void Initialize() {
        rectTransform = GetComponent<RectTransform>();
        if(m_data != null) {
            m_cover.sprite = m_data.m_cover;
            m_stageName.sprite = m_data.m_stageName;
            m_musicName.sprite = m_data.m_musicName;
        }

        if (m_stageName.sprite == null) {
            m_stageName.color = transparent;
        }
        else {
            m_stageName.color = Color.white;;
        }

        if (m_musicName.sprite == null) {
            m_musicName.color = transparent;
        }
        else {
            m_musicName.color = Color.white;
        }
    }
}