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

    [SerializeField] private bool m_isPervasive;

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

    public bool IsPervasive() {
        return m_isPervasive;
    }

    public void SetPervasive(bool isPervasive) {
        // change material?
        m_isPervasive = isPervasive;
    }

    public void HandleDrop() {
        // set cActive data
        SetNewCActiveData();

        // set appropriate layer for the current zone
    }

    private void SetNewCActiveData() {

        // generate key points with each tower
        foreach (RevolvePillar pillar in SceneManager.instance.GetPillars()) {
            // generate new data
            GenerateNewCATowerData(pillar);
        }
    }

    private void GenerateNewCATowerData(RevolvePillar pillar) {
        string towerID = pillar.GetID();

        float minAngle = 0;
        float maxAngle = 0;

        float objLookAtAngle = pillar.GetWoundedness(); // TODO: fix simulationPillar
        CATowerData newData;

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

        newData = new CATowerData(towerID, minAngle, maxAngle);
        AddData(newData);
    }
}
