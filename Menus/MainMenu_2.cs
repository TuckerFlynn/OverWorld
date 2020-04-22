using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu_2 : MonoBehaviour
{
    [Header("MENU PANELS")]
    public GameObject MainMenu;
    public GameObject LoadGameMenu;
    public GameObject NewWorldMenu;
    public GameObject NewCharacterMenu;
    [Header("MAIN MENU BUTTONS")]
    public GameObject ContinueButton;
    public GameObject LoadButton;
    public GameObject NewGameButton;
    public GameObject NewWorldButton;
    public GameObject NewCharacterButton;
    public GameObject BackButton;
    public GameObject SettingsButton;
    public GameObject QuitButton;
    [Header("TOOLTIP SCRIPT")]
    public MenuTooltip menuTooltip;

    private void Start()
    {
        // Hide inactive panels
        LoadGameMenu.SetActive(false);
        NewWorldMenu.SetActive(false);
        NewCharacterMenu.SetActive(false);
        // Hide unused buttons
        NewWorldButton.SetActive(false);
        NewCharacterButton.SetActive(false);
        BackButton.SetActive(false);
        // The "continue game" button requires a saved world map and a recorded LastCharacter
        if (PlayerPrefs.HasKey("LastCharacter") && File.Exists(Application.persistentDataPath + "/worldMap.dat"))
            ContinueButton.GetComponent<Button>().interactable = true;
        else
            ContinueButton.GetComponent<Button>().interactable = false;
    }

    public void OnContinueButton ()
    {
        SceneManager.LoadSceneAsync("MapManager");
    }

    public void OnLoadButton ()
    {
        MainMenu.SetActive(false);
        LoadGameMenu.SetActive(true);
        NewWorldMenu.SetActive(false);
        NewCharacterMenu.SetActive(false);

        menuTooltip.UpdateRaycaster();
    }
    
    public void OnNewGameButton ()
    {
        MainMenu.SetActive(true);
        LoadGameMenu.SetActive(false);
        NewWorldMenu.SetActive(false);
        NewCharacterMenu.SetActive(false);

        ContinueButton.SetActive(false);
        LoadButton.SetActive(false);
        NewGameButton.SetActive(false);
        NewWorldButton.SetActive(true);
        NewCharacterButton.SetActive(true);
        BackButton.SetActive(true);
        SettingsButton.SetActive(false);
        QuitButton.SetActive(false);
    }

    public void OnNewWorldButton ()
    {
        MainMenu.SetActive(false);
        LoadGameMenu.SetActive(false);
        NewWorldMenu.SetActive(true);
        NewCharacterMenu.SetActive(false);

        menuTooltip.UpdateRaycaster();
    }

    public void OnNewCharacterButton ()
    {
        MainMenu.SetActive(false);
        LoadGameMenu.SetActive(false);
        NewWorldMenu.SetActive(false);
        NewCharacterMenu.SetActive(true);

        menuTooltip.UpdateRaycaster();
    }

    public void ReturnToMainMenu ()
    {
        MainMenu.SetActive(true);
        LoadGameMenu.SetActive(false);
        NewWorldMenu.SetActive(false);
        NewCharacterMenu.SetActive(false);

        ContinueButton.SetActive(true);
        LoadButton.SetActive(true);
        NewGameButton.SetActive(true);
        NewWorldButton.SetActive(false);
        NewCharacterButton.SetActive(false);
        BackButton.SetActive(false);
        SettingsButton.SetActive(true);
        QuitButton.SetActive(true);

        menuTooltip.UpdateRaycaster();
    }

    public void OnSettingsButton ()
    {

    }

    public void OnQuitButton ()
    {
        Application.Quit();
    }
}
