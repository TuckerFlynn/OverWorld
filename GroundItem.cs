using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundItem : MonoBehaviour
{
    public int ID;
    public int Quantity;

    private void OnTriggerEnter2D(Collider2D col)
    {
        // If player collides with an item on the ground
        if (col.gameObject.TryGetComponent<GroundItem>(out GroundItem groundItem))
        {
            if(this.ID == groundItem.ID)
            {
                this.Quantity += groundItem.Quantity;
                if (this.GetInstanceID() < groundItem.GetInstanceID())
                {
                    Destroy(groundItem.gameObject);
                }
            }
        }
    }
}
