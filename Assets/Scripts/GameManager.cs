using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using System;

public class GameManager : MonoBehaviour
{
    public int m_LastScene, m_NewScene;
    private static GameManager m_Instance;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] l_GameManagers = GameObject.FindGameObjectsWithTag("Manager.Game");
        if (l_GameManagers.Length > 1)
        {
            Destroy(this.gameObject);
            m_Instance = null;
        }
        DontDestroyOnLoad(this.gameObject);

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

    private void Awake()
    {
        m_Instance = this;
    }

    static bool WantsToQuit()
    {
        Debug.Log("Player prevented from quitting.");
        m_Instance.StartCoroutine(SubmitFiles());
        return false;
    }

    [RuntimeInitializeOnLoadMethod]
    static void RunOnStart()
    {
        Application.wantsToQuit += WantsToQuit;
    }

    static IEnumerator SubmitFiles()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        DiscordWebhooks.PostToDiscord(a_FileName: "log", a_FileRename: PlayerPrefs.GetString("Username") + AnalyticsSessionInfo.sessionCount);
        yield return new WaitForSecondsRealtime(0.2f);
        Application.wantsToQuit -= WantsToQuit;
        Application.Quit();
    }

}
