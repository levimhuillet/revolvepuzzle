using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleMasker : MonoBehaviour
{
    [SerializeField]
    private Renderer[] m_renders;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Renderer render in m_renders) {
            render.material.renderQueue = 2002; // set their renderQueue
        }
    }
}
