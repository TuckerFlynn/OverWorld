using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public static class TilesetLoader
{
    public static List<EnvrTile> EnvrTiles = new List<EnvrTile>();
    public static List<ObjTile> PlantTiles = new List<ObjTile>();
    public static List<ObjTile> PropTiles = new List<ObjTile>();

    public static void LoadTiles()
    {
        // Get array of all tilesets
        TextAsset[] files = Resources.LoadAll<TextAsset>("Tilesets/Constructors");
        for (int i = 0; i < files.Length; i++)
        {
            // Loop through each tileset to build the contained tiles
            TilesetJson tileset = JsonUtility.FromJson<TilesetJson>(files[i].text);
            Sprite[] spriteSheet = Resources.LoadAll<Sprite>("Tilesets/Spritesheets/" + tileset.name);
            // Tileset file from Tiled considers empty spaces on the sprite as tiles, the real number of tiles is the spritesheet size
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
                        if (jsonProperty.name == "colliderType") toAdd.DefaultColliderType = (UnityEngine.Tilemaps.Tile.ColliderType) int.Parse(jsonProperty.value);
                    }
                    EnvrTiles.Add(toAdd);
                }
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
                    EnvrTiles.Add(toAdd);
                }
                // Plant tiles build as ObjTiles
                else if (jsonTile.type == "objTile" && tileset.name == "plants")
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
                        if (jsonProperty.name == "gameobject" && jsonProperty.value == "true")
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
                        if (jsonProperty.name == "group") toAdd.group = jsonProperty.value;
                    }
                    PlantTiles.Add(toAdd);
                }
                // Prop tiles, also built as ObjTiles
                else if (jsonTile.type == "objTile" && tileset.name == "props")
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
                        if (jsonProperty.name == "gameobject" && bool.TryParse(jsonProperty.value, out bool flag))
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
                        if (jsonProperty.name == "group") toAdd.group = jsonProperty.value;
                    }
                    PropTiles.Add(toAdd);
                }
            }
        }
        Debug.Log("Loaded " + EnvrTiles.Count + " environment tiles, " + PlantTiles.Count + " plant tiles, " + PropTiles.Count + " prop tiles");
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