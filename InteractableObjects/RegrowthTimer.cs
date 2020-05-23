using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RegrowthTimer : MonoBehaviour
{
    public float MinTime;
    public float MaxTime;
    [Header("REPLACEMENT TILE")]
    public bool replace;
    public string tileset;
    public int index;

    Tilemap tilemap;
    float Time;

    private void Start()
    {
        Time = Random.Range(MinTime, MaxTime);
        tilemap = GetComponentInParent<Tilemap>();

        StartCoroutine(Regrowth());
    }

    IEnumerator Regrowth ()
    {
        yield return new WaitForSeconds(Time);

        TileBase tile;
        // Check whether loaded tile should be an ObjTile or EnvrTile
        if (tileset == "PropTiles" || tileset == "PlantTiles")
            tile = TilesetLoader.GetTilesetByString<List<ObjTile>>(tileset)[index] as TileBase;
        else
            tile = TilesetLoader.GetTilesetByString<List<EnvrTile>>(tileset)[index] as TileBase;
        Vector3Int pos = Vector3Int.FloorToInt(transform.position);

        tilemap.SetTile(pos, tile);
    }
}
