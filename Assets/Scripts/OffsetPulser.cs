using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class OffsetPulser : MonoBehaviour {
    private MeshRenderer m_meshRenderer;
    private Material m_material;
    private Color m_initialColor;
    // Start is called before the first frame update
    void Start()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_material = m_meshRenderer.material;
        m_initialColor = m_material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_initialColor != m_material.color) {
            m_material.color = Color.Lerp(m_material.color, m_initialColor, 5 * Time.deltaTime);
        }   
    }
}
