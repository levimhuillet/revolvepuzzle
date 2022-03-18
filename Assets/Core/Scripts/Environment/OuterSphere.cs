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
        m_renderer.sharedMaterial.SetFloat("_TowerRotation", RevolvePillar.instance.gameObject.transform.rotation.eulerAngles.y);
        int revolveDirInt = 0;
        if (RevolvePillar.instance.GetEnterType() == RevolvePillar.EnterType.Clockwise) {
            revolveDirInt = 1;
        }
        else if (RevolvePillar.instance.GetEnterType() == RevolvePillar.EnterType.Anticlockwise) {
            revolveDirInt = 2;
        }
        m_renderer.sharedMaterial.SetInt("_RevolveDir", revolveDirInt);
        m_renderer.sharedMaterial.SetFloat("_Radius", RevolvePillar.instance.gameObject.transform.localScale.y);
        m_renderer.sharedMaterial.SetInt("_NumCycles", RevolvePillar.instance.GetCycleNum());
        m_renderer.sharedMaterial.SetInt("_PassedPrelim", RevolvePillar.instance.GetPassedPrelim() ? 1 : 0);
    }
}
