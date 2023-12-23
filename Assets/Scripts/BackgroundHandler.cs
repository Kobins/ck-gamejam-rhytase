using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundHandler : MonoBehaviour {
    public MeshRenderer m_spriteRenderer;
    public float m_scrollSpeed = 0.2f;

    // private Material m_material;
    private void Start() {
        // m_material = m_spriteRenderer.material;
    }

    private void Update() {
        var direction = Vector2.left;
        if (m_spriteRenderer.material) {
            m_spriteRenderer.material.mainTextureOffset += direction * (m_scrollSpeed * Time.deltaTime);
        }
    }
}
