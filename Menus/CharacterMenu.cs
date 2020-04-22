using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Inventory;

public class CharacterMenu : MonoBehaviour
{
    ItemsDatabase itemsDB;

    Sprite[] bodySprites;
    List<Item> chestItems = new List<Item>();
    List<Item> legItems = new List<Item>();
    Sprite[] hairSprites;

    public InputField playerName;

    [Header("CHARACTER SPRITES")]
    public Image body;
    int b;
    public Image chest;
    int localC;
    int c;
    public Image legs;
    int localL;
    int l;
    public Image hair;
    int h;


    void Awake()
    {
        bodySprites = Resources.LoadAll<Sprite>("Sprites/Character/body");
        hairSprites = Resources.LoadAll<Sprite>("Sprites/Character/hair");
    }

    void Start()
    {
        itemsDB = ItemsDatabase.itemsDatabase;

        for (int i = 0; i < itemsDB.ItemDatabase.Count; i++)
        {
            // Find starter chest items by their generic titles, this excludes any armours
            string[] chests = { "Belted Shirt", "Tunic", "Blouse", "Overalls", "Dress", "Shirt", "Croptop"};
            if ( Array.Exists(chests, item => item == itemsDB.ItemDatabase[i].Title))
            {
                chestItems.Add(itemsDB.ItemDatabase[i]);
            }
            // Like above but for legs
            string[] leg = { "Pants", "Boots", "Slippers" };
            if (Array.Exists(leg, item => item == itemsDB.ItemDatabase[i].Title))
            {
                legItems.Add(itemsDB.ItemDatabase[i]);
            }
        }
        RandomChar();
    }

    int NextImg(Sprite[] sprites, Image img, int index, bool next)
    {
        // Get the index of the next image to load
        int i = next ? 1 : -1;
        if (index + i >= sprites.Length)
            index = 0;
        else if (index + i < 0)
            index = sprites.Length - 1;
        else
            index += i;

        img.sprite = sprites[index];
        return index;
    }
    // Loop through a shortened list of items
    int NextItem(List<Item> items, Image img, int index, bool next)
    {
        // Get the index of the next item to load
        int i = next ? 1 : -1;
        if (index + i >= items.Count)
            index = 0;
        else if (index + i < 0)
            index = items.Count - 1;
        else
            index += i;

        img.sprite = items[index].Sprite;
        return index;
    }

    public void NextBody(bool next)
    {
        b = NextImg(bodySprites, body, b, next);
    }
    public void NextChest(bool next)
    {
        localC = NextItem(chestItems, chest, localC, next);
        c = chestItems[localC].ID;
    }
    public void NextLegs(bool next)
    {
        localL = NextItem(legItems, legs, localL, next);
        l = legItems[localL].ID;
    }
    public void NextHair(bool next)
    {
        h = NextImg(hairSprites, hair, h, next);
    }
    public void RandomChar()
    {
        b = UnityEngine.Random.Range(0, bodySprites.Length);
        body.sprite = bodySprites[b];

        localC = UnityEngine.Random.Range(0, chestItems.Count);
        c = chestItems[localC].ID;
        chest.sprite = chestItems[localC].Sprite;

        localL = UnityEngine.Random.Range(0, legItems.Count);
        l = legItems[localL].ID;
        legs.sprite = legItems[localL].Sprite;

        h = UnityEngine.Random.Range(0, hairSprites.Length);
        hair.sprite = hairSprites[h];
    }

    public void Accept()
    {
        // Check the name input is not empty
        if (string.IsNullOrWhiteSpace(playerName.text))
        {
            playerName.text = "DefAuLtNaME!!!1";
        }
        Character character = new Character
        {
            name = playerName.text,
            bodyIndex = b,
            hairIndex = h
        };
        character.equipment[0] = l;
        character.equipment[1] = c;

        SaveCharacter(character);
    }

    void SaveCharacter (Character character)
    {
        Character[] characters;
        // Check if the character config file already exists
        if (File.Exists(Application.persistentDataPath + "/characters.config"))
        {
            // Read the file and convert it to an array of Character objects
            string jsonIn = File.ReadAllText(Application.persistentDataPath + "/characters.config");
            characters = JsonConvert.DeserializeObject<Character[]>(jsonIn);
            // Check if a character with the same name already exists, and if it does- overwrite it
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i].name == character.name)
                {
                    characters[i] = character;
                    string jsonOut1 = JsonConvert.SerializeObject(characters, Formatting.Indented);
                    File.WriteAllText(Application.persistentDataPath + "/characters.config", jsonOut1);
                    Debug.Log("Character " + character.name + " has been overwritten.");
                    return;
                }
            }

            // If the function runs to here, there is no character with the same name to overwrite, so save a new one
            Character[] newCharacters = new Character[characters.Length + 1];
            // Add all old characters to the new array
            for (int i = 0; i < characters.Length; i++)
            {
                newCharacters[i] = characters[i];
            }
            // And add the new character to the end of the array
            newCharacters[newCharacters.Length - 1] = character;
            string jsonOut2 = JsonConvert.SerializeObject(newCharacters, Formatting.Indented);
            File.WriteAllText(Application.persistentDataPath + "/characters.config", jsonOut2);
            Debug.Log("New character " + character.name + " has been saved.");
        }
        // If there is no character config, make one and save the new character
        else
        {
            characters = new Character[] { character };
            string jsonOut = JsonConvert.SerializeObject(characters, Formatting.Indented);
            File.WriteAllText(Application.persistentDataPath + "/characters.config", jsonOut);
            Debug.Log("Config created and new character " + character.name + " has been saved.");
        }
    }
}
