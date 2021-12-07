using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using System;
using System.Linq;
using System.Threading;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class DiscordController : MonoBehaviour
{

    public Discord.Discord discord;
    public Discord.ActivityManager discordActivityManager;

    public string _topMessage = "I am a Top", _bottomMessage = "I am a bottom";
    public int _timeStamp = 0;
    bool _userGrab = false;
    public long _applicationID = 906221483272073287;



    private void Awake()
    {
        GameObject[] _discordManagers = GameObject.FindGameObjectsWithTag("DiscordManager");
        if (_discordManagers.Length > 1)
            Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        ResetTime();

        try
        {
            discord = new Discord.Discord(_applicationID, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Discord | Failed to initiate application client \n" + e.Message);
            Destroy(this.gameObject);
            return;
        }

        try
        {
            discordActivityManager = discord.GetActivityManager();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Discord | Failed to initiate activity manager \n" + e.Message);
            return;
        }



        var userManager = discord.GetUserManager();

        userManager.OnCurrentUserUpdate += () =>
      {
          var currentUser = userManager.GetCurrentUser();

          PlayerPrefs.SetString("UserID", currentUser.Id.ToString());
          PlayerPrefs.SetString("Username", currentUser.Username);
          PlayerPrefs.Save();
          _userGrab = true;
      };

        StartCoroutine(WaitForUsername());
    }

    void Update()
    {
        discord.RunCallbacks();
    }

    int CurrentUnixTime()
    {
        DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return ((int)(DateTime.UtcNow - unixStart).TotalSeconds);
    }

    public void ResetTime()
    {
        _timeStamp = CurrentUnixTime();
    }

    public Discord.Activity CreateActivity()
    {
        var activity = new Discord.Activity
        {
            Details = _topMessage,
            State = _bottomMessage,
            Timestamps =
             {
            Start = _timeStamp,
                End = 0
             },
            Assets =
            {
            LargeImage = "logo",
                LargeText = "This game kinda sus",
                SmallImage = "null",
                SmallText = "",
            }
        };
        return activity;
    }

    void UpdateActivity()
    {
        discordActivityManager.UpdateActivity(CreateActivity(), (res) =>
        {
            if (res != Discord.Result.Ok)
            {
                UnityEngine.Debug.LogError("Discord | Failed to update activity");
            }
        });
    }

    IEnumerator WaitForUsername()
    {
        yield return new WaitForEndOfFrame();
        discord.RunCallbacks();
        yield return new WaitForEndOfFrame();

        if (_userGrab)
        {
            GameObject.FindGameObjectWithTag("MainMenuManager").SendMessage("DiscordLoginSuccsess");
            StartCoroutine(UpdateTimer());
        }
        else
            StartCoroutine(WaitForUsername());
    }

    IEnumerator UpdateTimer()
    {
        UpdateActivity();
        yield return new WaitForSeconds(15f);
        StartCoroutine(UpdateTimer());
    }
}