using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class TutorialManager : MonoBehaviour
{
    public TextMeshProUGUI m_ButtomUIText;
    public GameObject m_PauseUI, m_EndUi, m_SpawnPoint, m_BloodSpawner, m_Bodys, m_DisposalArea, m_SmallJunkSpawner, m_PlayerPrefab, m_MopPrefab, m_GlovesPrefab, m_BrushPrefab;
    GameObject m_PlayerGameobject, m_SplatDecalPool;
    public bool m_PlayerHasLooked, m_PlayerHasMoved, m_PlayerHasMop, m_BloodCleaned, m_PlayerHasGloves, m_PlayerHasBody, m_PlayerInArea, m_PlayerHasBrush, m_TutorialFinsh;
    public LayerMask m_DisposalAreaLayerMask;
    bool m_Paused;

    // Start is called before the first frame update
    void Start()
    {
        DiscordWebhooks.AddLineToTextFile("Log", "----------------", false);
        DiscordWebhooks.AddLineToTextFile("Log", "TUTORIAL LOADED");
        DiscordWebhooks.AddLineToTextFile("Log", "----------------", false);

        AnalyticsEvent.TutorialStart();

        StartCoroutine(Tut());
        m_SplatDecalPool = GameObject.FindGameObjectWithTag("SpaltDecalPool");
    }

    // Update is called once per frame
    void Update()
    {
        if(m_PlayerGameobject != null)
        {
            if(m_PlayerGameobject.transform.rotation.y != 0)
            {
                m_PlayerHasLooked = true;
            }
            if(m_PlayerGameobject.transform.position.x != 10 
            && m_PlayerGameobject.transform.position.z != -4)
            {
                m_PlayerHasMoved = true;
            }
            try
            {
                if (m_PlayerGameobject.transform.GetChild(1).GetChild(0).gameObject.tag == "Pickup.Mop")
                {
                    m_PlayerHasMop = true;
                }
                else
                {
                    m_PlayerHasMop = false;
                }
            }
            catch (Exception e)
            {
                m_PlayerHasMop = false;
            }
            try
            {
                if (m_PlayerGameobject.transform.GetChild(1).GetChild(0).gameObject.tag == "Tool.Gloves")
                {
                    m_PlayerHasGloves = true;
                }
                else
                {
                    m_PlayerHasGloves = false;
                }
            }
            catch (Exception e)
            {
                m_PlayerHasGloves = false;
            }
            try
            {
                if (m_PlayerGameobject.transform.GetChild(1).GetChild(0).gameObject.tag == "Tool.Brush")
                {
                    m_PlayerHasBrush = true;
                }
                else
                {
                    m_PlayerHasBrush = false;
                }
            }
            catch (Exception e)
            {
                m_PlayerHasBrush = false;
            }

            if (m_PlayerGameobject.transform.GetChild(2).GetComponent<ConfigurableJoint>().connectedBody != null)
            {
                m_PlayerHasBody = true;
            }
            else
                m_PlayerHasBody = false;
        }
        if (m_BloodSpawner.activeSelf)
        {
           if(m_SplatDecalPool.GetComponent<ParticleDecalPool>().DecalsLeft() == 0)
            {
                m_BloodCleaned = true;
            }
           else
                m_BloodCleaned = false;
        }

        if(Physics.CheckSphere(m_PlayerGameobject.transform.position, 2, m_DisposalAreaLayerMask))
        {
            m_PlayerInArea = true;
        }
    }

    int LargeJunkCheck()
    {
        int l_LargeJunkLeft = 0;

        GameObject[] l_LargeJunk = GameObject.FindGameObjectsWithTag("Dirt.DeadBody");
        foreach (GameObject a_Junk in l_LargeJunk)
        {
            if (Physics.OverlapSphere(a_Junk.transform.position, 2, m_DisposalAreaLayerMask).Length == 0)
                l_LargeJunkLeft++;
        }

        return l_LargeJunkLeft;
    }

    void KillJunk()
    {
        GameObject[] l_LargeJunk = GameObject.FindGameObjectsWithTag("Dirt.DeadBody");
        foreach (GameObject a_Junk in l_LargeJunk)
        {
            Destroy(a_Junk);
        }
    }

    public void ButtonNext()
    {
        Time.timeScale = 1f;
        GameObject.FindGameObjectWithTag("Manager.Game").GetComponent<GameManager>().LoadNewScene(SceneManager.GetActiveScene().buildIndex, SceneManager.GetActiveScene().buildIndex + 1);
        DiscordWebhooks.AddLineToTextFile("Log", "Player Selected NExt Level");
    }

    public void ButtonReturnToMenu()
    {
        Time.timeScale = 1f;
        GameObject.FindGameObjectWithTag("Manager.Game").GetComponent<GameManager>().LoadNewScene(SceneManager.GetActiveScene().buildIndex, 0);
        DiscordWebhooks.AddLineToTextFile("Log", "Player Selected to return to menu");

    }

    public void ButtonRestart()
    {
        Time.timeScale = 1f;
        GameObject.FindGameObjectWithTag("Manager.Game").GetComponent<GameManager>().LoadNewScene(SceneManager.GetActiveScene().buildIndex, SceneManager.GetActiveScene().buildIndex);
        DiscordWebhooks.AddLineToTextFile("Log", "Player Selected to retry level");

    }


    void TryFinish()
    {
        if (m_TutorialFinsh)
        {
            DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Complete Tutorial", false);
            PlayerPrefs.SetInt("TutorialDone", 1);
            PlayerPrefs.Save();
            AnalyticsEvent.TutorialComplete();
            AnalyticsEvent.ScreenVisit("TutorialSucsess");
            m_EndUi.SetActive(true);
        }
    }

    void OnPause()
    {
        DoPause();
    }

    public void ButtonPause()
    {
        DoPause();
    }

    public void ButtonQuitToMenu()
    {
        Time.timeScale = 1f;
        DiscordWebhooks.AddLineToTextFile("Log", "Player Quit to Menu");
        AnalyticsEvent.TutorialSkip();
        GameObject.FindGameObjectWithTag("Manager.Game").GetComponent<GameManager>().LoadNewScene(SceneManager.GetActiveScene().buildIndex, 0);
    }

    void DoPause()
    {
        if (!m_Paused)
        {
            Time.timeScale = 0f;
            AnalyticsEvent.ScreenVisit("PauseScreen");
            m_Paused = true;
            m_PauseUI.SetActive(true);
            m_PlayerGameobject.SendMessage("AddPauseReason");
            DiscordWebhooks.AddLineToTextFile("Log", "GamePaused");
        }
        else
        {
            Time.timeScale = 1f;
            m_Paused = false;
            m_PauseUI.SetActive(false);
            m_PlayerGameobject.SendMessage("RemovePauseReason");
            DiscordWebhooks.AddLineToTextFile("Log", "Game UnPaused");
        }
    }


    IEnumerator Tut()
    {
        m_ButtomUIText.text = "Welcome Crewmate";
        yield return new WaitForSeconds(0.5f);
        m_PlayerGameobject = Instantiate(m_PlayerPrefab, transform.GetChild(0).position + Vector3.up, transform.GetChild(0).rotation);
        yield return new WaitForSeconds(2f);

        m_ButtomUIText.text = "Use Your Mouse To Look Around";
        yield return new WaitUntil(() => m_PlayerHasLooked == true);
        DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Learnt Look", false);
        AnalyticsEvent.TutorialStep(1);

        yield return new WaitForSeconds(2f);

        m_ButtomUIText.text = "Use WASD To Move Around";
        yield return new WaitUntil(() => m_PlayerHasMoved == true);
        DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Learnt Move", false);
        AnalyticsEvent.TutorialStep(2);
        yield return new WaitForSeconds(2f);

        m_ButtomUIText.text = "Go PickUp The Mop Using LMB";
        Instantiate(m_MopPrefab, m_SpawnPoint.transform.position, transform.rotation);
        yield return new WaitUntil(() => m_PlayerHasMop == true);
        DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Learnt PickUP", false);
        AnalyticsEvent.TutorialStep(3);
        yield return new WaitForSeconds(2f);

        m_ButtomUIText.text = "Clean Up The Blood Using LMB";
        m_BloodSpawner.SetActive(true);
        yield return new WaitUntil(() => m_BloodCleaned == true);
        DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Learnt Clean", false);
        AnalyticsEvent.TutorialStep(4);
        yield return new WaitForSeconds(2f);

        m_ButtomUIText.text = "Drop The Mop With E or RMB";
        m_BloodSpawner.SetActive(false);
        yield return new WaitUntil(() => m_PlayerHasMop == false);
        DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Learnt Drop", false);
        AnalyticsEvent.TutorialStep(5);
        yield return new WaitForSeconds(2f);

        m_ButtomUIText.text = "Pick Up The Gloves";
        Instantiate(m_GlovesPrefab, m_SpawnPoint.transform.position, transform.rotation);
        yield return new WaitUntil(() => m_PlayerHasGloves == true);
        DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Learnt PickUp (No Key Prompt)", false);
        AnalyticsEvent.TutorialStep(6);
        yield return new WaitForSeconds(2f);

        m_ButtomUIText.text = "Grab a Body Using LMB";
        m_Bodys.SetActive(true);
        yield return new WaitUntil(() => m_PlayerHasBody == true);
        DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Learnt Grab", false);
        AnalyticsEvent.TutorialStep(7);
        yield return new WaitForSeconds(2f);

        m_ButtomUIText.text = "Go To the Yellow and Grey Disposal Area";
        m_DisposalArea.SetActive(true);
        yield return new WaitUntil(() => m_PlayerInArea == true);
        DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Discoved Dispposal area", false);
        AnalyticsEvent.TutorialStep(8);
        yield return new WaitForSeconds(2f);

        m_ButtomUIText.text = "Drop The Body Using LMB";
        yield return new WaitUntil(() => m_PlayerHasBody == false);
        DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Learnt Stop Drag", false);
        AnalyticsEvent.TutorialStep(9);
        yield return new WaitForSeconds(2f);

        m_ButtomUIText.text = "Place All The Body Parts In The Disposal Area";
        yield return new WaitUntil(() => LargeJunkCheck() == 0);
        DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Learnt How To Dispose", false);
        AnalyticsEvent.TutorialStep(10);
        yield return new WaitForSeconds(2f);

        m_ButtomUIText.text = "Drop The Gloves";
        KillJunk();
        yield return new WaitUntil(() => m_PlayerHasGloves == false);
        DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Learnt Drop (No Key Prombt)", false);
        AnalyticsEvent.TutorialStep(11);
        yield return new WaitForSeconds(2f);

        m_ButtomUIText.text = "PickUp The Brush";
        Instantiate(m_BrushPrefab, m_SpawnPoint.transform.position, transform.rotation);
        yield return new WaitUntil(() => m_PlayerHasBrush == true);
        DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Learnt PickUp (No Key Propmpt)", false);
        AnalyticsEvent.TutorialStep(12);
        yield return new WaitForSeconds(2f);

        m_ButtomUIText.text = "Clean Up Trash";
        m_SmallJunkSpawner.SetActive(true);
        yield return new WaitForSeconds(5f);
        m_ButtomUIText.text = "Cleaning Up Trash Makes Bags, Use Gloves To Dispose Of Them";
        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Dirt.SmallJunk").Length == 0);
        yield return new WaitUntil(() => LargeJunkCheck() == 0);
        DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Leart about small trash", false);
        AnalyticsEvent.TutorialStep(13);
        yield return new WaitForSeconds(2f);

        m_ButtomUIText.text = "Go and LMB Click on the Laptop";
        m_TutorialFinsh = true;
    }
}
