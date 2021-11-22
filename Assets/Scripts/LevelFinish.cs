using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelFinish : MonoBehaviour
{
    GameObject m_LevelManagerGameobject;

    void TriggerInteraction()
    {
        m_LevelManagerGameobject = GameObject.FindGameObjectWithTag("Manager.Level");
        m_LevelManagerGameobject.SendMessage("TryFinish");
    }
}
