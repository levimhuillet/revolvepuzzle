using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public void HandleOnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            EventManager.OnInteractEnter.Invoke();
        }
    }

    public void HandleOnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            EventManager.OnInteractExit.Invoke();
        }
    }
}
