using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolvePillar : MonoBehaviour
{
    public static RevolvePillar instance; // TODO: remove instance

    // Note: EnterX refers to things from the player camera perspective, such as unveiling a new zone 180 degrees away.
    // ZoneType refers to the zone the player is actively standing in
    private enum EnterState {
        Frost,
        Flame,
        Generic
    }

    public enum EnterType {
        Clockwise,
        Anticlockwise,
        None
    }

    public enum ZoneType {
        Frost,
        Flame,
        Generic
    }

    private float m_genericThreshold = .5f; //50f;

    private float m_currAngle;
    private float m_prevAngle;

    private int m_completedCycles;
    private bool m_passedPrelimZone;
    private EnterState m_enteringState;
    private EnterType m_enterType;
    private ZoneType m_zoneType;

    [SerializeField]
    private GameObject m_frostMasker, m_flameMasker;

    [SerializeField]
    private GameObject m_simulationPillar;

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

        m_enteringState = EnterState.Generic;
        m_enterType = EnterType.None;
        m_zoneType = ZoneType.Generic;

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
        Vector3 targetPosition = Player.instance.transform.position;
        targetPosition.y = this.transform.position.y;

        this.transform.LookAt(targetPosition);

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
        if (m_enteringState == EnterState.Generic) {
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
        if (m_enteringState != EnterState.Generic) {
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

        //update ZoneType
        // if in a region that could update and moving
        if (m_completedCycles == 0 && m_prevAngle != m_currAngle) {
            if (angle >= 180 && angle <= 180 + m_genericThreshold) {
                // check for enter Flame or leave Frost
                // enter Flame
                if (m_enterType == EnterType.Clockwise) {
                    if (m_zoneType == ZoneType.Generic) {
                        m_zoneType = ZoneType.Flame;
                    }
                }
                else {
                    // leave Frost
                    if (m_zoneType == ZoneType.Frost) {
                        m_zoneType = ZoneType.Generic;
                    }
                }
            }
            else if (angle >= 180 - m_genericThreshold && angle <= 180) {
                // check for enter Frost or leave Flame
                // enter Frost
                if (m_enterType == EnterType.Anticlockwise) {
                    if (m_zoneType == ZoneType.Generic) {
                        m_zoneType = ZoneType.Frost;
                    }
                }
                else {
                    // leave Flame
                    if (m_zoneType == ZoneType.Flame) {
                        m_zoneType = ZoneType.Generic;
                    }
                }
            }
        }
    }

    #region EventHandlers

    private void HandleEnterFrost() {
        m_enteringState = EnterState.Frost;
        m_enterType = EnterType.Anticlockwise;

        m_frostMasker.SetActive(true);
    }

    private void HandleExitRealms() {
        m_enteringState = EnterState.Generic;
        m_enterType = EnterType.None;

        m_frostMasker.SetActive(false);
        m_flameMasker.SetActive(false);
    }

    private void HandleEnterFlame() {
        m_enteringState = EnterState.Flame;
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

    public float SimulateLookAtEuler(GameObject targetObj) {
        Vector3 targetPos = targetObj.transform.position;
        targetPos.y = this.transform.position.y;
        m_simulationPillar.transform.LookAt(targetPos);
        return m_simulationPillar.transform.localRotation.eulerAngles.y;
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

    public bool GetPassedPrelim() {
        return m_passedPrelimZone;
    }

    public EnterType GetEnterType() {
        return m_enterType;
    }

    public ZoneType GetZoneType() {
        return m_zoneType;
    }
}
