using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    private Transform m_PlayerTransform;

    private Vector3
        m_CameraToPlayerOffset,
        m_Velocity = Vector3.zero;

    [SerializeField]
    private float
        m_MaxSpeed = 2000,
        m_SmoothTime = 1;

    void Start()
    {
        m_PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_CameraToPlayerOffset = m_PlayerTransform.position + this.transform.position;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, (m_PlayerTransform.position + m_CameraToPlayerOffset), ref m_Velocity, m_SmoothTime, m_MaxSpeed, Time.deltaTime);
    }
}
