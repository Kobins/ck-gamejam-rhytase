using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BPMScouter : MonoBehaviour {
    public float assumePeriod = 0f;
    private float assumeBPM = 0f;
    public Text bpmText;
    private float elapsed;
    private float lastPressed;
    private List<float> gaps;
    private void Start() {
        elapsed = 0f;
        lastPressed = 0f;
        gaps = new List<float>(100);
        ResetBPM();
    }

    private void Update() {
        elapsed += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ResetBPM();
            return;
        }
        if (Input.anyKeyDown) {
            if (lastPressed != 0f) {
                gaps.Add(elapsed - lastPressed);
            }

            lastPressed = elapsed;
            CalculateBPM();
        } 
    }

    void ResetBPM() {
        gaps.Clear();
        lastPressed = 0f;
        assumeBPM = 0f;
        bpmText.text = $"BPM: ???";
    }
    void CalculateBPM() {
        if (gaps.Count <= 2) {
            return;
        }

        assumePeriod = gaps.Sum() / gaps.Count;
        assumeBPM = (1 / assumePeriod) * 60;
        bpmText.text = $"BPM: {assumeBPM}";
    }
}
