using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public Rigidbody2D rb2D;
    public float speed = 5.0f;
    bool inputUse = false;

    CharacterManager charMngr;
    InventoryManager invenMngr;

    private void Start()
    {
        charMngr = CharacterManager.characterManager;
        invenMngr = FindObjectOfType<InventoryManager>();
    }

    // Update is called once per frame
    void Update()
    {
        float horiIn = Input.GetAxis("Horizontal");
        float vertIn = Input.GetAxis("Vertical");
        // Don't try loading fields if the player is in a dungeon
        if (!charMngr.InDungeon)
        {
            // Check if the player has just started moving or is still moving from last frame
            if ((int)horiIn != 0 || (int)vertIn != 0)
            {
                if (!inputUse && MapManager.mapManager != null)
                {
                    Vector2Int charWorldPos = new Vector2Int(charMngr.activeChar.worldPos.x, charMngr.activeChar.worldPos.y);
                    // Check if the player is near an edge, and is trying to move out of the field
                    if (transform.position.x < 1.0f && horiIn < 0.0f)
                    {
                        transform.position = new Vector3(63.5f, transform.position.y);
                        MapManager.mapManager.SaveFieldFile(charWorldPos);
                        MapManager.mapManager.LoadField(charWorldPos + Vector2Int.left);
                    }
                    else if (transform.position.x > 63.0f && horiIn > 0.0f)
                    {
                        transform.position = new Vector3(0.5f, transform.position.y);
                        MapManager.mapManager.SaveFieldFile(charWorldPos);
                        MapManager.mapManager.LoadField(charWorldPos + Vector2Int.right);
                    }
                    else if (transform.position.y < 0.5f && vertIn < 0.0f)
                    {
                        transform.position = new Vector3(transform.position.x, 63.0f);
                        MapManager.mapManager.SaveFieldFile(charWorldPos);
                        MapManager.mapManager.LoadField(charWorldPos + Vector2Int.down);
                    }
                    else if (transform.position.y > 62.5f && vertIn > 0.0f)
                    {
                        transform.position = new Vector3(transform.position.x, 0.0f);
                        MapManager.mapManager.SaveFieldFile(charWorldPos);
                        MapManager.mapManager.LoadField(charWorldPos + Vector2Int.up);
                    }
                    inputUse = true;
                }
            }
            if ((int)horiIn == 0 && (int)vertIn == 0)
            {
                inputUse = false;
            }
        }

        // Move character based on input
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);
        direction *= speed;
        //transform.position += direction;
        rb2D.velocity = new Vector2(direction.x, direction.y);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // If player collides with an item on the ground
        if (col.gameObject.TryGetComponent<GroundItem>(out GroundItem groundItem))
        {
            int added = 0;
            // Items are added one at a time to ensure correct stacking
            for (int i = 0; i < groundItem.Quantity; i++)
            {
                // Add to inven returns false if there is no space
                if (invenMngr.AddToInventory(groundItem.ID))
                {
                    added++;
                }
            }
            if (added == groundItem.Quantity)
                Destroy(col.gameObject);
            else
                groundItem.Quantity -= added;
        }
    }
}
