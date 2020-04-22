using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class LoadMenu_2 : MonoBehaviour
{
    public Character[] characters;
    public Character activeChar;
    [Header("UI ELEMENTS")]
    public GameObject CharButtonPrefab;
	public GameObject ScrollviewContent;
    public ToggleGroup toggleGroup;

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
        // Check if character config file exists
        if (File.Exists(Application.persistentDataPath + "/characters.config"))
        {
            // Read the file and convert it to an array of Character objects
            string jsonIn = File.ReadAllText(Application.persistentDataPath + "/characters.config");
            characters = JsonConvert.DeserializeObject<Character[]>(jsonIn);
        }
        if (characters != null)
        {
            // Set viewport content to fit the number of characters shown
            Vector2 delta = ScrollviewContent.GetComponent<RectTransform>().sizeDelta;
            delta = new Vector2(delta.x, characters.Length * 90.0f);
            ScrollviewContent.GetComponent<RectTransform>().sizeDelta = delta;

            foreach (Character character in characters)
            {
                // Check the PlayerPrefs for a record of the last played character, and if so try to set that character as the activeChar
                if (PlayerPrefs.HasKey("LastCharacter") && character.name == PlayerPrefs.GetString("LastCharacter"))
                {
                    activeChar = character;
                    LoadParameters.loadParameters.activeChar = activeChar;
                }
                // Add a character preview to the scrollview
                GameObject preview = Instantiate(CharButtonPrefab, ScrollviewContent.transform);
                PreviewSetup(preview, character);
            }
        }
    }

    void PreviewSetup (GameObject preview, Character character)
    {
        Transform charPreview = preview.transform.GetChild(1);
        preview.GetComponent<Toggle>().group = toggleGroup;
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