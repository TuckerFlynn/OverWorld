using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager mapManager;

    public int worldSize;
    [Header("Tilemaps")]
    public Tilemap Ground;
    public Tilemap Objects;

    private MapField[] masterMap;
    public Vector2Int worldPos;
    public FieldGenerator fieldGenerator;

    CharacterManager charMngr;
    
    private void Awake()
    {
        if (mapManager == null)
        {
            DontDestroyOnLoad(gameObject);
            mapManager = this;
        }
        else if (mapManager != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        masterMap = LoadWorld();
        worldSize = (int)Mathf.Sqrt(masterMap.Length);
        // Load the player's character (defaults to first character if one has not been set already)
        charMngr = CharacterManager.characterManager;
        charMngr.LoadCharacter();
        worldPos = charMngr.activeChar.worldPos;
        if (worldPos == new Vector2Int(-1,-1))
        {
            // ** this will be changed to force new players to load in a grassland biome with starter prefab
            worldPos = new Vector2Int(Mathf.FloorToInt(Random.value * worldSize), Mathf.FloorToInt(Random.value * worldSize));
        }
        // Build and load all tilesets from the JSON files
        TilesetLoader.LoadTiles();
        if (!Directory.Exists(Application.persistentDataPath + "/Fields"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Fields");
        }
        LoadField(worldPos);
    }
    // Load a field from file or generate a new one
    public void LoadField (Vector2Int pos)
    {
        if (pos.x < 0 || pos.x > 63 || pos.y < 0 || pos.y > 63)
        {
            Debug.Log("Attempt to leave this world denied. (Requested field outside worldMap limits)");
            return;
        }
        // Apply changes to worldPos
        worldPos = pos;
        charMngr.activeChar.worldPos = pos;
        // try to load the requested field from file
        string file = worldPos.x + "_" + worldPos.y + ".json";
        if (File.Exists(Application.persistentDataPath + "/Fields/" + file))
        {
            LoadFieldFile(worldPos);
        }
        // Otherwise generate the map and save it
        else
        {
            fieldGenerator.MainBiomeGen(masterMap[CoordToId(worldPos, worldSize)]);
            SaveFieldFile(worldPos);

            Debug.Log("Succesfully generated field " + worldPos + " with biome: " + masterMap[CoordToId(worldPos, worldSize)].MainBiome);
        }
    }
    // Save a field to file, formatted so that it can be opened by Tiled
    public void SaveFieldFile(Vector2Int pos)
    {
        // Create TiledMap object to convert to JSON
        TiledMap tiled = TiledHelper.CreateGeneric();
        TiledTileLayer groundLayer  = (TiledTileLayer)tiled.layers[0];
        TiledObjectLayer objectLayer = (TiledObjectLayer)tiled.layers[1];

        for (int i = 0; i < tiled.height * tiled.width; i++)
        {
            // TILED display has an inverted y axis, so y coordinates will be reveresed as part of saving and loading
            int X = i % 64;
            int Y = 63 - Mathf.FloorToInt(i / 64);
            Vector3Int vect = new Vector3Int(X, Y, 0);

            if (Ground.HasTile(vect))
            {
                if (IsTileOfType<EnvrTile>(Ground, vect) || IsTileOfType<EnvrAdvTile>(Ground, vect))
                {
                    EnvrTile current = (EnvrTile)Ground.GetTile(vect);
                    groundLayer.data[i] = TilesetLoader.EnvrTiles.IndexOf(current) + 1;
                }
            }
            if (Objects.HasTile(vect))
            {
                if (IsTileOfType<ObjTile>(Objects, vect))
                {
                    ObjTile current = (ObjTile)Objects.GetTile(vect);

                    TiledObject obj = new TiledObject
                    {
                        id = objectLayer.objects.Count,
                        x = X * 16,
                        y = (64*16) - Y * 16
                    };
                    if (current.group == "plants")
                    {
                        // gid in TILED is a unique int for every tile or object in all connected tilesets
                        obj.gid = TilesetLoader.PlantTiles.IndexOf(current) + tiled.tilesets[1].firstgid;
                    }
                    else if (current.group == "props")
                    {
                        obj.gid = TilesetLoader.PropTiles.IndexOf(current) + tiled.tilesets[2].firstgid;
                    }

                    objectLayer.objects.Add(obj);
                }
            }
        }
        tiled.layers[0] = groundLayer as TiledLayer;
        tiled.layers[1] = objectLayer as TiledLayer;

        string path = Application.persistentDataPath + "/Fields/" + pos.x + "_" + pos.y + ".json";
        string json = TiledHelper.WriteJsonMap(tiled);
        File.WriteAllText(path, json);
    }

    public void LoadFieldFile(Vector2Int pos)
    {
        string path = Application.persistentDataPath + "/Fields/" + pos.x + "_" + pos.y + ".json";
        TiledMap tiled = TiledHelper.OpenJsonMap(path);

        Vector3Int[] positions = new Vector3Int[tiled.height * tiled.width];
        TileBase[] groundArray = new TileBase[tiled.height * tiled.width];
        TileBase[] objectsArray = new TileBase[tiled.height * tiled.width];

        for (int i = 0; i < tiled.height * tiled.width; i++)
        {
            int X = i % 64;
            int Y = 63 - Mathf.FloorToInt(i / 64);
            positions[i] = new Vector3Int(X, Y, 0);
            TiledLayer tileLayer = tiled.layers[0];
            int tileIndex = tileLayer.data[i] - tiled.tilesets[0].firstgid;

            if (tileIndex < 0 || tileIndex > TilesetLoader.EnvrTiles.Count)
            {
                Debug.Log("Index out of range for ground tile: " + tileIndex);
                continue;
            }

            groundArray[i] = TilesetLoader.EnvrTiles[tileIndex];
        }

        TiledLayer objectLayer = tiled.layers[1];
        for (int i = 0; i < objectLayer.objects.Count; i++)
        {
            TiledObject tObj = objectLayer.objects[i];
            // Get the tilemap position index (not coords) from the TiledObject
            int X = Mathf.FloorToInt(tObj.x / 16);
            int Y = Mathf.FloorToInt(tObj.y / 16) - 1;
            int posIndex = CoordToId(new Vector2Int(X, Y), 64);
            // Get the tile ID adjusted by firstgid for the corresponding tileset
            if (tObj.gid < tiled.tilesets[2].firstgid)
            {
                int gidAdjust = tiled.tilesets[1].firstgid;
                int tileIndex = tObj.gid - gidAdjust;
                if (tileIndex < 0 || tileIndex > TilesetLoader.PlantTiles.Count)
                {
                    // If the tileIndex is found to be -1, this means a tile was saved that doesn't exist in the TilesetLoader
                    Debug.Log("Index out of range for plant tile: " + tileIndex);
                    continue;
                }
                objectsArray[posIndex] = TilesetLoader.PlantTiles[tileIndex];
            }
            else
            {
                int gidAdjust = tiled.tilesets[2].firstgid;
                int tileIndex = tObj.gid - gidAdjust;
                if (tileIndex < 0 || tileIndex > TilesetLoader.PropTiles.Count)
                {
                    // If the tileIndex is found to be -1, this means a tile was saved that doesn't exist in the TilesetLoader
                    Debug.Log("Index out of range for prop tile: " + tileIndex);
                    continue;
                }
                objectsArray[posIndex] = TilesetLoader.PropTiles[tileIndex];
            }
        }

        Ground.SetTiles(positions, groundArray);
        Objects.SetTiles(positions, objectsArray);
        Debug.Log("Field loaded from file");
    }

    // Load the world map from file
    MapField[] LoadWorld ()
    {
        MapField[] map;
        if (File.Exists(Application.persistentDataPath + "/worldMap.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.OpenRead(Application.persistentDataPath + "/worldMap.dat");

            map = (MapField[])bf.Deserialize(file);
            file.Close();
            Debug.Log("Loaded worldMap.dat");
            return map;
        }
        else
        {
            Debug.LogWarning("Unable to find worldMap.dat");
            return new MapField[0];
        }
    }

    // Convert a Vector2 map coordinate to the corresponding field index
    int CoordToId(Vector2Int vect, int size)
    {
        return vect.y * size + vect.x;
    }

    // Check tilebase for type of tile
    public bool IsTileOfType<T>(Tilemap tilemap, Vector3Int position) where T : TileBase
    {
        TileBase targetTile = tilemap.GetTile(position);

        if (targetTile != null && targetTile is T)
        {
            return true;
        }

        return false;
    }
}
