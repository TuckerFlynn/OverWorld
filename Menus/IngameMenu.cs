using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IngameMenu : MonoBehaviour
{
    public GameObject escapeMenu;
    public GameObject[] disableOnQuit;
    public GameObject settingsMenu;
    // HUD objects and related values
    [Header("HUD")]
    public GameObject HUD;
    public Image minimapDisplay;
    public Text coordText;
    Texture2D minimapTex;
    public int zoom = 10;
    // GUI objects and related values
    [Header("GUI")]
    public GameObject inventory;
    public GameObject logger;

    private void Start()
    {
        escapeMenu.SetActive(false);
        settingsMenu.SetActive(false);
        HUD.SetActive(true);
        inventory.SetActive(false);

        logger = GameObject.Find("Logger");
    }

    void Update()
    {
        // Prevents normal key actions while the looger input field is active
        if (logger != null)
        {
            if (logger.GetComponentInChildren<InputField>().enabled)
            {
                return;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventory.activeSelf)
            {
                inventory.SetActive(false);
            }
            else
            {
                escapeMenu.SetActive(!escapeMenu.activeSelf);
                if (System.Math.Abs(Time.timeScale) > float.Epsilon)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;
                settingsMenu.SetActive(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventory.SetActive(!inventory.activeSelf);
        }
    }

    public void QuitToMain(Camera main)
    {
        foreach(GameObject obj in disableOnQuit)
        {
            obj.SetActive(false);
        }
        Destroy(CharacterManager.characterManager.gameObject);
        Destroy(MapManager.mapManager.gameObject);
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void Settings()
    {
        escapeMenu.SetActive(!escapeMenu.activeSelf);
        settingsMenu.SetActive(!settingsMenu.activeSelf);
    }
    // Settings options
    public void AdjustHudTransparency (Slider slider)
    {
        Image[] images = HUD.GetComponentsInChildren<Image>();
        Text[] texts = HUD.GetComponentsInChildren<Text>();
        foreach (Image image in images)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, slider.value);
        }
        foreach (Text text in texts)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, slider.value);
        }
    }
}