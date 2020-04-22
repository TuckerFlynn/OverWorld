using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class LoadMenu_2 : MonoBehaviour
{
    public Character[] characters;
    public Character activeChar;
    [Header("UI ELEMENTS")]
    public GameObject CharButtonPrefab;
	public GameObject ScrollviewContent;
    public ToggleGroup toggleGroup;
    GameObject[] CharButtons;
    [Header("MENU SCRIPTS")]
    public MainMenu_2 mainMenuScript;


    ItemsDatabase itemsDB;

    private Sprite[] bodySprites;
    private Sprite[] hairSprites;

    private void Awake()
    {
        bodySprites = Resources.LoadAll<Sprite>("Sprites/Character/body");
        hairSprites = Resources.LoadAll<Sprite>("Sprites/Character/hair");
    }

    void Start()
    {
        itemsDB = ItemsDatabase.itemsDatabase;
        UpdateCharacterList();
    }
    // Display the list of all saved character
    public void UpdateCharacterList ()
    {
        EraseCharacterList();

        // Check if character config file exists
        if (File.Exists(Application.persistentDataPath + "/characters.config"))
        {
            // Read the file and convert it to an array of Character objects
            string jsonIn = File.ReadAllText(Application.persistentDataPath + "/characters.config");
            characters = JsonConvert.DeserializeObject<Character[]>(jsonIn);
        }
        if (characters != null)
        {
            // Set length of array to store buttons
            CharButtons = new GameObject[characters.Length];
            // Set viewport content to fit the number of characters shown
            Vector2 delta = ScrollviewContent.GetComponent<RectTransform>().sizeDelta;
            delta = new Vector2(delta.x, characters.Length * 90.0f);
            ScrollviewContent.GetComponent<RectTransform>().sizeDelta = delta;

            for (int i = 0; i < characters.Length; i++)
            {
                Character character = characters[i];
                // Check the PlayerPrefs for a record of the last played character, and if so try to set that character as the activeChar
                if (PlayerPrefs.HasKey("LastCharacter") && character.name == PlayerPrefs.GetString("LastCharacter"))
                {
                    Debug.Log(string.Format("Last played character ({0}) loaded from PlayerPrefs", PlayerPrefs.GetString("LastCharacter")));
                    activeChar = character;
                    LoadParameters.loadParameters.activeChar = activeChar;
                }
                // Add a character preview to the scrollview
                GameObject preview = Instantiate(CharButtonPrefab, ScrollviewContent.transform);
                PreviewSetup(preview, character);
                CharButtons[i] = preview;
            }
        }
    }
    // Permanently remove a saved character, and then update the display
    public void DeleteCharacter()
    {
        for (int i = 0; i < CharButtons.Length; i++)
        {
            if (CharButtons[i].GetComponent<Toggle>().isOn)
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
                break;
            }
        }

        UpdateCharacterList();
    }
    // Remove all Character Buttons from the display
    void EraseCharacterList()
    {
        // Destroy all buttons and then refresh character list
        foreach (Transform child in ScrollviewContent.transform)
        {
            Destroy(child.gameObject);
        }
    }
    // Try to start a game with the selected character; if there is no saved world map the user will be redirected to the new world menu
    public void StartGame()
    {
        for (int i = 0; i < CharButtons.Length; i++)
        {
            if (CharButtons[i].GetComponent<Toggle>().isOn)
            {
                LoadParameters.loadParameters.activeChar = characters[i];
                break;
            }
        }
        // If the play button is clicked without selecting a character, open the character creation window
        if (string.IsNullOrWhiteSpace(LoadParameters.loadParameters.activeChar.name))
        {
            mainMenuScript.OnNewCharacterButton();
            return;
        }
        if (File.Exists(Application.persistentDataPath + "/worldMap.dat"))
        {
            PlayerPrefs.SetString("LastCharacter", LoadParameters.loadParameters.activeChar.name);
            SceneManager.LoadSceneAsync("MapManager");
        }
        else
        {
            mainMenuScript.OnNewWorldButton();
        }
    }

    // Set the character button display options based on the Character object
    void PreviewSetup (GameObject preview, Character character)
    {
        preview.GetComponent<Toggle>().group = toggleGroup;

        Transform charPreview = preview.transform.GetChild(1);
        charPreview.GetChild(0).gameObject.GetComponent<Image>().sprite = bodySprites[character.bodyIndex];
        charPreview.GetChild(1).gameObject.GetComponent<Image>().sprite = itemsDB.GetItem(character.equipment[0]).Sprite;
        charPreview.GetChild(2).gameObject.GetComponent<Image>().sprite = itemsDB.GetItem(character.equipment[1]).Sprite;
        charPreview.GetChild(3).gameObject.GetComponent<Image>().sprite = hairSprites[character.hairIndex];

        Text txt = preview.transform.GetChild(2).gameObject.GetComponent<Text>();
        StringBuilder builder = new StringBuilder();
        builder.Append("<size=12>").Append(character.name).Append("</size>").AppendLine();
        builder.Append("level ").Append(character.level).AppendLine();
        builder.Append("<size=8>date created: ").Append(character.date).Append("</size>");
        txt.text = builder.ToString();
    }
}