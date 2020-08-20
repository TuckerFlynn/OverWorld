using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerTile : MonoBehaviour
{
    InventoryManager invenMngr;

    public int SlotNum = 20;
    public bool locked;
    public InvenItem[] Items;

    GameObject InvenSlotParent;
    public GameObject InvenSlotPrefab;

    private void Start()
    {
        invenMngr = InventoryManager.inventoryManager;
        Items = new InvenItem[SlotNum];

        InvenSlotParent = invenMngr.containerSlotParent;
    }

    public void OpenContainer ()
    {
        // Add the correct amount of inventory slots to the container window
        for (int i = 0; i < SlotNum; i++)
        {
            // Prefab has variables set so no need to save reference! (?)
            Instantiate(InvenSlotPrefab, InvenSlotParent.transform);
        }
        invenMngr.Container = Items;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerControl playerControl) && !locked)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                OpenContainer();
            }
        }
    }
}