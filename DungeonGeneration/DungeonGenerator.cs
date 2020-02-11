using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    public Tilemap Sub;
    public Tilemap Ground;
    public Tilemap Wall;
    public Tilemap Objects;
    public GameObject Areas;

    public int mapSize = 64;
    [Header("Walker Input")]
    public int minSteps = 500;
    public int maxSteps = 2000;
    public int minBranches = 1;
    public int maxBranches = 4;
    public bool OneOrigin = true;

    int[,] Map;
    Vector3Int[] positions;
    TileBase[] tileArray;

    private void Start()
    {
        positions = new Vector3Int[mapSize * mapSize];
        for (int index = 0; index < positions.Length; index++)
        {
            positions[index] = new Vector3Int(index % mapSize, index / mapSize, 0);
        }
        Map = new int[mapSize, mapSize];
    }

    public void MainDungeonGen()
    {
        Ground.ClearAllTiles();
        Wall.ClearAllTiles();
        Objects.ClearAllTiles();
    }

    private void Update ()
    {
        int randStep = Random.Range(minSteps, maxSteps);
        int randBranch = Random.Range(minBranches, maxBranches);

        BasicWalker(randStep, randBranch, OneOrigin);
        Map = ExpandByOne(Map);

        QualityCheck();

        Sub.ClearAllTiles();
        Ground.ClearAllTiles();
        Wall.ClearAllTiles();

        TileBase ground1 = TilesetLoader.DungeonTiles[12];
        TileBase ground2 = TilesetLoader.DungeonTiles[13];
        TileBase ground3 = TilesetLoader.DungeonTiles[14];
        TileBase ground4 = TilesetLoader.DungeonTiles[15];
        TileBase ground5 = TilesetLoader.DungeonTiles[16];
        TileBase[] groundTiles = { ground1, ground2, ground3, ground4, ground5 };
        PaintByNumber(Ground, groundTiles, 1);
        TileBase roof1 = TilesetLoader.DungeonTiles[17];
        TileBase roof2 = TilesetLoader.DungeonTiles[23];
        TileBase[] roofTiles = { roof1, roof2 };
        PaintByNumber(Wall, roofTiles, 0);
        TileBase wall1 = TilesetLoader.DungeonTiles[49];
        TileBase[] wallTiles = { wall1 };
        PaintTheWall(Ground, wallTiles, 0);
        PaintTheWall(Sub, groundTiles, 0);
    }

    void BasicWalker (int steps, int branches, bool singleOrigin)
    {
        Map = new int[mapSize, mapSize];
        Vector3Int origin = new Vector3Int(1 + Random.Range(0, mapSize - 2), 1 + Random.Range(0, mapSize - 2), 0);

        for (int b = 0; b < branches; b++)
        {
            Vector3Int walkerPos;
            if (singleOrigin)
            {
                walkerPos = origin;
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
        if (index < 0 || index > 15) return false;
        return true;
    }

    // Fill tiles based on the values set in the Map array
    void PaintByNumber(Tilemap tilemap, TileBase[] tiles, int key)
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                if (Map[x,y] == key)
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
