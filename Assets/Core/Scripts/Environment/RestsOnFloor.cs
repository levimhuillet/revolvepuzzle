using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestsOnFloor : MonoBehaviour
{
    private Floor m_restingFloor;

    // Start is called before the first frame update
    void Start()
    {
        m_restingFloor = Floor.Instance;

        this.transform.SetParent(m_restingFloor.gameObject.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
