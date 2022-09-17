using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Hanging : MonoBehaviour
{
    [SerializeField] private float m_minY;

    private Rigidbody m_rb;
    
    // Start is called before the first frame update
    void Start()
    {
        m_rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.y < m_minY) {
            this.transform.position = new Vector3(transform.position.x, m_minY, transform.position.z);
            m_rb.velocity = new Vector3(0, 0, 0);
        }

    }
}
