using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PickupProp : MonoBehaviour
{
    [Header("ITEM ATTRIBUTES")]
    public int ItemID;
    public int Quantity;
    [Header("REPLACEMENT TILE")]
    public bool replace;
    public string tileset;
    public int[] index;

    bool canInteract = true;
    Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponentInParent<Tilemap>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerControl playerControl) && canInteract)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                // Prevent repeat interactions
                canInteract = false;
                // Try to add <Quanity> of <ItemID> to the character's inventory
                bool success = false;
                for (int i = 0; i < Quantity; i++)
                {
                    success = InventoryManager.inventoryManager.AddToInventory(ItemID);
                    if (!success)
                    {
                        Quantity -= i;
                        break;
                    }
                }
                // If all items can be added, remove or replace the tile
                if (success)
                {
                    StartCoroutine(DestroyOrReplaceTile());
                }
            }
        }
    }

    IEnumerator DestroyOrReplaceTile()
    {
        yield return new WaitForEndOfFrame();

        Vector3Int pos = Vector3Int.FloorToInt(transform.position);
        if (!replace)
            tilemap.SetTile(pos, null);
        else
        {
            TileBase tile;
            // If multiple replacement tiles are set, one will be choosen randomly from possibilities
            int RandIndex = index[Random.Range(0, index.Length)];
            // Check whether loaded tile should be an ObjTile or EnvrTile
            if (tileset == "PropTiles" || tileset == "PlantTiles")
                tile = TilesetLoader.GetTilesetByString<List<ObjTile>>(tileset)[RandIndex] as TileBase;
            else
                tile = TilesetLoader.GetTilesetByString<List<EnvrTile>>(tileset)[RandIndex] as TileBase;

            tilemap.SetTile(pos, tile); // This is effectively DestroyImmediate for this script
        }
    }
}
