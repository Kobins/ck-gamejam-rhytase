using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stage : MonoBehaviour {
    [HideInInspector]
    public List<Tile> m_tiles;
    [HideInInspector] public GameManager m_gameManager;
    public AudioClip m_music;
    public float m_offset = 0f;
    public float m_startCountBPM = 0f;
    public float m_startCountBPMMultiply = 1f;
    [HideInInspector]
    public float m_endTime;

    private void Awake() {
        RegisterTiles();

        if (m_startCountBPM <= 0f && m_tiles.Count > 0) {
            m_startCountBPM = m_tiles[0].m_bpmSetOnThisTile;
        }
    }

    private void Start() {
        foreach (var tile in m_tiles) {
            tile.m_gameManager = m_gameManager;
        }
    }
    
    
    private void RegisterTiles() {
        m_tiles = new List<Tile>(transform.childCount);
        // Debug.Log($"childCount: {transform.childCount}");
        foreach (var childTransform in GetComponentsInChildren<Transform>()) {
            var tile = childTransform.gameObject.GetComponent<Tile>();
            if (tile) {
                m_tiles.Add(tile);
                // Debug.Log($"- add tile: {tile.name}");
                continue;
            }
/*
            var tileSet = childTransform.gameObject.GetComponent<TileSet>();
            if (tileSet) {
                m_tiles.AddRange(tileSet.GetTiles());
                Debug.Log($"- add tileSet: {tileSet.name}");
                continue;
            }
            */
        }
        
        float time = m_offset;
        var elapseTime = 0f;
        var size = m_tiles.Count;
        // Debug.Log($"final size: {size}");
        for (var i = 0; i < size; i++) {
            var tile = m_tiles[i];
            if (tile.m_bpmSetOnThisTile > 0) {
                elapseTime = 1 / (tile.m_bpmSetOnThisTile / 60);
            }

            tile.m_expectedTime = time;
            time += elapseTime;
        }

        m_endTime = time - elapseTime;
    }
}
