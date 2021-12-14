using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine;

//* Created by Morgan Finney 17/11/21
//* Thanks to Liam Rickman for helping get this started


public static class DiscordWebhooks
{
    private static string m_WebhhokURL = "https://discord.com/api/webhooks/920307303251603477/BenXg49FDmPOEi_mkz9vl2Ntm5rodJGTuWBFroJa_gx_R1ziDcuHIMLMCxB6EeTnQ8-5";

    private static bool m_Debugging = true;

    public static void SetDebugging(bool a_Debugging)
    {
        m_Debugging = a_Debugging;
        return;
    }

    public static void SetWebhookURL(string a_WebhookURL)
    {
        m_WebhhokURL = a_WebhookURL;
        return;
    }

    public static void TakeScreenShot(string a_FileName, int a_Res = 1)
    {
        ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "/" + a_FileName + ".png", a_Res);
        Debug.Log("<color=#5865F2><size=12>Discord Webhooks | </size></color><color=#57F287><b><size=12>ScreenShot: </size></b></color> \n<color=#FEE75C><i><size=10>" + Application.persistentDataPath + "/" + a_FileName + ".png" + "</size></i></color>");

    }

    public static void ClearTextFile(string a_FileName)
    {
        string l_FilePath = GetFilePath(a_FileName, ".txt");

        if (File.Exists(l_FilePath))
        {
            File.Delete(l_FilePath);
        }
    }

    public static void AddLineToTextFile(string a_FileName, string a_Content, bool a_AddTimestamp = true)
    {
        string l_FilePath = GetFilePath(a_FileName, ".txt");

        if (a_AddTimestamp)
            a_Content = System.DateTime.Now.ToString() + " | " + a_Content;

        using (StreamWriter sw = File.AppendText(l_FilePath))
        {
            sw.WriteLine(a_Content);
            sw.Close();
        }
    }
    public static void PostToDiscord(string a_FileName = "", string a_File2Name = "", string a_FileType = ".txt", string a_File2Type = ".txt", string a_FileRename = "", string a_File2Rename = "", string a_Payload = "")
    {
        var form = new MultipartFormDataContent();

        if (a_FileName != "")
        {
            string l_FilePath = GetFilePath(a_FileName, a_FileType);
            if (!File.Exists(l_FilePath))
            {
                if (m_Debugging)
                    Debug.LogWarning("<color=#5865F2><size=12>Discord Webhooks | </size></color><color=#ED4245><b><size=12>File Does Not Exist: </size></b></color> \n<color=#FEE75C><i><size=10>File could not be found at path: " + l_FilePath + "</size></i></color>");
                return;
            }

            //* These next few lines are adapted from https://github.com/Not-Cyrus/discord-file-webhook-upload/blob/main/webhook.cs;
            var l_FileContent = new ByteArrayContent(File.ReadAllBytes(l_FilePath));
            l_FileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

            if (a_FileRename == "")
                a_FileRename = a_FileName;

            form.Add(l_FileContent, "file", a_FileRename + a_FileType);
        }

        if (a_File2Name != "")
        {
            string l_FilePath2 = GetFilePath(a_File2Name, a_File2Type);
            if (!File.Exists(l_FilePath2))
            {
                if (m_Debugging)
                    Debug.LogWarning("<color=#5865F2><size=12>Discord Webhooks | </size></color><color=#ED4245><b><size=12>File Does Not Exist: </size></b></color> \n<color=#FEE75C><i><size=10>File could not be found at path: " + l_FilePath2 + "</size></i></color>");
                return;
            }

            //* These next few lines are adapted from https://github.com/Not-Cyrus/discord-file-webhook-upload/blob/main/webhook.cs;
            var l_FileContent2 = new ByteArrayContent(File.ReadAllBytes(l_FilePath2));
            l_FileContent2.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

            if (a_File2Rename == "")
                a_File2Rename = a_File2Name;

            form.Add(l_FileContent2, "file2", a_File2Rename + a_File2Type);
        }

        if (a_Payload != "")
        {
            form.Add(new StringContent(a_Payload), "payload_json");
        }

        if (a_FileName == "" && a_Payload == "")
        {
            if (m_Debugging)
                Debug.LogWarning("<color=#5865F2><size=12>Discord Webhooks | </size></color><color=#ED4245><b><size=12>Args Error: </size></b></color> \n<color=#FEE75C><i><size=10>You must provide a File, Payload or Both.</size></i></color>");
            return;
        }


        var t = Task.Run(() => GetForm(form));
        t.Wait();

        return;
    }

    public static void PostToDiscordWithSucsess(out bool isSucsess,
        string a_FileName = "", string a_FileRename = "", string a_FileType = ".txt",
        string a_File2Name = "", string a_File2Rename = "", string a_File2Type = ".txt",
        string a_File3Name = "", string a_File3Rename = "", string a_File3Type = ".txt",
        string a_File4Name = "", string a_File4Rename = "", string a_File4Type = ".txt",
        string a_Payload = "")
    {
        var form = new MultipartFormDataContent();

        if (a_FileName != "")
        {
            string l_FilePath = GetFilePath(a_FileName, a_FileType);
            if (!File.Exists(l_FilePath))
            {
                if (m_Debugging)
                    Debug.LogWarning("<color=#5865F2><size=12>Discord Webhooks | </size></color><color=#ED4245><b><size=12>File Does Not Exist: </size></b></color> \n<color=#FEE75C><i><size=10>File could not be found at path: " + l_FilePath + "</size></i></color>");
                isSucsess = false;
                return;
            }

            //* These next few lines are adapted from https://github.com/Not-Cyrus/discord-file-webhook-upload/blob/main/webhook.cs;
            var l_FileContent = new ByteArrayContent(File.ReadAllBytes(l_FilePath));
            l_FileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

            if (a_FileRename == "")
                a_FileRename = a_FileName;

            form.Add(l_FileContent, "file", a_FileRename + a_FileType);
        }

        if (a_File2Name != "")
        {
            string l_FilePath2 = GetFilePath(a_File2Name, a_File2Type);
            if (!File.Exists(l_FilePath2))
            {
                if (m_Debugging)
                    Debug.LogWarning("<color=#5865F2><size=12>Discord Webhooks | </size></color><color=#ED4245><b><size=12>File Does Not Exist: </size></b></color> \n<color=#FEE75C><i><size=10>File could not be found at path: " + l_FilePath2 + "</size></i></color>");
                isSucsess = false;
                return;
            }

            //* These next few lines are adapted from https://github.com/Not-Cyrus/discord-file-webhook-upload/blob/main/webhook.cs;
            var l_FileContent2 = new ByteArrayContent(File.ReadAllBytes(l_FilePath2));
            l_FileContent2.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

            if (a_File2Rename == "")
                a_File2Rename = a_File2Name;

            form.Add(l_FileContent2, "file2", a_File2Rename + a_File2Type);
        }

        if (a_File3Name != "")
        {
            string l_FilePath3 = GetFilePath(a_File3Name, a_File3Type);
            if (!File.Exists(l_FilePath3))
            {
                if (m_Debugging)
                    Debug.LogWarning("<color=#5865F3><size=13>Discord Webhooks | </size></color><color=#ED4345><b><size=13>File Does Not Exist: </size></b></color> \n<color=#FEE75C><i><size=10>File could not be found at path: " + l_FilePath3 + "</size></i></color>");
                isSucsess = false;
                return;
            }

            //* These next few lines are adapted from https://github.com/Not-Cyrus/discord-file-webhook-upload/blob/main/webhook.cs;
            var l_FileContent3 = new ByteArrayContent(File.ReadAllBytes(l_FilePath3));
            l_FileContent3.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

            if (a_File3Rename == "")
                a_File3Rename = a_File3Name;

            form.Add(l_FileContent3, "file3", a_File3Rename + a_File3Type);
        }

        if (a_File4Name != "")
        {
            string l_FilePath4 = GetFilePath(a_File4Name, a_File4Type);
            if (!File.Exists(l_FilePath4))
            {
                if (m_Debugging)
                    Debug.LogWarning("<color=#5865F4><size=14>Discord Webhooks | </size></color><color=#ED4445><b><size=14>File Does Not Exist: </size></b></color> \n<color=#FEE75C><i><size=10>File could not be found at path: " + l_FilePath4 + "</size></i></color>");
                isSucsess = false;
                return;
            }

            //* These next few lines are adapted from https://github.com/Not-Cyrus/discord-file-webhook-upload/blob/main/webhook.cs;
            var l_FileContent4 = new ByteArrayContent(File.ReadAllBytes(l_FilePath4));
            l_FileContent4.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

            if (a_File4Rename == "")
                a_File4Rename = a_File4Name;

            form.Add(l_FileContent4, "file4", a_File4Rename + a_File4Type);
        }

        if (a_Payload != "")
        {
            form.Add(new StringContent(a_Payload), "payload_json");
        }

        if (a_FileName == "" && a_Payload == "")
        {
            if (m_Debugging)
                Debug.LogWarning("<color=#5865F2><size=12>Discord Webhooks | </size></color><color=#ED4245><b><size=12>Args Error: </size></b></color> \n<color=#FEE75C><i><size=10>You must provide a File, Payload or Both.</size></i></color>");
            isSucsess = false;
            return;
        }


        var t = Task.Run(() => GetForm(form));
        t.Wait();

        Data returnData = t.Result;
        isSucsess = returnData.Suuc;
        return;
    }

    public class Data
    {
        public bool Suuc { get; set; }
    }

    private static async Task<Data> GetForm(MultipartFormDataContent a_Form)
    {
        var returnVar = new Data();

        using (HttpClient httpClient = new HttpClient())
        {
            HttpResponseMessage result = await httpClient.PostAsync(m_WebhhokURL, a_Form);
            if (result.IsSuccessStatusCode)
            {
                if (m_Debugging)
                    Debug.Log("<color=#5865F2><size=12>Discord Webhooks | </size></color><color=#57F287><b><size=12>Webhook Post Success: </size></b></color> \n<color=#FEE75C><i><size=10>" + await result.Content.ReadAsStringAsync() + "</size></i></color>");
                returnVar.Suuc = true;
                return returnVar;
            }
            else
            {
                if (m_Debugging)
                    Debug.LogWarning("<color=#5865F2><size=12>Discord Webhooks | </size></color><color=#ED4245><b><size=12>Webhook Post Failer: </size></b></color> \n<color=#FEE75C><i><size=10>" + await result.Content.ReadAsStringAsync() + "</size></i></color>");
                returnVar.Suuc = false;
                return returnVar;
            }
        }
    }

    public static void EasyPlainText(string a_Message)
    {
        string l_Payload = PayloadBuilder(a_Content: a_Message);
        PostToDiscord(a_Payload: l_Payload);
    }

    public static string PayloadBuilder(string a_Username = "", string a_AvatarURL = "", string a_Content = "", string a_Embeds = "", string a_Buttons = "")
    {
        string l_Payload = "{" +
              "\"username\":\"" + a_Username + "\"," +
              "\"avatar_url\":\"" + a_AvatarURL + "\"," +
              "\"content\":\"" + a_Content + "\"";

        if (a_Embeds != "")
            l_Payload = l_Payload + "," + a_Embeds;

        if (a_Buttons != "")
            l_Payload = l_Payload + "," + a_Buttons;


        l_Payload = l_Payload + "}";

        if (m_Debugging)
            Debug.Log("<color=#5865F2><size=12>Discord Webhooks | </size></color><color=#57F287><b><size=12>Built Payload: </size></b></color> \n<color=#FEE75C><i><size=10>" + l_Payload + "</size></i></color>");

        return l_Payload;
    }

    public static string EmbedsConstructor(string a_Embed01, string a_Embed02 = "", string a_Embed03 = "", string a_Embed04 = "", string a_Embed05 = "", string a_Embed06 = "", string a_Embed07 = "", string a_Embed08 = "", string a_Embed09 = "", string a_Embed10 = "")
    {
        string l_Embeds = "\"embeds\":[" +
            a_Embed01;

        if (a_Embed02 != "") l_Embeds = l_Embeds + "," + a_Embed02;
        if (a_Embed03 != "") l_Embeds = l_Embeds + "," + a_Embed03;
        if (a_Embed04 != "") l_Embeds = l_Embeds + "," + a_Embed04;
        if (a_Embed05 != "") l_Embeds = l_Embeds + "," + a_Embed05;
        if (a_Embed06 != "") l_Embeds = l_Embeds + "," + a_Embed06;
        if (a_Embed07 != "") l_Embeds = l_Embeds + "," + a_Embed07;
        if (a_Embed08 != "") l_Embeds = l_Embeds + "," + a_Embed08;
        if (a_Embed09 != "") l_Embeds = l_Embeds + "," + a_Embed09;
        if (a_Embed10 != "") l_Embeds = l_Embeds + "," + a_Embed10;


        l_Embeds = l_Embeds + "]  ";

        return l_Embeds;
    }

    public static string EmbedBuilder(Color a_Color, string a_Title = "", string a_Description = "", string a_Image01URL = "", string a_Image02URL = "", string a_Image03URL = "", string a_Image04URL = "", bool a_ShowTimestamp = true, string a_URL = "", string a_AuthorName = "", string a_AuthorURL = "", string a_AuthorIconURL = "", string a_ThumbnailURL = "", string a_FooterText = "", string a_FooterIconURL = "", string a_Feilds = "")
    {
        string ls_Color = ColorUtility.ToHtmlStringRGB(a_Color);
        int li_Color = int.Parse(ls_Color, System.Globalization.NumberStyles.HexNumber);

        string l_Time = "";
        if (a_ShowTimestamp)
            l_Time = DateTime.UtcNow.ToString("o");

        string l_Embed = "{" +
            "\"title\":\"" + a_Title + "\", " +
            "\"color\":" + li_Color + "," +
            "\"description\":\"" + a_Description + "\", " +
            "\"image\": {" +
                "\"url\": \"" + a_Image01URL + " \"}, " +
            "\"timestamp\":\"" + l_Time + "\", " +
            "\"url\":\"" + a_URL + "\", " +
            "\"author\":{" +
                "\"name\": \"" + a_AuthorName + "\"," +
                "\"url\": \"" + a_AuthorURL + "\"," +
                "\"icon_url\": \"" + a_AuthorIconURL + "\"}, " +
            "\"thumbnail\":{" +
                "\"url\": \"" + a_ThumbnailURL + "\"}, " +
            "\"footer\":{" +
                "\"text\": \"" + a_FooterText + "\"," +
                "\"icon_url\": \"" + a_FooterIconURL + "\"}";

        if (a_Feilds != "")
            l_Embed = l_Embed + "," + a_Feilds;


        l_Embed = l_Embed + "}";

        if (a_Image02URL != "")
        {
            if (m_Debugging && a_URL == "")
            {
                Debug.LogWarning("<color=#5865F2><size=12>Discord Webhooks | </size></color><color=#ED4245><b><size=12>Args Error: </size></b></color> \n<color=#FEE75C><i><size=10>You must provide a_URL when linking more than one image.</size></i></color>");
            }
            else
            {
                l_Embed = l_Embed + ",{" +
                "\"url\":\"" + a_URL + "\", " +
            "\"image\": {" +
                "\"url\": \"" + a_Image02URL + " \"}}";
            }
        }


        if (a_Image03URL != "")
        {
            if (m_Debugging && a_URL == "")
            {
                Debug.LogWarning("<color=#5865F2><size=12>Discord Webhooks | </size></color><color=#ED4245><b><size=12>Args Error: </size></b></color> \n<color=#FEE75C><i><size=10>You must provide a_URL when linking more than one image.</size></i></color>");
            }
            else
            {
                l_Embed = l_Embed + ",{" +
                "\"url\":\"" + a_URL + "\", " +
            "\"image\": {" +
                "\"url\": \"" + a_Image03URL + " \"}}";
            }
        }
        if (a_Image04URL != "")
        {
            if (m_Debugging && a_URL == "")
            {
                Debug.LogWarning("<color=#5865F2><size=12>Discord Webhooks | </size></color><color=#ED4245><b><size=12>Args Error: </size></b></color> \n<color=#FEE75C><i><size=10>You must provide a_URL when linking more than one image.</size></i></color>");
            }
            else
            {
                l_Embed = l_Embed + ",{" +
                "\"url\":\"" + a_URL + "\", " +
            "\"image\": {" +
                "\"url\": \"" + a_Image04URL + " \"}}";
            }
        }

        return l_Embed;
    }

    public static string FeildsConstructor(string a_Feild01, string a_Feild02 = "", string a_Feild03 = "", string a_Feild04 = "", string a_Feild05 = "", string a_Feild06 = "", string a_Feild07 = "", string a_Feild08 = "", string a_Feild09 = "", string a_Feild10 = "", string a_Feild11 = "", string a_Feild12 = "", string a_Feild13 = "", string a_Feild14 = "", string a_Feild15 = "", string a_Feild16 = "", string a_Feild17 = "", string a_Feild18 = "", string a_Feild19 = "", string a_Feild20 = "", string a_Feild21 = "", string a_Feild22 = "", string a_Feild23 = "", string a_Feild24 = "", string a_Feild25 = "")
    {
        string l_Feilds = "\"fields\": [" +
            a_Feild01;

        if (a_Feild02 != "") l_Feilds = l_Feilds + "," + a_Feild02;
        if (a_Feild03 != "") l_Feilds = l_Feilds + "," + a_Feild03;
        if (a_Feild04 != "") l_Feilds = l_Feilds + "," + a_Feild04;
        if (a_Feild05 != "") l_Feilds = l_Feilds + "," + a_Feild05;
        if (a_Feild06 != "") l_Feilds = l_Feilds + "," + a_Feild06;
        if (a_Feild07 != "") l_Feilds = l_Feilds + "," + a_Feild07;
        if (a_Feild08 != "") l_Feilds = l_Feilds + "," + a_Feild08;
        if (a_Feild09 != "") l_Feilds = l_Feilds + "," + a_Feild09;
        if (a_Feild10 != "") l_Feilds = l_Feilds + "," + a_Feild10;
        if (a_Feild11 != "") l_Feilds = l_Feilds + "," + a_Feild11;
        if (a_Feild12 != "") l_Feilds = l_Feilds + "," + a_Feild12;
        if (a_Feild13 != "") l_Feilds = l_Feilds + "," + a_Feild13;
        if (a_Feild14 != "") l_Feilds = l_Feilds + "," + a_Feild14;
        if (a_Feild15 != "") l_Feilds = l_Feilds + "," + a_Feild15;
        if (a_Feild16 != "") l_Feilds = l_Feilds + "," + a_Feild16;
        if (a_Feild17 != "") l_Feilds = l_Feilds + "," + a_Feild17;
        if (a_Feild18 != "") l_Feilds = l_Feilds + "," + a_Feild18;
        if (a_Feild19 != "") l_Feilds = l_Feilds + "," + a_Feild19;
        if (a_Feild20 != "") l_Feilds = l_Feilds + "," + a_Feild20;
        if (a_Feild21 != "") l_Feilds = l_Feilds + "," + a_Feild21;
        if (a_Feild22 != "") l_Feilds = l_Feilds + "," + a_Feild22;
        if (a_Feild23 != "") l_Feilds = l_Feilds + "," + a_Feild23;
        if (a_Feild24 != "") l_Feilds = l_Feilds + "," + a_Feild24;
        if (a_Feild25 != "") l_Feilds = l_Feilds + "," + a_Feild25;


        l_Feilds = l_Feilds + "]";

        return l_Feilds;
    }

    public static string FeildBuilder(string a_Name, string a_Value, bool a_Inline = false)
    {
        return "{" +
            "\"name\": \"" + a_Name + "\"," +
            "\"value\": \"" + a_Value + "\"," +
            "\"inline\":" + a_Inline.ToString().ToLower() +
        "}";
    }

    //* Discord Changed how this works, the webhook url need to be made by a bot, and that same bot must send the post request using the bot api.
    //* This makes it near imposable to send buttons via webhook. Unless you have some advanced infostructure that sent this json to a bot on a server that would then post that json itself. 
    //public static string ButtonConstructor(string a_Button01, string a_Button02 = "", string a_Button03 = "", string a_Button04 = "", string a_Button05 = "")
    //{
    //    string l_Buttons = "\"components\": [{" +
    //        "\"type\": 1,";
    //    l_Buttons = l_Buttons + a_Button01;

    //    if(a_Button02 != "") l_Buttons = l_Buttons + "," + a_Button02;
    //    if (a_Button03 != "") l_Buttons = l_Buttons + "," + a_Button03;
    //    if (a_Button04 != "") l_Buttons = l_Buttons + "," + a_Button04;
    //    if (a_Button05 != "") l_Buttons = l_Buttons + "," + a_Button05;

    //    l_Buttons = l_Buttons + "}]";

    //    return l_Buttons;
    //}

    //public static string ButtonBuilder(string a_Label, string a_URL)
    //{
    //    return "\"components\": [{" +
    //       "\"type\": 2," +
    //       "\"style\": 5," +
    //       "\"label\": \"" + a_Label + "\"," +
    //       "\"url\": \"" + a_URL + "\"}]";
    //}

    private static string GetFilePath(string _fileName, string a_FileType)
    {
        return Application.persistentDataPath + "/" + _fileName + a_FileType;
    }
}