using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RevolvePillar;

public class ConditionallyActive : MonoBehaviour
{
    [Serializable]
    public struct CATowerData
    {
        public string TowerID;

        public float MinAngle;

        public float MaxAngle;

        public CATowerData(string id, float minAngle, float maxAngle) {
            TowerID = id;

            // swap min and max if backwards
            if (minAngle > maxAngle) {
                float temp = minAngle;
                minAngle = maxAngle;
                maxAngle = temp;
            }
            MinAngle = minAngle;
            MaxAngle = maxAngle;
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

    public void SetData(List<CATowerData> data) {
        m_data = data;
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
            GenerateNewCATowerData(pillar);

            // generate anticlockwise-ward data
            GenerateNewCATowerData(pillar);
        }
    }

    private void GenerateNewCATowerData(RevolvePillar pillar) {
        string towerID = pillar.GetID();

        float minAngle = 0;
        float maxAngle = 0;

        float objLookAtAngle = pillar.GetWoundedness(); // TODO: fix simulationPillar
        CATowerData newData;
        // TODO: test edge case where object is placed on different zone than player's angle
        // may need to project object, then calculate. Otherwise keep object at exact same angle as player
        // TODO: check for prelim zone edge cases

        switch (pillar.GetZoneType()) {
            case RevolvePillar.ZoneType.Generic:
                // in a generic zone

                if (objLookAtAngle <= 180) {
                    // if less than or equal to 180:
                    // objLookAtAngle to 360 in Flame(clock) cycle 0
                    minAngle = objLookAtAngle;
                    maxAngle = 360;
                    Debug.Log("Generate 1: min: " + minAngle + " || max: " + maxAngle);

                    newData = new CATowerData(towerID, minAngle, maxAngle);
                    AddData(newData);

                    // 0 to (180 - objLookAtAngle) in Flame(clock) cycle 0
                    minAngle = 0;
                    maxAngle = 180 - objLookAtAngle;
                    Debug.Log("Generate 2: min: " + minAngle + " || max: " + maxAngle);


                    newData = new CATowerData(towerID, minAngle, maxAngle);
                    AddData(newData);

                    // (180 - objLookAtAngle) to 360 in Flame(clock) cycle 1
                    minAngle = 180 - objLookAtAngle;
                    maxAngle = 360;
                    Debug.Log("Generate 3: min: " + minAngle + " || max: " + maxAngle);


                    newData = new CATowerData(towerID, minAngle, maxAngle);
                    AddData(newData);

                    // TODO: more cases if there are more than one max cycle
                }
                else {
                    // if greater than 180:
                    // objLookAngle to (360 - m_genericThreshold) in Frost(anti) cycle 0
                    minAngle = objLookAtAngle;
                    maxAngle = 360 - SceneManager.GenericAngleThreshold;
                    Debug.Log("Generate 4: min: " + minAngle + " || max: " + maxAngle);


                    newData = new CATowerData(towerID, minAngle, maxAngle);
                    AddData(newData);

                    // (360 - m_genericThreshold) to 360 in None cycle 0
                    minAngle = 360 - SceneManager.GenericAngleThreshold;
                    maxAngle = 360;
                    Debug.Log("Generate 5: min: " + minAngle + " || max: " + maxAngle);


                    newData = new CATowerData(towerID, minAngle, maxAngle);
                    AddData(newData);

                    // 0 to (objLookAngle - 180) in None cycle 0
                    minAngle = 0;
                    maxAngle = objLookAtAngle - 180;
                    Debug.Log("Generate 6: min: " + minAngle + " || max: " + maxAngle);


                    newData = new CATowerData(towerID, minAngle, maxAngle);
                    AddData(newData);
                }

                break;
            case RevolvePillar.ZoneType.Frost:
                // in frost zone

                if (objLookAtAngle <= 180) {
                    // if less than or equal to 180:
                    // objLookAngle to (360 - m_genericThreshold) in Frost(anti) cycle 0
                    minAngle = objLookAtAngle;
                    maxAngle = Mathf.Min(objLookAtAngle + 180, 360 - SceneManager.GenericAngleThreshold);
                    Debug.Log("Generate 7: min: " + minAngle + " || max: " + maxAngle);


                    newData = new CATowerData(towerID, minAngle, maxAngle);
                    AddData(newData);

                    if (objLookAtAngle + 180 > 360 - SceneManager.GenericAngleThreshold) {
                        // (360 - m_genericThreshold) to (objLookAngle + 180) in None cycle 0
                        minAngle = 360 - SceneManager.GenericAngleThreshold;
                        maxAngle = objLookAtAngle + 180;
                        Debug.Log("Generate 8: min: " + minAngle + " || max: " + maxAngle);

                        newData = new CATowerData(towerID, minAngle, maxAngle);
                        AddData(newData);
                    }

                    // TODO: more cases here if more than one max cycle
                }
                else {
                    // if greater than 180:
                    // objLookAngle to (360 - m_genericThreshold) in Frost(anti) cycle 1
                    minAngle = objLookAtAngle;
                    maxAngle = 360 - SceneManager.GenericAngleThreshold;

                    newData = new CATowerData(towerID, minAngle, maxAngle);
                    AddData(newData);

                    // (360 - m_genericThreshold) to 360 in Frost(anti) cycle 0
                    minAngle = 360 - SceneManager.GenericAngleThreshold;
                    maxAngle = 360;
                    Debug.Log("Generate 9: min: " + minAngle + " || max: " + maxAngle);


                    newData = new CATowerData(towerID, minAngle, maxAngle);
                    AddData(newData);

                    // 0 to (objLookAngle - 180) in Frost(anti) cycle 0
                    minAngle = 0;
                    maxAngle = objLookAtAngle - 180;
                    Debug.Log("Generate 10: min: " + minAngle + " || max: " + maxAngle);


                    newData = new CATowerData(towerID, minAngle, maxAngle);
                    AddData(newData);

                    // TODO: more cases here if more than one max cycle
                }

                break;
            case RevolvePillar.ZoneType.Flame:
                // in flame zone

                if (objLookAtAngle <= 180) {
                    // if less than or equal to 180:
                    // objLookAngle to 360 in Flame(clock) cycle 1
                    minAngle = objLookAtAngle;
                    maxAngle = 360;
                    Debug.Log("Generate 11: min: " + minAngle + " || max: " + maxAngle);


                    newData = new CATowerData(towerID, minAngle, maxAngle);
                    AddData(newData);

                    // TODO: more cases here if more than one max cycle
                }
                else {
                    // if greater than 180:
                    // objLookAngle to 360 in Flame(clock) cycle 0
                    minAngle = objLookAtAngle;
                    maxAngle = 360;
                    Debug.Log("Generate 12: min: " + minAngle + " || max: " + maxAngle);


                    newData = new CATowerData(towerID, minAngle, maxAngle);
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
                    minAngle = Mathf.Min(objLookAtAngle - 180, SceneManager.GenericAngleThreshold);
                    maxAngle = 360 - SceneManager.GenericAngleThreshold;
                    Debug.Log("Generate 14: min: " + minAngle + " || max: " + maxAngle);


                    newData = new CATowerData(towerID, minAngle, maxAngle);
                    AddData(newData);

                    // TODO: more cases here if more than one max cycle
                }

                break;
            default:
                break;
        }
    }
}
