using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSet : MonoBehaviour {
    [SerializeField]
    protected List<Tile> m_tiles;

    public List<Tile> GetTiles() {
        return m_tiles;
    }

    private void Awake() {
        if (m_tiles.Count == 0) {
            var tiles = GetComponentsInChildren<Tile>();
            m_tiles.AddRange(tiles);
        }
    }
}
