using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionallyActive : MonoBehaviour {
    [Serializable]
    public struct CATowerData {
        public string TowerID;

        public RevolvePillar.EnterType EnterType;

        public int CycleNum;

        public bool PassedPrelim;

        public float MinAngle;

        public float MaxAngle;

        public bool ClockwiseBounds; // ex: min = 50, max = 310 -> clockwise is 50 <= x <= 310; anticlockwise is 50 >= x || x >= 310

        public CATowerData(string id, RevolvePillar.EnterType enterType, int cycleNum, bool passedPrelim, float minAngle, float maxAngle, bool clockwiseBounds) {
            TowerID = id;
            EnterType = enterType;
            CycleNum = cycleNum;
            PassedPrelim = passedPrelim;
            // TODO: swap min and max if backwards
            MinAngle = minAngle;
            MaxAngle = maxAngle;
            ClockwiseBounds = clockwiseBounds;
        }
    }

    public bool MarkedActive { get; set; }

    [SerializeField]
    private List<CATowerData> m_data;

    private void Start() {
        if (SceneManager.instance != null) {
            SceneManager.instance.AddCActive(this);
        }
        this.gameObject.SetActive(false);
    }

    public List<CATowerData> GetData() {
        return m_data;
    }

    public void ClearData() {
        m_data.Clear();
    }

    public void AddData(CATowerData data) {
        m_data.Add(data);
    }

    public void HandleDrop() {
        // set cActive data
        SetNewCActiveData();

        // set appropriate layer for the current zone
    }

    private void SetNewCActiveData() {
        // TODO: this

        // generate key points with each tower
        foreach (RevolvePillar pillar in SceneManager.instance.GetPillars()) {
            // get current object angle and zone (enter type)

            // if player is in original zone, appear
            //GenerateNewCATowerData(RevolvePillar.EnterType.None, pillar);

            // generate clockwise-ward data
            GenerateNewCATowerData(RevolvePillar.EnterType.Clockwise, pillar);

            // generate anticlockwise-ward data
            GenerateNewCATowerData(RevolvePillar.EnterType.Anticlockwise, pillar);
        }
    }

    private void GenerateNewCATowerData(RevolvePillar.EnterType newAngleDir, RevolvePillar pillar) {
        string towerID = pillar.GetID();

        RevolvePillar.EnterType enterType = RevolvePillar.EnterType.None;
        int cycleNum = 0;
        bool passedPrelim = pillar.GetPassedPrelim();
        float minAngle = 0;
        float maxAngle = 0;
        bool clockwiseBounds = true;

        RevolvePillar.EnterType currEnterType = pillar.GetEnterType();
        float currAngle = pillar.SimulateLookAtEuler(this.gameObject);
        // TODO: test edge case where object is placed on different zone than player's angle
        // may need to project object, then calculate. Otherwise keep object at exact same angle as player
        // TODO: check for prelim zone edge cases

        switch (newAngleDir) {
            case RevolvePillar.EnterType.None:
                break;
            case RevolvePillar.EnterType.Clockwise:
                switch (pillar.GetZoneType()) {
                    case RevolvePillar.ZoneType.Generic:
                        // in a generic zone

                        if (currAngle <= 180) {
                            // if less than or equal to 180:
                            // objLookAtAngle to 360 in Flame(clock) cycle 0
                            // 0 to (180 - objLookAtAngle) in Flame(clock) cycle 0
                            // (180 - objLookAtAngle) in Flame(clock) cycle 1
                            // TODO: more cases if there are more than one max cycle
                        }
                        else {
                            // if greater than 180:
                            // objLookAngle to (360 - m_genericThreshold) in Frost(anti) cycle 0
                            // (360 - m_genericThreshold) to 360 in None cycle 0
                            // 0 to (objLookAngle - 180) in None cycle 0
                        }

                        break;
                    case RevolvePillar.ZoneType.Frost:
                        // in frost zone

                        if (currAngle <= 180) {
                            // if less than or equal to 180:
                            // objLookAngle to (360 - m_genericThreshold) in Frost(anti) cycle 0
                            // (360 - m_genericThreshold) to (objLookAngle + 180) in None cycle 0
                            // TODO: more cases here if more than one max cycle
                        }
                        else {
                            // if greater than 180:
                            // objLookAngle to (360 - m_genericThreshold) in Frost(anti) cycle 1
                            // (360 - m_genericThreshold) to 360 in Frost cycle 0
                            // 0 to (objLookAngle - 180) in Frost cycle 0
                            // TODO: more cases here if more than one max cycle
                        }

                        break;
                    case RevolvePillar.ZoneType.Flame:
                        // in flame zone

                        if (currAngle <= 180) {
                            // if less than or equal to 180:
                            // objLookAngle to 360 in Flame(clock) cycle 1
                            // TODO: more cases here if more than one max cycle
                        }
                        else {
                            // if greater than 180:
                            // objLookAngle to 360 in Flame(clock) cycle 0
                            // 0 to (objLookAngle - 180) in Flame(clock) cycle 0
                            // (objLookAngle - 180) to (360 - m_genericThreshold) (since only one max cycle) in Flame(clock) cycle 1
                            // TODO: more cases here if more than one max cycle
                        }

                        break;
                    default:
                        break;
                }
                break;
            case RevolvePillar.EnterType.Anticlockwise:
                switch (pillar.GetZoneType()) {
                    case RevolvePillar.ZoneType.Generic:
                        // in a generic zone

                        if (currAngle <= 180) {
                            // if less than or equal to 180:
                            // 0 to objLookAtAngle in None cycle 0, Flame(clock) cycle 0
                            // (180 + objLookAtAngle) to 360 in None cycle 0
                            // TODO: more cases if more than one max cycle
                        }
                        else {
                            // if greater than 180:
                            // (objLookAngle - 180) to objLookAtAngle in Frost(anti) cycle 0
                            // (objLookAtAngle - 180) to 180 Frost(anti) cycle 0
                        }
                        break;
                    case RevolvePillar.ZoneType.Frost:
                        // in a frost zone

                        if (currAngle <= 180) {
                            // if less than or equal to 180:
                            // 0 to objLookAngle in Frost(anti) cycle 0
                            // (180 + objLookAngle) to 360 in Frost(anti) cycle 0
                            // 0 to 360 (since only one max cycle) in Frost(anti) cycle 1
                            // TODO: more cases here if more than one max cycle
                        }
                        else {
                            // if greater than 180:
                            // 0 to objLookAngle in Frost(anti) cycle 1
                            // TODO: more cases here if more than one max cycle
                        }

                        break;
                    case RevolvePillar.ZoneType.Flame:
                        // in flame zone

                        if (currAngle <= 180) {
                            // if less than or equal to 180:
                            // m_genericThreshold to objLookAngle in Flame(clock) cycle 1
                            // 0 to (180 - objLookAngle) in Flame(clock) cycle 0
                            // (180 + objLookAngle) to 360 in Flame(clock) cycle 0
                            // TODO: more cases here if more than one max cycle
                        }
                        else {
                            // if greater than 180:
                            // m_genericThreshold to objLookAngle in Flame(clock) cycle 0
                            // (objLookAngle - 180) to m_genericThreshold (or 180) in None cycle 0
                        }

                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }

        CATowerData newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
        AddData(newData);
    }
}
