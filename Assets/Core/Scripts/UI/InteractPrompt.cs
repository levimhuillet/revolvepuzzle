using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPrompt : MonoBehaviour
{
    private void Start() {
        EventManager.OnInteractionsAvailable.AddListener(HandleInteractionsAvailable);
        EventManager.OnNoMoreInteractions.AddListener(HandleNoMoreInteractions);
        this.gameObject.SetActive(false);
    }

    void HandleInteractionsAvailable() {
        this.gameObject.SetActive(true);
    }
    void HandleNoMoreInteractions() {
        this.gameObject.SetActive(false);
    }
}
