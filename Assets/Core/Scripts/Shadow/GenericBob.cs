using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericBob : MonoBehaviour
{
    [SerializeField] private float m_dist;
    [SerializeField] private float m_speed;

    private float m_startX;

    private bool m_leftward;

    void Start() {
        m_startX = this.transform.position.x;
        m_leftward = true;
    }


    // Update is called once per frame
    void Update() {
        if (m_leftward) {
            this.transform.position += Vector3.left * m_speed;
        }
        else {
            this.transform.position += Vector3.right * m_speed;
        }

        if (this.transform.position.x < m_startX - m_dist) {
            this.transform.position = new Vector3(m_startX - m_dist, this.transform.position.y, this.transform.position.z);
            m_leftward = false;
        }
        else if (this.transform.position.x > m_startX + m_dist) {
            this.transform.position = new Vector3(m_startX + m_dist, this.transform.position.y, this.transform.position.z);
            m_leftward = true;
        }
    }
}
