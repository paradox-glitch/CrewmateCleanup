using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using System;
using System.IO;

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
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        m_Instance = this;
        Application.wantsToQuit += WantsToQuit;

        DiscordWebhooks.ClearTextFile("Level1");
        DiscordWebhooks.AddLineToTextFile("Level1", "", false);
        DiscordWebhooks.ClearTextFile("Level2");
        DiscordWebhooks.AddLineToTextFile("Level2", "", false);
        DiscordWebhooks.ClearTextFile("Level3");
        DiscordWebhooks.AddLineToTextFile("Level3", "", false);
        DiscordWebhooks.ClearTextFile("Log");
        DiscordWebhooks.AddLineToTextFile("Log", "Game Started");
        DiscordWebhooks.AddLineToTextFile("Log", "App version is: " + Application.version);

    }

    void OnScreenShot()
    {
        System.DateTime l_EPOCH = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        int l_UNIX = (int)(System.DateTime.UtcNow - l_EPOCH).TotalSeconds;
        string l_Name = "SS-" + l_UNIX;
        DiscordWebhooks.TakeScreenShot(l_Name);
        DiscordWebhooks.AddLineToTextFile("Log", "!!!!!!!!!");
        DiscordWebhooks.AddLineToTextFile("Log", "SCREENSHOT");
        DiscordWebhooks.AddLineToTextFile("Log", "!!!!!!!!!");

        StartCoroutine(WaitForScreenShot(l_Name));
    }

    IEnumerator WaitForScreenShot(string a_FileName)
    {
        yield return new WaitForSeconds(0.1f);

        string pay = DiscordWebhooks.PayloadBuilder(a_Username: PlayerPrefs.GetString("Username") + " | " + PlayerPrefs.GetString("UserID"));

        if (File.Exists(Application.persistentDataPath + "/" + a_FileName + ".png"))
            DiscordWebhooks.PostToDiscord(a_FileName: a_FileName, a_FileType: ".png", a_Payload: pay);
        else
            StartCoroutine(WaitForScreenShot(a_FileName));
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
        string embed = DiscordWebhooks.EmbedBuilder(Color.green, "Player Ended The Game", PlayerPrefs.GetString("Username") + " | " + PlayerPrefs.GetString("UserID"));
        embed = DiscordWebhooks.EmbedsConstructor(embed);

        string pay = DiscordWebhooks.PayloadBuilder(a_Username: PlayerPrefs.GetString("Username") + " | " + PlayerPrefs.GetString("UserID"), a_Content: "Gameplay Files:", a_Embeds: embed); 
        bool scucs = false;
        DiscordWebhooks.PostToDiscordWithSucsess(out scucs, a_FileName: "log", a_File2Name: "Level1", a_File3Name: "Level2", a_File4Name: "Level3", a_Payload: pay);
        yield return WaitUntilTrue(scucs);
        Application.wantsToQuit -= WantsToQuit;
        Application.Quit();
    }

    public IEnumerator WaitUntilTrue(bool checkMethod)
    {
        while (checkMethod == false)
        {
            yield return null;
        }
    }

}
