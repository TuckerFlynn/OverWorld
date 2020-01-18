using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    bool nope = false;
    public void Settings(GameObject go)
    {
        if (!nope)
        {
            go.transform.GetComponent<Text>().text = "lol nope :D";
            nope = true;
        }
        else
        {
            go.transform.GetComponent<Text>().text = "settings";
            nope = false;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
