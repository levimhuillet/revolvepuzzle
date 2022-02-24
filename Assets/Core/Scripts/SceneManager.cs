using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {
    [SerializeField]
    private ConditionallyActive[] m_cActives;

    [SerializeField]
    private RevolvePillar[] m_pillars;

    [SerializeField]
    private CentralCamera m_centralCam; // hack until consolidation

    // Update is called once per frame
    void Update() {
        foreach (ConditionallyActive cActive in m_cActives) {
            cActive.MarkedActive = false;

            ConditionallyActive.CATowerData[] data = cActive.GetData();

            foreach (ConditionallyActive.CATowerData d in data) {
                foreach (RevolvePillar pillar in m_pillars) {
                    string pillarID = pillar.GetID();
                    if (pillar.GetID() == d.TowerID) {
                        // Determine if should be set active
                        float rotation = pillar.GetEulerRotation();

                        if (rotation >= d.MinAngle
                            && rotation <= d.MaxAngle
                            && m_centralCam.GetNumCycles() == d.CycleNum
                            && m_centralCam.GetPassedPrelim() == d.PassedPrelim
                            && m_centralCam.GetEnterType() == d.EnterType) {
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
}
