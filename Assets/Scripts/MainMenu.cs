using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using TMPro;

using _SDiag = System.Diagnostics;

public class MainMenu : MonoBehaviour
{
    public Button _discordButton, _retryButton, _guestButton;
    public GameObject _loginPanel, _mainMenuPanel, _discordManager;
    public TextMeshProUGUI _discordErrorText, _guestErrorText;
    public TMP_InputField _usernameInput;

    private void Awake()
    {
        Analytics.enabled = true;

        PlayerPrefs.DeleteAll();

        if(AnalyticsSessionInfo.sessionFirstRun)
        {
            LoginSetup();
        }
        else
        {
            //MenuSetup();
            LoginSetup();
        }
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

    void MenuSetup()
    {
        _loginPanel.SetActive(false);
        _mainMenuPanel.SetActive(true);
    }

    void LoginSetup()
    {
        _mainMenuPanel.SetActive(false);
        _loginPanel.SetActive(true);
        bool _discordRunning = DiscordRunning();
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
        else if (testv == AnalyticsResult.Ok)
            Debug.Log("nice");


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
