using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using TMPro;

using _SDiag = System.Diagnostics;


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System;

public class MainMenu : MonoBehaviour
{

    private const string scoreURL = "https://crewmatecleanup.pdox.uk/highscore.php";

    [Header("Consent")]
    [SerializeField] private GameObject _consentPanel;
    [SerializeField] private Scrollbar _policyScroll;
    [SerializeField] private Toggle _consentCheck;
    [SerializeField] private Button _consentNext;

    [Header("Login")]
    [SerializeField] private TMP_InputField _usernameInput;
    [SerializeField] private GameObject _loginPanel, _discordManager, m_PatchNotesPannel;
    [SerializeField] private Button _discordButton, _retryButton, _guestButton;
    [SerializeField] private TextMeshProUGUI _discordErrorText, _guestErrorText;

    [Header("MainMenu")]
    [SerializeField] private GameObject _mainMenuPanel, m_PlayerPrefab;

    public GameObject[] questions = new GameObject[8];
    public GameObject ddd;
    bool m_HasLogin = false;

    DiscordController m_DiscordController;

    private void Awake()
    {
        Analytics.enabled = true;

        //PlayerPrefs.DeleteAll();

        if (!PlayerPrefs.HasKey("Username"))
        {

            DiscordWebhooks.AddLineToTextFile("Log", "No Previous User LogOn Detected");
            ConsentSetup();
        }
        else
        {
            DiscordWebhooks.AddLineToTextFile("Log", "Previous User LogOn Detected, Reirected user past data Consent and sign in");

            //* DATA CLEAN ON NEW VERSION

            if (PlayerPrefs.HasKey("LastPlayedVersion"))
            {
                if(PlayerPrefs.GetString("LastPlayedVersion") != Application.version)
                {
                    //Clean Data
                    Debug.Log("Clean Data - Wrong Version");

                    if (PlayerPrefs.GetString("LastPlayedVersion") != Application.version)
                    {
                        if (PlayerPrefs.HasKey("TheShip101HighScore"))
                            ScoreboardConnection.PostScoreWithSucsess("0101", PlayerPrefs.GetInt("TheShip101HighScore"), out _);
                        if (PlayerPrefs.HasKey("TheShip102HighScore"))
                            ScoreboardConnection.PostScoreWithSucsess("0102", PlayerPrefs.GetInt("TheShip102HighScore"), out _);
                        if (PlayerPrefs.HasKey("TheShip103HighScore"))
                            ScoreboardConnection.PostScoreWithSucsess("0103", PlayerPrefs.GetInt("TheShip103HighScore"), out _);
                    }




                        CleanDataAndShowPatchNotes();
                }
                else
                {
                    Debug.Log("No Clean Data - Matched Version");

                }
            }
            else
            {
                if (AnalyticsSessionInfo.sessionFirstRun)
                {
                    Debug.Log("No Clean Data - First Version");
                }
                else
                {

                    //* Fix for V0.0.1 

                    //* Clean Data
                    Debug.Log("Clean Data - No Version");

                    CleanDataAndShowPatchNotes();
                }

            }

            MenuSetup();





        }
        PlayerPrefs.SetString("LastPlayedVersion", (string)Application.version);
        PlayerPrefs.Save();

        Instantiate(_discordManager);
    }

    void CleanDataAndShowPatchNotes()
    {
        PlayerPrefs.DeleteKey("CurrentLevel");

        m_PatchNotesPannel.SetActive(true);
    }

    public void ButtonCloseNotes()
    {
        m_PatchNotesPannel.SetActive(false);
    }

    public void DidScroll()
    {
        if (_policyScroll.value <= 0f)
        {
            _consentCheck.interactable = true;
        }
    }

    public void DidCheck()
    {
        DiscordWebhooks.AddLineToTextFile("Log", "User Consented To Data Collection");
        _consentNext.interactable = _consentCheck.isOn;
    }

    void UsernameCheck()
    {
        if (_usernameInput.text.Length < 3)
        {
            _guestButton.interactable = false;
            _guestErrorText.gameObject.SetActive(true);
        }
        else
        {
            _guestButton.interactable = true;
            _guestErrorText.gameObject.SetActive(false);
        }
    }

    void ConsentSetup()
    {
        AnalyticsEvent.ScreenVisit("Consent Form");
        _loginPanel.SetActive(false);
        _mainMenuPanel.SetActive(false);
        _consentPanel.SetActive(true);
        PlayerPrefs.SetInt("CrewmateColor", 1);
        PlayerPrefs.Save();
    }

    void MenuSetup()
    {
        DiscordWebhooks.AddLineToTextFile("Log", "---");
            DiscordWebhooks.AddLineToTextFile("Log", "Username: " + PlayerPrefs.GetString("Username"));
            DiscordWebhooks.AddLineToTextFile("Log", "UserID: " + PlayerPrefs.GetString("UserID"));
            DiscordWebhooks.AddLineToTextFile("Log", "Session Number: " + AnalyticsSessionInfo.sessionCount);
            DiscordWebhooks.AddLineToTextFile("Log", "Session ID: " + AnalyticsSessionInfo.sessionId);
            DiscordWebhooks.AddLineToTextFile("Log", "Analytics UserID: " + AnalyticsSessionInfo.userId);
            if(PlayerPrefs.HasKey("TutorialDone"))
            DiscordWebhooks.AddLineToTextFile("Log", "Tutorial Done: Yes");
            else
            DiscordWebhooks.AddLineToTextFile("Log", "Tutorial Done: No");

        DiscordWebhooks.AddLineToTextFile("Log", "---");


        string embed = DiscordWebhooks.EmbedBuilder(Color.blue, "Player Started The Game", PlayerPrefs.GetString("Username") + " | " + PlayerPrefs.GetString("UserID"));
        embed = DiscordWebhooks.EmbedsConstructor(embed);

        string pay = DiscordWebhooks.PayloadBuilder(a_Username: PlayerPrefs.GetString("Username") + " | " + PlayerPrefs.GetString("UserID"), a_Embeds: embed);
        DiscordWebhooks.PostToDiscord(a_Payload: pay);


        _consentPanel.SetActive(false);
        _loginPanel.SetActive(false);
        _mainMenuPanel.SetActive(false);

        Instantiate(m_PlayerPrefab, transform.GetChild(0).transform.position + Vector3.up, transform.GetChild(0).transform.rotation);
    }

    IEnumerator WaitForLogin()
    {
        Debug.Log("waiting");
        yield return new WaitUntil(() => m_HasLogin == true);
        Debug.Log("done");
        StartCoroutine(test());
    }

    public void LoginSetup()
    {
        AnalyticsEvent.ScreenVisit("Login");

        m_HasLogin = false;
        _consentPanel.SetActive(false);
        _mainMenuPanel.SetActive(false);
        _loginPanel.SetActive(true);
        bool _discordRunning = false;
        _discordRunning = DiscordRunning();
        Debug.Log("discord running = " + _discordRunning);
        _discordButton.interactable = _discordRunning;
        _retryButton.interactable = !_discordRunning;
        _discordErrorText.gameObject.SetActive(!_discordRunning);
        StartCoroutine(WaitForLogin());
    }

    void DiscordLoginSuccsess()
    {
        DiscordWebhooks.AddLineToTextFile("Log", "User Login With Discord");
        AnalyticsEvent.UserSignup(new AuthorizationNetwork());
        m_HasLogin = true;
    }

    IEnumerator test()
    {
        string _userID = PlayerPrefs.GetString("UserID");
        string _userName = PlayerPrefs.GetString("Username");

        Debug.Log("UserID - " + _userID + " Username - " + _userName);

        AnalyticsResult testv = AnalyticsEvent.Custom("userLogin", new Dictionary<string, object>
        {
            { "user_name", _userName },
            { "user_id", _userID }
        });

        if (testv != AnalyticsResult.Ok)
            Debug.Log("fuck shit bad");

        Analytics.CustomEvent("userLogin");

        GameObject.FindGameObjectWithTag("Manager.Game").GetComponent<GameManager>().LoadNewScene(
    0,
    2);

        yield return null;
    }

    public void DiscordLogin()
    {
        GameObject.FindGameObjectWithTag("DiscordManager").SendMessage("Login");
    }

    public void RetryDiscord()
    {
        LoginSetup();
    }

    public void NormalLogin()
    {
        if (_usernameInput.text.Length < 3)
            return;

        PlayerPrefs.SetString("UserID", "" + System.Guid.NewGuid());
        PlayerPrefs.SetString("Username", _usernameInput.text);
        PlayerPrefs.Save();

        DiscordWebhooks.AddLineToTextFile("Log", "User Playing as Guest");


        m_HasLogin = true;
    }

    public void ButtonLogOut()
    {
        PlayerPrefs.DeleteAll();
        DiscordWebhooks.AddLineToTextFile("Log", "User Logout");
        GameObject.FindGameObjectWithTag("Manager.Game").GetComponent<GameManager>().LoadNewScene(
    0,
    0);
    }

    public void SubmitForm()
    {
        string[] feilds = new string[8];

        for(int i = 0; i < questions.Length; i++)
        {
            string l_text = "N/A";
            if(questions[i].transform.GetChild(0).gameObject.GetComponent<TMP_InputField>().text != "" && questions[i].transform.GetChild(0).gameObject.GetComponent<TMP_InputField>().text != null && questions[i].transform.GetChild(0).gameObject.GetComponent<TMP_InputField>().text != " ")
            {
                l_text = questions[i].transform.GetChild(0).gameObject.GetComponent<TMP_InputField>().text;
                l_text = l_text.Replace("\r", "").Replace("\n", "");
            }

            feilds[i] = DiscordWebhooks.FeildBuilder(questions[i].GetComponent<TextMeshProUGUI>().text, l_text);
            questions[i].transform.GetChild(0).gameObject.GetComponent<TMP_InputField>().text = "";
        }


        string dd = DiscordWebhooks.FeildBuilder(ddd.GetComponent<TextMeshProUGUI>().text, ddd.transform.GetChild(0).gameObject.GetComponent<TMP_Dropdown>().options[ddd.transform.GetChild(0).gameObject.GetComponent<TMP_Dropdown>().value].text);

        ddd.transform.GetChild(0).gameObject.GetComponent<TMP_Dropdown>().value = 0;

        string f = DiscordWebhooks.FeildsConstructor(feilds[0], feilds[1], feilds[2], feilds[3], feilds[4], feilds[5], feilds[6], feilds[7], dd);
        DiscordWebhooks.AddLineToTextFile("Log", f);
        string e = DiscordWebhooks.EmbedBuilder(a_Color: Color.magenta, a_Title: "Player Feedback", a_Feilds: f, a_FooterText: Application.version.ToString());
        string ec = DiscordWebhooks.EmbedsConstructor(e);
        string pay = DiscordWebhooks.PayloadBuilder(a_Username: PlayerPrefs.GetString("Username") + " | " + PlayerPrefs.GetString("UserID"), a_Embeds: ec);
        DiscordWebhooks.PostToDiscord(a_Payload: pay);
    }

    private static bool DiscordRunning()
    {
        if (_SDiag.Process.GetProcessesByName("Discord").Length != 0)
            return true;
        else if (_SDiag.Process.GetProcessesByName("DiscordPTB").Length != 0)
            return true;
        else if (_SDiag.Process.GetProcessesByName("DiscordCanary").Length != 0)
            return true;
        else
            return false;
    }

    void Start()
    {
        ScoreboardConnection.GetScoreWithSucsess("0101", out _, Application.persistentDataPath);
        ScoreboardConnection.GetScoreWithSucsess("0102", out _, Application.persistentDataPath);
        ScoreboardConnection.GetScoreWithSucsess("0103", out _, Application.persistentDataPath);


        _usernameInput.onValueChanged.AddListener(delegate { UsernameCheck(); });


        GameObject[] _discordManagers = GameObject.FindGameObjectsWithTag("DiscordManager");
        if (_discordManagers.Length != 0)
        {
            m_DiscordController = _discordManagers[0].GetComponent<DiscordController>();
        }

        if (m_DiscordController != null)
            m_DiscordController._topMessage = "On The Main Menu";
        if (m_DiscordController != null)
            m_DiscordController._bottomMessage = "Exploring the home ship";
    }
}
