using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
