using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Analytics;
using TMPro;

public class LevelLoader : MonoBehaviour
{
    AsyncOperation m_UnloadOperation, m_PreloadOperation;
    GameManager m_GameManager;
    public Slider m_ProgessBar;
    public TextMeshProUGUI m_Title, m_Description;
    int m_OldScene, m_NewScene;
    float m_UnloadProgress, m_LoadProgress, m_TotalProgress, m_TotalDefinedProgress;

    // Start is called before the first frame update
    void Start()
    {
        AnalyticsEvent.ScreenVisit("Loading Screen");

        m_GameManager = GameObject.FindGameObjectWithTag("Manager.Game").GetComponent<GameManager>();
        m_OldScene = m_GameManager.m_LastScene;
        m_NewScene = m_GameManager.m_NewScene;
        m_Title.text = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(m_NewScene)); ;
        StartCoroutine(SceneUnloadAndLoad());
    }

    // Update is called once per frame
    void Update()
    {
        if(m_UnloadOperation != null)
        m_UnloadProgress = Mathf.Clamp01(m_UnloadOperation.progress / 0.9f);

        if(m_PreloadOperation != null)
        m_LoadProgress = Mathf.Clamp01(m_PreloadOperation.progress / 0.9f);

        m_UnloadProgress = (m_UnloadProgress / 10f) * 4f;
        m_LoadProgress = (m_LoadProgress / 10f) * 4f;

        m_TotalProgress = m_TotalDefinedProgress + m_LoadProgress + m_UnloadProgress;

        m_ProgessBar.value = m_TotalProgress;

        if(m_TotalProgress >= 1)
        {
            m_PreloadOperation.allowSceneActivation = true;
        }

    }

    IEnumerator SceneUnloadAndLoad()
    {
        m_Description.text = "Setting Active Scene...";
        yield return new WaitForEndOfFrame();
        SceneManager.SetActiveScene(SceneManager.GetActiveScene());
        m_TotalDefinedProgress += 0.1f;

        DiscordWebhooks.AddLineToTextFile("Log", "---");
        DiscordWebhooks.AddLineToTextFile("Log", "Unloaded: " + m_OldScene);
        m_Description.text = "Unloading Scene...";
        yield return new WaitForEndOfFrame();
        try { m_UnloadOperation = SceneManager.UnloadSceneAsync(m_OldScene); }
        catch (Exception e) { m_TotalDefinedProgress += 0.4f; };

        m_Description.text = "Unloading Assets...";
        yield return new WaitForEndOfFrame();
        Resources.UnloadUnusedAssets();
        m_TotalDefinedProgress += 0.1f;

        m_Description.text = "Loading Scene...";
        yield return new WaitForEndOfFrame();
        m_PreloadOperation = SceneManager.LoadSceneAsync(m_NewScene, LoadSceneMode.Single);
        DiscordWebhooks.AddLineToTextFile("Log", "Loaded: " + m_NewScene);
        DiscordWebhooks.AddLineToTextFile("Log", "---");
        m_PreloadOperation.allowSceneActivation = false;
    }
}
