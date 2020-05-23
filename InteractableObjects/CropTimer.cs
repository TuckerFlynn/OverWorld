using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CropTimer : MonoBehaviour
{
    public float age;
    public float minMatureAge;
    public float maxMatureAge;
    float matureAge;
    [Header("REPLACEMENT TILE")]
    public bool replace;
    public string tileset;
    public int[] index;
    private Tilemap tilemap;

    void Start()
    {
        tilemap = GetComponentInParent<Tilemap>();
        matureAge = Random.Range(minMatureAge, maxMatureAge);
    }

    private void Update()
    {
        age += Time.deltaTime;

        if (age > matureAge)
        {
            TileBase tile;
            // If multiple replacement tiles are set, one will be choosen randomly from possibilities
            int RandIndex = index[Random.Range(0, index.Length)];
            // Check whether loaded tile should be an ObjTile or EnvrTile
            if (tileset == "PropTiles" || tileset == "PlantTiles")
                tile = TilesetLoader.GetTilesetByString<List<ObjTile>>(tileset)[RandIndex] as TileBase;
            else
                tile = TilesetLoader.GetTilesetByString<List<EnvrTile>>(tileset)[RandIndex] as TileBase;

            tilemap.SetTile(Vector3Int.FloorToInt(transform.position), tile); // This is effectively DestroyImmediate for this script
        }
    }
}