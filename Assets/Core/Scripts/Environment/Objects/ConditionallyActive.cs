using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionallyActive : MonoBehaviour
{
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
    }

    private void SetNewCActiveData() {
        // TODO: this

        // generate key points with each tower
        foreach (RevolvePillar pillar in SceneManager.instance.GetPillars()) {
            // get current object angle and zone (enter type)

            // if player is in original zone, appear
            CATowerData newData = GenerateNewCATowerData(RevolvePillar.EnterType.None, pillar);
            AddData(newData);

            // if player is < 180 and clockwise-ward in a new zone, appear
            newData = GenerateNewCATowerData(RevolvePillar.EnterType.Clockwise, pillar);
            AddData(newData);

            // if player is > 180 and anticlockwise-ward a new zone, appear
            newData = GenerateNewCATowerData(RevolvePillar.EnterType.Anticlockwise, pillar);
            AddData(newData);
        }
    }

    private CATowerData GenerateNewCATowerData(RevolvePillar.EnterType zoneType, RevolvePillar pillar) {
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

        switch (zoneType) {
            case RevolvePillar.EnterType.None:
                // if player is in original zone, appear
                enterType = currEnterType;
                cycleNum = pillar.GetCycleNum();
                maxAngle = 360;
                break;
            case RevolvePillar.EnterType.Clockwise:
                // if player is < 180 and clockwise-ward in a new zone, appear
                if (currEnterType == RevolvePillar.EnterType.Clockwise) {
                    // continue through cycles
                    enterType = currEnterType;
                    cycleNum = pillar.GetCycleNum() + 1;
                    maxAngle = 360;
                }
                else {
                    // check if enough to leave Anticlockwise or None for a new zone
                }

                break;
            case RevolvePillar.EnterType.Anticlockwise:
                // if player is > 180 and anticlockwise-ward a new zone, appear
                if (currEnterType == RevolvePillar.EnterType.Anticlockwise) {
                    // continue through cycles
                    enterType = currEnterType;
                    cycleNum = pillar.GetCycleNum() + 1;
                    maxAngle = 360;
                }
                else {
                    // check if enough to leave Clockwise or None for a new zone
                }


                break;
            default:
                break;
        }

        return new CATowerData(towerID, enterType, cycleNum, passedPrelim, minAngle, maxAngle, clockwiseBounds);
    }
}
