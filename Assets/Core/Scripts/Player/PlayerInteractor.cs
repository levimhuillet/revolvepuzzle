using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    private GameObject m_currInteractingObj;
    private List<GameObject> m_availableInteractableObjs;
    [SerializeField] private Transform m_carryTransform;

    #region Unity Callbacks

    private void Start() {
        m_currInteractingObj = null;
        m_availableInteractableObjs = new List<GameObject>();
    }

    public void HandleOnTriggerEnter(Collider other) {
        InteractZone interactZone = other.GetComponent<InteractZone>();
        if (interactZone != null) {
            AddAvailableInteractable(other.transform.parent.gameObject);
        }
    }

    public void HandleOnTriggerExit(Collider other) {
        InteractZone interactZone = other.GetComponent<InteractZone>();
        if (interactZone != null) {
            RemoveAvailableInteractable(other.transform.parent.gameObject);
        }
    }

    #endregion // Unity Callbacks

    #region Helper Methods

    public void AddAvailableInteractable(GameObject interactableObj) {
        if (m_availableInteractableObjs.Count == 0) {
            EventManager.OnInteractionsAvailable.Invoke();
        }
        m_availableInteractableObjs.Add(interactableObj);
    }

    public void RemoveAvailableInteractable(GameObject interactableObj) {
        m_availableInteractableObjs.Remove(interactableObj);
        if (m_availableInteractableObjs.Count == 0) {
            EventManager.OnNoMoreInteractions.Invoke();
        }
    }

    #endregion // Helper Methods

    #region Interactions

    public void TryInteract() {
        // if currently interacting with an object, respond accordingly
        if (m_currInteractingObj != null) {
            // drop carried object
            Drop();
        }
        // if no currently interacting object, check if any are avaialble
        else if (m_availableInteractableObjs.Count > 0) {
            PickUp();
        }
        else {
            //Debug.Log("nothing to interact with");
        }
    }

    public void Carry() {
        // TODO: handle cases when object interaction is not to be carried
        if (m_currInteractingObj != null) {
            // TODO: keep object within view
            m_currInteractingObj.transform.position = m_carryTransform.position;
        }
    }

    private void Drop() {
        Interactable interactableToDrop = m_currInteractingObj.GetComponent<Interactable>();
        int dropDist = 2;
        Vector3 restHeightOffset = new Vector3(0, interactableToDrop.GetRestHeightOffset(), 0);
        // TODO: add restHeight from groundPos, not this.transform's y
        // TODO: check for collisions within other objects
        Vector3 dropPos = this.transform.position + this.transform.forward * dropDist + restHeightOffset;
        m_currInteractingObj.transform.position = dropPos;

        ConditionallyActive cActiveToDrop = m_currInteractingObj.GetComponent<ConditionallyActive>();
        cActiveToDrop.HandleDrop();

        // TODO: currently does not pop up interaction prompt when object is dropped
        m_currInteractingObj = null;
    }

    private void PickUp() {
        //Debug.Log("object available to interact with");
        // TODO: select nearest obj
        m_currInteractingObj = m_availableInteractableObjs[0];
        m_availableInteractableObjs.Remove(m_currInteractingObj);
        if (m_availableInteractableObjs.Count == 0) {
            EventManager.OnNoMoreInteractions.Invoke();
        }

        // clear old cActive data
        ConditionallyActive cActiveToDrop = m_currInteractingObj.GetComponent<ConditionallyActive>();
        // should never be null because currently required by Interactable, but to be safe:
        if (cActiveToDrop != null) {
            cActiveToDrop.ClearData();
        }
    }

    #endregion // Interactions
}
