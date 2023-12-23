using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TouchPredictType {
    None,
    TooFast,
    Fast,
    LittleFast,
    Perfect,
    LittleSlow,
    Slow,
    TooSlow,
}
public class TouchPredictManager : MonoBehaviour {
    public Camera m_camera;
    [SerializeField] private TouchPredict m_tooFast;
    [SerializeField] private TouchPredict m_fast;
    [SerializeField] private TouchPredict m_littleFast;
    [SerializeField] private TouchPredict m_perfect;
    [SerializeField] private TouchPredict m_littleSlow;
    [SerializeField] private TouchPredict m_slow;
    [SerializeField] private TouchPredict m_tooSlow;
    private Dictionary<TouchPredictType, TouchPredict> m_predicts;

    private void Awake() {
        m_predicts = new Dictionary<TouchPredictType, TouchPredict>() {
            {TouchPredictType.TooFast, m_tooFast},
            {TouchPredictType.Fast, m_fast},
            {TouchPredictType.LittleFast, m_littleFast},
            {TouchPredictType.Perfect, m_perfect},
            {TouchPredictType.LittleSlow, m_littleSlow},
            {TouchPredictType.Slow, m_slow},
            {TouchPredictType.TooSlow, m_tooSlow},
        };
    }

    public void Create(TouchPredictType type, Vector3 at) {
        var prefab = m_predicts[type];
        if (prefab == null) {
            return;
        }

        var position = m_camera.WorldToScreenPoint(at);
        var instance = Instantiate(prefab, position, Quaternion.identity, transform);
        instance.m_worldPosition = at;
        instance.m_camera = m_camera;
    }
}
