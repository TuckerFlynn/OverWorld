using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;
using Inventory;
using UnityEngine.Tilemaps;

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

            // Build and load all tilesets from the JSON files. This is run before building the items database because some items require Tiles
            if (!TilesetLoader.Loaded)
                TilesetLoader.LoadTiles();

            string JsonIn = Resources.Load<TextAsset>("Json/items").text;
            // Allows deserializing into multiple classes based on the $type value in the Json file
            TypeNameSerializationBinder binder = new TypeNameSerializationBinder
            {
                KnownTypes = new List<Type> { typeof(Item), typeof(Mainhand), typeof(Consumable), typeof(Building) }
            };
            Item[] items = JsonConvert.DeserializeObject<Item[]>(JsonIn, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = binder
            });

            for (int i = 0; i < items.Length; i++)
            {
                ItemDatabase.Add(items[i]);
            }
            for (int i = 0; i < ItemDatabase.Count; i++)
            {
                // Set the inventory display sprite
                Sprite[] sheet = Resources.LoadAll<Sprite>("Sprites/Items/" + ItemDatabase[i].SpriteName);
                ItemDatabase[i].Sprite = sheet[ItemDatabase[i].SpriteID];

                if (ItemDatabase[i] is Building building)
                {
                    // Some placeable items will be props tiles and some will be ground tiles, need to check in order to load the correct tilebase
                    if (building.Tileset == "PropTiles" || building.Tileset == "PlantTiles")
                        building.tileBase = TilesetLoader.GetTilesetByString<List<ObjTile>>(building.Tileset)[building.TilesetID] as TileBase;
                    else
                        building.tileBase = TilesetLoader.GetTilesetByString<List<EnvrTile>>(building.Tileset)[building.TilesetID] as TileBase;
                }
            }
            Debug.Log("ItemDatabase built with " + ItemDatabase.Count + " items.");
        }
        else if (itemsDatabase != this)
        {
            Destroy(this.gameObject);
        }
    }
    // Get item matches the ID value, while the item index within the array SHOULD be the same, it is not garaunteed
    public Item GetItem(int id)
    {
        // This is iffy, may cause unpredicted results, but for now it prevents errors when trying to request item with ID=0
        if (id == 0)
        {
            return new Item();
        }
        if (id > ItemDatabase.Count)
        {
            Debug.Log("Item ID " + id + " outside of ItemDatabase range");
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

    // Use to rewrite the items.json file if widespread internal class changes have been made an need to be applied

    //private void OnDisable()
    //{
    //    // Allows preserving multiple classes through serialization by setting the $type value
    //    TypeNameSerializationBinder binder = new TypeNameSerializationBinder
    //    {
    //        KnownTypes = new List<Type> { typeof(Item), typeof(Consumable) }
    //    };

    //    string jsonOut = JsonConvert.SerializeObject(ItemDatabase, Formatting.Indented, new JsonSerializerSettings
    //    {
    //        TypeNameHandling = TypeNameHandling.Objects,
    //        SerializationBinder = binder
    //    });

    //    File.WriteAllText(Application.persistentDataPath + "/items.json", jsonOut);
    //}
}

namespace Inventory
{
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
        // ToString is automatically called when logging an object, by default it will just give the class name so this makes debugging easier
        public override string ToString()
        {
            return Slug + " (ID: " + ID + ")";
        }
    }
    [Serializable]
    public class Mainhand : Item
    {
        /// <summary>
        /// 0 = none, 1 = axe, 2 = pickaxe, 3 = shovel, 4 = melee weapon, 5 = range weapon
        /// </summary>
        public int DmgType { get; set; }
        public float Power { get; set; }
    }
    [Serializable]
    public class Consumable : Item
    {
        public List<StatusEffect> Effects = new List<StatusEffect>();
    }
    [Serializable]
    public class StatusEffect
    {
        public string Status { get; set; }
        public bool Discrete { get; set; }
        public float Effect { get; set; }
        public float Time { get; set; }
    }
    [Serializable]
    public class Building : Item
    {
        public bool isBuilding { get; set; }
        public float Durability { get; set; }
        public string Tileset { get; set; }
        public int TilesetID { get; set; }
        public string Target { get; set; }
        [JsonIgnore]
        public TileBase tileBase { get; set; }
    }
}