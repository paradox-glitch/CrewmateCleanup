using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class LevelMenus : MonoBehaviour
{
    public GameObject m_LevelsPanelGameobject, m_TheShipPanelGameobject, m_AxisPlanetPanelGameobject, m_OmicronCetiBasePanelGameobject, m_PlayerGameobject;


    void TriggerInteraction()
    {
        FindPlayer();
        m_PlayerGameobject.SendMessage("AddPauseReason");
        TheShipSetUp();
    }

    void TheShipSetUp()
    {
        m_LevelsPanelGameobject.SetActive(true);
        m_TheShipPanelGameobject.SetActive(true);
        m_AxisPlanetPanelGameobject.SetActive(false);
        m_OmicronCetiBasePanelGameobject.SetActive(false);
    }

    public void ButtonClose()
    {
        m_PlayerGameobject.SendMessage("RemovePauseReason");
        m_TheShipPanelGameobject.SetActive(false);
        m_AxisPlanetPanelGameobject.SetActive(false);
        m_OmicronCetiBasePanelGameobject.SetActive(false);
        m_LevelsPanelGameobject.SetActive(false);
    }

    public void LoadLevel(int a_SceneName)
    {
        string l_Payload = DiscordWebhooks.PayloadBuilder(a_Content: "Player Loaded Level: " + a_SceneName);
        DiscordWebhooks.PostToDiscord(a_Payload: l_Payload);

        GameObject.FindGameObjectWithTag("Manager.Game").GetComponent<GameManager>().LoadNewScene(
            SceneManager.GetActiveScene().buildIndex,
            a_SceneName);
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
}
