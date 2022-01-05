using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System;

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine;

public static class ScoreboardConnection
{


    private const string scoreURL = "https://crewmatecleanup.pdox.uk/highscore.php";
    private const string highscoreURL = "https://crewmatecleanup.pdox.uk/leaderboard.php";

    public class SucsessData
    {
        public bool taskSucsess { get; set; }
    }

    public static void PostScoreWithSucsess(string a_LevelCode, int a_Score, out bool a_IsSucsess)
    {
        MultipartFormDataContent l_Form = new MultipartFormDataContent();
        l_Form.Add(new StringContent(PlayerPrefs.GetString("Username")), "username");
        l_Form.Add(new StringContent(PlayerPrefs.GetString("UserID")), "userid");
        l_Form.Add(new StringContent(a_LevelCode), "levelcode");
        l_Form.Add(new StringContent(Application.version), "gameversion");
        l_Form.Add(new StringContent(a_Score.ToString()), "score");

        var t = Task.Run(() => PostScoreToTable(l_Form));
        t.Wait();

        SucsessData returnData = t.Result;
        a_IsSucsess = returnData.taskSucsess;
        return;
    }

    private static async Task<SucsessData> PostScoreToTable(MultipartFormDataContent a_Form)
    {
        var returnVar = new SucsessData();

        using (HttpClient httpClient = new HttpClient())
        {
            HttpResponseMessage result = await httpClient.PostAsync(scoreURL, a_Form);
            if (result.IsSuccessStatusCode)
            {
                Debug.Log("Post Score good");
                returnVar.taskSucsess = true;
                return returnVar;
            }
            else
            {
                Debug.Log("Post Score Error");
                returnVar.taskSucsess = false;
                return returnVar;
            }
        }
    }

    public static void GetScoreWithSucsess(string a_LevelCodeFilter, out bool a_IsSucsess, string a_ApplicationPersistantDataPath)
    {
        MultipartFormDataContent l_Form = new MultipartFormDataContent();
        l_Form.Add(new StringContent(a_LevelCodeFilter), "levelcodefilter");


        var t = Task.Run(() => GetScoresTableCSV(l_Form, a_LevelCodeFilter, a_ApplicationPersistantDataPath));
        t.Wait();

        SucsessData returnData = t.Result;
        a_IsSucsess = returnData.taskSucsess;
        return;
    }

    private static async Task<SucsessData> GetScoresTableCSV(MultipartFormDataContent a_Form, string a_LevelCodeFilter, string a_ApplicationPersistantDataPath)
    {
        var returnVar = new SucsessData();

        using (HttpClient httpClient = new HttpClient())
        {
            HttpResponseMessage result = await httpClient.PostAsync(highscoreURL, a_Form);
            if (result.IsSuccessStatusCode)
            {
                Debug.Log("Get Score good");
                string l_leaderboardCommaSeperated = await result.Content.ReadAsStringAsync();

                string l_FilePath = a_ApplicationPersistantDataPath + "/ScoreboardData" + a_LevelCodeFilter + ".csv";

                if (File.Exists(l_FilePath))
                {
                    File.Delete(l_FilePath);
                }

                using (StreamWriter sw = File.AppendText(l_FilePath))
                {
                    sw.WriteLine(l_leaderboardCommaSeperated);
                    sw.Close();
                }

                returnVar.taskSucsess = true;
                return returnVar;
            }
            else
            {
                Debug.Log("Get Score Error");
                returnVar.taskSucsess = false;
                return returnVar;
            }
        }
    }


    public static string DisplayWithSuffix(this int num)
    {
        string number = num.ToString();
        if (number == "0") return "n/a";
        if (number.EndsWith("11")) return number + "th";
        if (number.EndsWith("12")) return number + "th";
        if (number.EndsWith("13")) return number + "th";
        if (number.EndsWith("1")) return number + "st";
        if (number.EndsWith("2")) return number + "nd";
        if (number.EndsWith("3")) return number + "rd";
        return number + "th";
    }
}
