using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Analytics;
using TMPro;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


public class LevelMenus : MonoBehaviour
{
    public GameObject m_LevelsPanelGameobject, m_TheShipPanelGameobject, m_AxisPlanetPanelGameobject, m_OmicronCetiBasePanelGameobject, m_PlayerGameobject;
    public Button[] buttons;
    public TextMeshProUGUI[] ggggg;

    private void Start()
    {
        if(!PlayerPrefs.HasKey("CurrentLevel"))
        {
            PlayerPrefs.SetInt("CurrentLevel", 1);
            PlayerPrefs.Save();
        }

        for (int i=0; i <= PlayerPrefs.GetInt("CurrentLevel") - 1 && i <= 3 - 1; i++)
        {
            Debug.Log(i + "hiid");
            buttons[i].interactable = true;

            string l_FilePath = Application.persistentDataPath + "/ScoreboardData0" + (101 + i).ToString() + ".csv";

            int test = GetPlace(l_FilePath);

            ggggg[i].text = (ggggg[i].text.Split('\n'))[0] + "\n\nHighScore: " + PlayerPrefs.GetInt("TheShip" + (101 + i).ToString() + "HighScore") + "\nGlobal Position: " + test.DisplayWithSuffix();
        }
    }

    int GetPlace(string a_FilePath)
    {
        int li_Line = 1;
        if (!File.Exists(a_FilePath))
        {
            Debug.Log(a_FilePath);
            return 0;
        }


        foreach (string line in File.ReadLines(a_FilePath))
        {

            try
            {
                if ((line.Split(','))[1] == PlayerPrefs.GetString("UserID"))
                {
                    return li_Line;
                }
            }
            catch (Exception e)
            {
                return 0;
            }

            li_Line++;
        }
        return 0;
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
