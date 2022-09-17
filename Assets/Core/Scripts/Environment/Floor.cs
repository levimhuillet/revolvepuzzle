using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public static Floor Instance;

    private void Awake() {
        if (Instance != null) {
            Destroy(this.gameObject);
        }
        else if (Instance == null) {
            Instance = this;
        }
    }
}
