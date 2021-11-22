using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CamFollow : MonoBehaviour
{
    private Transform m_PlayerTransform;

    [SerializeField]
    private Vector3
        m_CameraToPlayerOffset,
        m_Velocity = Vector3.zero;

    [SerializeField]
    private float
        m_MaxSpeed = 2000,
        m_SmoothTime = 1;

    void Start()
    {
        m_CameraToPlayerOffset = Vector3.zero + this.transform.position;
        FindPlayer();
        
    }

    void FindPlayer()
    {

        try
        {
            m_PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        catch (Exception e)
        {
            Debug.LogWarning("<color=#7fd6fd><size=12>Camera Follow | </size></color><color=#ED4245><b><size=12>Failed to Find Player: </size></b></color> \n<color=#FEE75C><i><size=10>" + e.Message + "</size></i></color>");
            return;
        }

        
    }

    private void LateUpdate()
    {
        if(m_PlayerTransform == null)
        {
            FindPlayer();
            return;
        }

        transform.position = Vector3.SmoothDamp(transform.position, (m_PlayerTransform.position + m_CameraToPlayerOffset), ref m_Velocity, m_SmoothTime, m_MaxSpeed, Time.deltaTime);
    }
}
