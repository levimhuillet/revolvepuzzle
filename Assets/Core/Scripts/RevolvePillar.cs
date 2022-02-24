using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolvePillar : MonoBehaviour
{
    public static RevolvePillar instance;

    [SerializeField]
    private string m_id;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (this != instance) {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPostition = new Vector3(Player.instance.transform.position.x,
                                       this.transform.position.y,
                                       Player.instance.transform.position.z);
        this.transform.LookAt(targetPostition);
    }

    public float GetEulerRotation() {
        return this.transform.localRotation.eulerAngles.y;
    }

    public string GetID() {
        return m_id;
    }
}
