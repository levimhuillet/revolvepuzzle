using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConditionallyActive))]
public class Interactable : MonoBehaviour
{
    private ConditionallyActive _cActive;

    private float m_restHeightOffset;
    private List<ConditionallyActive.CATowerData> m_resetData;

    private void Awake() {
        _cActive = this.GetComponent<ConditionallyActive>();
    }

    private void Update() {
        Debug.Log("reset data size: " + m_resetData.Count);
    }

    private void Start() {
        m_restHeightOffset = this.transform.position.y;

        // TODO: DEBUG THIS
        m_resetData = new List<ConditionallyActive.CATowerData>();
        List<ConditionallyActive.CATowerData> resetList = _cActive.GetData();
        foreach (ConditionallyActive.CATowerData data in resetList) {
            m_resetData.Add(data);
        }
    }

    private void OnDestroy() {
        _cActive.ClearData();
        _cActive.SetData(m_resetData);
    }

    public void HandleOnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            //EventManager.OnInteractEnter.Invoke();
        }
    }

    public void HandleOnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            //EventManager.OnInteractExit.Invoke();
        }
    }

    public float GetRestHeightOffset() {
        return m_restHeightOffset;
    }
}
