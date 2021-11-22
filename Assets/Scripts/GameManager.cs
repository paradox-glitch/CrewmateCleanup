using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public int m_LastScene, m_NewScene;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] l_GameManagers = GameObject.FindGameObjectsWithTag("Manager.Game");
        if (l_GameManagers.Length > 1)
            Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
    }

    void OnApplicationQuit()
    {
        DiscordWebhooks.EasyPlainText("AppExit");
    }

    public void LoadNewScene(int a_CurrentScene, int a_NewScene)
    {
        m_LastScene = a_CurrentScene;
        m_NewScene = a_NewScene;
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }


}
