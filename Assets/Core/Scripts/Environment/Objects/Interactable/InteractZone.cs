using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractZone : MonoBehaviour
{
    private Interactable m_parentInteractable;

    private void OnEnable() {
        m_parentInteractable = this.transform.parent.GetComponent<Interactable>();
    }

    private void OnTriggerEnter(Collider other) {
        m_parentInteractable.HandleOnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other) {
        m_parentInteractable.HandleOnTriggerExit(other);
    }
}
