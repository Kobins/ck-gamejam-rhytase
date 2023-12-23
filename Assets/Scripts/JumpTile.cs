using System;
using System.Collections.Generic;
using UnityEngine;

public class JumpTile : Tile {
    public float m_parabolaCenterY = 2f;
    public Tile m_assumeNextTile;
    
    public GameObject m_effectOnNextTileTouch;
    public Transform m_effectPositionNextTileOnTouch;

    private float a = 0f, p = 0f;

    private void Awake() {
        CalculateFactors();
    }

    private void CalculateFactors() {
        // https://www.desmos.com/calculator/1np6e05gys?lang=ko
        var from = transform.position;
        var to = m_assumeNextTile.transform.position;
        var ray = (to - from);
        var x1 = 1f; // 0 ~ 1이기 때문에
        var y1 = ray.y;
        var m = y1 / x1;
        var k = m_parabolaCenterY;
        a = (y1 - 2 * (k + Mathf.Sqrt(k * (k - y1))));
        p = x1 - m / a;
    }

    private float GetParabola(float x) {
        return a * x * (x - p);
    }

    public override void UpdatePlayer(Player playerObject, Tile nextTile, float percentage) {
        var from = transform.position;
        var to = nextTile.transform.position;
        var xz = Vector3.Lerp(
            from,
            to,
            percentage
        );
        var y = from.y + GetParabola(percentage);
        playerObject.transform.position = new Vector3(xz.x, y, xz.z);
    }

    private void OnDrawGizmos() {
        if (m_assumeNextTile) {
            var from = transform.position;
            var to = m_assumeNextTile.transform.position;
            var center = Vector3.Lerp(from, to, 0.5f);
            var top = new Vector3(center.x, from.y + m_parabolaCenterY, center.z);
            Gizmos.DrawIcon(top, "Light Gizmo.tiff");
            Gizmos.DrawLine(from, top);
            Gizmos.DrawLine(top, to);
        }
    }

    private void OnDrawGizmosSelected() {
        if (m_assumeNextTile) {
            var from = transform.position;
            var to = m_assumeNextTile.transform.position;
            CalculateFactors();
            // Debug.Log($"a: {a}, p: {p}");
            var positions = new List<Vector3>(11);
            for (int i = 0; i <= 10; i++) {
                var percentage = i / 10f;
                var xz = Vector3.Lerp(
                    from,
                    to,
                    percentage
                );
                var y = from.y + GetParabola(percentage);
                var pos = new Vector3(xz.x, y, xz.z);
                positions.Add(pos);
            }

            for (int i = 0; i < 10; i++) {
                Gizmos.DrawLine(positions[i], positions[i+1]);
            }
        }
    }
}