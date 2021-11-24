using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using TMPro;

using _SDiag = System.Diagnostics;

public class MainMenu : MonoBehaviour
{

    [Header("Consent")]
    [SerializeField] private GameObject _consentPanel;
    [SerializeField] private Scrollbar _policyScroll;
    [SerializeField] private Toggle _consentCheck;
    [SerializeField] private Button _consentNext;

    [Header("Login")]
    [SerializeField] private TMP_InputField _usernameInput;
    [SerializeField] private GameObject _loginPanel, _discordManager;
    [SerializeField] private Button _discordButton, _retryButton, _guestButton;
    [SerializeField] private TextMeshProUGUI _discordErrorText, _guestErrorText;

    [Header("MainMenu")]
    [SerializeField] private GameObject _mainMenuPanel, m_PlayerPrefab;

    public GameObject[] questions = new GameObject[8];
    public GameObject ddd;

    private void Awake()
    {
        Analytics.enabled = true;

        PlayerPrefs.DeleteAll();

        if (AnalyticsSessionInfo.sessionFirstRun)
        {
            DiscordWebhooks.AddLineToTextFile("Log", "No Previous User LogOn Detected");
            ConsentSetup();
        }
        else
        {
            DiscordWebhooks.AddLineToTextFile("Log", "Previous User LogOn Detected, Reirected user past data concent and sign in");
            //MenuSetup();
            ConsentSetup();
        }
    }

    public void DidScroll()
    {
        if (_policyScroll.value <= 0)
        {
            _consentCheck.interactable = true;
        }
    }

    public void DidCheck()
    {
        DiscordWebhooks.AddLineToTextFile("Log", "User Concented To Data Collection");
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
        _loginPanel.SetActive(false);
        _mainMenuPanel.SetActive(false);
        _consentPanel.SetActive(true);
        PlayerPrefs.SetInt("CrewmateColor", 1);
        PlayerPrefs.Save();
    }

    void MenuSetup()
    {
        DiscordWebhooks.AddLineToTextFile("Log", "-----");
        DiscordWebhooks.AddLineToTextFile("Log", "Username: " + PlayerPrefs.GetString("Username"));
        DiscordWebhooks.AddLineToTextFile("Log", "UserID: " + PlayerPrefs.GetString("UserID"));
        DiscordWebhooks.AddLineToTextFile("Log", "Session Number: " + AnalyticsSessionInfo.sessionCount);
        DiscordWebhooks.AddLineToTextFile("Log", "Session ID: " + AnalyticsSessionInfo.sessionId);
        DiscordWebhooks.AddLineToTextFile("Log", "Analytics UserID: " + AnalyticsSessionInfo.userId);
        DiscordWebhooks.AddLineToTextFile("Log", "-----");


        _consentPanel.SetActive(false);
        _loginPanel.SetActive(false);
        //_mainMenuPanel.SetActive(true);

        Instantiate(m_PlayerPrefab, transform.GetChild(0).transform.position + Vector3.up, transform.GetChild(0).transform.rotation);
    }

    public void LoginSetup()
    {
        _consentPanel.SetActive(false);
        _mainMenuPanel.SetActive(false);
        _loginPanel.SetActive(true);
        bool _discordRunning = false;
        _discordRunning = DiscordRunning();
        Debug.Log("discord running = " + _discordRunning);
        _discordButton.interactable = _discordRunning;
        _retryButton.interactable = !_discordRunning;
        _discordErrorText.gameObject.SetActive(!_discordRunning);
    }

    void DiscordLoginSuccsess()
    {
        DiscordWebhooks.AddLineToTextFile("Log", "User Login With Discord");
        StartCoroutine(test());
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

        MenuSetup();

        yield return null;
    }

    public void DiscordLogin()
    {
        Instantiate(_discordManager);
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


        StartCoroutine(test());
    }

    public void SubmitForm()
    {
        string[] feilds = new string[8];

        for(int i = 0; i < questions.Length; i++)
        {
            feilds[i] = DiscordWebhooks.FeildBuilder(questions[i].GetComponent<TextMeshProUGUI>().text, questions[i].transform.GetChild(0).gameObject.GetComponent<TMP_InputField>().text);
        }


        string dd = DiscordWebhooks.FeildBuilder(ddd.GetComponent<TextMeshProUGUI>().text, ddd.transform.GetChild(0).gameObject.GetComponent<TMP_Dropdown>().options[ddd.transform.GetChild(0).gameObject.GetComponent<TMP_Dropdown>().value].text);

        string f = DiscordWebhooks.FeildsConstructor(feilds[0], feilds[1], feilds[2], feilds[3], feilds[4], feilds[5], feilds[6], feilds[7], dd);
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
        _usernameInput.onValueChanged.AddListener(delegate { UsernameCheck(); });
    }
}
