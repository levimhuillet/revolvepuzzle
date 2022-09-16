using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OuterSphere : MonoBehaviour
{
    Renderer m_renderer;

    // Start is called before the first frame update
    void Start()
    {
        m_renderer = this.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        m_renderer.sharedMaterial.SetFloat("_TowerRotation", RevolvePillar.instance.GetWoundedness());       
        m_renderer.sharedMaterial.SetFloat("_Radius", RevolvePillar.instance.gameObject.transform.localScale.y);
    }
}
