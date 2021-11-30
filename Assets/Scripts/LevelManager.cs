using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject
        m_PlayerPrefab,
        m_PassUI,
        m_FailUI,
        m_PauseUI;

    private GameObject
        m_DropDecalPool,
        m_SplatDecalPool,
        m_PlayerGameobject;
    [SerializeField]
    private int
        m_StartDecals,
        m_StartSmallJunk,
        m_StartLargeJunk,
        m_StartChildren,
        m_StartRats,
        m_ThisLevelNumber,
        m_StartTotalPoints,
        lastper = 100;

    [SerializeField]
    private int
        m_CurrentDecals,
        m_CurrentSmallJunk,
        m_CurrentLargeJunk,
        m_CurrentChildren,
        m_CurrentRats;

    [SerializeField]
    private int
        m_PointsDecals = 1,
        m_PointsSmallJunk = 1,
        m_PointsLargeJunk = 10,
        m_PointsChildren = 5,
        m_PointsRats = 12;

    public TextMeshProUGUI text, percenttext, health;

     public float m_TimeLimit = 10f;
     float m_TimeLimitStart;

    public float persentofdirt = 100f;

    public Slider perslide;

    [SerializeField]
    private LayerMask DisposalAreaLayer;

    public TextMeshProUGUI scoretext, reasontext;

    bool once = false, m_Paused;


    // Start is called before the first frame update
    void Start()
    {
        AnalyticsEvent.LevelStart("Level" + m_ThisLevelNumber);


        m_DropDecalPool = GameObject.FindGameObjectWithTag("DropDecalPool");
        m_SplatDecalPool = GameObject.FindGameObjectWithTag("SpaltDecalPool");

        m_TimeLimitStart = m_TimeLimit;

        StartCoroutine(DelayedStart());


        DiscordWebhooks.AddLineToTextFile("Log", "----------------", false);
        DiscordWebhooks.AddLineToTextFile("Log", "LEVEL LOADED");
        DiscordWebhooks.AddLineToTextFile("Log", "----------------", false);


        m_PlayerGameobject = Instantiate(m_PlayerPrefab, transform.GetChild(0).position + Vector3.up, transform.GetChild(0).rotation);
        DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Spawned ", false);

    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForEndOfFrame();
        m_CurrentDecals = m_StartDecals = DecalCheck();
        m_CurrentSmallJunk = m_StartSmallJunk = GameObject.FindGameObjectsWithTag("Dirt.SmallJunk").Length;
        m_CurrentLargeJunk = m_StartLargeJunk = LargeJunkCheck();
        m_CurrentChildren = m_StartChildren = GameObject.FindGameObjectsWithTag("Dirt.Kid").Length;
        m_CurrentRats = m_StartRats = GameObject.FindGameObjectsWithTag("Dirt.Rat").Length;

        m_StartTotalPoints = DoMath();

        DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Caculated Total Dirt: " + m_StartTotalPoints, false);

    }

    public void ButtonNext()
    {
        Time.timeScale = 1f;
        DiscordWebhooks.AddLineToTextFile("Log", "Player Selected NExt Level");
        GameObject.FindGameObjectWithTag("Manager.Game").GetComponent<GameManager>().LoadNewScene(SceneManager.GetActiveScene().buildIndex, SceneManager.GetActiveScene().buildIndex + 1);

    }

    public void ButtonReturnToMenu()
    {
        Time.timeScale = 1f;
        DiscordWebhooks.AddLineToTextFile("Log", "Player Selected to return to menu");
        GameObject.FindGameObjectWithTag("Manager.Game").GetComponent<GameManager>().LoadNewScene(SceneManager.GetActiveScene().buildIndex, 0);


    }

    public void ButtonRestart()
    {
        Time.timeScale = 1f;
        DiscordWebhooks.AddLineToTextFile("Log", "Player Selected to retry level");
        GameObject.FindGameObjectWithTag("Manager.Game").GetComponent<GameManager>().LoadNewScene(SceneManager.GetActiveScene().buildIndex, SceneManager.GetActiveScene().buildIndex);


    }

    void TryFinish()
    {


        if (persentofdirt <= 10.5f)
        {
            DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Selected to leave and passed", false);

            m_PlayerGameobject.SendMessage("AddPauseReason");
            Time.timeScale = 0;
            DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Had Time Left: " + m_TimeLimit, false);
            DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Had Dirt% Left: " + persentofdirt, false);

            AnalyticsEvent.ScreenVisit("LevelPass");
            AnalyticsEvent.LevelComplete("Level" + m_ThisLevelNumber);

            m_PassUI.SetActive(true);
            int score = FinalPointsMath();
            string scoreMessage = score.ToString();
            DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Had Score: " + score, false);

            PlayerPrefs.SetInt("CurrentLevel", m_ThisLevelNumber + 1);
            PlayerPrefs.Save();

            DiscordWebhooks.AddLineToTextFile("Log", "----------------", false);
            DiscordWebhooks.AddLineToTextFile("Log", "LEVEL END");
            DiscordWebhooks.AddLineToTextFile("Log", "----------------", false);

            DiscordWebhooks.AddLineToTextFile("Level" + m_ThisLevelNumber.ToString(), DirtLeftPos(), false);


            scoretext.text = scoreMessage;
        }
        else
        {
            DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Selected to leave and Failed", false);
            DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Had Time Left: " + m_TimeLimit, false);
            DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Had Dirt% Left: " + persentofdirt, false);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_TimeLimit > 0)
        {
            m_TimeLimit -= Time.deltaTime;
            string l_Time = TimeSpan.FromSeconds(m_TimeLimit).ToString();
            l_Time = l_Time.Replace("00:", "");
            l_Time = l_Time.Replace("0000", "");
            text.text = "Time: " + l_Time;
        }

        if (m_TimeLimit <= 0 && !once)
        {
            once = true;
            text.text = "Time: 00.000";
            GameFail("You Ran Out Of Time");
        }
    }

    void GameFail(string a_Reason)
    {
        DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Failed Game: " + a_Reason, false);
        DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Player Had Dirt% Left: " + persentofdirt, false);

        DiscordWebhooks.AddLineToTextFile("Log", "----------------", false);
        DiscordWebhooks.AddLineToTextFile("Log", "LEVEL END");
        DiscordWebhooks.AddLineToTextFile("Log", "----------------", false);

        Time.timeScale = 0f;

        AnalyticsEvent.LevelFail("Level" + m_ThisLevelNumber);
        AnalyticsEvent.ScreenVisit("LevelFail");
        m_FailUI.SetActive(true);
        reasontext.text = a_Reason;
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
        AnalyticsEvent.LevelQuit("Level" + m_ThisLevelNumber);
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

    int DoMath()
    {
        int TotalPoints = 0;
        TotalPoints += (m_CurrentDecals * m_PointsDecals);
        TotalPoints += (m_CurrentSmallJunk * m_PointsSmallJunk);
        TotalPoints += (m_CurrentLargeJunk * m_PointsLargeJunk);
        TotalPoints += (m_CurrentChildren * m_PointsChildren);
        TotalPoints += (m_CurrentRats * m_PointsRats);
        return TotalPoints;
    }

    float DoOtherMaths()
    {
        float per = 100f;

        float worth = 100f / m_StartTotalPoints;

        per = worth * DoMath();

        return per;
    }

    void SetPlayerHealth(int a_NewHealth)
    {
        health.text = "Health: " + a_NewHealth;

        if(a_NewHealth <= 0)
        {
            GameFail("You Ran Out Of Life");
        }
    }

    int FinalPointsMath()
    {
        float cleanpoints = 10000;
        cleanpoints = cleanpoints / 100;
        cleanpoints = cleanpoints * (100 - persentofdirt);
        cleanpoints = cleanpoints / m_TimeLimitStart;
        cleanpoints = cleanpoints * m_TimeLimit;

        return Mathf.RoundToInt(cleanpoints);
    }

    void DoCheck()
    {
        m_CurrentDecals = DecalCheck();
        m_CurrentSmallJunk = GameObject.FindGameObjectsWithTag("Dirt.SmallJunk").Length;
        m_CurrentLargeJunk = LargeJunkCheck();
        m_CurrentChildren = GameObject.FindGameObjectsWithTag("Dirt.Kid").Length;
        m_CurrentRats = GameObject.FindGameObjectsWithTag("Dirt.Rat").Length;

        persentofdirt = DoOtherMaths();

        if (persentofdirt <= lastper - 10 || persentofdirt >= lastper + 10)
        {
            int cleanness = (int)Math.Round(persentofdirt / 10);
            cleanness = cleanness * 10;
            DiscordWebhooks.AddLineToTextFile("Log", TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString() + " | " + "Cleanness% " + cleanness, false);
            lastper = cleanness;
        }


        percenttext.text = ((int)persentofdirt).ToString() + "%";
        perslide.value = persentofdirt / 100;
    }

    int LargeJunkCheck()
    {
        int l_LargeJunkLeft = 0;

        GameObject[] l_LargeJunk = GameObject.FindGameObjectsWithTag("Dirt.DeadBody");
        foreach(GameObject a_Junk in l_LargeJunk)
        {
            if (Physics.OverlapSphere(a_Junk.transform.position, 2, DisposalAreaLayer).Length == 0)
                l_LargeJunkLeft++;
        }

        return l_LargeJunkLeft;
    }

    int DecalCheck()
    {
        int l_DecalsLeft = 0;

        l_DecalsLeft += m_DropDecalPool.GetComponent<ParticleDecalPool>().DecalsLeft();

        l_DecalsLeft += m_SplatDecalPool.GetComponent<ParticleDecalPool>().DecalsLeft();

        return l_DecalsLeft;
    }

    string DirtLeftPos()
    {
        string l_positions = "";
        l_positions = l_positions + m_DropDecalPool.GetComponent<ParticleDecalPool>().DecalLeftPos();
        l_positions = l_positions + m_SplatDecalPool.GetComponent<ParticleDecalPool>().DecalLeftPos();


        GameObject[] l_Temp = GameObject.FindGameObjectsWithTag("Dirt.SmallJunk");
        for (int i = 0; i < l_Temp.Length; i++)
        {
            l_positions = l_positions + l_Temp[i].transform.position.ToString() + "; ";
        }
        l_Temp = GameObject.FindGameObjectsWithTag("Dirt.Kid");
        for (int i = 0; i < l_Temp.Length; i++)
        {
            l_positions = l_positions + l_Temp[i].transform.position.ToString() + "; ";
        }
       l_Temp = GameObject.FindGameObjectsWithTag("Dirt.Rat");
        for (int i = 0; i < l_Temp.Length; i++)
        {
            l_positions = l_positions + l_Temp[i].transform.position.ToString() + "; ";
        }

        GameObject[] l_LargeJunk = GameObject.FindGameObjectsWithTag("Dirt.DeadBody");
        foreach (GameObject a_Junk in l_LargeJunk)
        {
            if (Physics.OverlapSphere(a_Junk.transform.position, 2, DisposalAreaLayer).Length == 0)
                l_positions = l_positions + a_Junk.transform.position.ToString() + "; ";
        }

        return l_positions;
    }

    private void OnDrawGizmos()
    {
        GameObject[] l_LargeJunk = GameObject.FindGameObjectsWithTag("Dirt.DeadBody");
        foreach (GameObject a_Junk in l_LargeJunk)
        {
            Gizmos.DrawWireSphere(a_Junk.transform.position, 2);
        }
    }
}
