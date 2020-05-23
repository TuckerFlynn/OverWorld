using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapField {
    // Each MapField has a unique index which can be directly translated to a global x/y position
    public int index;
    public int x;
    public int y;

    public MapField(int index, int worldSize) {
        this.index = index;
        y = Mathf.FloorToInt(index / worldSize);
        x = index % worldSize;
    }

    public Vector2Int Position
    {
        get
        {
            return new Vector2Int(x, y);
        }
        set
        {
            x = value.x;
            y = value.y;
        }
    }

    // Additional properties for MapFields
    public float Temp
    { get; set; }

    public float Precip
    { get; set; }

    public string MainBiome
    { get; set; }

    public float Elevation
    { get; set; }

    public Dictionary<string, object[]> Modifier = new Dictionary<string, object[]>();

    /// <summary>
    /// Add a modifier to the map field; Returned value indicates whether the modifier has been successfully added or not 
    /// </summary>
    /// <param name="mod">Modifier name</param>
    /// <param name="obj">Array of different values used by field generators</param>
    /// <returns></returns>
    public bool AddMod (string mod, params object[] obj) {
        // Check the Modifiers dict does not already contain the modifier being added
        if (!Modifier.ContainsKey(mod)) {
            Modifier.Add(mod, obj);
            return true;
        } else {
            //Debug.LogWarning("Key '" + mod + "' is already used @ " + Position);
            return false;
        }
    }
}