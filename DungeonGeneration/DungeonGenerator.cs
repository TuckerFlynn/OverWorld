﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    public Tilemap Sub;
    public Tilemap Ground;
    public Tilemap Wall;
    public Tilemap Roof;
    public Tilemap Objects;
    public GameObject Areas;

    public int mapSize = 64;

    XXHash hash;
    [Header("Dungeon Gen Seed")]
    public int hashSeed;
    [Header("Dungeon Style")]
    public int STYLE_ID = 1;
    [Header("Walker Input")]
    public int minSteps = 200;
    public int maxSteps = 1000;
    public int minBranches = 1;
    public int maxBranches = 6;
    public bool OneOrigin = true;
    
    public int[,] Map;
    Vector3Int[] positions;
    Vector3Int stairUp;
    Vector3Int stairDown;

    private void Start()
    {
        positions = new Vector3Int[mapSize * mapSize];
        for (int index = 0; index < positions.Length; index++)
        {
            positions[index] = new Vector3Int(index % mapSize, index / mapSize, 0);
        }
        Map = new int[mapSize, mapSize];
        ResetStairPositions();
    }
    // Returns a Vector3Int that marks the position of the stairs going up
    public Vector3Int MainDungeonGen(int seed, bool descent)
    {
        int h = 0;
        hashSeed = seed;
        hash = new XXHash(hashSeed);

        STYLE_ID = GetDungeonStyleFromBiome();

        // Rerun the walker if the quality check fails
        do
        {
            //int randStep = Random.Range(minSteps, maxSteps);
            int randStep = hash.Range(minSteps, maxSteps, h);
            h++;
            //int randBranch = Random.Range(minBranches, maxBranches);
            int randBranch = hash.Range(minBranches, maxBranches, h);
            h++;
            // Generation methods
            BasicWalker(randStep, randBranch, OneOrigin);
            Map = ExpandByOne(Map);
            Map = BoundaryWalls(Map);
        }
        while (!QualityCheck());

        Sub.ClearAllTiles();
        Ground.ClearAllTiles();
        Wall.ClearAllTiles();
        Roof.ClearAllTiles();
        Objects.ClearAllTiles();

        // Rough ground tiles; the entire ground layer is filled because eventually the cave walls will be destructible
        TileBase[] groundTiles = GetGroundTiles(STYLE_ID);
        PaintByNumber(Ground, groundTiles, 1);
        PaintByNumber(Ground, groundTiles, 0);

        // Dark tiles for solid areas
        TileBase[] roofTiles = GetRoofTiles(STYLE_ID);
        PaintByNumber(Roof, roofTiles, 0);

        // Roof Border tiles, and ground tiles under the boundaries (Sub layer)
        TileBase[] borderTiles = GetRoofBorderTiles(STYLE_ID);
        TileBase[] verticals = GetWallTiles(STYLE_ID);

        PaintTheWall(Roof, borderTiles, 0);
        PaintVerticals(Wall, Roof, verticals);
        //PaintTheWall(Sub, groundTiles, 0);

        // Add up and down stairs
        TileBase[] stairTiles = GetStairTiles(STYLE_ID);
        Objects.SetTile(stairUp, stairTiles[0]);
        // Mines will have a maximum of 100 floors, once floor 100 is reached there will be no stairs leading down
        if (DungeonMaster.dungeonMaster.CurrentDepth < 100)
            Ground.SetTile(stairDown, stairTiles[1]);

        // If the player is descending, character position is set to the stair going up (connecting to the previous level)
        if (descent)
            return stairUp;
        // If player is ascending, position is set to the stairs going down (arriving from the previous level)
        else
            return stairDown;
    }

    public void ResetStairPositions()
    {
        stairUp = new Vector3Int(-1, -1, 0);
        stairDown = new Vector3Int(-1, -1, 0);
    }

    int GetDungeonStyleFromBiome ()
    {
        MapManager mngr = MapManager.mapManager;
        string biome = mngr.masterMap[CoordToId(mngr.worldPos, mngr.worldSize)].MainBiome;

        switch (biome)
        {
            case "grassland":
            case "temperateForest":
                return 1;
            case "bog":
            case "swamp":
            case "temperateJungle":
            case "tropicalJungle":
                return 2;
            case "desert":
                return 3;
            case "arctic":
            case "arcticWaste":
            case "tundra":
            case "borealForest":
                return 4;
            default:
                return 2;
        }
    }

    // ---- TILE COMBINATION PRESETS ----

    public TileBase[] GetGroundTiles (int ID)
    {
        switch (ID)
        {
            // STONE
            case 1:
                return new TileBase[] { TilesetLoader.DungeonTiles[0], TilesetLoader.DungeonTiles[1], TilesetLoader.DungeonTiles[2], TilesetLoader.DungeonTiles[3], TilesetLoader.DungeonTiles[4] };
            // DIRT
            case 2:
                return new TileBase[] { TilesetLoader.DungeonTiles[12], TilesetLoader.DungeonTiles[13], TilesetLoader.DungeonTiles[14], TilesetLoader.DungeonTiles[15], TilesetLoader.DungeonTiles[16] };
            // SAND
            case 3:
                return new TileBase[] { TilesetLoader.DungeonTiles[24], TilesetLoader.DungeonTiles[25], TilesetLoader.DungeonTiles[26], TilesetLoader.DungeonTiles[27], TilesetLoader.DungeonTiles[28] };
            // COLD STONE
            case 4:
                return new TileBase[] { TilesetLoader.DungeonTiles[36], TilesetLoader.DungeonTiles[37], TilesetLoader.DungeonTiles[38], TilesetLoader.DungeonTiles[39], TilesetLoader.DungeonTiles[40] };
            // DEFAULT TO DIRT
            default:
                return new TileBase[] { TilesetLoader.DungeonTiles[12], TilesetLoader.DungeonTiles[13], TilesetLoader.DungeonTiles[14], TilesetLoader.DungeonTiles[15], TilesetLoader.DungeonTiles[16] };
        }
    }
    TileBase[] GetRoofTiles (int ID)
    {
        switch (ID) 
        {
            // STONE
            case 1:
                return new TileBase[] { TilesetLoader.DungeonTiles[5], TilesetLoader.DungeonTiles[11] };
            // DIRT
            case 2:
                return new TileBase[] { TilesetLoader.DungeonTiles[17], TilesetLoader.DungeonTiles[23] };
            // SAND
            case 3:
                return new TileBase[] { TilesetLoader.DungeonTiles[29], TilesetLoader.DungeonTiles[35] };
            // COLD STONE
            case 4:
                return new TileBase[] { TilesetLoader.DungeonTiles[41], TilesetLoader.DungeonTiles[47] };
            // DEFAULT TO DIRT
            default:
                return new TileBase[] { TilesetLoader.DungeonTiles[17], TilesetLoader.DungeonTiles[23] };
        }
    }
    TileBase[] GetRoofBorderTiles(int ID )
    {
        switch (ID)
        {
            // STONE
            case 1:
                return new TileBase[] { TilesetLoader.DungeonTiles[48] };
            // DIRT
            case 2:
                return new TileBase[] { TilesetLoader.DungeonTiles[49] };
            // SAND
            case 3:
                return new TileBase[] { TilesetLoader.DungeonTiles[50] };
            // COLD STONE
            case 4:
                return new TileBase[] { TilesetLoader.DungeonTiles[51] };
            // DEFAULT TO DIRT
            default:
                return new TileBase[] { TilesetLoader.DungeonTiles[49] };
        }
    }
    TileBase[] GetWallTiles(int ID)
    {
        switch (ID)
        {
            // STONE
            case 1:
                return new TileBase[] { TilesetLoader.DungeonTiles[56], TilesetLoader.DungeonTiles[57], TilesetLoader.DungeonTiles[58] };
            // DIRT
            case 2:
                return new TileBase[] { TilesetLoader.DungeonTiles[56], TilesetLoader.DungeonTiles[57], TilesetLoader.DungeonTiles[58] };
            // SAND
            case 3:
                return new TileBase[] { TilesetLoader.DungeonTiles[56], TilesetLoader.DungeonTiles[57], TilesetLoader.DungeonTiles[58] };
            // COLD STONE
            case 4:
                return new TileBase[] { TilesetLoader.DungeonTiles[56], TilesetLoader.DungeonTiles[57], TilesetLoader.DungeonTiles[58] };
            // DEFAULT TO DIRT
            default:
                return new TileBase[] { TilesetLoader.DungeonTiles[56], TilesetLoader.DungeonTiles[57], TilesetLoader.DungeonTiles[58] };
        }
    }
    TileBase[] GetStairTiles (int ID)
    {
        switch (ID)
        {
            // STONE
            case 1:
                return new TileBase[] { TilesetLoader.DungeonPropTiles[0], TilesetLoader.DungeonTiles[52] };
            // DIRT
            case 2:
                return new TileBase[] { TilesetLoader.DungeonPropTiles[1], TilesetLoader.DungeonTiles[53] };
            // SAND
            case 3:
                return new TileBase[] { TilesetLoader.DungeonPropTiles[2], TilesetLoader.DungeonTiles[54] };
            // COLD STONE
            case 4:
                return new TileBase[] { TilesetLoader.DungeonPropTiles[3], TilesetLoader.DungeonTiles[55] };
            // DEFAULT TO DIRT
            default:
                return new TileBase[] { TilesetLoader.DungeonPropTiles[1], TilesetLoader.DungeonTiles[53] };
        }
    }

    // ---- CAVE GENERATION METHODS ----

    void BasicWalker (int steps, int branches, bool singleOrigin)
    {
        Map = new int[mapSize, mapSize];
        stairUp = new Vector3Int(2 + Random.Range(0, mapSize - 4), 2 + Random.Range(0, mapSize - 4), 0);

        for (int b = 0; b < branches; b++)
        {
            Vector3Int walkerPos;
            if (singleOrigin || b == 0)
            {
                walkerPos = stairUp;
            }
            else
            {
                // Set a random start point for each branch of the walker; z component corresponds to the direction of the previous step
                walkerPos = new Vector3Int(1 + Random.Range(0, mapSize - 2), 1 + Random.Range(0, mapSize - 2), 0);
            }
            Map[walkerPos.x, walkerPos.y] = 1;

            // Each step then moves the walker 
            for (int s = 0; s < steps; s++)
            {
                walkerPos = WalkerStep(walkerPos);
                // End the branch if the walker has moved off the map
                if (!IsWithin(walkerPos, 0))
                    break;

                Map[walkerPos.x, walkerPos.y] = 1;
            }
            // Add the stairs down at the last step of the last branch
            if (b == branches - 1)
            {
                stairDown = walkerPos;
            }
        }
    }

    Vector3Int WalkerStep(Vector3Int step)
    {
        Vector3Int nextStep = Vector3Int.zero;
        // If the walker has no recorded previous direction, choose one at random
        if (step.z == 0)
        {
            //North = 1, East = 2, South = 3, West = 4
            nextStep.z = 1 + Random.Range(0, 4);
        }
        else
        {
            nextStep.z = 1 + Random.Range(0, 4);
            // Prevent the walker from going back to where it just came from
            if (nextStep.z == OppositeDir(step.z))
            {
                nextStep.z = step.z;
            }
        }
        // Get the unit vector for the direction of the next step
        Vector2Int vect = DirVect(nextStep);

        // Checks if TWO steps ahead is off the map, and if so force a turn
        Vector3Int check = new Vector3Int(step.x + 2 * vect.x, step.y + 2 * vect.y, step.z);
        if (!IsWithin(check, 1))
        {
            // Adding or subtracting 1 to the z component of the step equates to turning to the left or right
            int turn = Random.value > 0.5 ? 1 : -1;
            int newDir = nextStep.z + turn;
            if (newDir > 4) newDir = 1;
            if (newDir < 1) newDir = 4;
            // Get the new direction vector after turning
            nextStep.z = newDir;
            vect = DirVect(nextStep);
        }

        nextStep.x = step.x + vect.x;
        nextStep.y = step.y + vect.y;

        return nextStep;
    }
    // Return the opposite direction of d
    int OppositeDir (int d)
    {
        switch (d)
        {
            case 1:
                return 3;
            case 2:
                return 4;
            case 3:
                return 1;
            case 4:
                return 2;
            default:
                return 0;
        }
    }
    // Get the unit vector for the direction int in step.z
    Vector2Int DirVect (Vector3Int step)
    {
        switch (step.z)
        {
            case 1:
                return Vector2Int.up;
            case 2:
                return Vector2Int.right;
            case 3:
                return Vector2Int.down;
            case 4:
                return Vector2Int.left;
            default:
                return Vector2Int.zero;
        }
    }
    // Quickly check that position is within map limits
    bool IsWithin (Vector3Int pos, int border)
    {
        if (pos.x < 0 + border || pos.x >= mapSize - border || pos.y < 0 + border || pos.y >= mapSize - border)
            return false;
        return true;
    }
    // Returns true if at the tile at (x, y) has one neighbouring tile with the value of 'key'
    private bool HasNeighbour(int x, int y, int key)
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
    // Expand all cleared area by one more tile
    int[,] ExpandByOne(int[,] map)
    {
        int[,] newMap = new int[mapSize, mapSize];
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                if (map[x, y] == 1 || HasNeighbour(x, y, 1))
                {
                    newMap[x, y] = 1;
                }
            }
        }
        return newMap;
    }

    int[,] BoundaryWalls(int[,] map)
    {
        int[,] newMap = map;
        for (int x = 0; x < mapSize; x++)
        {
            newMap[x, 0] = 0;
            newMap[x, mapSize - 1] = 0;
        }
        for (int y = 0; y < mapSize; y++)
        {
            newMap[0, y] = 0;
            newMap[mapSize - 1, y] = 0;
        }
        return newMap;
    }
    // Fill tiles based on the values set in the Map array
    void PaintByNumber(Tilemap tilemap, TileBase[] tiles, int key)
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                if (Map[x, y] == key)
                {
                    tilemap.SetTile(positions[CoordToId(new Vector2Int(x, y), mapSize)], RndFromTiles(tiles));
                }
            }
        }
    }

    void PaintTheWall(Tilemap tilemap, TileBase[] tiles, int key)
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                if (Map[x, y] == 1 && HasNeighbour(x, y, key))
                {
                    tilemap.SetTile(positions[CoordToId(new Vector2Int(x, y), mapSize)], RndFromTiles(tiles));
                }
            }
        }
    }
    /// <summary>
    /// Add Tiles below wall edges to give impression of height
    /// </summary>
    /// <param name="drawTilemap"></param>
    /// <param name="checkTilemap"></param>
    /// <param name="tiles">Must contain only the three tiles for vertical walls</param>
    void PaintVerticals(Tilemap drawTilemap, Tilemap checkTilemap, TileBase[] tiles)
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize - 1; y++)
            {
                if (Map[x, y] == 1 && HasNeighbour(x, y, 0))
                {
                    // Check tiles above
                    bool a2 = false;
                    if (IsWithin(new Vector3Int(x - 1, y + 1, 0), 0))
                    {
                        a2 = (checkTilemap.GetTile(new Vector3Int(x, y + 1, 0)) != null);
                    }
                    // Check tile to left
                    bool b1IsRoof = false;
                    if (IsWithin(new Vector3Int(x - 1, y, 0), 0))
                    {
                        b1IsRoof = checkTilemap.GetTile(new Vector3Int(x - 1, y, 0)) != null;
                    }
                    // Check tile in same pos
                    bool b2IsRoof = checkTilemap.GetTile(new Vector3Int(x, y, 0)) != null;
                    // Check tile to right
                    bool b3IsRoof = false;
                    if (IsWithin(new Vector3Int(x + 1, y, 0), 0))
                    {
                        b3IsRoof = checkTilemap.GetTile(new Vector3Int(x + 1, y, 0)) != null;
                    }

                    if (a2 && !b1IsRoof && b2IsRoof && b3IsRoof)
                        drawTilemap.SetTile(positions[CoordToId(new Vector2Int(x, y), mapSize)], tiles[0]);
                    if (a2 && Map[x, y + 1] == 0 && b1IsRoof && b2IsRoof && b3IsRoof)
                        drawTilemap.SetTile(positions[CoordToId(new Vector2Int(x, y), mapSize)], tiles[1]);
                    if (a2 && b1IsRoof && b2IsRoof && !b3IsRoof)
                        drawTilemap.SetTile(positions[CoordToId(new Vector2Int(x, y), mapSize)], tiles[2]);
                }
            }
        }
    }

    // --- RESULTS-ANALYSIS FUNCTIONS ---

    float TotalClearDungeon ()
    {
        float count = 0;
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                if (Map[x, y] > 0)
                {
                    count++;
                }
            }
        }
        return count / (mapSize * mapSize);
    }

    float[] GridClearDungeon ()
    {
        float[] counts = new float[16];
        // Exterior loops for grid points
        for (int x0 = 1; x0 < 5; x0++)
        {
            for (int y0 = 1; y0 < 5; y0++)
            {
                float count = 0;
                // Interior loop within grid areas
                for (int x = 0; x < mapSize / 4; x++)
                {
                    for (int y = 0; y < mapSize / 4; y++)
                    {
                        if (Map[(x + (x0-1) * mapSize / 4), (y + (y0-1) * mapSize / 4)] > 0)
                            count++;
                    }
                }
                counts[(x0-1) + (y0-1)*4] = count / (mapSize * mapSize / 16);
            }
        }
        return counts;
    }

    bool QualityCheck ()
    {
        float totalClear = TotalClearDungeon();
        float[] gridClear = GridClearDungeon();

        if (totalClear > 0.7f)
        {
            Debug.Log(string.Format("Quality Check Failed: total cleared area too high ({0})", totalClear));
            return false;
        }
        else if (totalClear < 0.15f) {
            Debug.Log(string.Format("Quality Check Failed: total cleared area too low ({0})", totalClear));
            return false;
        }

        // Grid checks
        int clearNine = 0;
        float maxClear = 0.0f;
        int clearZero = 0;
        for (int g = 0; g < 16; g++)
        {
            // Find the grid with the highest amount cleared
            if (gridClear[g] > maxClear)
                maxClear = gridClear[g];
            // Check for grids above 90% cleared
            if (gridClear[g] > 0.9f)
            {
                clearNine++;
                // Check if adjacent grids cleared above 'key'
                float key = 0.75f;
                int adjEight = 0;
                int adjCount = 0;
                if (InGrid(g + 4))
                {
                    adjCount++;
                    if (gridClear[g + 4] > key) adjEight++;
                }
                if (InGrid(g + 1))
                {
                    adjCount++;
                    if (gridClear[g + 1] > key) adjEight++;
                }
                if (InGrid(g - 4))
                {
                    adjCount++;
                    if (gridClear[g - 4] > key) adjEight++;
                }
                if (InGrid(g - 1))
                {
                    adjCount++;
                    if (gridClear[g - 1] > key) adjEight++;
                }
                // If more than half of adjacent grids are cleared above 'key'
                if (adjEight / adjCount >= 0.5f)
                {
                    Debug.Log("Quality Check Failed: too many adjacent grids with +80% area cleared.");
                    return false;
                }
            }
            // Check for grids that have no cleared area (0%)
            if (gridClear[g] < Mathf.Epsilon)
            {
                clearZero++;
            }
        }
        // If total cleared area is less than 20% and more than 6 grids are empty
        if (totalClear < 0.20f && clearZero > 6)
        {
            Debug.Log(string.Format("Quality Check Failed: total cleared area is too low ({0}), and too condensed.", totalClear));
            return false;
        }
        // If more than two/one grid is above 90% and more than four/six grids are 0% cleared
        if ((clearNine > 2 && clearZero > 4) || (clearNine > 1 && clearZero > 6))
        {
            Debug.Log(string.Format("Quality Check Failed: imbalanced generation, too many grids above 90% ({0}) and 0% ({1}).", clearNine, clearZero));
            return false;
        }
        // If the max. cleared area is +95% and there are any empty grids. This leaves a small chance of allowing the most open dungeons.
        if (maxClear >= 0.95f && clearZero != 0)
        {
            Debug.Log("Quality Check Failed: max. cleared grid is above 95%.");
            return false;
        }

        Debug.Log(string.Format("Cleared Area: {0}; Max. Cleared Grid: {1}; Clear Grids: {2}, Empty Grids: {3}", totalClear, maxClear, clearNine, clearZero));
        return true;
    }

    bool InGrid (int index)
    {
        if (index < 0 || index > 15)
            return false;
        return true;
    }

    // ---- RESOURCE LAYOUT METHODS ----



    // ---- TILE UTILITIES ----

    TileBase RndFromTiles(TileBase[] tiles)
    {
        int index = Mathf.FloorToInt(UnityEngine.Random.value * tiles.Length);
        if (index >= tiles.Length) index = tiles.Length - 1;
        return tiles[index];
    }

    // Convert a Vector2 map coordinate to the corresponding array index
    int CoordToId(Vector2Int vect, int size)
    {
        return vect.y * size + vect.x;
    }
}
