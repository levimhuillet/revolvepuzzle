using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    private void Awake() {
        instance = this;
    }

    private void Update() {
        return;
    }
}
