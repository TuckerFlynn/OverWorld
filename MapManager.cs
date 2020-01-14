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
    public static Tilemap ground;
    public static Tilemap objects;

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

        fieldGenerator.LoadTiles();

        LoadField(worldPos);
    }

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

        }
        // Otherwise generate the map and save it
        else
        {
            fieldGenerator.MainBiomeGen(masterMap[CoordToId(worldPos)]);
            Debug.Log("Succesfully generated field " + worldPos + " with biome: " + masterMap[CoordToId(worldPos)].MainBiome);
        }
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
    int CoordToId(Vector2Int vect)
    {
        return vect.y * worldSize + vect.x;
    }
}
