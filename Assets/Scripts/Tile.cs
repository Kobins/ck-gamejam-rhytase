using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    [HideInInspector] public GameManager m_gameManager;
    public float m_bpmSetOnThisTile = -1;
    public PlayerAnimationType m_playerAnimationSetOnThisTile = PlayerAnimationType.NONE;
    public Color m_colorOnTouch = Color.green;
    public GameObject m_effectOnTouch;
    public Transform m_effectPositionOnTouch;
    public Material m_backgroundSetOnThisTile;

    public MeshRenderer m_meshRenderer;
    public Material m_targetMaterial;
    private Color m_initialMaterialColor;
    private static readonly Dictionary<TouchPredictType, Color> PredictColorMap = new Dictionary<TouchPredictType, Color>() {
        {TouchPredictType.Perfect, Color.green},
        {TouchPredictType.LittleFast, Color.green},
        {TouchPredictType.LittleSlow, Color.green},
        {TouchPredictType.Slow, new Color(1, 0.5f, 0.0f, 1.0f)},
        {TouchPredictType.Fast, new Color(1, 0.5f, 0.0f, 1.0f)},
        {TouchPredictType.TooFast, Color.red},
        {TouchPredictType.TooSlow, Color.red},
    };
    private void Start() {
        if (m_targetMaterial == null && m_meshRenderer != null) {
            m_targetMaterial = m_meshRenderer.material;
        }

        if (m_targetMaterial) {
            m_initialMaterialColor = m_targetMaterial.color;
        }
    }

    [HideInInspector]
    public float m_expectedTime;

    public virtual void Touch(Player playerObject, Tile prevTile, TouchPredictType type) {
        if (m_targetMaterial) {
            if(PredictColorMap.TryGetValue(type, out var color)) {
                m_targetMaterial.color = color;
            }
        }
        // 애니메이션 변경 (실패했으면 그냥 다음 프레임)
        if (!playerObject.SetAnimation(m_playerAnimationSetOnThisTile)) {
            playerObject.NextSprite();
        }

        if (m_effectOnTouch) {
            var position = m_effectPositionOnTouch != null ? m_effectPositionOnTouch.position : transform.position;
            Instantiate(m_effectOnTouch, position, Quaternion.identity);
        }

        if (m_backgroundSetOnThisTile) {
            m_gameManager.m_background.m_spriteRenderer.material = m_backgroundSetOnThisTile;
        }
    }

    public virtual void TouchNextTile(Player playerObject, Tile nextTile) {
        
    }

    private void Update() {
        if (m_targetMaterial != null && m_targetMaterial.color != m_initialMaterialColor) {
            m_targetMaterial.color = Color.Lerp(m_targetMaterial.color, m_initialMaterialColor, Time.deltaTime * 5f);
        }
    }

    public virtual void UpdatePlayer(Player playerObject, Tile nextTile, float percentage) {
        playerObject.transform.position = Vector3.Lerp(
            transform.position,
            nextTile.transform.position,
            percentage
        );
    }
}
