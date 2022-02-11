using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    [SerializeField]
    private Camera[] cameras;
    [SerializeField]
    private CinemachineVirtualCamera[] vcameras;

    private float swapTime = 2f;
    private float swapTimer = 0f;
    private int camIndex;

    private void Awake() {
        instance = this;
        swapTimer = swapTime;
        camIndex = 0;
    }

    private void Update() {
        if (swapTimer > 0) {
            //swapTimer = swapTimer - Time.deltaTime;
        }
        else {
            swapTimer = swapTime;
            camIndex = (camIndex + 1) % 3;

            if (camIndex == 0) {
                cameras[0].gameObject.SetActive(true);
                cameras[1].gameObject.SetActive(false);
                cameras[2].gameObject.SetActive(false);
                vcameras[0].Priority = 3;
                vcameras[0].gameObject.SetActive(true);
                vcameras[1].Priority = 2;
                vcameras[1].gameObject.SetActive(false);
                vcameras[1].Priority = 1;
                vcameras[2].gameObject.SetActive(false);
            }
            else if (camIndex == 1) {
                cameras[1].gameObject.SetActive(true);
                cameras[0].gameObject.SetActive(false);
                cameras[2].gameObject.SetActive(false);
                vcameras[1].Priority = 3;
                vcameras[1].gameObject.SetActive(true);
                vcameras[0].Priority = 2;
                vcameras[0].gameObject.SetActive(false);
                vcameras[1].Priority = 1;
                vcameras[2].gameObject.SetActive(false);
            }
            else if (camIndex == 2) {
                cameras[2].gameObject.SetActive(true);
                cameras[0].gameObject.SetActive(false);
                cameras[1].gameObject.SetActive(false);
                vcameras[2].Priority = 3;
                vcameras[2].gameObject.SetActive(true);
                vcameras[0].Priority = 2;
                vcameras[0].gameObject.SetActive(false);
                vcameras[1].Priority = 1;
                vcameras[1].gameObject.SetActive(false);
            }
        }

    }
}
