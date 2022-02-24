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

    [SerializeField]
    private GameObject m_frostMasker, m_flameMasker;

    private List<Camera> m_cams;

    private float m_genericThreshold = .5f; //50f;
    private float m_prevAngle; // angle during the previous frame
    private int m_completedCycles;
    private bool m_passedPrelimZone;

    private State m_enteringState;
    private EnterType m_enterType;

    private void Start() {
        m_enteringState = State.Generic;
        m_enterType = EnterType.None;

        m_cams = new List<Camera>();
        m_cams.Add(m_genericCam);
        m_cams.Add(m_frostCam);
        m_cams.Add(m_flameCam);

        m_completedCycles = 0;
        m_passedPrelimZone = false;

        m_frostMasker.SetActive(false);
        m_flameMasker.SetActive(false);
    }

    // Update is called once per frame
    void Update() {

        foreach (Camera cam in m_cams) {
            cam.gameObject.transform.position = this.transform.position;
            cam.gameObject.transform.rotation = this.transform.rotation;
            cam.gameObject.transform.localScale = this.transform.localScale;
        }

        float angle = RevolvePillar.instance.GetEulerRotation();
        UpdateCycleState(angle);

        Debug.Log("Current cycles: " + m_completedCycles);

        m_prevAngle = angle;
    }

    private void UpdateCycleState(float angle) {
        float clockwiseInner = m_genericThreshold * 2;
        float clockwiseOuter = m_genericThreshold;
        float aclockwiseInner = 360 - m_genericThreshold * 2;
        float aclockwiseOuter = 360 - m_genericThreshold;

        // Check if enter from generic
        if (m_enteringState == State.Generic) {
            if (angle <= aclockwiseOuter && angle >= aclockwiseInner && m_prevAngle > aclockwiseOuter) {
                EnterFrost();
                return;
            }
            else if (angle >= clockwiseOuter && angle <= clockwiseInner && m_prevAngle < clockwiseOuter) {
                EnterFlame();
                return;
            }
        }

        // Check if exiting is a possibility
        if (m_completedCycles == 0 && !m_passedPrelimZone) {
            // Check if exit from Anticlockwise (Frost)
            if (m_enterType == EnterType.Anticlockwise) {
                if (angle > aclockwiseOuter && m_prevAngle <= aclockwiseOuter && m_prevAngle >= aclockwiseInner) {
                    Debug.Log("exiting Frost");
                    ExitRealms();
                    return;
                }
            }
            // Check if exit from Clockwise (Flame)
            else if (m_enterType == EnterType.Clockwise) {
                if (angle < clockwiseOuter && m_prevAngle >= clockwiseOuter && m_prevAngle <= clockwiseInner) {
                    Debug.Log("exiting Flame");
                    ExitRealms();
                    return;
                }
            }
        }

        // Check if furthering loops
        if (m_enteringState != State.Generic) {
            // within Frost:
            if (m_enterType == EnterType.Anticlockwise) {
                // check if entered threshold zone
                if (angle <= aclockwiseOuter && angle >= aclockwiseInner) {
                    // check if entered from enter (versus staying within zone)
                    if (m_prevAngle > aclockwiseOuter) {
                        // enter
                        m_completedCycles++;
                        m_passedPrelimZone = false;
                        return;
                    }
                    // else remain within zone
                }
                else if (angle > aclockwiseOuter && m_prevAngle <= aclockwiseOuter && m_prevAngle >= aclockwiseInner) {
                    m_completedCycles--;
                    m_passedPrelimZone = true;
                    return;
                }
                // check if entered prelim zone
                else if (angle <= clockwiseInner && angle >= clockwiseOuter) {
                    m_passedPrelimZone = true;
                }
                else if (angle > clockwiseInner && angle < 90) {
                    m_passedPrelimZone = false;
                }

                // Invisible Masker
                if (!m_frostMasker.activeSelf && angle > 90  && angle < 95 && m_completedCycles == 0) {
                    m_frostMasker.SetActive(true);
                }
                else if (m_frostMasker.activeSelf && angle < 90 && angle > 85 && m_completedCycles == 0) {
                    m_frostMasker.SetActive(false);
                }
            }
            // within Flame:
            else if (m_enterType == EnterType.Clockwise) {
                // check if entered threshold zone
                if (angle >= clockwiseOuter && angle <= clockwiseInner) {
                    // check if entered from enter (versus staying within zone)
                    if (m_prevAngle < clockwiseOuter) {
                        // enter
                        m_completedCycles++;
                        m_passedPrelimZone = false;
                        return;
                    }
                    // else remain within zone
                }
                else if (angle < clockwiseOuter && m_prevAngle >= clockwiseOuter && m_prevAngle <= clockwiseInner) {
                    m_completedCycles--;
                    m_passedPrelimZone = true;
                    return;
                }
                // check if entered prelim zone
                else if (angle >= aclockwiseInner && angle <= aclockwiseOuter) {
                    m_passedPrelimZone = true;
                }
                else if (angle < aclockwiseInner && angle > 270) {
                    m_passedPrelimZone = false;
                }

                // Invisible Masker
                if (!m_flameMasker.activeSelf && angle < 270 && angle > 265 && m_completedCycles == 0) {
                    m_flameMasker.SetActive(true);
                }
                else if (m_flameMasker.activeSelf && angle > 270 && angle < 275 && m_completedCycles == 0) {
                    m_flameMasker.SetActive(false);
                }
            }
        }
    }

    private void EnterFrost() {
        m_enteringState = State.Frost;
        m_enterType = EnterType.Anticlockwise;

        m_frostCam.gameObject.SetActive(true);
        m_genericCam.gameObject.SetActive(false);

        m_frostMasker.SetActive(true);
    }

    private void ExitRealms() {
        m_enteringState = State.Generic;
        m_enterType = EnterType.None;

        m_genericCam.gameObject.SetActive(true);
        m_frostCam.gameObject.SetActive(false);
        m_flameCam.gameObject.SetActive(false);

        m_frostMasker.SetActive(false);
        m_flameMasker.SetActive(false);
    }

    private void EnterFlame() {
        m_enteringState = State.Flame;
        m_enterType = EnterType.Clockwise;

        m_flameCam.gameObject.SetActive(true);
        m_genericCam.gameObject.SetActive(false);

        m_flameMasker.SetActive(true);
    }

    public EnterType GetRevolveDir() {
        return m_enterType;
    }

    public int GetNumCycles() {
        return m_completedCycles;
    }

    public int GetPassedPrelim() {
        if (m_passedPrelimZone) {
            return 1;
        }
        else {
            return 0;
        }
    }

    public EnterType GetEnterType() {
        return m_enterType;
    }
}
