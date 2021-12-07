using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class LevelMenus : MonoBehaviour
{
    public GameObject m_LevelsPanelGameobject, m_TheShipPanelGameobject, m_AxisPlanetPanelGameobject, m_OmicronCetiBasePanelGameobject, m_PlayerGameobject;
    public Button[] buttons;

    private void Start()
    {
        if(!PlayerPrefs.HasKey("CurrentLevel"))
        {
            PlayerPrefs.SetInt("CurrentLevel", 0);
            PlayerPrefs.Save();
        }

        for (int i=0; i <= PlayerPrefs.GetInt("CurrentLevel"); i++)
        {
            buttons[i].interactable = true;
        }
    }


    void TriggerInteraction()
    {
        FindPlayer();
        m_PlayerGameobject.SendMessage("AddPauseReason");
        DiscordWebhooks.AddLineToTextFile("Log", "---");
        DiscordWebhooks.AddLineToTextFile("Log", "Player Opened Levels Menu");
        TheShipSetUp();
    }

    void TheShipSetUp()
    {
        AnalyticsEvent.ScreenVisit("The Ship Levels");
        m_LevelsPanelGameobject.SetActive(true);
        m_TheShipPanelGameobject.SetActive(true);
        m_AxisPlanetPanelGameobject.SetActive(false);
        m_OmicronCetiBasePanelGameobject.SetActive(false);
        DiscordWebhooks.AddLineToTextFile("Log", "Player Is On The Ship Menu");
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
        DiscordWebhooks.AddLineToTextFile("Log", "Player Selected " + a_SceneName  + "to be loaded");

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
