using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TiledHelper
{
    public static TiledMap CreateGeneric()
    {
        // Create a generic TiledMap object to convert to JSON
        TiledMap tiled = new TiledMap();
        // DEFINE LAYERS
        tiled.layers[0] = new TiledTileLayer() // Ground
        {
            name = "Ground"
        };
        tiled.layers[1] = new TiledTileLayer() // Roof
        {
            name = "Roof"
        };
        tiled.layers[2] = new TiledObjectLayer() // Objects
        {
            name = "Objects"
        };
        tiled.layers[3] = new TiledObjectLayer() // Areas
        {
            name = "Areas"
        };
        // DEFINE TILESETS
        tiled.tilesets[0] = new TiledTileset
        {
            firstgid = 1,
            source = "../Constructors/groundTileset.json"
        };
        tiled.tilesets[1] = new TiledTileset
        {
            firstgid = 49,
            source = "../Constructors/plantsTileset.json"
        };
        tiled.tilesets[2] = new TiledTileset
        {
            firstgid = 109,
            source = "../Constructors/propsTileset.json"
        };
        tiled.tilesets[3] = new TiledTileset
        {
            firstgid = 205,
            source = "../Constructors/buildingsTileset.json"
        };
        tiled.tilesets[4] = new TiledTileset
        {
            firstgid = 301,
            source = "../Constructors/dungeonTileset.json"
        };
        tiled.tilesets[5] = new TiledTileset
        {
            firstgid = 361,
            source = "../Constructors/dungeonPropsTileset.json"
        };
        return tiled;
    }

    public static TiledMap OpenJsonMap(string path)
    {
        string jsonIn = File.ReadAllText(path);
        TiledMap jsonOut = JsonConvert.DeserializeObject<TiledMap>(jsonIn);
        return jsonOut;
    }

    public static TiledMap OpenJsonMapPrefab(string prefabName)
    {
        string jsonIn = Resources.Load<TextAsset>("Tilesets/Prefabs/" + prefabName).text;
        TiledMap jsonOut = JsonConvert.DeserializeObject<TiledMap>(jsonIn);
        return jsonOut;
    }

    public static string WriteJsonMap(TiledMap tiled)
    {
        string json = JsonConvert.SerializeObject(tiled, Formatting.Indented);
        return json;
    }
    // Reads deserialized json data to return the tile at a specific position, in a specific TILE layer
    public static TileBase ReadTileLayer (TiledMap tiledMap, TiledLayer layer, int i)
    {
        TileBase tile = null;
        if (layer.data[i] < TilesetLoader.GroundTiles.Count)
        {
            // Tiled assigns a unique GID to every tile used, so this number must be adjusted to get the id within each tileset
            int gidAdjust = tiledMap.tilesets[0].firstgid;
            int tileIndex = layer.data[i] - gidAdjust;
            // GID = 0 must be ignored but without throwing an error, it's just an empty tile
            if (tileIndex == -1 * gidAdjust)
            {
                return tile;
            }
            else if (tileIndex < 0 || tileIndex > TilesetLoader.BuildingTiles.Count)
            {
                Debug.Log("Index out of range for roof tile (" + tileIndex + ")");
                return tile;
            }

            return TilesetLoader.GroundTiles[tileIndex];
        }
        else if (layer.data[i] < tiledMap.tilesets[3].firstgid + TilesetLoader.BuildingTiles.Count)
        {
            int gidAdjust = tiledMap.tilesets[3].firstgid;
            int tileIndex = layer.data[i] - gidAdjust;
            // Unlike the ground layer, the roof layer is not totally filled, so GID = 0 must be ignored
            if (tileIndex == -1 * gidAdjust)
            {
                return tile;
            }
            else if (tileIndex < 0 || tileIndex > TilesetLoader.BuildingTiles.Count)
            {
                Debug.Log("Index out of range for roof tile (" + tileIndex + ")");
                return tile;
            }

            return TilesetLoader.BuildingTiles[tileIndex];
        }
        return tile;
    }
    // Reads deserialized json data to return the tile at a specific position, in a specific OBJECT layer (this doesn't include AREAS)
    public static ObjectRef ReadObjectLayer (TiledMap tiledMap, TiledLayer layer, int i)
    {
        TiledObject tObj = layer.objects[i];
        // Get the tilemap position index (not coords) from the TiledObject
        int X = Mathf.FloorToInt(tObj.x / 16);
        int Y = Mathf.FloorToInt(tObj.y / 16) - 1;
        int posIndex = CoordToId(new Vector2Int(X, Y), tiledMap.width);
        // Get the tile ID adjusted by firstgid for the corresponding tileset
        if (tObj.gid < tiledMap.tilesets[2].firstgid)
        {
            int gidAdjust = tiledMap.tilesets[1].firstgid;
            int tileIndex = tObj.gid - gidAdjust;
            if (tileIndex < 0 || tileIndex > TilesetLoader.PlantTiles.Count)
            {
                // If the tileIndex is found to be -1, this means a tile was saved that doesn't exist in the TilesetLoader
                Debug.Log("Index out of range for plant tile (" + tileIndex + ")");
                return null;
            }
            return new ObjectRef(posIndex, TilesetLoader.PlantTiles[tileIndex]);
        }
        else
        {
            int gidAdjust = tiledMap.tilesets[2].firstgid;
            int tileIndex = tObj.gid - gidAdjust;
            if (tileIndex < 0 || tileIndex > TilesetLoader.PropTiles.Count+1)
            {
                // If the tileIndex is found to be -1, this means a tile was saved that doesn't exist in the TilesetLoader
                Debug.Log("Index out of range for prop tile (" + tileIndex + ")");
                return null;
            }
            return new ObjectRef(posIndex, TilesetLoader.PropTiles[tileIndex]);
        }
    }


    // Convert a Vector2 map coordinate to the corresponding field index
    static int CoordToId(Vector2Int vect, int width)
    {
        return (vect.y * width + vect.x);
    }
}

[Serializable]
public class TiledMap
{
    public int compressionlevel = -1;
    public int height = 64;
    public bool infinite = false;
    public TiledLayer[] layers = { new TiledTileLayer(), new TiledTileLayer(), new TiledObjectLayer(), new TiledObjectLayer() };
    public string orientation = "orthogonal";
    public string renderorder = "left-down";
    public string tiledversion = "1.3.1";
    public int tileheight = 16;
    public TiledTileset[] tilesets = new TiledTileset[6];
    public int tilewidth = 16;
    public string type = "map";
    public double version = 1.2;
    public int width = 64;
}
[Serializable]
public class TiledLayer
{
    public int opacity = 1;
    public bool visible = true;
    public int[] data = new int[64 * 64];
    public List<TiledObject> objects = new List<TiledObject>();
    public int x;
    public int y;
}
[Serializable]
public class TiledTileLayer : TiledLayer
{
    public int[] data = new int[64 * 64];
    public int height = 64;
    public int id = 1;
    public string name = "Ground";
    public string type = "tilelayer";
    public int width = 64;
    public int x = 0;
    public int y = 0;
}
[Serializable]
public class TiledObjectLayer : TiledLayer
{
    public string draworder = "topdown";
    public int id = 2;
    public string name = "Objects";
    public List<TiledObject> objects = new List<TiledObject>();
    public string type = "objectgroup";
    public int x = 0;
    public int y = 0;
    public int[] data = null;
}
[Serializable]
public class TiledObject
{
    public int gid;
    public int height = 16;
    public int id;
    public string name = "";
    public int rotation = 0;
    public string type = "";
    public bool visible = true;
    public int width = 16;
    public int x;
    public int y;
}
[Serializable]
public class TiledTileset
{
    public int firstgid;
    public string source;
}
/// <summary>
/// Contains an int index associated with the tile position, and a TileBase tileBase
/// </summary>
public class ObjectRef
{
    public int index;
    public TileBase tileBase;

    public ObjectRef(int index, TileBase tileBase)
    {
        this.index = index;
        this.tileBase = tileBase;
    }
}