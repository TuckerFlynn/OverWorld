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
    public DungeonResources dunResources;
    public MineBuilder mineBuilder;
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

    public DungeonConfig activeConfig;
    public int CurrentDepth = 0;

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

    public void EnterMine()
    {
        // Hide mine entrance UI
        MineEntranceUI.SetActive(false);
        // Save the current field and the player's overworld position for when they return to the surface
        MapManager.mapManager.SaveFieldFile(MapManager.mapManager.worldPos);
        charMngr.EnterDungeon();

        CurrentDepth++;
        LoadDungeon(true);
    }

    public void LoadDungeon(bool descent)
    {
        // Clear all chil dren of the Areas gameobject
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in Areas.transform)
            children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
        // Set character position to the stairs up or down, depending on which direction the player is moving
        charMngr.charObject.transform.position = dunGen.MainDungeonGen(activeConfig.Seed, descent);
        dunResources.ResourceGenMaster();
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
                // Disable the 'Refresh' and 'Submit' buttons
                buttons[0].interactable = false;
                buttons[1].interactable = false;
                enterButton.interactable = true;
                // Retrieve the saved name
                seedInput.text = activeConfig.SeedString;
            }
            else
            {
                // Enable input and disable enter
                seedInput.interactable = true;
                // Enable the 'Refresh' and 'Submit' buttons
                buttons[0].interactable = true;
                buttons[1].interactable = true;
                // Disable the 'Enter' button
                enterButton.interactable = false;
                // Get a generated name
                seedInput.text = MineNameGen.MakeName();
                // Get character level, skill level, and build level
                activeConfig.BaseLevel = CharacterManager.characterManager.activeChar.level;
            }
            WriteMineUIInfo();
            mineBuilder.WriteMineImprovementInfo();
        }
    }

    public void RefreshMineName ()
    {
        seedInput.text = MineNameGen.MakeName();
    }

    public void ImproveMine()
    {
        // Get the required materials and remove them from the player inventory
        InvenItem[] requiredItems = mineBuilder.GetResources(1);
        for (int i = 0; i < 3; i++)
        {
            if (requiredItems[i].Quantity != 0)
                InventoryManager.inventoryManager.RemoveFromInventory(requiredItems[i].Item.ID, requiredItems[i].Quantity);
        }
        activeConfig.BuildLevel++;
        // Refresh the UI display
        WriteMineUIInfo();
        mineBuilder.WriteMineImprovementInfo();
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
    // Display mine level and resource density info
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