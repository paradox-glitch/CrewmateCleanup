using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using System;

public class GameManager : MonoBehaviour
{
    public int m_LastScene, m_NewScene;
    private GameManager m_Instance;

    // Start is called before the first frame update
    void Awake()
    {
        GameObject[] l_GameManagers = GameObject.FindGameObjectsWithTag("Manager.Game");
        if (l_GameManagers.Length > 1)
        {
            m_Instance = null;
            Destroy(this.gameObject);
            Debug.Log("Nuke");
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        m_Instance = this;
        Application.wantsToQuit += WantsToQuit;

        DiscordWebhooks.ClearTextFile("Log");
        DiscordWebhooks.AddLineToTextFile("Log", "Game Started");
        DiscordWebhooks.AddLineToTextFile("Log", "App version is: " + Application.version);
    }

    void OnApplicationQuit()
    {
       
    }

    public void LoadNewScene(int a_CurrentScene, int a_NewScene)
    {
        m_LastScene = a_CurrentScene;
        m_NewScene = a_NewScene;
        SceneManager.LoadScene(1, LoadSceneMode.Single);
        
    }

    //private void Awake()
    //{
    //    m_Instance = this;
    //}

     bool WantsToQuit()
    {
        Debug.Log("Player prevented from quitting.");
        m_Instance.StartCoroutine(SubmitFiles());
        return false;
    }

    //[RuntimeInitializeOnLoadMethod]
    // static void RunOnStart()
    //{
    //    Application.wantsToQuit += WantsToQuit;
    //}

     IEnumerator SubmitFiles()
    {
        string pay = DiscordWebhooks.PayloadBuilder(a_Username: PlayerPrefs.GetString("Username") + " | " + PlayerPrefs.GetString("UserID"), a_Content: "Gameplay File:");
        DiscordWebhooks.PostToDiscord(a_FileName: "log", a_FileRename: PlayerPrefs.GetString("Username") + AnalyticsSessionInfo.sessionCount, a_Payload: pay);
        yield return new WaitForSecondsRealtime(0.2f);
        Application.wantsToQuit -= WantsToQuit;
        Application.Quit();
    }

}
