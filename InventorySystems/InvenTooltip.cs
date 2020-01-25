using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvenTooltip : MonoBehaviour
{
    public Vector3 offset = new Vector3(0.0f, 10.0f, 0.0f);
    public string panel = "Inventory";
    public int index = 0;

    private void Start()
    {
        if (panel == "Inventory")
        {
            Transform grandParent = transform.parent.parent;
            int childCount = grandParent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                if (this.transform.IsChildOf(grandParent.GetChild(i)))
                {
                    this.index = i;
                }
            }
        }
    }
}
