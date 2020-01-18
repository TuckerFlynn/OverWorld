using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterMenu : MonoBehaviour
{
    private Sprite[] bodySprites;
    private Sprite[] chestSprites;
    private Sprite[] legSprites;
    private Sprite[] hairSprites;

    public Image body;
    int b;
    public Image chest;
    int c;
    public Image legs;
    int l;
    public Image hair;
    int h;

    public InputField playerName;

    void Awake()
    {
        bodySprites = Resources.LoadAll<Sprite>("Sprites/Character/body");
        chestSprites = Resources.LoadAll<Sprite>("Sprites/Character/chest");
        legSprites = Resources.LoadAll<Sprite>("Sprites/Character/legs");
        hairSprites = Resources.LoadAll<Sprite>("Sprites/Character/hair");
    }

    private void Start()
    {
        RandomChar();
    }

    int nextImg(Sprite[] sprites, Image img, int index, bool next)
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
    public void nextBody(bool next)
    {
        b = nextImg(bodySprites, body, b, next);
    }
    public void nextChest(bool next)
    {
        c = nextImg(chestSprites, chest, c, next);
    }
    public void nextLegs(bool next)
    {
        l = nextImg(legSprites, legs, l, next);
    }
    public void nextHair(bool next)
    {
        h = nextImg(hairSprites, hair, h, next);
    }
    public void RandomChar()
    {
        b = Random.Range(0, bodySprites.Length);
        body.sprite = bodySprites[b];
        c = Random.Range(0, chestSprites.Length);
        chest.sprite = chestSprites[c];
        l = Random.Range(0, legSprites.Length);
        legs.sprite = legSprites[l];
        h = Random.Range(0, hairSprites.Length);
        hair.sprite = hairSprites[h];
    }

    public void Back()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void Accept()
    {
        if (playerName.text != "")
        {
            Character character = new Character()
            {
                name = playerName.text,
                bodyIndex = b,
                chestIndex = c,
                legsIndex = l,
                hairIndex = h
            };

            SaveCharacter(character);
        }
    }

    void SaveCharacter (Character character)
    {
        Character[] characters;
        // Check if the character config file already exists
        if (File.Exists(Application.persistentDataPath + "/characters.config"))
        {
            // Read the file and convert it to an array of Character objects
            string jsonIn = File.ReadAllText(Application.persistentDataPath + "/characters.config");
            characters = JsonHelper.FromJson<Character>(jsonIn);
            // Check if a character with the same name already exists, and if it does- overwrite it
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i].name == character.name)
                {
                    characters[i] = character;
                    string jsonOut1 = JsonHelper.ToJson(characters, true);
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
            string jsonOut2 = JsonHelper.ToJson(newCharacters, true);
            File.WriteAllText(Application.persistentDataPath + "/characters.config", jsonOut2);
            Debug.Log("New character " + character.name + " has been saved.");
        }
        // If there is no character config, make one and save the new character
        else
        {
            characters = new Character[] { character };
            string jsonOut = JsonHelper.ToJson(characters, true);

            File.WriteAllText(Application.persistentDataPath + "/characters.config", jsonOut);
            Debug.Log("Config created and new character " + character.name + " has been saved.");
        }
    }
}
