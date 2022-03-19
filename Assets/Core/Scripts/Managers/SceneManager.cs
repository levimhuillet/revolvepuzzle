using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {
    public static SceneManager instance;

    public const float GenericAngleThreshold = 0.5f;

    [SerializeField]
    private RevolvePillar[] m_pillars;

    private List<ConditionallyActive> m_cActives;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (this != instance) {
            Destroy(this.gameObject);
        }

        m_cActives = new List<ConditionallyActive>();
    }

    // Update is called once per frame
    void Update() {
        if (!AnglesHaveChanged()) {
            return;
        }

        foreach (ConditionallyActive cActive in m_cActives) {
            cActive.MarkedActive = false;

            List<ConditionallyActive.CATowerData> data = cActive.GetData();

            // if data is empty, present everywhere
            if (data.Count == 0) {
                cActive.MarkedActive = true;
                continue;
            }

            foreach (ConditionallyActive.CATowerData d in data) {
                foreach (RevolvePillar pillar in m_pillars) {
                    string pillarID = pillar.GetID();
                    if (pillar.GetID() == d.TowerID) {
                        // Determine if should be set active
                        float rotation = pillar.GetCurrEuler();

                        bool withinAngle;
                        if (d.ClockwiseBounds) {
                            withinAngle = (rotation >= d.MinAngle && rotation <= d.MaxAngle);
                        }
                        else {
                            withinAngle = (rotation <= d.MinAngle || rotation >= d.MaxAngle);
                        }

                        if (withinAngle
                            && pillar.GetCycleNum() == d.CycleNum
                            && (pillar.GetPassedPrelim() == d.PassedPrelim)
                            && pillar.GetEnterType() == d.EnterType) {
                            cActive.MarkedActive = true;
                        }
                    }
                }
            }
        }

        foreach (ConditionallyActive cActive in m_cActives) {
            cActive.gameObject.SetActive(cActive.MarkedActive);
        }
    }

    private bool AnglesHaveChanged() {
        foreach (RevolvePillar p in m_pillars) {
            if (p.GetCurrEuler() != p.GetPrevEuler()) {
                return true;
            }
        }

        return false;
    }

    public void AddCActive(ConditionallyActive cActive) {
        m_cActives.Add(cActive);
    }

    public RevolvePillar[] GetPillars() {
        return m_pillars;
    }
}
