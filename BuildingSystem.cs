using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingSystem : MonoBehaviour
{
    public InventoryManager invenMngr;
    public CharacterManager charMngr;
    public Tilemap Ground;
    public Tilemap Objects;
    public Tilemap Roof;
    private Tilemap target;

    public TileBase placeholder;
    public TileBase toBuild;
    private string source;
    private int index;

    bool BuildActive;
    private Vector3Int pos;
    private Vector3Int pastPos = Vector3Int.zero;

    private void Start()
    {
        charMngr = CharacterManager.characterManager;
    }

    private void Update()
    {
        if (BuildActive)
        {
            Vector3 gamePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos = Vector3Int.FloorToInt(gamePos);
            pos.z = 0;
            // Prevent placing the item directly under the player, and also set a max distance from player
            if (Vector3Int.FloorToInt(charMngr.charObject.transform.position) == pos || (Vector3Int.FloorToInt(charMngr.charObject.transform.position)-pos).magnitude > 4)
            {
                pos = pastPos;
            }

            // If the mouse position has changed, reset the tile at the previous position to the placeholder
            if (pos != pastPos)
            {
                target.SetTile(pastPos, placeholder);
                placeholder = target.HasTile(pos) ? target.GetTile<ObjTile>(pos) : null;
            }

            target.SetTile(pos, toBuild);

            // Disable build mode and place the object
            if (Input.GetMouseButtonDown(0))
            {
                EndBuild(true);
            }
            // Disable build mode without placing object
            if (Input.GetKeyDown(KeyCode.Escape)
                || Input.GetKeyDown(KeyCode.I)
                || Input.GetKeyDown(KeyCode.O)
                || Input.GetKeyDown(KeyCode.U) )
            {
                EndBuild(false);
            }
        }

        pastPos = pos;
    }

    public void StartBuild(string source, int index, Building item)
    {
        BuildActive = true;
        this.source = source;
        this.index = index;
        toBuild = item.tileBase;
        target = GetTilemapByString<Tilemap>(item.Target);
    }

    void EndBuild(bool place)
    {
        if (place)
        {
            invenMngr.RemoveFromInventory(source, index);
            // Check for specific buildings that need to load a file or run a startup function
            // ** Note: GetTile seems to return the asset and not instance, so it doesn't work to access the gameobject via GetTile because it also references the asset
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.transform.gameObject.TryGetComponent(out MineEntrance script))
                {
                    script.SelfStart();
                }
            }
        }
        else
        {
            target.SetTile(pos, placeholder);
        }
        BuildActive = false;
        toBuild = null;
        pos = Vector3Int.zero;
        pastPos = Vector3Int.zero;
    }

    public T GetTilemapByString<T>(string str)
    {
        return (T)typeof(BuildingSystem).GetField(str).GetValue(this);
    }
}