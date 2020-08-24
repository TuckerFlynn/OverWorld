using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundItem : MonoBehaviour
{
    public int ID;
    public int Quantity;

    private void OnTriggerEnter2D(Collider2D col)
    {
        // If two of the same items on the ground are overlapping, combine them into one stack
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
