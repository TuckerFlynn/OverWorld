using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonMaster : MonoBehaviour
{
    public static DungeonMaster dungeonMaster;
    CharacterManager charMngr;

    public int worldSize;
    public DungeonGenerator dunGen;
    [Header("Tilemaps")]
    public Tilemap Ground;
    public Tilemap Roof;
    public Tilemap Objects;
    [Header("Area Objects")]
    public GameObject Areas;
    public GameObject[] AreaObjects;

    private void Awake()
    {
        if (dungeonMaster == null)
        {
            dungeonMaster = this;
        }
        else if (dungeonMaster != this)
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        // Force character to reload when the dungeonMaster is started
        charMngr = CharacterManager.characterManager;
        charMngr.LoadCharacter();

        // Build and load all tilesets from the JSON files
        TilesetLoader.LoadTiles();
        if (!Directory.Exists(Application.persistentDataPath + "/Dungeons"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Dungeons");
        }

        LoadDungeon();
    }

    void LoadDungeon()
    {
        // Clear all children of the Areas gameobject
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in Areas.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));

        dunGen.MainDungeonGen();
    }
}
