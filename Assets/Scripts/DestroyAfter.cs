using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour {
    public float m_duration = 5f;
    protected float m_leftTime = 0f;

    protected void Awake() {
        m_leftTime = m_duration;
    }

    protected void Update() {
        if (m_leftTime <= 0) {
            var thisObject = gameObject;
            thisObject.SetActive(false);
            Destroy(thisObject);
            return;
        }

        m_leftTime -= Time.deltaTime;
    }
}
