using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class DungeonMaster : MonoBehaviour
{
    public static DungeonMaster dungeonMaster;
    public CharacterManager charMngr;

    public DungeonGenerator dunGen;
    [Header("Mine Entrance UI")]
    public GameObject MineEntranceUI;
    public InputField seedInput;
    public Button[] buttons;
    public Button enterButton;
    public Text mineInfoText;
    [Header("Tilemaps")]
    public Tilemap Ground;
    public Tilemap Roof;
    public Tilemap Objects;
    [Header("Area Objects")]
    public GameObject Areas;
    public GameObject[] AreaObjects;

    private DungeonConfig activeConfig;

    private void Awake()
    {
        if (dungeonMaster == null)
        {
            dungeonMaster = this;
        }
        else if (dungeonMaster != this)
        {
            Destroy(this.gameObject);
        }
        // Check for the dungeons file directory
        if (!Directory.Exists(Application.persistentDataPath + "/Dungeons"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Dungeons");
        }
    }

    private void Start()
    {
        charMngr = CharacterManager.characterManager;
    }

    public void LoadDungeon()
    {
        // Clear all children of the Areas gameobject
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in Areas.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
        // Hide mine entrance UI
        MineEntranceUI.SetActive(false);
        // Save the current field and the player's overworld position for when they return to the surface
        MapManager.mapManager.SaveFieldFile(MapManager.mapManager.worldPos);
        charMngr.EnterDungeon();
        charMngr.charObject.transform.position = dunGen.MainDungeonGen(activeConfig.Seed);
    }

    public void EnableMineUI (bool uiActive, DungeonConfig config)
    {
        activeConfig = config;
        MineEntranceUI.SetActive(uiActive);

        if (uiActive)
        {
            if (activeConfig.SetSeed)
            {
                // Disable input and enable enter
                seedInput.interactable = false;
                foreach (Button obj in buttons)
                    obj.interactable = false;
                enterButton.interactable = true;
                // Retrieve the saved name
                seedInput.text = activeConfig.SeedString;
            }
            else
            {
                // Enable input and disable enter
                seedInput.interactable = true;
                foreach (Button obj in buttons)
                    obj.interactable = true;
                enterButton.interactable = false;
                // Get a generated name
                seedInput.text = MineNameGen.MakeName();
                // Get character level, skill level, and build level

            }
            WriteMineUIInfo();
        }
    }

    public void RefreshMineName ()
    {
        seedInput.text = MineNameGen.MakeName();
    }

    public void ImproveMine()
    {
        activeConfig.BuildLevel++;
        WriteMineUIInfo();
    }

    public void ConfirmMineConfig ()
    {
        activeConfig.SeedString = seedInput.text;
        activeConfig.SetSeed = true;
        activeConfig.Seed = StringToInt(activeConfig.SeedString);
        activeConfig.SetResourceDensity();

        string JsonOut = JsonConvert.SerializeObject(activeConfig);
        File.WriteAllText(Application.persistentDataPath + "/Dungeons/" + activeConfig.Path, JsonOut);
        Debug.Log("Confirmed dungeon config " + activeConfig.Path);

        EnableMineUI(true, activeConfig);
    }

    void WriteMineUIInfo ()
    {
        StringBuilder mineInfo = new StringBuilder();
        mineInfo.Append("Base Level: ").Append(activeConfig.BaseLevel).AppendLine();
        mineInfo.Append("Total Level: ").Append(activeConfig.BaseLevel + activeConfig.SkillLevel + activeConfig.BuildLevel).AppendLine();
        mineInfo.Append("Resources: ").Append(activeConfig.ResourceDensity).AppendLine();
        mineInfo.Append("Max. Depth: ").Append(activeConfig.MaxDepth);
        mineInfoText.text = mineInfo.ToString();
    }

    // Return an int based on the input string
    int StringToInt(string str)
    {
        return str.GetHashCode();
    }
}