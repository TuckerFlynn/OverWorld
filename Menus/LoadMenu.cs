using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class LoadMenu : MonoBehaviour
{
    ItemsDatabase itemDB;

    public GameObject buttonToggle;
    public GameObject viewportContent;
    public Character[] characters;
    public GameObject[] buttons;

    private Sprite[] bodySprites;
    private Sprite[] hairSprites;

    private void Awake()
    {
        bodySprites = Resources.LoadAll<Sprite>("Sprites/Character/body");
        hairSprites = Resources.LoadAll<Sprite>("Sprites/Character/hair");
    }

    void Start()
    {
        itemDB = ItemsDatabase.itemsDatabase;
        RefreshCharacters();
    }

    public void Back()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void DeleteCharacter()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].GetComponent<Toggle>().isOn)
            {
                Character[] newCharacters = new Character[characters.Length - 1];
                int adj = 0;
                for (int f = 0; f < characters.Length; f++)
                {
                    if (f == i)
                    {
                        adj = -1;
                        continue;
                    }
                    newCharacters[f + adj] = characters[f];
                }
                string jsonOut = JsonConvert.SerializeObject(newCharacters, Formatting.Indented);
                File.WriteAllText(Application.persistentDataPath + "/characters.config", jsonOut);
                Debug.Log("Character " + characters[i].name + " has been deleted. RIP.");
            }
        }
        // Destroy all buttons and then refresh character list
        foreach (Transform child in viewportContent.transform)
        {
            Destroy(child.gameObject);
        }
        RefreshCharacters();
    }

    void RefreshCharacters()
    {
        // Check if character config file exists
        if (File.Exists(Application.persistentDataPath + "/characters.config"))
        {
            // Read the file and convert it to an array of Character objects
            string jsonIn = File.ReadAllText(Application.persistentDataPath + "/characters.config");
            characters = JsonConvert.DeserializeObject<Character[]>(jsonIn);
        }

        if (characters.Length > 0)
        {
            // Set viewport content to fit the number of characters shown
            Vector2 delta = viewportContent.GetComponent<RectTransform>().sizeDelta;
            delta = new Vector2(delta.x, characters.Length * 90.0f);
            viewportContent.GetComponent<RectTransform>().sizeDelta = delta;
            // Set the size of array to hold gameObject
            buttons = new GameObject[characters.Length];

            for (int i = 0; i < characters.Length; i++)
            {
                GameObject preview = Instantiate(buttonToggle);
                preview.transform.SetParent(viewportContent.transform, false);

                Transform charPreview = preview.transform.GetChild(1);
                preview.GetComponent<Toggle>().group = viewportContent.GetComponent<ToggleGroup>();
                charPreview.GetChild(0).gameObject.GetComponent<Image>().sprite = bodySprites[characters[i].bodyIndex];
                charPreview.GetChild(1).gameObject.GetComponent<Image>().sprite = itemDB.GetItem(characters[i].equipment[0]).Sprite;
                charPreview.GetChild(2).gameObject.GetComponent<Image>().sprite = itemDB.GetItem(characters[i].equipment[1]).Sprite;
                charPreview.GetChild(3).gameObject.GetComponent<Image>().sprite = hairSprites[characters[i].hairIndex];

                Text txt = preview.transform.GetChild(2).gameObject.GetComponent<Text>();
                txt.text = characters[i].name;
                // save the button to this array to easily grab it in other methods
                buttons[i] = preview;
            }
        }
    }

    public void PlayPlay()
    {
        if (File.Exists(Application.persistentDataPath + "/worldMap.dat"))
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].GetComponent<Toggle>().isOn)
                {
                    CharacterManager charMngr = CharacterManager.characterManager;
                    charMngr.activeChar = characters[i];
                    break;
                }
            }
            SceneManager.LoadScene("MapManager");
        }
        else
        {
            SceneManager.LoadScene("WorldGenMenu");
        }
    }
}