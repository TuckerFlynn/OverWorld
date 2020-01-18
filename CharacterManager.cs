using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// This class is used to load, hold, update and save any data related to the active
/// character. This will include the inventory, health, experience, skills, and anything
/// else that comes up.
/// </summary>
public class CharacterManager : MonoBehaviour
{
    public static CharacterManager characterManager;

    public GameObject charObject;

    public Sprite body;
    public Sprite legs;
    public Sprite chest;
    public Sprite hair;
    public Sprite head;
    public Sprite mainHand;
    public Sprite offHand;

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
    }

    private void OnDisable()
    {
        SaveCharacter();
    }

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
                activeChar = JsonHelper.FromJson<Character>(jsonIn)[0];
            }
        }
        SpriteRenderer[] rends = charObject.GetComponentsInChildren<SpriteRenderer>();
        if (rends.Length == 7)
        {
            rends[0].sprite = body;
            rends[1].sprite = legs;
            rends[2].sprite = chest;
            rends[3].sprite = hair;
            rends[4].sprite = head;
            rends[5].sprite = mainHand;
            rends[6].sprite = offHand;
        }
        charObject.transform.position = new Vector3(activeChar.fieldPos.x, activeChar.fieldPos.y, 0.0f);
    }

    public void SaveCharacter()
    {
        // Update the active character ...
        activeChar.fieldPos = new Vector2(charObject.transform.position.x, charObject.transform.position.y);

        // ... And save the changes to the character config file
        Character[] characters;
        if (File.Exists(Application.persistentDataPath + "/characters.config"))
        {
            // Read the file and convert it to an array of Character objects
            string jsonIn = File.ReadAllText(Application.persistentDataPath + "/characters.config");
            characters = JsonHelper.FromJson<Character>(jsonIn);
            // Check if a character with the same name already exists, and if it does- overwrite it
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i].name == activeChar.name)
                {
                    characters[i] = activeChar;
                    string jsonOut1 = JsonHelper.ToJson(characters, true);
                    File.WriteAllText(Application.persistentDataPath + "/characters.config", jsonOut1);
                    Debug.Log("Character " + activeChar.name + " has been saved.");
                    return;
                }
            }
        }
    }
}
