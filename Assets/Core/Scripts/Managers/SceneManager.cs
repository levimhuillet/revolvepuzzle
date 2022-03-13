using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {
    [SerializeField]
    private ConditionallyActive[] m_cActives;

    [SerializeField]
    private RevolvePillar[] m_pillars;

    // Update is called once per frame
    void Update() {
        if (!AnglesHaveChanged()) {
            return;
        }

        foreach (ConditionallyActive cActive in m_cActives) {
            cActive.MarkedActive = false;

            ConditionallyActive.CATowerData[] data = cActive.GetData();

            foreach (ConditionallyActive.CATowerData d in data) {
                foreach (RevolvePillar pillar in m_pillars) {
                    string pillarID = pillar.GetID();
                    if (pillar.GetID() == d.TowerID) {
                        // Determine if should be set active
                        float rotation = pillar.GetCurrEuler();

                        if (rotation >= d.MinAngle
                            && rotation <= d.MaxAngle
                            && pillar.GetCycleNum() == d.CycleNum
                            && pillar.GetPassedPrelim() == d.PassedPrelim
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
}
