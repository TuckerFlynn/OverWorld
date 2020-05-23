using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public static class TilesetLoader
{
    public static bool Loaded = false;

    public static List<EnvrTile> GroundTiles = new List<EnvrTile>();
    public static List<ObjTile> PlantTiles = new List<ObjTile>();
    public static List<ObjTile> PropTiles = new List<ObjTile>();
    public static List<EnvrTile> BuildingTiles = new List<EnvrTile>();
    public static List<EnvrTile> DungeonTiles = new List<EnvrTile>();
    public static List<ObjTile> DungeonPropTiles = new List<ObjTile>();

    private static TilesetJson tileset;
    private static Sprite[] spriteSheet;

    public static void LoadTiles()
    {
        // Get array of all tilesets
        TextAsset[] files = Resources.LoadAll<TextAsset>("Tilesets/Constructors");
        for (int i = 0; i < files.Length; i++)
        {
            // Loop through each tileset to build the contained tiles
            tileset = JsonUtility.FromJson<TilesetJson>(files[i].text);
            spriteSheet = Resources.LoadAll<Sprite>("Tilesets/Spritesheets/" + tileset.name);

            // Each tileset is compiled slightly differently, so the tileset 'name' property is used to determine which compiler is needed
            if (tileset.name == "ground")
                LoadGroundTiles();
            if (tileset.name == "plants")
                LoadPlantTiles();
            if (tileset.name == "props")
                LoadPropTiles();
            if (tileset.name == "buildings")
                LoadBuildingTiles();
            if (tileset.name == "dungeon")
                LoadDungeonTiles();
            if (tileset.name == "dungeonProps")
                LoadDungeonPropTiles();

        }
        Debug.Log("Loaded " + GroundTiles.Count + " environment tiles and " + PlantTiles.Count + " plant tiles");
        Debug.Log("Loaded " + PropTiles.Count + " prop tiles and " + BuildingTiles.Count + " building tiles");
        Debug.Log("Loaded " + DungeonTiles.Count + " dungeon tiles and " + DungeonPropTiles.Count + " dungeon prop tiles");
        Loaded = true;
    }

    // Loads the Ground tileset as envrTiles or envrAdvTiles
    static void LoadGroundTiles()
    {
        for (int t = 0; t < spriteSheet.Length; t++)
        {
            // Loop through tiles, checking the tile's type to know how to create it
            JsonTile jsonTile = tileset.tiles[t];
            if (jsonTile.type == "envrTile")
            {
                // Tile asset to build
                EnvrTile toAdd = ScriptableObject.CreateInstance<EnvrTile>();
                toAdd.DefaultSprite = spriteSheet[t];
                // Loop through properties on tile
                for (int p = 0; p < jsonTile.properties.Length; p++)
                {
                    JsonTileProperty jsonProperty = jsonTile.properties[p];
                    if (jsonProperty.name == "moveCost") toAdd.moveCost = float.Parse(jsonProperty.value);
                    if (jsonProperty.name == "colliderType") toAdd.DefaultColliderType = (Tile.ColliderType)int.Parse(jsonProperty.value);
                    if (jsonProperty.name == "group")
                        toAdd.group = jsonProperty.value;
                }
                GroundTiles.Add(toAdd);
            }
            // NOTE: envrAdvTiles are not built at runtime but loaded from Resources by the name property
            else if (jsonTile.type == "envrAdvTile")
            {
                string name = "";
                for (int p = 0; p < jsonTile.properties.Length; p++)
                {
                    if (jsonTile.properties[p].name == "name")
                    {
                        name = jsonTile.properties[p].value;
                    }
                }
                EnvrAdvTile toAdd = Resources.Load<EnvrAdvTile>("Tilesets/Ground/" + name) as EnvrAdvTile;
                GroundTiles.Add(toAdd);
            }
        }
    }
    // Loads the Plants tileset as objTiles
    static void LoadPlantTiles()
    {
        for (int t = 0; t < spriteSheet.Length; t++)
        {
            // Plant tiles build as ObjTiles
            JsonTile jsonTile = tileset.tiles[t];
            if (jsonTile.type == "objTile")
            {
                ObjTile toAdd = ScriptableObject.CreateInstance<ObjTile>();
                toAdd.DefaultSprite = spriteSheet[t];
                // Loop through properties on tile
                for (int p = 0; p < jsonTile.properties.Length; p++)
                {
                    JsonTileProperty jsonProperty = jsonTile.properties[p];
                    if (jsonProperty.name == "colliderType")
                    {
                        toAdd.DefaultColliderType = (Tile.ColliderType)int.Parse(jsonProperty.value);
                    }
                    // Add gameobjects if the property is set to true
                    if (jsonProperty.name == "gameobject")
                    {
                        bool.TryParse(jsonProperty.value, out bool hasGameObject);

                        if (hasGameObject)
                        {
                            if (Resources.Load<GameObject>("Tilesets/Gameobjects/PlantObjects/plants_" + t) != null)
                            {
                                // Test for solving reference issue
                                //GameObject go = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Tilesets/Gameobjects/plants_" + t));
                                //toAdd.DefaultGameObject = go;
                                toAdd.DefaultGameObject = Resources.Load<GameObject>("Tilesets/Gameobjects/PlantObjects/plants_" + t);
                            }
                            else
                            {
                                toAdd.DefaultGameObject = Resources.Load<GameObject>("Tilesets/Gameobjects/genericSmolObstacle");
                            }
                        }

                    }
                    if (jsonProperty.name == "group") toAdd.group = jsonProperty.value;
                }
                PlantTiles.Add(toAdd);
            }
        }
    }
    // Loads the props tileset as objTiles
    static void LoadPropTiles()
    {
        for (int t = 0; t < spriteSheet.Length; t++)
        {
            // Prop tiles, also built as ObjTiles
            JsonTile jsonTile = tileset.tiles[t];
            if (jsonTile.type == "objTile")
            {
                ObjTile toAdd = ScriptableObject.CreateInstance<ObjTile>();
                toAdd.DefaultSprite = spriteSheet[t];

                // Name property is used in conjunction with the gameobject property to assign specific non-unique gameobjects, so it has to be found outside the loop of properties
                JsonTileProperty name = Array.Find(jsonTile.properties, p => p.name == "name");

                // Loop through properties on tile
                for (int p = 0; p < jsonTile.properties.Length; p++)
                {
                    JsonTileProperty jsonProperty = jsonTile.properties[p];
                    // Set the collider type as none, sprite, or grid
                    if (jsonProperty.name == "colliderType")
                    {
                        toAdd.DefaultColliderType = (Tile.ColliderType)int.Parse(jsonProperty.value);
                    }
                    // Add gameobjects if the property is set to true
                    if (jsonProperty.name == "gameobject")
                    {
                        bool.TryParse(jsonProperty.value, out bool hasGameObject);

                        if (hasGameObject)
                        {
                            if (Resources.Load<GameObject>("Tilesets/Gameobjects/props_" + t) != null)
                            {
                                toAdd.DefaultGameObject = Resources.Load<GameObject>("Tilesets/Gameobjects/props_" + t);
                            }
                            else if (name != null && !String.IsNullOrWhiteSpace(name.value))
                            {
                                toAdd.DefaultGameObject = Resources.Load<GameObject>("Tilesets/Gameobjects/" + name.value);
                            }
                            else
                            {
                                toAdd.DefaultGameObject = Resources.Load<GameObject>("Tilesets/Gameobjects/genericSmolObstacle");
                            }
                        }

                    }
                    // Set the tile group
                    if (jsonProperty.name == "group")
                        toAdd.group = jsonProperty.value;
                }
                PropTiles.Add(toAdd);
            }
        }
    }
    // Load the buildings tileset
    static void LoadBuildingTiles()
    {
        for (int t = 0; t < spriteSheet.Length; t++)
        {
            // Loop through tiles, checking the tile's type to know how to create it
            JsonTile jsonTile = tileset.tiles[t];
            if (jsonTile.type == "envrTile")
            {
                // Tile asset to build
                EnvrTile toAdd = ScriptableObject.CreateInstance<EnvrTile>();
                toAdd.DefaultSprite = spriteSheet[t];
                // Loop through properties on tile
                for (int p = 0; p < jsonTile.properties.Length; p++)
                {
                    JsonTileProperty jsonProperty = jsonTile.properties[p];
                    if (jsonProperty.name == "moveCost") toAdd.moveCost = float.Parse(jsonProperty.value);
                    if (jsonProperty.name == "colliderType") toAdd.DefaultColliderType = (UnityEngine.Tilemaps.Tile.ColliderType)int.Parse(jsonProperty.value);
                    // Add gameobjects if the property is set to true
                    if (jsonProperty.name == "gameobject")
                    {
                        bool.TryParse(jsonProperty.value, out bool hasGameObject);

                        if (hasGameObject)
                        {
                            if (Resources.Load<GameObject>("Tilesets/Gameobjects/buildings_" + t) != null)
                            {
                                toAdd.DefaultGameObject = Resources.Load<GameObject>("Tilesets/Gameobjects/buildings_" + t);
                            }
                            else
                            {
                                toAdd.DefaultGameObject = Resources.Load<GameObject>("Tilesets/Gameobjects/genericGridObstacle");
                            }
                        }
                        else
                        {
                            toAdd.DefaultGameObject = Resources.Load<GameObject>("Tilesets/Gameobjects/genericGridShadow");
                        }
                    }
                    if (jsonProperty.name == "group")
                    {
                        toAdd.group = jsonProperty.value;
                    }
                }
                BuildingTiles.Add(toAdd);
            }
        }
    }
    // Load the dungeon tileset
    static void LoadDungeonTiles()
    {
        for (int t = 0; t < spriteSheet.Length; t++)
        {
            // Loop through tiles, checking the tile's type to know how to create it
            JsonTile jsonTile = tileset.tiles[t];
            if (jsonTile.type == "envrTile")
            {
                // Tile asset to build
                EnvrTile toAdd = ScriptableObject.CreateInstance<EnvrTile>();
                toAdd.DefaultSprite = spriteSheet[t];

                JsonTileProperty name = Array.Find(jsonTile.properties, p => p.name == "name");
                // Loop through properties on tile
                for (int p = 0; p < jsonTile.properties.Length; p++)
                {

                    JsonTileProperty jsonProperty = jsonTile.properties[p];
                    if (jsonProperty.name == "moveCost") toAdd.moveCost = float.Parse(jsonProperty.value);
                    if (jsonProperty.name == "colliderType") toAdd.DefaultColliderType = (Tile.ColliderType)int.Parse(jsonProperty.value);
                    if (jsonProperty.name == "group")
                        toAdd.group = jsonProperty.value;
                    // Very few ground tiles will have gameobjects, but some will!
                    if (jsonProperty.name == "gameobject")
                    {
                        bool.TryParse(jsonProperty.value, out bool hasGameObject);

                        if (hasGameObject)
                        {
                            if (Resources.Load<GameObject>("Tilesets/Gameobjects/dungeon_" + t) != null)
                            {
                                toAdd.DefaultGameObject = Resources.Load<GameObject>("Tilesets/Gameobjects/dungeon_" + t);
                            }
                            else if (name != null && !string.IsNullOrWhiteSpace(name.value))
                            {
                                toAdd.DefaultGameObject = Resources.Load<GameObject>("Tilesets/Gameobjects/" + name.value);
                            }
                            else
                            {
                                Debug.Log("Unable to find a gameobject for dungeon tile '" + name.value + "'");
                            }
                        }

                    }
                }
                DungeonTiles.Add(toAdd);
            }
            // NOTE: envrAdvTiles are not built at runtime but loaded from Resources by the name property
            else if (jsonTile.type == "envrAdvTile")
            {
                string name = "";
                for (int p = 0; p < jsonTile.properties.Length; p++)
                {
                    if (jsonTile.properties[p].name == "name")
                    {
                        name = jsonTile.properties[p].value;
                    }
                }
                EnvrAdvTile toAdd = Resources.Load<EnvrAdvTile>("Tilesets/Ground/" + name) as EnvrAdvTile;
                int adj;
                switch (name)
                {
                    case "dungeonWall1":
                        adj = 0;
                        break;
                    case "dungeonWall2":
                        adj = 12;
                        break;
                    case "dungeonWall3":
                        adj = 24;
                        break;
                    case "dungeonWall4":
                        adj = 36;
                        break;
                    default:
                        adj = 0;
                        break;
                }
                toAdd.sister = new TileBase[] {
                    // ORIGINAL
                    //DungeonTiles[0 + adj], DungeonTiles[1 + adj], DungeonTiles[2 + adj], DungeonTiles[3 + adj], DungeonTiles[4 + adj],
                    //DungeonTiles[6 + adj], DungeonTiles[7 + adj], DungeonTiles[8 + adj], DungeonTiles[9 + adj], DungeonTiles[10 + adj]

                    // TEST
                    DungeonTiles[5 + adj], DungeonTiles[11 + adj]
                };

                // TEST INVERT
                toAdd.invert = true;

                DungeonTiles.Add(toAdd);
            }
        }
    }
    // Load the dungeon prop tileset
    static void LoadDungeonPropTiles()
    {
        for (int t = 0; t < spriteSheet.Length; t++)
        {
            // Prop tiles, also built as ObjTiles
            JsonTile jsonTile = tileset.tiles[t];
            if (jsonTile.type == "objTile")
            {
                ObjTile toAdd = ScriptableObject.CreateInstance<ObjTile>();
                toAdd.DefaultSprite = spriteSheet[t];

                // Name property is used in conjunction with the gameobject property to assign specific non-unique gameobjects, so it has to be found outside the loop of properties
                JsonTileProperty name = Array.Find(jsonTile.properties, p => p.name == "name");

                // Loop through properties on tile
                for (int p = 0; p < jsonTile.properties.Length; p++)
                {
                    JsonTileProperty jsonProperty = jsonTile.properties[p];
                    // Set the collider type as none, sprite, or grid
                    if (jsonProperty.name == "colliderType")
                    {
                        toAdd.DefaultColliderType = (Tile.ColliderType)int.Parse(jsonProperty.value);
                    }
                    // Add gameobjects if the property is set to true
                    if (jsonProperty.name == "gameobject")
                    {
                        bool.TryParse(jsonProperty.value, out bool hasGameObject);

                        if (hasGameObject)
                        {
                            if (Resources.Load<GameObject>("Tilesets/Gameobjects/dungeonProps_" + t) != null)
                            {
                                toAdd.DefaultGameObject = Resources.Load<GameObject>("Tilesets/Gameobjects/dungeonProps_" + t);
                            }
                            else if (name != null && !String.IsNullOrWhiteSpace(name.value))
                            {
                                toAdd.DefaultGameObject = Resources.Load<GameObject>("Tilesets/Gameobjects/" + name.value);
                            }
                            else
                            {
                                toAdd.DefaultGameObject = Resources.Load<GameObject>("Tilesets/Gameobjects/genericSmolObstacle");
                            }
                        }

                    }
                    // Set the tile group
                    if (jsonProperty.name == "group")
                        toAdd.group = jsonProperty.value;
                }
                DungeonPropTiles.Add(toAdd);
            }
        }
    }

    // Get a field from this class by string, ie. List<TileBase> GroundTiles = TilesetLoader.GetInvenByString<>("GroundTiles");
    public static T GetTilesetByString<T>(string set)
    {
        return (T)typeof(TilesetLoader).GetField(set).GetValue(null);
    }
}

[System.Serializable]
class TilesetJson
{
    public string name;
    public JsonTile[] tiles;
}
[System.Serializable]
class JsonTile
{
    public int id;
    public JsonTileProperty[] properties;
    public string type;
}
[System.Serializable]
class JsonTileProperty
{
    public string name;
    public string type;
    public string value;
}