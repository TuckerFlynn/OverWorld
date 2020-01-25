using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvenActive : MonoBehaviour
{
    public InventoryManager invenMngr;

    // Update is called once per frame
    void Update()
    {
        invenMngr.InvenActiveUpdate();
    }
}
