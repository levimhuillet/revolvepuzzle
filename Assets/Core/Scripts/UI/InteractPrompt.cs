using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPrompt : MonoBehaviour
{
    private void Start() {
        EventManager.OnInteractEnter.AddListener(HandleInteractEnter);
        EventManager.OnInteractExit.AddListener(HandleInteractExit);
        this.gameObject.SetActive(false);
    }

    // TODO: make a more robust system through player for when next to multiple objects at once
    void HandleInteractEnter() {
        this.gameObject.SetActive(true);
    }
    void HandleInteractExit() {
        this.gameObject.SetActive(false);
    }
}
