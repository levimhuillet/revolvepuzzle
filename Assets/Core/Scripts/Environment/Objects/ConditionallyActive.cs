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


        minAngle = objLookAtAngle - 180;
        maxAngle = objLookAtAngle + 180;

        switch (pillar.GetZoneType()) {
            case RevolvePillar.ZoneType.Generic:
                // do nothing additional
                break;
            case RevolvePillar.ZoneType.Frost:
                // extend into frost
                minAngle -= 360;
                break;
            case RevolvePillar.ZoneType.Flame:
                // extend into flame
                maxAngle += 360;
                break;
            default:
                break;
        }

        Debug.Log("Generate 1: min: " + minAngle + " || max: " + maxAngle);

        newData = new CATowerData(towerID, minAngle, maxAngle);
        AddData(newData);
    }
}
