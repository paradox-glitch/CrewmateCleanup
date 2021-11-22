using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    public GameObject m_mainmenu, m_Options, m_Creewmat, m_FeedbackPanelGameobject, m_PlayerGameobject;
    public TextMeshProUGUI m_UserDetails;

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

    public void SetColor(int a_ColorIndex)
    {
        PlayerPrefs.SetInt("CrewmateColor", a_ColorIndex);
        PlayerPrefs.Save();
        m_PlayerGameobject.SendMessage("ChangeColor");
        //string l_Payload = DiscordWebhooks.PayloadBuilder(a_Content: "Player Choose Color: " + m_PlayerGameobject.transform.GetChild(0).GetComponent<Renderer>().material.name);
        //DiscordWebhooks.PostToDiscord(a_Payload: l_Payload);
    }


    void TriggerInteraction()
    {
        
        if(m_PlayerGameobject == null)
        {
            FindPlayer();
            m_UserDetails.text = "Crewmate Name: " + PlayerPrefs.GetString("Username") + "\n" + "Crewmate ID: " + PlayerPrefs.GetString("UserID");
        }
        if(m_PlayerGameobject != null)
        {
            CrewmateSetUp();
            m_PlayerGameobject.SendMessage("AddPauseReason");
        }
    }


    public void CrewmateSetUp()
    {
        m_mainmenu.SetActive(true);
        m_Creewmat.SetActive(true);
        m_Options.SetActive(false);
        m_FeedbackPanelGameobject.SetActive(false);
    }

    public void OptionsSetUp()
    {
        m_mainmenu.SetActive(true);
        m_Creewmat.SetActive(false);
        m_Options.SetActive(true);
        m_FeedbackPanelGameobject.SetActive(false);
    }

    public void FeedbackSetUp()
    {
        m_mainmenu.SetActive(true);
        m_Creewmat.SetActive(false);
        m_Options.SetActive(false);
        m_FeedbackPanelGameobject.SetActive(true);
    }

    public void Close()
    {
        m_PlayerGameobject.SendMessage("RemovePauseReason");
        m_Creewmat.SetActive(false);
        m_Options.SetActive(false);
        m_FeedbackPanelGameobject.SetActive(false);
        m_mainmenu.SetActive(false);
    }


}
