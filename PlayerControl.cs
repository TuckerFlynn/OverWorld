using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public Rigidbody2D rb2D;
    public float speed = 5.0f;
    bool inputUse = false;

    CharacterManager charMngr;

    private void Start()
    {
        charMngr = CharacterManager.characterManager;
    }

    // Update is called once per frame
    void Update()
    {
        float horiIn = Input.GetAxis("Horizontal");
        float vertIn = Input.GetAxis("Vertical");
        // Check if the player has just started moving or is still moving from last frame
        if ((int)horiIn != 0 || (int)vertIn != 0)
        {
            if (!inputUse && MapManager.mapManager != null)
            {
                Vector2Int charWorldPos = charMngr.activeChar.worldPos;
                // Check if the player is near an edge, and is trying to move out of the field
                if (transform.position.x < 1.0f && horiIn < 0.0f)
                {
                    MapManager.mapManager.LoadField(charWorldPos + Vector2Int.left);
                    transform.position = new Vector3(63.5f, transform.position.y);
                }
                else if (transform.position.x > 63.0f && horiIn > 0.0f)
                {
                    MapManager.mapManager.LoadField(charWorldPos + Vector2Int.right);
                    transform.position = new Vector3(0.5f, transform.position.y);
                }
                else if (transform.position.y < 0.5f && vertIn < 0.0f)
                {
                    MapManager.mapManager.LoadField(charWorldPos + Vector2Int.down);
                    transform.position = new Vector3(transform.position.x, 63.0f);
                }
                else if (transform.position.y > 62.5f && vertIn > 0.0f)
                {
                    MapManager.mapManager.LoadField(charWorldPos + Vector2Int.up);
                    transform.position = new Vector3(transform.position.x, 0.0f);
                }
                inputUse = true;
            }
        }
        if ((int)horiIn == 0 && (int)vertIn == 0)
        {
            inputUse = false;
        }

        // Move character based on input
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);
        direction *= speed;
        //transform.position += direction;
        rb2D.velocity = new Vector2(direction.x, direction.y);
    }
}
