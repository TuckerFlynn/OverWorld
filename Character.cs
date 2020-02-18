using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Character
{
    public string name;
    // Character has the following equpiment slots:
    /// <summary>
    /// By index: 0=Legs, 1=Chest, 2=Head, 3=Mainhand, 4=Offhand, 5=Amulet, 6=Backpack
    /// </summary>
    public int[] equipment = new int[7];
    public BasicInvenItem[] inventory = new BasicInvenItem[66];
    public int bodyIndex;
    public int hairIndex;

    // Default both positions to outside the map, indicating a new player
    public Vector2IntJson worldPos = new Vector2IntJson( Vector2Int.one * -1 );
    // note: fieldPos is used as transform position, doesn't need to be int
    public Vector2Json fieldPos = new Vector2Json( new Vector2(32,32) );

    public int level;
    public int experience;

    public Character()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = new BasicInvenItem();
        }
    }
}
/// <summary>
/// Simplified Vector2Int for serializing to JSON,
/// does not include magnitude and normalized properties
/// </summary>
[Serializable]
public class Vector2IntJson
{
    public int x { get; set; }
    public int y { get; set; }

    public Vector2IntJson(Vector2Int vect)
    {
        this.x = vect.x;
        this.y = vect.y;
    }
}
/// <summary>
/// Simplified Vector2 for serializing to JSON,
/// does not include magnitude and normalized properties
/// </summary>
[Serializable]
public class Vector2Json
{
    public float x { get; set; }
    public float y { get; set; }

    public Vector2Json(Vector2 vect)
    {
        this.x = vect.x;
        this.y = vect.y;
    }
    public Vector2Json(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}

public static class Vetor2JsonExtension
{
    public static Vector3 FromJsonVect(this Vector2Json vect)
    {
        return new Vector3(vect.x, vect.y);
    }
}
