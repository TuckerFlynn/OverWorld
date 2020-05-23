using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileDataLoader : MonoBehaviour
{
    public static TileDataLoader tileDataLoader;

    public Tilemap Objects;

    private void Awake()
    {
        if (tileDataLoader == null)
            tileDataLoader = this;
        else if (tileDataLoader != this)
            Destroy(this);
    }

    public void SaveTileData ()
    {
        foreach (Transform child in Objects.transform)
        {

        }
    }

    public void LoadTileData()
    {

    }
}

[Serializable]
public class PlantData
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Age { get; set; }
}