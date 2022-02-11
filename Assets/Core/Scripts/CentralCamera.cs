using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralCamera : MonoBehaviour {
    private enum State {
        Frost,
        Flame,
        Generic
    }

    public enum EnterType {
        Clockwise,
        Anticlockwise,
        None
    }

    [SerializeField]
    private Camera m_genericCam;
    [SerializeField]
    private Camera m_frostCam;
    [SerializeField]
    private Camera m_flameCam;

    private List<Camera> m_cams;

    private float m_genericThreshold = 5; //50f;
    private int m_newCycle;
    private int m_completedCycles;

    private State m_enteringState;
    private EnterType m_enterType;

    private void Start() {
        m_enteringState = State.Generic;
        m_enterType = EnterType.None;

        m_cams = new List<Camera>();
        m_cams.Add(m_genericCam);
        m_cams.Add(m_frostCam);
        m_cams.Add(m_flameCam);

        m_newCycle = 0;
        m_completedCycles = 0;
    }

    // Update is called once per frame
    void Update() {

        foreach (Camera cam in m_cams) {
            cam.gameObject.transform.position = this.transform.position;
            cam.gameObject.transform.rotation = this.transform.rotation;
            cam.gameObject.transform.localScale = this.transform.localScale;
        }

        float angle = RevolvePillar.instance.GetEulerRotation();
        if (m_enteringState == State.Generic) {
            if (angle < 360 - m_genericThreshold && angle > 360 - m_genericThreshold * 2) {
                Debug.Log("entering frost");
                EnterFrost();
                m_enteringState = State.Frost;
                m_enterType = EnterType.Anticlockwise;
            }
            else if (angle > m_genericThreshold && angle < m_genericThreshold * 2) {
                Debug.Log("entering flame");
                EnterFlame();
                m_enteringState = State.Flame;
                m_enterType = EnterType.Clockwise;
            }
        }
        else {
            // starting new cycle but have not yet passed into new loop threshold
            if ((angle > 360 - m_genericThreshold + 1)
                && (angle < m_genericThreshold - 1)
                && m_enterType != EnterType.None
                && m_newCycle != m_completedCycles + 1) {
                m_newCycle = m_completedCycles;
                m_newCycle++;
            }
            else {
                if ((angle < m_genericThreshold && m_enterType == EnterType.Clockwise && m_newCycle != m_completedCycles + 1)
                    || (angle > 360 - m_genericThreshold && m_enterType == EnterType.Anticlockwise && m_newCycle != m_completedCycles + 1)) {
                    // TODO: keep track of cycles and preserve realm when going around pillar
                        // decrement m_new cycle when reversing in loop threshold
                        // once past the new loop threshold, increment completed cycles
                    Debug.Log("exiting all");
                    m_completedCycles++;
                    m_enteringState = State.Generic;
                    m_enterType = EnterType.None;
                    ExitRealms();
                }
            }
        }

    }

    private void EnterFrost() {
        m_frostCam.gameObject.SetActive(true);
        m_genericCam.gameObject.SetActive(false);
    }

    private void ExitRealms() {
        m_genericCam.gameObject.SetActive(true);
        m_frostCam.gameObject.SetActive(false);
        m_flameCam.gameObject.SetActive(false);
    }

    private void EnterFlame() {
        m_flameCam.gameObject.SetActive(true);
        m_genericCam.gameObject.SetActive(false);
    }

    public EnterType GetRevolveDir() {
        return m_enterType;
    }
}
