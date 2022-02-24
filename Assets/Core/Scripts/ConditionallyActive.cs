using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionallyActive : MonoBehaviour
{
    [Serializable]
    public struct CATowerData {
        public string TowerID;
        
        public CentralCamera.EnterType EnterType;

        public int CycleNum;

        public int PassedPrelim; // 0 or 1

        public float MinAngle;

        public float MaxAngle;

    }

    public bool MarkedActive { get; set; }

    [SerializeField]
    private CATowerData[] m_data;

    public CATowerData[] GetData() {
        return m_data;
    }
}
