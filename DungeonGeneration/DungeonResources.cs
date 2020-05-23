using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonResources : MonoBehaviour
{
    DungeonMaster dungeonMaster;
    public DungeonGenerator dunGen;

    [Header("TILEMAPS")]
    public Tilemap Sub;
    public Tilemap Ground;
    public Tilemap Wall;
    public Tilemap Roof;
    public Tilemap Objects;
    public GameObject Areas;

    public int mapSize = 64;
    int[,] Map;
    DungeonConfig activeConfig;

    private void Start()
    {
        dungeonMaster = DungeonMaster.dungeonMaster;
    }

    public void ResourceGenMaster ()
    {
        Map = dunGen.Map;
        activeConfig = dungeonMaster.activeConfig;

        PlaceStones(Map);
        GenerateOrePatches(Map);
    }

    void PlaceStones (int[,] map)
    {
        for(int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                if (map[x,y] == 1 && Random.value < activeConfig.ResourceDensity && !HasNeighbour(x, y, 0))
                {
                    // Prevent overwriting the stairs to the level above (also in the objects layer)
                    if (Objects.GetTile(new Vector3Int(x, y, 0)) == null)
                    {
                        TileBase[] stones = { TilesetLoader.PropTiles[0], TilesetLoader.PropTiles[1], TilesetLoader.PropTiles[2], TilesetLoader.PropTiles[3], TilesetLoader.PropTiles[4], TilesetLoader.PropTiles[5] };
                        Objects.SetTile(new Vector3Int(x, y, 0), RndFromTiles(stones));
                    }
                }
            }
        }
    }

    void GenerateOrePatches (int[,] map)
    {
        // The possible ores to spawn are based on the current depth
        TileBase[] ores = GetOresAtDepth(dungeonMaster.CurrentDepth);
        // Base number of ore patches is resourceDensity * 25, with the minimun amount of ore patches limited to 2 
        int BasePatches = activeConfig.ResourceDensity * 25 > 2 ? 2 : Mathf.FloorToInt(activeConfig.ResourceDensity * 25);
        // Plus a random amount of additional patches between 0 and 4
        int TotalPatches = BasePatches + Mathf.FloorToInt(Random.value * 4);
        Debug.Log("Total ore patches: " + TotalPatches);
        for (int p = 0; p < TotalPatches; p++)
        {
            Vector3Int centre;
            int X, Y;
            // The first 3 ore patches are forced into open areas of the cave
            if (p < 3)
            {
                do
                {
                    X = Mathf.FloorToInt(Random.value * 63);
                    Y = Mathf.FloorToInt(Random.value * 63);
                    centre = new Vector3Int(X, Y, 0);
                } while (map[X, Y] != 1);
            }
            // Otherwise any position is fair game
            else
            {
                X = Mathf.FloorToInt(Random.value * 63);
                Y = Mathf.FloorToInt(Random.value * 63);
                centre = new Vector3Int(X, Y, 0);
            }
            // The centre tile always gets set
            TileBase tile = RndFromTiles(ores);
            // Prevent overwriting stais down
            if (Ground.GetTile(centre) == null)
            {
                Sub.SetTile(centre, RndFromTiles(dunGen.GetGroundTiles(dunGen.STYLE_ID)));
                Ground.SetTile(centre, tile);
            }

            // Other tiles within the radius have a chance of spawning ore
            float radius = 1 + Mathf.Pow(Random.value, 2) * 3;
            for (int x = X - 5; x < X + 5; x++)
            {
                for (int y = Y - 5; y < Y + 5; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    // Skip positions outside the map
                    if (!IsWithin(pos, 0))
                        continue;
                    if ((centre-pos).magnitude < radius && Random.value + (radius - (centre - pos).magnitude) > 0.5f)
                    {
                        // Prevent overwriting stais down
                        if (Ground.GetTile(pos) != null)
                        {
                            Sub.SetTile(pos, RndFromTiles(dunGen.GetGroundTiles(dunGen.STYLE_ID)));
                            Ground.SetTile(pos, tile);
                        }
                    }
                }
            }
        }
    }

    // --- UTILITIES ---

    // Quickly check that position is within map limits
    bool IsWithin(Vector3Int pos, int border)
    {
        if (pos.x < 0 + border || pos.x >= mapSize - border || pos.y < 0 + border || pos.y >= mapSize - border)
            return false;
        return true;
    }
    // Returns true if at the tile at (x, y) has one neighbouring tile with the value of 'key'
    bool HasNeighbour(int x, int y, int key)
    {
        for (int a = -1; a <= 1; a++)
        {
            for (int b = -1; b <= 1; b++)
            {
                if (!IsWithin(new Vector3Int(x + a, y + b, 0), 0))
                    continue;
                if (Map[x + a, y + b] == key)
                    return true;
            }
        }
        return false;
    }

    TileBase RndFromTiles(TileBase[] tiles)
    {
        int index = Mathf.FloorToInt(UnityEngine.Random.value * tiles.Length);
        if (index >= tiles.Length) index = tiles.Length - 1;
        return tiles[index];
    }

    TileBase[] GetOresAtDepth(int depth)
    {
        TileBase coal = TilesetLoader.DungeonPropTiles[8];
        TileBase iron = TilesetLoader.DungeonPropTiles[9];
        TileBase copper = TilesetLoader.DungeonPropTiles[10];
        TileBase gold = TilesetLoader.DungeonPropTiles[11];

        switch (depth)
        {
            case 1:
                return new TileBase[] { coal };
            case 2:
                return new TileBase[] { coal, copper };
            case 3:
                return new TileBase[] { coal, copper, iron };
            case 4:
            default:
                return new TileBase[] { coal, copper, iron, gold };
        }
    }
}