using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MoveCostTest : MonoBehaviour
{
    Tilemap Ground;

    // Start is called before the first frame update
    void Start()
    {
        Ground = FieldDemo.fieldDemo.Ground;
    }

    // Update is called once per frame
    void Update()
    {
        if (Ground.HasTile(Vector3Int.FloorToInt(transform.position)))
        {
            if (IsTileOfType<EnvrTile>(Ground, Vector3Int.FloorToInt(transform.position)))
            {
                EnvrTile current = (EnvrTile)Ground.GetTile(Vector3Int.FloorToInt(transform.position));
                Debug.Log(current.moveCost);
            }
        }
    }

    public bool IsTileOfType<T>(Tilemap tilemap, Vector3Int position) where T : TileBase
    {
        TileBase targetTile = tilemap.GetTile(position);

        if (targetTile != null && targetTile is T)
        {
            return true;
        }

        return false;
    }
}
