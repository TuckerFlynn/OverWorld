using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Tilemaps;

/// <summary>
/// This class is used to load, hold, update and save any data related to the active
/// character. This will include the health, experience, skills, and anything
/// else that comes up.
/// </summary>
public class CharacterManager : MonoBehaviour
{
    public static CharacterManager characterManager;

    ItemsDatabase itemDB;

    public GameObject charObject;
    public Vector3 surfacePos;
    public bool InDungeon;

    public Sprite[] bodySprites;
    public Sprite[] hairSprites;

    public Character activeChar;
    public HUDBars hudBars;
    public TilemapCollider2D roofCollider;

    // Events that trigger updates to UI elements
    public event Action OnExperienceGain;
    public event Action OnLevelUp;

    private void Awake()
    {
        if (characterManager == null)
            characterManager = this;
        else if (characterManager != this)
            Destroy(this);

        bodySprites = Resources.LoadAll<Sprite>("Sprites/Character/body");
        hairSprites = Resources.LoadAll<Sprite>("Sprites/Character/hair");

        charObject.SetActive(false);
        LoadCharacter();
    }

    private void Start()
    {
        itemDB = ItemsDatabase.itemsDatabase;
    }

    private void OnDisable()
    {
        if (MapManager.mapManager != null)
        {
            ExitDungeon();
        }
        SaveCharacter();
    }
    // Used to ensure that some character has been loaded as activeChar
    public void LoadCharacter()
    {
        // if no character has been set in LoadParameters, load the first by default
        if (LoadParameters.loadParameters == null || string.IsNullOrEmpty(LoadParameters.loadParameters.activeChar.name))
        {
            // Check if character config file exists
            if (File.Exists(Application.persistentDataPath + "/characters.config"))
            {
                // Read the file and convert it to an array of Character objects
                string jsonIn = File.ReadAllText(Application.persistentDataPath + "/characters.config");
                activeChar = JsonConvert.DeserializeObject<Character[]>(jsonIn)[0];
            }
        }
        else
        {
            activeChar = LoadParameters.loadParameters.activeChar;
        }
        charObject.transform.position = new Vector3(activeChar.fieldPos.x, activeChar.fieldPos.y, 0.0f);
    }
    // Refresh character sprites (frequently called from InventoryManager)
    public void UpdateCharacter()
    {
        SpriteRenderer[] rends = charObject.GetComponentsInChildren<SpriteRenderer>();
        rends[1].sprite = bodySprites[activeChar.bodyIndex];
        rends[2].sprite = itemDB.GetItem(activeChar.equipment[0]).Sprite; // legs
        rends[3].sprite = itemDB.GetItem(activeChar.equipment[1]).Sprite; // chest
        rends[4].sprite = hairSprites[activeChar.hairIndex];
        rends[5].sprite = itemDB.GetItem(activeChar.equipment[2]).Sprite; // head
        rends[6].sprite = itemDB.GetItem(activeChar.equipment[3]).Sprite; // mainhand
        rends[7].sprite = itemDB.GetItem(activeChar.equipment[4]).Sprite; // offhand
        charObject.SetActive(true);
    }
    // Overwrite the character save with updated values
    public void SaveCharacter()
    {
        // Update the active character ...
        if (InDungeon)
            activeChar.fieldPos = new Vector2Json(surfacePos.x, surfacePos.y);
        else
            activeChar.fieldPos = new Vector2Json(charObject.transform.position.x, charObject.transform.position.y);
        activeChar.hunger = HungerManager.hungerManager.hunger;

        // ... And save the changes to the character config file
        Character[] characters;
        if (File.Exists(Application.persistentDataPath + "/characters.config"))
        {
            // Read the file and convert it to an array of Character objects
            string jsonIn = File.ReadAllText(Application.persistentDataPath + "/characters.config");
            characters = JsonConvert.DeserializeObject<Character[]>(jsonIn);
            // Check if a character with the same name already exists, and if it does- overwrite it
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i].name == activeChar.name)
                {
                    characters[i] = activeChar;
                    string jsonOut1 = JsonConvert.SerializeObject(characters, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    File.WriteAllText(Application.persistentDataPath + "/characters.config", jsonOut1);
                    Debug.Log("Character " + activeChar.name + " has been saved.");
                    return;
                }
            }
        }
    }

    // Experience is added via public method to check for level-ups
    public void AddExperience(float exp)
    {
        activeChar.experience += exp;

        if (FlynnsGlobalUtilities.ExperienceToLevel(2, activeChar.experience) > activeChar.level)
        {
            activeChar.level = FlynnsGlobalUtilities.ExperienceToLevel(2, activeChar.experience);
            activeChar.skillPoints += 4;
            SkillManager.skillManager.UpdateSkillPointCount();
            FloatingText floatTxt = FloatingText.floatingText;
            string info = string.Format("+Level Up!");
            Vector3 pos = charObject.transform.position + new Vector3(0, -0.1f);
            floatTxt.CreateText(info, pos, 1.0f);

            if (OnLevelUp != null)
                OnLevelUp();
        }
        // Lowering experience is only possible via commands, but this handles it properly!
        if (FlynnsGlobalUtilities.ExperienceToLevel(2, activeChar.experience) < activeChar.level)
        {
            activeChar.level = FlynnsGlobalUtilities.ExperienceToLevel(2, activeChar.experience);
            FloatingText floatTxt = FloatingText.floatingText;
            string info = "Level down? U Suck.";
            Vector3 pos = charObject.transform.position + new Vector3(0, -0.1f);
            floatTxt.CreateText(info, pos, 1.0f);
        }

        // Publish the OnExperienceGain event (after potentially leveling up)
        if (OnExperienceGain != null)
            OnExperienceGain();
    }

    // Save the surface position when entering a dungeon, and reset the character position to the saved point when exiting
    public void EnterDungeon()
    {
        InDungeon = true;
        surfacePos = charObject.transform.position;

        roofCollider.isTrigger = false;
    }
    public void ExitDungeon()
    {
        if (InDungeon)
        {
            InDungeon = false;
            charObject.transform.position = surfacePos;

            roofCollider.isTrigger = true;
        }
    }
}