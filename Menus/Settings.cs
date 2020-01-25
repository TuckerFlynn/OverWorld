using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Toggle LoggerIsOn;
    public Image LoggerImage;
    public Text LoggerText;

    private void Start()
    {
        LoggerImage = GameObject.Find("LoggerImage").GetComponent<Image>();
        LoggerText = GameObject.Find("LoggerText").GetComponent<Text>();
        if (PlayerPrefs.GetInt("DebugMode") == 0)
        {
            LoggerImage.enabled = false;
            LoggerText.enabled = false;
        }
    }

    public void Back()
    {
        gameObject.SetActive(false);
    }

    public void ToggleLogger(Toggle toggle)
    {
        LoggerImage.enabled = toggle.isOn;
        LoggerText.enabled = toggle.isOn;
        int setting = toggle.isOn ? 1 : 0;
        PlayerPrefs.SetInt("DebugMode", setting);
        Debug.Log("Debug mode enabled.");
    }
}
