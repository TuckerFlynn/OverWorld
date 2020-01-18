using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class TiledHelper
{
    public static TiledMap CreateGeneric()
    {
        // Create a generic TiledMap object to convert to JSON
        TiledMap tiled = new TiledMap();
        // Define layers
        tiled.layers[0] = new TiledTileLayer();
        tiled.layers[1] = new TiledObjectLayer();
        // Define tilesets
        tiled.tilesets[0] = new TiledTileset
        {
            firstgid = 1,
            source = "../Constructors/groundTileset.json"
        };
        tiled.tilesets[1] = new TiledTileset
        {
            firstgid = 31,
            source = "../Constructors/plantsTileset.json"
        };
        tiled.tilesets[2] = new TiledTileset
        {
            firstgid = 91,
            source = "../Constructors/propsTileset.json"
        };
        return tiled;
    }

    public static TiledMap OpenJsonMap(string path)
    {
        string jsonIn = File.ReadAllText(path);
        TiledMap jsonOut = JsonConvert.DeserializeObject<TiledMap>(jsonIn);
        return jsonOut;
    }
    public static string WriteJsonMap(TiledMap tiled)
    {
        string json = JsonConvert.SerializeObject(tiled, Formatting.Indented);
        return json;
    }
}

[Serializable]
public class TiledMap
{
    public int compressionlevel = -1;
    public int height = 64;
    public bool infinite = false;
    public TiledLayer[] layers = { new TiledTileLayer(), new TiledObjectLayer() };
    public string orientation = "orthogonal";
    public string renderorder = "left-down";
    public string tiledversion = "1.3.1";
    public int tileheight = 16;
    public TiledTileset[] tilesets = new TiledTileset[3];
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
    public List<TiledObject> objects;
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
