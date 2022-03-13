using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    [SerializeField]
    private Camera m_genericCam;
    [SerializeField]
    private Camera m_frostCam;
    [SerializeField]
    private Camera m_flameCam;

    private List<Camera> m_cams;

    private void Start() {
        m_cams = new List<Camera>();
        m_cams.Add(m_genericCam);
        m_cams.Add(m_frostCam);
        m_cams.Add(m_flameCam);

        EventManager.OnEnterFrost.AddListener(HandleEnterFrost);
        EventManager.OnEnterFlame.AddListener(HandleEnterFlame);
        EventManager.OnExitRealms.AddListener(HandleExitRealms);
    }

    void LateUpdate() {
        foreach (Camera cam in m_cams) {
            cam.gameObject.transform.position = this.transform.position;
            cam.gameObject.transform.rotation = this.transform.rotation;
            cam.gameObject.transform.localScale = this.transform.localScale;
        }
    }

    private void HandleEnterFrost() {
        m_frostCam.gameObject.SetActive(true);
        m_genericCam.gameObject.SetActive(false);
    }

    private void HandleExitRealms() {
        m_genericCam.gameObject.SetActive(true);
        m_frostCam.gameObject.SetActive(false);
        m_flameCam.gameObject.SetActive(false);
    }

    private void HandleEnterFlame() {
        m_flameCam.gameObject.SetActive(true);
        m_genericCam.gameObject.SetActive(false);
    }
}
