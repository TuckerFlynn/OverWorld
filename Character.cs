using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    public string name;
    public int bodyIndex;
    public int chestIndex;
    public int legsIndex;
    public int hairIndex;

    // Default both positions to outside the map, indicating a new player
    public Vector2Int worldPos = Vector2Int.one * -1;
    // note: fieldPos is used as transform position, doesn't need to be int
    public Vector2 fieldPos = Vector2.one * -1;
}