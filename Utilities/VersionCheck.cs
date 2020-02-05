using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VersionCheck : MonoBehaviour
{
    void Awake()
    {
        // Current version of the game
        string thisVersion = Application.version;
        // Check if the most recently played version of the game has been recorded in playerprefs
        if (PlayerPrefs.HasKey("Version"))
        {
            // If the most recently played version is not the current version, erase any saved data
            if (!thisVersion.Equals(PlayerPrefs.GetString("Version")))
            {
                File.Delete(Application.persistentDataPath + "/worldMap.dat");
                File.Delete(Application.persistentDataPath + "/characters.config");
                Directory.Delete(Application.persistentDataPath + "/Fields", true);
                Debug.Log("Save files from previous game versions have been deleted.");
            }
            else
            {
                Debug.Log("Playing Overworld v" + thisVersion);
            }
        }
        PlayerPrefs.SetString("Version", thisVersion);
    }

    private void Start()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
