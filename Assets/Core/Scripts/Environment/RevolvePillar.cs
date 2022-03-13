using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolvePillar : MonoBehaviour
{
    public static RevolvePillar instance; // TODO: remove instance

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

    private float m_genericThreshold = .5f; //50f;

    private float m_currAngle;
    private float m_prevAngle;

    private int m_completedCycles;
    private bool m_passedPrelimZone;
    private State m_enteringState;
    private EnterType m_enterType;

    [SerializeField]
    private GameObject m_frostMasker, m_flameMasker;

    [SerializeField]
    private string m_id;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (this != instance) {
            Destroy(this.gameObject);
        }
    }

    private void Start() {
        m_prevAngle = m_currAngle = CalcEulerRotation();

        m_enteringState = State.Generic;
        m_enterType = EnterType.None;

        m_completedCycles = 0;
        m_passedPrelimZone = false;

        m_frostMasker.SetActive(false);
        m_flameMasker.SetActive(false);

        EventManager.OnEnterFrost.AddListener(HandleEnterFrost);
        EventManager.OnEnterFlame.AddListener(HandleEnterFlame);
        EventManager.OnExitRealms.AddListener(HandleExitRealms);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPostition = new Vector3(Player.instance.transform.position.x,
                                       this.transform.position.y,
                                       Player.instance.transform.position.z);
        this.transform.LookAt(targetPostition);

        m_prevAngle = m_currAngle;
        m_currAngle = CalcEulerRotation();

        UpdateCycleState(m_currAngle);
    }

    private void UpdateCycleState(float angle) {
        float clockwiseInner = m_genericThreshold * 2;
        float clockwiseOuter = m_genericThreshold;
        float aclockwiseInner = 360 - m_genericThreshold * 2;
        float aclockwiseOuter = 360 - m_genericThreshold;

        // Check if enter from generic
        if (m_enteringState == State.Generic) {
            if (angle <= aclockwiseOuter && angle >= aclockwiseInner && m_prevAngle > aclockwiseOuter) {
                EventManager.OnEnterFrost.Invoke();
                return;
            }
            else if (angle >= clockwiseOuter && angle <= clockwiseInner && m_prevAngle < clockwiseOuter) {
                EventManager.OnEnterFlame.Invoke();
                return;
            }
        }

        // Check if exiting is a possibility
        if (m_completedCycles == 0 && !m_passedPrelimZone) {
            // Check if exit from Anticlockwise (Frost)
            if (m_enterType == EnterType.Anticlockwise) {
                if (angle > aclockwiseOuter && m_prevAngle <= aclockwiseOuter && m_prevAngle >= aclockwiseInner) {
                    EventManager.OnExitRealms.Invoke();
                    return;
                }
            }
            // Check if exit from Clockwise (Flame)
            else if (m_enterType == EnterType.Clockwise) {
                if (angle < clockwiseOuter && m_prevAngle >= clockwiseOuter && m_prevAngle <= clockwiseInner) {
                    EventManager.OnExitRealms.Invoke();
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
                if (!m_frostMasker.activeSelf && angle > 90 && angle < 95 && m_completedCycles == 0) {
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

    #region EventHandlers

    private void HandleEnterFrost() {
        m_enteringState = State.Frost;
        m_enterType = EnterType.Anticlockwise;

        m_frostMasker.SetActive(true);
    }

    private void HandleExitRealms() {
        m_enteringState = State.Generic;
        m_enterType = EnterType.None;

        m_frostMasker.SetActive(false);
        m_flameMasker.SetActive(false);
    }

    private void HandleEnterFlame() {
        m_enteringState = State.Flame;
        m_enterType = EnterType.Clockwise;

        m_flameMasker.SetActive(true);
    }

    #endregion

    public float GetCurrEuler() {
        return m_currAngle;
    }

    public float GetPrevEuler() {
        return m_prevAngle;
    }

    private float CalcEulerRotation() {
        return this.transform.localRotation.eulerAngles.y;
    }

    public string GetID() {
        return m_id;
    }

    public int GetCycleNum() {
        return m_completedCycles;
    }

    public int GetPassedPrelim() {
        return m_passedPrelimZone ? 1 : 0;
    }

    public EnterType GetEnterType() {
        return m_enterType;
    }
}
