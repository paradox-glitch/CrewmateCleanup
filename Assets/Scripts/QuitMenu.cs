using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using System;

public class QuitMenu : MonoBehaviour
{
    public GameObject m_PanelQuitGameobject, m_PlayerGameobject;

    void TriggerInteraction()
    {
        AnalyticsEvent.ScreenVisit("Exit Game");
        m_PanelQuitGameobject.SetActive(true);
        FindPlayer();
        m_PlayerGameobject.SendMessage("AddPauseReason");
    }

    void FindPlayer()
    {
        try
        {
            m_PlayerGameobject = GameObject.FindGameObjectWithTag("Player");
        }
        catch (Exception e)
        {
            Debug.LogWarning("<color=#5865F2><size=12>Camera Follow | </size></color><color=#ED4245><b><size=12>Failed to Find Player: </size></b></color> \n<color=#FEE75C><i><size=10>" + e.Message + "</size></i></color>");
            return;
        }
    }

    public void ButtonYes()
    {
        Application.Quit();
    }

    public void ButtonNo()
    {
        m_PlayerGameobject.SendMessage("RemovePauseReason");
        m_PanelQuitGameobject.SetActive(false);
    }
}