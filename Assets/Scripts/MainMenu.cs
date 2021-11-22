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

    private void Awake()
    {
        Analytics.enabled = true;

        PlayerPrefs.DeleteAll();

        if (AnalyticsSessionInfo.sessionFirstRun)
        {
            ConsentSetup();
        }
        else
        {
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

        SendMessage("DiscordLoginSuccsess");
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
