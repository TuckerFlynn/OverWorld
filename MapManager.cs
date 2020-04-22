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
    public Tilemap Sub;
    public Tilemap Ground;
    public Tilemap Walls;
    public Tilemap Roof;
    public Tilemap Objects;
    [Header("Area Objects")]
    public GameObject Areas;
    public GameObject[] AreaObjects;
    public GameObject GroundItems;

    public MapField[] masterMap;
    public Vector2Int worldPos;
    public FieldGenerator fieldGenerator;
    public OverworldMinimap minimap;

    CharacterManager charMngr;
    
    private void Awake()
    {
        if (mapManager == null)
        {
            mapManager = this;
        }
        else if (mapManager != this)
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        // Force character to reload when the mapManager is started
        charMngr = CharacterManager.characterManager;
        charMngr.UpdateCharacter();

        masterMap = LoadWorld();
        worldSize = (int)Mathf.Sqrt(masterMap.Length);
        worldPos = new Vector2Int( charMngr.activeChar.worldPos.x, charMngr.activeChar.worldPos.y );
        if (worldPos == new Vector2Int(-1,-1))
        {
            // ** this will be changed to force new players to load in a grassland biome with starter prefab
            worldPos = new Vector2Int(Mathf.FloorToInt(Random.value * worldSize), Mathf.FloorToInt(Random.value * worldSize));
        }
        if (!Directory.Exists(Application.persistentDataPath + "/Fields"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Fields");
        }
        minimap = FindObjectOfType<OverworldMinimap>();
        LoadField(worldPos);
    }

    private void OnDisable()
    {
        // Only save fields on disable if the player is not in a dungeon
        if (charMngr != null && !charMngr.InDungeon)
        {
            SaveFieldFile(worldPos);
        }
    }

    // Load a field from file or generate a new one
    public void LoadField (Vector2Int pos)
    {
        if (pos.x < 0 || pos.x > 63 || pos.y < 0 || pos.y > 63)
        {
            Debug.Log("Attempt to leave this world denied. (Requested field outside worldMap limits)");
            return;
        }
        // Apply changes to worldPos, and then update the current character's position to worldPos
        worldPos = pos;
        charMngr.activeChar.worldPos = new Vector2IntJson(worldPos);

        // Clear tiles and all children of the Areas gameobject
        Ground.ClearAllTiles();
        Sub.ClearAllTiles();
        Walls.ClearAllTiles();
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in Areas.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
        // Items dropped on the ground are also removed when moving to a new field
        List<GameObject> drops = new List<GameObject>();
        foreach (Transform child in GroundItems.transform)
            drops.Add(child.gameObject);
        drops.ForEach(drop => Destroy(drop));

        // try to load the requested field from file
        string file = worldPos.x + "_" + worldPos.y + ".json";
        if (File.Exists(Application.persistentDataPath + "/Fields/" + file))
        {
            LoadFieldFile(worldPos);
            Debug.Log("Succesfully loaded field " + worldPos + " with biome: " + masterMap[CoordToId(worldPos, worldSize)].MainBiome);
        }
        // Otherwise generate the map and save it
        else
        {
            fieldGenerator.MainBiomeGen(masterMap[CoordToId(worldPos, worldSize)]);
            SaveFieldFile(worldPos);

            Debug.Log("Succesfully generated field " + worldPos + " with biome: " + masterMap[CoordToId(worldPos, worldSize)].MainBiome);
        }

        // Update minimap display
        minimap.UpdateMinimap(worldPos);
    }
    // Save a field to file, formatted so that it can be opened by Tiled
    public void SaveFieldFile(Vector2Int pos)
    {
        // Create TiledMap object to convert to JSON
        TiledMap tiled = TiledHelper.CreateGeneric();
        TiledTileLayer groundLayer = (TiledTileLayer)tiled.layers[0];
        TiledTileLayer roofLayer = (TiledTileLayer)tiled.layers[1];
        TiledObjectLayer objectLayer = (TiledObjectLayer)tiled.layers[2];
        TiledObjectLayer areasLayer = (TiledObjectLayer)tiled.layers[3];

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
                    if (current.group == "ground" || current.group == "liquid")
                        groundLayer.data[i] = TilesetLoader.GroundTiles.IndexOf(current) + 1;
                    else if (current.group == "building" || current.group == "roof")
                        groundLayer.data[i] = TilesetLoader.BuildingTiles.IndexOf(current) + tiled.tilesets[3].firstgid;
                }
            }
            if (Roof.HasTile(vect))
            {
                if (IsTileOfType<EnvrTile>(Roof, vect) || IsTileOfType<EnvrAdvTile>(Roof, vect))
                {
                    EnvrTile current = (EnvrTile)Roof.GetTile(vect);
                    if (current.group == "ground" || current.group == "liquid")
                        roofLayer.data[i] = TilesetLoader.GroundTiles.IndexOf(current) + 1;
                    else if (current.group == "building" || current.group == "roof")
                        roofLayer.data[i] = TilesetLoader.BuildingTiles.IndexOf(current) + tiled.tilesets[3].firstgid;
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

        // Loop through children of Areas and grab type, location, and size info
        int count = Areas.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            GameObject area = Areas.transform.GetChild(i).gameObject;
            TiledObject tiledObject;

            if (area.TryGetComponent<RoofArea>(out RoofArea script))
            {
                tiledObject = script.thisTiledObject;
                areasLayer.objects.Add(tiledObject);
            }
            
        }

        tiled.layers[0] = groundLayer as TiledLayer;
        tiled.layers[1] = roofLayer as TiledLayer;
        tiled.layers[2] = objectLayer as TiledLayer;
        tiled.layers[3] = areasLayer as TiledLayer;

        string path = Application.persistentDataPath + "/Fields/" + pos.x + "_" + pos.y + ".json";
        string json = TiledHelper.WriteJsonMap(tiled);
        File.WriteAllText(path, json);
    }

    public void LoadFieldFile(Vector2Int pos)
    {
        // Open the json file and convert the contents into a TiledMap object
        string path = Application.persistentDataPath + "/Fields/" + pos.x + "_" + pos.y + ".json";
        TiledMap tiled = TiledHelper.OpenJsonMap(path);

        Vector3Int[] positions = new Vector3Int[tiled.height * tiled.width];
        TileBase[] groundArray = new TileBase[tiled.height * tiled.width];
        TileBase[] roofArray = new TileBase[tiled.height * tiled.width];
        TileBase[] objectsArray = new TileBase[tiled.height * tiled.width];

        // GROUND & ROOF LAYERS
        for (int i = 0; i < tiled.height * tiled.width; i++)
        {
            // Get the tilemap position vector from the index
            int X = i % 64;
            int Y = 63 - Mathf.FloorToInt(i / 64);
            positions[i] = new Vector3Int(X, Y, 0);

            // GROUND
            TiledLayer groundLayer = tiled.layers[0];
            groundArray[i] = TiledHelper.ReadTileLayer(tiled, groundLayer, i);

            // ROOF
            TiledLayer roofLayer = tiled.layers[1];
            roofArray[i] = TiledHelper.ReadTileLayer(tiled, roofLayer, i);
        }
        // OBJECT LAYER
        TiledLayer objectLayer = tiled.layers[2];
        for (int i = 0; i < objectLayer.objects.Count; i++)
        {
            ObjectRef obj = TiledHelper.ReadObjectLayer(tiled, objectLayer, i);
            objectsArray[obj.index] = obj.tileBase;
        }

        Ground.SetTiles(positions, groundArray);
        Roof.SetTiles(positions, roofArray);
        Objects.SetTiles(positions, objectsArray);
        // Check for gameobjects with scripts that need to be manually started (ie. MineEntrance)
        MineEntrance[] mineEntrances = Objects.gameObject.GetComponentsInChildren<MineEntrance>();
        foreach (MineEntrance script in mineEntrances)
            script.SelfStart();

        // AREA LAYER: must be run after the tilemaps have been set because generated areas need to read the tiles underneath depending on their type
        TiledLayer areaLayer = tiled.layers[3];
        for (int i = 0; i < areaLayer.objects.Count; i++)
        {
            TiledObject tObj = areaLayer.objects[i];
            // The area type is used to determine what gameobject needs to be instantiated, and passes the other area properties to that gameobject
            if (tObj.type == "roof")
            {
                // The TiledObject data is passed to the attached script to handle additonal setup
                GameObject instance = Instantiate(AreaObjects[0]);
                instance.transform.SetParent(Areas.transform);
                instance.GetComponent<RoofArea>().CreateRoofArea(tObj, Roof, Vector3Int.zero, 0);
            }
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
