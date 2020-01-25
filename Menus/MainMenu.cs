using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject settings;

    private void Start()
    {
        settings.SetActive(false);
    }
    public void LoadWorldGenMenu()
    {
        SceneManager.LoadSceneAsync("WorldGenMenu");
    }

    public void LoadGame()
    {
        SceneManager.LoadSceneAsync("LoadMenu");
    }

    public void NewCharacter()
    {
        SceneManager.LoadSceneAsync("CharacterMenu");
    }

    public void Settings()
    {
        settings.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
