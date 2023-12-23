using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TouchPredict : DestroyAfter {
    public float m_alphaDecreaseStart;

    [HideInInspector] public Camera m_camera;
    [HideInInspector] public Vector3 m_worldPosition;

    private Text m_text;
    private Color m_textInitialColor;
    private Color m_textAlphaColor;

    private new void Awake() {
        base.Awake();
        m_text = GetComponent<Text>();
        m_textInitialColor = m_text.color;
        m_textAlphaColor = new Color(
            m_textInitialColor.r,
            m_textInitialColor.g,
            m_textInitialColor.b,
            0f
        );
    }
    private new void Update()
    {
        base.Update();
        if (!gameObject.activeSelf) {
            return;
        }

        if (m_leftTime <= m_alphaDecreaseStart) {
            m_text.color = Color.Lerp(m_textInitialColor, m_textAlphaColor, Mathf.InverseLerp(m_alphaDecreaseStart, 0f, m_leftTime));
        }

        transform.position = m_camera.WorldToScreenPoint(m_worldPosition);
    }

    
}
