using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public static class TilesetLoader
{
    public static List<EnvrTile> GroundTiles = new List<EnvrTile>();
    public static List<ObjTile> PlantTiles = new List<ObjTile>();
    public static List<ObjTile> PropTiles = new List<ObjTile>();
    public static List<EnvrTile> BuildingTiles = new List<EnvrTile>();

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

        }
        Debug.Log("Loaded " + GroundTiles.Count + " environment tiles and " + PlantTiles.Count + " plant tiles");
        Debug.Log("Loaded " + PropTiles.Count + " prop tiles and " + BuildingTiles.Count + " building tiles");
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
                    if (jsonProperty.name == "colliderType") toAdd.DefaultColliderType = (UnityEngine.Tilemaps.Tile.ColliderType)int.Parse(jsonProperty.value);
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
                        toAdd.DefaultColliderType = (UnityEngine.Tilemaps.Tile.ColliderType)int.Parse(jsonProperty.value);
                    }
                    // Add gameobjects if the property is set to true
                    if (jsonProperty.name == "gameobject")
                    {
                        bool.TryParse(jsonProperty.value, out bool hasGameObject);

                        if (hasGameObject)
                        {
                            if (Resources.Load<GameObject>("Tilesets/Gameobjects/plants_" + t) != null)
                            {
                                toAdd.DefaultGameObject = Resources.Load<GameObject>("Tilesets/Gameobjects/plants_" + t);
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
                        toAdd.DefaultColliderType = (UnityEngine.Tilemaps.Tile.ColliderType)int.Parse(jsonProperty.value);
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