using System.Collections;
using UnityEngine;
using System.IO;
using static DiscordWebhooks;

public class Examples : MonoBehaviour
{
    /// Usage 
    /// 
    /// SetUp Functions
    /// 
    /// SetWebhookURL("your discord webhook url here") [You could change m_WebhookURL in DiscordWebhooks.cs is you prefer]
    /// SetDebugging(true/false) [Recommended you leave this on while created your game, turn off for build. You can also set this directly in DiscordWebhooks.cs with m_Debugging]
    /// 
    /// Main Functions
    /// 
    ///     PostToDiscord() has 4 arguments. All 4 are optional but at least a_Filename or a_Payload must be provided.
    ///         a_FileName = The name of the file you would like to send.
    ///         a_FileType = Defaults to .txt but you can provide any other extension.
    ///         a_FileRename = Renames the file that is sent to discord, the local file will still have the same name.
    ///         a_Payload = Json formatted payload that is used for messages, changing username, embeds
    ///
    ///     PayloadBuilder() is used to make your Json payload. 
    ///         a_Username = Will change the username in discord for this payload
    ///         a_AvatarURL = Must be an external URL this will set the PFP of the payload. [You can use the Imgur API if you would like to upload something from the game and show it in webhooks)
    ///         a_Content = Simple plain text message content 
    ///         a_Embeds = A Preformatted Json created using EmbedsConstructor()
    ///         a_Buttons = !!!Deprecated!!! Discord changed how this works :(
    ///     
    /// Embed Functions 
    /// This is where it all gets a little complicated. 
    /// There are 4 functions for creating embeds
    /// 
    ///     EmbedConstructor() Allows you to chain up to 10 embeds for use in one payload. 
    ///     EmbedBuilder() Allows you to fill the content to an embed.
    ///     FeildConstuctor() Allows you to chain up to 25 fields for use in one embed.
    ///     FeildBuilder() Allows you to fill the content of an field.
    ///     
    /// QoL Functions
    ///     
    ///     AddLineToTextFile()
    ///     ClearTextFile()
    ///     TakeScreenShot()
    /// 
    /// The Examples below cover most usage examples.

    void Start()
    {
        DiscordWebhooks.SetWebhookURL("https://discord.com/api/webhooks/910175802891599932/c5Abb5K1myQtMEH0sNHf6Kp5UiebmSbBSCtDDE0mxNevY2hLMIg-br28c46vRuIHcWMP");
        //* you can use "using static DiscordWebhooks;" at the top of the script to avoid writing it every time.
        SetDebugging(true);
        ExampleEmbed();
        ExampleScreenShot();
    }

    void ExampleSetWebhookURL()
    {
        DiscordWebhooks.SetWebhookURL("https://discord.com/api/webhooks/910175802891599932/c5Abb5K1myQtMEH0sNHf6Kp5UiebmSbBSCtDDE0mxNevY2hLMIg-br28c46vRuIHcWMP");
    }

    void ExampleSetDebugging()
    {
        DiscordWebhooks.SetDebugging(true);
    }

    void ExamplePlainText()
    {
        string l_ExamplePayLoad = DiscordWebhooks.PayloadBuilder(a_Content: "This is some plane text");

        DiscordWebhooks.PostToDiscord(a_Payload: l_ExamplePayLoad);
    }

    void ExampleChangeUser()
    {
        string l_ExamplePayLoad = DiscordWebhooks.PayloadBuilder(a_Username: "OwO", a_AvatarURL: "https://upload.wikimedia.org/wikipedia/commons/thumb/2/22/MOREmoji_owo.svg/1200px-MOREmoji_owo.svg.png", a_Content: "This also has some text");

        DiscordWebhooks.PostToDiscord(a_Payload: l_ExamplePayLoad);
    }

    void ExampleEmbed()
    {
        //* Building Some Fields.
        string l_FeildExample01 = DiscordWebhooks.FeildBuilder("This is a Feild Title", "This is a Feild Body", true);
        string l_FeildExample02 = DiscordWebhooks.FeildBuilder("This is Another Feild Title", "Both these Feilds are inline", true);

        //* Chaining Fields.
        string l_ConstructedFeildExample = DiscordWebhooks.FeildsConstructor(l_FeildExample01, l_FeildExample02);


        //* Building an embed. This embed includes every args as an example.
        string l_EmbedExample01 = DiscordWebhooks.EmbedBuilder(a_Color: Color.blue, a_Title: "This is a Title", a_Description: "This is a description",
            a_Image01URL: "https://upload.wikimedia.org/wikipedia/commons/thumb/e/e5/Eo_circle_deep-purple_number-1.svg/1024px-Eo_circle_deep-purple_number-1.svg.png",
            a_Image02URL: "https://upload.wikimedia.org/wikipedia/commons/thumb/9/92/Eo_circle_amber_number-2.svg/1024px-Eo_circle_amber_number-2.svg.png",
            a_Image03URL: "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3e/Eo_circle_blue_number-3.svg/1024px-Eo_circle_blue_number-3.svg.png",
            a_Image04URL: "https://upload.wikimedia.org/wikipedia/commons/thumb/4/49/Eo_circle_green_number-4.svg/1024px-Eo_circle_green_number-4.svg.png",
            a_URL: "https://www.google.com/", a_AuthorName: "Embed Author", a_AuthorURL: "https://duckduckgo.com/",
            a_AuthorIconURL: "https://i.imgur.com/G42c97N.jpeg", a_ThumbnailURL: "https://s26.q4cdn.com/977690160/files/design/U_Logo_White_RGB.png",
            a_FooterText: "This is a footer", a_FooterIconURL: "https://i.imgur.com/PiiTH5U.png", a_Feilds: l_ConstructedFeildExample);


        //* Building some fields to show what inline does.
        string l_FeildExample03 = DiscordWebhooks.FeildBuilder("This is a Feild Title", "This is not inline", false);
        string l_FeildExample04 = DiscordWebhooks.FeildBuilder("This is a Feild Title", "This is not inline as deafult is false");
        string l_FeildExample05 = DiscordWebhooks.FeildBuilder("This is Another Feild Title", "Both these Feilds are inline", true);
        string l_FeildExample06 = DiscordWebhooks.FeildBuilder("This is Another Feild Title", "Both these Feilds are inline", true);

        //* Chaining them fields.
        string l_ConstructedFeildExample02 = DiscordWebhooks.FeildsConstructor(l_FeildExample03, l_FeildExample04, l_FeildExample05, l_FeildExample06);

        //* Embed for the fields example.
        string l_EmbedExample02 = DiscordWebhooks.EmbedBuilder(a_ShowTimestamp: false, a_Color: Color.red, a_Title: "This is a Title", a_Description: "This is a description", a_Feilds: l_ConstructedFeildExample02);

        //* Small embed example.
        string l_EmbedExample03 = DiscordWebhooks.EmbedBuilder(a_Color: Color.green, a_Title: "This is a Title", a_Description: "This is a description");

        //* Chanining the 3 embeds.
        string l_ConstructedEmbedsExample = DiscordWebhooks.EmbedsConstructor(l_EmbedExample01, l_EmbedExample02, l_EmbedExample03);

        //* Embeds to a payload.
        string l_ExamplePayLoad = DiscordWebhooks.PayloadBuilder(a_Embeds: l_ConstructedEmbedsExample);

        //* Posting it to discord.
        DiscordWebhooks.PostToDiscord(a_Payload: l_ExamplePayLoad);
    }

    void ExampleTextFile()
    {
        DiscordWebhooks.PostToDiscord(a_FileName: "testFile");
    }

    void ExampleRenameFile()
    {
        DiscordWebhooks.PostToDiscord(a_FileName: "testFile", a_FileRename: "TheFileWillHaveThisNameInDiscord");
    }

    void ExampleImageFile()
    {
        DiscordWebhooks.PostToDiscord(a_FileName: "thisIsAnImageSavedToTheApplicationDataPath", a_FileType: ".png", a_FileRename: "YouCouldRenameThisTo");
    }

    void ExampleFileWithUser()
    {

        string l_username = "Unknown User";
        if (PlayerPrefs.HasKey("Username"))
            l_username = PlayerPrefs.GetString("Username");

        string l_ExamplePayLoad = DiscordWebhooks.PayloadBuilder(a_Username: l_username);

        DiscordWebhooks.PostToDiscord(a_FileName: "testFile", a_Payload: l_ExamplePayLoad);
    }

    void ExampleScreenShot()
    {
        System.DateTime l_EPOCH = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        int l_UNIX = (int)(System.DateTime.UtcNow - l_EPOCH).TotalSeconds;
        string l_Name = "SS-" + l_UNIX;
        TakeScreenShot(l_Name);
        StartCoroutine(WaitForScreenShot(l_Name));
    }

    IEnumerator WaitForScreenShot(string a_FileName)
    {
        yield return new WaitForSeconds(0.1f);

        if(File.Exists(Application.persistentDataPath + "/" + a_FileName + ".png"))
            DiscordWebhooks.PostToDiscord(a_FileName: a_FileName, a_FileType: ".png");
        else
            StartCoroutine(WaitForScreenShot(a_FileName));
    }
}
