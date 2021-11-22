using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject
        m_PlayerPrefab,
        m_PassUI,
        m_FailUI;

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
        m_StartTotalPoints;

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

    bool once = false;


    // Start is called before the first frame update
    void Start()
    {
        m_DropDecalPool = GameObject.FindGameObjectWithTag("DropDecalPool");
        m_SplatDecalPool = GameObject.FindGameObjectWithTag("SpaltDecalPool");

        m_TimeLimitStart = m_TimeLimit;

        StartCoroutine(DelayedStart());


        m_PlayerGameobject = Instantiate(m_PlayerPrefab, transform.GetChild(0).position + Vector3.up, transform.GetChild(0).rotation);
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
    }

    public void ButtonReturnToMenu()
    {
        Time.timeScale = 1f;
        GameObject.FindGameObjectWithTag("Manager.Game").GetComponent<GameManager>().LoadNewScene(2, 0);
    }

    public void ButtonRestart()
    {
        Time.timeScale = 1f;
        GameObject.FindGameObjectWithTag("Manager.Game").GetComponent<GameManager>().LoadNewScene(2,2);
    }

    void TryFinish()
    {
        if (persentofdirt <= 10f)
        {
            Debug.Log("Nice");
            m_PlayerGameobject.SendMessage("AddPauseReason");
            Time.timeScale = 0;
            m_PassUI.SetActive(true);
            int score = FinalPointsMath();
            string scoreMessage = score.ToString();




            scoretext.text = scoreMessage;
        }
        else
        {
            Debug.Log("Too Much Dirt");
            
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
        Time.timeScale = 0f;
        m_FailUI.SetActive(true);
        reasontext.text = a_Reason;
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

    private void OnDrawGizmos()
    {
        GameObject[] l_LargeJunk = GameObject.FindGameObjectsWithTag("Dirt.DeadBody");
        foreach (GameObject a_Junk in l_LargeJunk)
        {
            Gizmos.DrawWireSphere(a_Junk.transform.position, 2);
        }
    }
}
