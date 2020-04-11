using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class DiscordWebhook : MonoBehaviour
{
    readonly string hookURL = "https://discordapp.com/api/webhooks/698233163071356959/monYlm_SVu0ZNeuxnI830PYteFe2iIjXZAutspflW61Ml3KsFoicrjHLKNU-Ve3vQPaB";

    public Dropdown dropdown;
    enum dropdownOptions { BUG, CRASH, FEEDBACK, SPAM };
    public InputField inputField;

    public void ConfirmSubmit ()
    {
        DiscordJson discordJson = CreateDiscordJson();
        string json = JsonConvert.SerializeObject(discordJson);
        StartCoroutine(Post(hookURL, json));
        inputField.text = "";
    }

    IEnumerator Post(string url, string bodyJsonString)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        Debug.Log("Discord feedback POST response: " + request.responseCode);
    }

    DiscordJson CreateDiscordJson ()
    {
        DiscordEmbed embed = new DiscordEmbed
        {
            title = ((dropdownOptions)dropdown.value).ToString(),
            color = GetEmbedColor(dropdown.value),
            description = inputField.text
        };

        DiscordJson obj = new DiscordJson()
        {
            embeds = new DiscordEmbed[] { embed }
        };

        return obj;
    }

    int GetEmbedColor(int option)
    {
        int color = 0;
        switch (option)
        {
            case 0:
                // bug --> yellow
                color = 10385452;
                break;
            case 1:
                // crash --> red
                color = 10365996;
                break;
            case 2:
                // feedback --> green
                color = 5414444;
                break;
            case 3:
                // spam --> blue
                color = 2904734;
                break;
            default:
                break;
        }
        return color;
    }
}
[System.Serializable]
class DiscordJson
{
    public DiscordEmbed[] embeds { get; set; }
}
[System.Serializable]
class DiscordEmbed
{
    public string title { get; set; }
    public int color { get; set; }
    public string description { get; set; }
}