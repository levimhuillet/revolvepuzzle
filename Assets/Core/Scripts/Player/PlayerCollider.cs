using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    FirstPersonController m_fpc;

    void Awake()
    {
        m_fpc = this.transform.parent.GetComponent<FirstPersonController>();
    }

    private void OnTriggerEnter(Collider other) {
        m_fpc.HandleOnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other) {
        m_fpc.HandleOnTriggerExit(other);
    }
}
