using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoofArea : MonoBehaviour
{
    private Tilemap Roof;
    public TiledObject thisTiledObject;
    public BoxCollider2D thisCollider;
    private List<Vector3Int> positions = new List<Vector3Int>();

    public void CreateRoofArea(TiledObject tiledObject, Tilemap tilemap, Vector3Int pos, int height)
    {
        Roof = tilemap;
        thisTiledObject = tiledObject;
        // Once again funky Tiled positions need to be translated
        int X = Mathf.FloorToInt(tiledObject.x / 16) + pos.x;
        int Y = Mathf.FloorToInt(64 - tiledObject.y / 16 - tiledObject.height / 16) + pos.y;
        // If the roof is being added due to spawning a prefab, the Tiled position need to be adjusted from prefab position to field position
        if (pos != Vector3Int.zero)
        {
            tiledObject.x += pos.x * 16;
            tiledObject.y -= pos.y * 16;
        }

        Vector3Int origin = new Vector3Int(X, Y, 0);
        Vector2Int colliderSize = new Vector2Int(Mathf.FloorToInt(tiledObject.width / 16), Mathf.FloorToInt(tiledObject.height / 16));

        transform.position = origin;
        thisCollider.size = colliderSize - new Vector2(0.2f, 0.2f);
        thisCollider.offset = colliderSize * new Vector2(0.5f, 0.5f);

        for (int x = origin.x; x < origin.x + colliderSize.x; x++)
        {
            for (int y = origin.y; y < origin.y + colliderSize.y; y++)
            {
                positions.Add(new Vector3Int(x, y, 0));
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerControl>(out PlayerControl player))
        { 
            foreach (Vector3Int pos in positions)
            {
                Roof.SetColor(pos, new Color(0, 0, 0, 0.1f));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerControl>(out PlayerControl player))
        {
            foreach (Vector3Int pos in positions)
            {
                Roof.SetColor(pos, Color.white);
            }
        }
    }
}
