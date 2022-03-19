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
        float objLookAtAngle = pillar.GetCurrEuler(); // TODO: fix simulationPillar
        CATowerData newData;
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

                        if (objLookAtAngle <= 180) {
                            // if less than or equal to 180:
                            // objLookAtAngle to 360 in Flame(clock) cycle 0
                            enterType = RevolvePillar.EnterType.Clockwise;
                            cycleNum = 0;
                            minAngle = objLookAtAngle;
                            maxAngle = 360;
                            Debug.Log("Generate 1: min: " + minAngle + " || max: " + maxAngle);

                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

                            // 0 to (180 - objLookAtAngle) in Flame(clock) cycle 0
                            enterType = RevolvePillar.EnterType.Clockwise;
                            cycleNum = 0;
                            minAngle = 0;
                            maxAngle = 180 - objLookAtAngle;
                            Debug.Log("Generate 2: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

                            // (180 - objLookAtAngle) to 360 in Flame(clock) cycle 1
                            enterType = RevolvePillar.EnterType.Clockwise;
                            cycleNum = 1;
                            minAngle = 180 - objLookAtAngle;
                            maxAngle = 360;
                            Debug.Log("Generate 3: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

                            // TODO: more cases if there are more than one max cycle
                        }
                        else {
                            // if greater than 180:
                            // objLookAngle to (360 - m_genericThreshold) in Frost(anti) cycle 0
                            enterType = RevolvePillar.EnterType.Anticlockwise;
                            cycleNum = 0;
                            minAngle = objLookAtAngle;
                            maxAngle = 360 - SceneManager.GenericAngleThreshold;
                            Debug.Log("Generate 4: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

                            // (360 - m_genericThreshold) to 360 in None cycle 0
                            enterType = RevolvePillar.EnterType.None;
                            cycleNum = 0;
                            minAngle = 360 - SceneManager.GenericAngleThreshold;
                            maxAngle = 360;
                            Debug.Log("Generate 5: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

                            // 0 to (objLookAngle - 180) in None cycle 0
                            enterType = RevolvePillar.EnterType.None;
                            cycleNum = 0;
                            minAngle = 0;
                            maxAngle = objLookAtAngle - 180;
                            Debug.Log("Generate 6: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);
                        }

                        break;
                    case RevolvePillar.ZoneType.Frost:
                        // in frost zone

                        if (objLookAtAngle <= 180) {
                            // if less than or equal to 180:
                            // objLookAngle to (360 - m_genericThreshold) in Frost(anti) cycle 0
                            enterType = RevolvePillar.EnterType.Anticlockwise;
                            cycleNum = 0;
                            minAngle = objLookAtAngle;
                            maxAngle = Mathf.Min(objLookAtAngle + 180, 360 - SceneManager.GenericAngleThreshold);
                            Debug.Log("Generate 7: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

                            if (objLookAtAngle + 180 > 360 - SceneManager.GenericAngleThreshold) {
                                // (360 - m_genericThreshold) to (objLookAngle + 180) in None cycle 0
                                enterType = RevolvePillar.EnterType.None;
                                cycleNum = 0;
                                minAngle = 360 - SceneManager.GenericAngleThreshold;
                                maxAngle = objLookAtAngle + 180;
                                Debug.Log("Generate 8: min: " + minAngle + " || max: " + maxAngle);

                                newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                                AddData(newData);
                            }

                            // TODO: more cases here if more than one max cycle
                        }
                        else {
                            // if greater than 180:
                            // objLookAngle to (360 - m_genericThreshold) in Frost(anti) cycle 1
                            enterType = RevolvePillar.EnterType.Anticlockwise;
                            cycleNum = 1;
                            minAngle = objLookAtAngle;
                            maxAngle = 360 - SceneManager.GenericAngleThreshold;

                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

                            // (360 - m_genericThreshold) to 360 in Frost(anti) cycle 0
                            enterType = RevolvePillar.EnterType.Anticlockwise;
                            cycleNum = 0;
                            minAngle = 360 - SceneManager.GenericAngleThreshold;
                            maxAngle = 360;
                            Debug.Log("Generate 9: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

                            // 0 to (objLookAngle - 180) in Frost(anti) cycle 0
                            enterType = RevolvePillar.EnterType.Anticlockwise;
                            cycleNum = 0;
                            minAngle = 0;
                            maxAngle = objLookAtAngle - 180;
                            Debug.Log("Generate 10: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

                            // TODO: more cases here if more than one max cycle
                        }

                        break;
                    case RevolvePillar.ZoneType.Flame:
                        // in flame zone

                        if (objLookAtAngle <= 180) {
                            // if less than or equal to 180:
                            // objLookAngle to 360 in Flame(clock) cycle 1
                            enterType = RevolvePillar.EnterType.Clockwise;
                            cycleNum = 1;
                            minAngle = objLookAtAngle;
                            maxAngle = 360;
                            Debug.Log("Generate 11: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

                            // TODO: more cases here if more than one max cycle
                        }
                        else {
                            // if greater than 180:
                            // objLookAngle to 360 in Flame(clock) cycle 0
                            enterType = RevolvePillar.EnterType.Clockwise;
                            cycleNum = 0;
                            minAngle = objLookAtAngle;
                            maxAngle = 360;
                            Debug.Log("Generate 12: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

                            /*
                            // 0 to (objLookAngle - 180) in Flame(clock) cycle 0
                            enterType = RevolvePillar.EnterType.Clockwise;
                            cycleNum = 0;
                            minAngle = 0;
                            maxAngle = Mathf.Min(objLookAtAngle - 180, SceneManager.GenericAngleThreshold);
                            Debug.Log("Generate 13: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);
                            */

                            // (objLookAngle - 180) to (360 - m_genericThreshold) (since only one max cycle) in Flame(clock) cycle 1
                            enterType = RevolvePillar.EnterType.Clockwise;
                            cycleNum = 1;
                            minAngle = Mathf.Min(objLookAtAngle - 180, SceneManager.GenericAngleThreshold);
                            maxAngle = 360 - SceneManager.GenericAngleThreshold;
                            Debug.Log("Generate 14: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

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

                        if (objLookAtAngle <= 180) {
                            // if less than or equal to 180:
                            // 0 to objLookAtAngle in None cycle 0, Flame(clock) cycle 0
                            enterType = RevolvePillar.EnterType.Clockwise;
                            cycleNum = 0;
                            minAngle = 0;
                            maxAngle = objLookAtAngle;
                            Debug.Log("Generate 15: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

                            // (180 + objLookAtAngle) to 360 in None cycle 0
                            enterType = RevolvePillar.EnterType.None;
                            cycleNum = 0;
                            minAngle = 180 + objLookAtAngle;
                            maxAngle = 360;
                            Debug.Log("Generate 16: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

                            // TODO: more cases if more than one max cycle
                        }
                        else {
                            // if greater than 180:
                            // (objLookAngle - 180) to objLookAtAngle in Frost(anti) cycle 0
                            enterType = RevolvePillar.EnterType.Anticlockwise;
                            cycleNum = 0;
                            minAngle = objLookAtAngle - 180;
                            maxAngle = objLookAtAngle;
                            Debug.Log("Generate 17: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

                            // 0 to (objLookAngle - 180) in Frost(anti) cycle 0
                            enterType = RevolvePillar.EnterType.Anticlockwise;
                            cycleNum = 0;
                            minAngle = 0;
                            maxAngle = objLookAtAngle - 180;
                            Debug.Log("Generate 18: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);
                        }
                        break;
                    case RevolvePillar.ZoneType.Frost:
                        // in a frost zone

                        if (objLookAtAngle <= 180) {
                            // if less than or equal to 180:
                            // 0 to objLookAngle in Frost(anti) cycle 0
                            enterType = RevolvePillar.EnterType.Anticlockwise;
                            cycleNum = 0;
                            minAngle = 0;
                            maxAngle = objLookAtAngle;
                            Debug.Log("Generate 19: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

                            /*
                            // (180 + objLookAngle) to 360 in Frost(anti) cycle 0
                            enterType = RevolvePillar.EnterType.Anticlockwise;
                            cycleNum = 0;
                            minAngle = 180 + objLookAtAngle;
                            maxAngle = 360;
                            Debug.Log("Generate 20: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);
                            */

                            // 0 to 360 (since only one max cycle) in Frost(anti) cycle 1
                            enterType = RevolvePillar.EnterType.Anticlockwise;
                            cycleNum = 1;
                            minAngle = 0;
                            maxAngle = 360;
                            Debug.Log("Generate 21: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

                            // TODO: more cases here if more than one max cycle
                        }
                        else {
                            // if greater than 180:
                            // 0 to objLookAngle in Frost(anti) cycle 1
                            enterType = RevolvePillar.EnterType.Anticlockwise;
                            cycleNum = 1;
                            minAngle = 0;
                            maxAngle = objLookAtAngle;
                            Debug.Log("Generate 22: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

                            // TODO: more cases here if more than one max cycle
                        }

                        break;
                    case RevolvePillar.ZoneType.Flame:
                        // in flame zone

                        if (objLookAtAngle <= 180) {
                            // if less than or equal to 180:
                            // m_genericThreshold to objLookAngle in Flame(clock) cycle 1
                            enterType = RevolvePillar.EnterType.Clockwise;
                            cycleNum = 1;
                            minAngle = SceneManager.GenericAngleThreshold;
                            maxAngle = objLookAtAngle;
                            Debug.Log("Generate 23: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

                            // 0 to (180 - objLookAngle) in Flame(clock) cycle 0
                            enterType = RevolvePillar.EnterType.Clockwise;
                            cycleNum = 0;
                            minAngle = 0;
                            maxAngle = 180 - objLookAtAngle;
                            Debug.Log("Generate 24: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

                            // (180 + objLookAngle) to 360 in Flame(clock) cycle 0
                            enterType = RevolvePillar.EnterType.Clockwise;
                            cycleNum = 0;
                            minAngle = 180 + objLookAtAngle;
                            maxAngle = 360;
                            Debug.Log("Generate 25: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

                            // TODO: more cases here if more than one max cycle
                        }
                        else {
                            // if greater than 180:
                            // m_genericThreshold to objLookAngle in Flame(clock) cycle 0
                            enterType = RevolvePillar.EnterType.Clockwise;
                            cycleNum = 0;
                            minAngle = Mathf.Max(objLookAtAngle - 180, SceneManager.GenericAngleThreshold);
                            maxAngle = objLookAtAngle;
                            Debug.Log("Generate 26: min: " + minAngle + " || max: " + maxAngle);


                            newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                            AddData(newData);

                            if (objLookAtAngle - 180 < SceneManager.GenericAngleThreshold) {
                                // (objLookAngle - 180) to m_genericThreshold (or 180) in None cycle 0
                                enterType = RevolvePillar.EnterType.None;
                                cycleNum = 0;
                                minAngle = objLookAtAngle - 180;
                                maxAngle = SceneManager.GenericAngleThreshold;
                                Debug.Log("Generate 27: min: " + minAngle + " || max: " + maxAngle);


                                newData = new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
                                AddData(newData);
                            }
                        }

                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }
}
