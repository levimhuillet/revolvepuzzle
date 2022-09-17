using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericBob : MonoBehaviour
{
    public enum Axis {
        X,
        Y,
        Z
    }

    [SerializeField] private Axis m_axis;
    [SerializeField] private float m_dist;
    [SerializeField] private float m_speed;

    private float m_startVal;
    private Vector3 m_boundedPos;

    private int m_dir; // -1 for reverse, 1 for forward

    void Start() {
        // start value
        switch (m_axis) {
            case Axis.X:
                m_startVal = this.transform.position.x;
                break;
            case Axis.Y:
                m_startVal = this.transform.position.y;
                break;
            case Axis.Z:
                m_startVal = this.transform.position.z;
                break;
            default:
                break;
        }

        m_dir = 1;
    }


    // Update is called once per frame
    void Update() {
        // apply movement
        Vector3 dirVec = Vector3.zero;
        switch (m_axis) {
            case Axis.X:
                dirVec = Vector3.right;
                break;
            case Axis.Y:
                dirVec = Vector3.up;
                break;
            case Axis.Z:
                dirVec = Vector3.forward;
                break;
            default:
                break;
        }

        if (m_dir == 1) {
            this.transform.Translate(dirVec * m_speed * Time.deltaTime);
        }
        else {
            this.transform.Translate(-dirVec * m_speed * Time.deltaTime);
        }

        // calc bounds
        switch (m_axis) {
            case Axis.X:
                m_boundedPos = new Vector3(m_startVal + (m_dir * m_dist), this.transform.position.y, this.transform.position.z);
                break;
            case Axis.Y:
                m_boundedPos = new Vector3(this.transform.position.x, m_startVal + (m_dir * m_dist), this.transform.position.z);
                break;
            case Axis.Z:
                m_boundedPos = new Vector3(this.transform.position.x, this.transform.position.y, m_startVal + (m_dir * m_dist));
                break;
            default:
                break;
        }

        // enforce bounds
        float checkPos = 0;
        switch (m_axis) {
            case Axis.X:
                checkPos = this.transform.position.x;
                break;
            case Axis.Y:
                checkPos = this.transform.position.y;
                break;
            case Axis.Z:
                checkPos = this.transform.position.z;
                break;
            default:
                break;
        }

        if (checkPos > m_startVal + m_dist) {
            this.transform.SetPositionAndRotation(m_boundedPos, transform.rotation);
            m_dir = -1;
        }
        else if (checkPos < m_startVal - m_dist) {
            this.transform.SetPositionAndRotation(m_boundedPos, transform.rotation);
            m_dir = 1;
        }
    }
}
