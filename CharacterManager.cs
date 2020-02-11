using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

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

    public Sprite[] bodySprites;
    public Sprite[] hairSprites;

    public Character activeChar;

    private void Awake()
    {
        if (characterManager == null)
        {
            DontDestroyOnLoad(gameObject);
            characterManager = this;
        }
        else if (characterManager != this)
        {
            Destroy(this.gameObject);
        }
        bodySprites = Resources.LoadAll<Sprite>("Sprites/Character/body");
        hairSprites = Resources.LoadAll<Sprite>("Sprites/Character/hair");

        charObject.SetActive(false);
    }

    private void Start()
    {
        itemDB = ItemsDatabase.itemsDatabase;
    }

    private void OnDisable()
    {
        SaveCharacter();
    }
    // Used to ensure that some character has been loaded as activeChar
    public void LoadCharacter()
    {
        // if no character has been loaded, load the first by default
        if (activeChar.name == "")
        {
            // Check if character config file exists
            if (File.Exists(Application.persistentDataPath + "/characters.config"))
            {
                // Read the file and convert it to an array of Character objects
                string jsonIn = File.ReadAllText(Application.persistentDataPath + "/characters.config");
                activeChar = JsonConvert.DeserializeObject<Character[]>(jsonIn)[0];
            }
        }
        charObject.transform.position = new Vector3(activeChar.fieldPos.x, activeChar.fieldPos.y, 0.0f);
        UpdateCharacter();
    }
    // Refresh character sprites (frequently called from InventoryManager)
    public void UpdateCharacter()
    {
        SpriteRenderer[] rends = charObject.GetComponentsInChildren<SpriteRenderer>();
        if (rends.Length == 7)
        {
            rends[0].sprite = bodySprites[activeChar.bodyIndex];
            rends[1].sprite = itemDB.GetItem(activeChar.equipment[0]).Sprite; // legs
            rends[2].sprite = itemDB.GetItem(activeChar.equipment[1]).Sprite; // chest
            rends[3].sprite = hairSprites[activeChar.hairIndex];
            rends[4].sprite = itemDB.GetItem(activeChar.equipment[2]).Sprite; // head
            rends[5].sprite = itemDB.GetItem(activeChar.equipment[3]).Sprite; // mainhand
            rends[6].sprite = itemDB.GetItem(activeChar.equipment[4]).Sprite; // offhand
        }
        charObject.SetActive(true);
    }

    public void SaveCharacter()
    {
        // Update the active character ...
        activeChar.fieldPos = new Vector2Json(charObject.transform.position.x, charObject.transform.position.y);
        // Set the dungeonSeed back to zero ( no saving in dungeons, character is sent back to the surface )
        activeChar.dungeonSeed = 0;

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
}