using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;

/// <summary>
/// The ItemDatabase list contains the complete list of all Item class items available in the game
/// </summary>
public class ItemsDatabase : MonoBehaviour
{
    public static ItemsDatabase itemsDatabase;
    [SerializeField]
    public List<Item> ItemDatabase = new List<Item>();

    void Awake()
    {
        if (itemsDatabase == null)
        {
            DontDestroyOnLoad(gameObject);
            itemsDatabase = this;
        }
        else if (itemsDatabase != this)
        {
            Destroy(this.gameObject);
        }

        string JsonIn = Resources.Load<TextAsset>("Json/items").text;
        Item[] items = JsonConvert.DeserializeObject<Item[]>(JsonIn);
        for (int i = 0; i < items.Length; i++)
        {
            ItemDatabase.Add(items[i]);
        }
        for (int i = 0; i < ItemDatabase.Count; i++)
        {
            Sprite[] sheet = Resources.LoadAll<Sprite>("Sprites/Items/" + ItemDatabase[i].SpriteName);
            ItemDatabase[i].Sprite = sheet[ItemDatabase[i].SpriteID];
        }
        Debug.Log("ItemDatabase built with " + ItemDatabase.Count + " items.");
    }
    // Get item matches the ID value, while the item index within the array SHOULD be the same, it is not garaunteed
    public Item GetItem(int id)
    {
        // This is iffy, may cause unpredicted results, but for now it prevents errors when trying to request item with ID=0
        if (id == 0)
        {
            return new Item();
        }

        return ItemDatabase.Find(item => item.ID == id);
    }

    public Item GetItem(string slug)
    {
        // This is iffy, may cause unpredicted results, but for now it prevents errors when trying to request item with ID=0
        if (slug == "")
        {
            return new Item();
        }

        return ItemDatabase.Find(item => item.Slug == slug);
    }

    // Use to rewrite the items.json file if internal class changes have been made an need to be applied
    //private void OnDisable()
    //{
    //    string jsonOut = JsonConvert.SerializeObject(ItemDatabase, Formatting.Indented);
    //    File.WriteAllText(Application.persistentDataPath + "/items.json", jsonOut);
    //}
}

/// <summary>
/// Object to hold all generic game items; has unique ID and Slug and non-unique Title
/// </summary>
[Serializable]
public class Item
{
    public int ID { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    // SpriteName and SpriteID instruct where to find the item's sprite
    public string SpriteName { get; set; }
    public int SpriteID { get; set; }
    [JsonIgnore]
    public Sprite Sprite;
    // Determines what equipment slot an item can be placed in
    public int Equip { get; set; }
    // Whether or not the item can be stacked
    public bool Unique { get; set; }
    public int Stack { get; set; }

    public Item()
    {
        this.ID = 0;
        this.Stack = 1;
    }
    // ToString is automatically called when logging an object, by default it will just give the class name
    public override string ToString()
    {
        return Slug + " (ID: " + ID + ")";
    }
}