using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerTile : MonoBehaviour
{
    InvenManager2 invenMngr;

    public int SlotNum = 20;
    public bool open;
    public bool locked;
    public InvenItem[] Items;

    GameObject InvenSlotParent;
    public GameObject InvenSlotPrefab;

    private void Awake()
    {
        Items = new InvenItem[SlotNum];

        for (int i = 0; i < SlotNum; i++)
        {
            Items[i] = new InvenItem();
        }
    }

    private void Start()
    {
        //invenMngr = InventoryManager.inventoryManager;
        invenMngr = InvenManager2.invenManager2;

        InvenSlotParent = invenMngr.ExternalInvenPanel;
        // Fetch saved inventory via TileDataLoader
        TileDataLoader.tileDataLoader.FetchContainerData(this, this.gameObject.transform.position.ToInt2());
    }

    public void OpenContainer ()
    {
        if (!open)
        {
            // If the container UI doesn't already have the right number of slots displaying
            if (InvenSlotParent.transform.childCount != SlotNum)
            {
                for (int i = 0; i < InvenSlotParent.transform.childCount; i++)
                {
                    Destroy(InvenSlotParent.transform.GetChild(i).gameObject);
                }
                // Add the correct amount of inventory slots to the container window
                for (int i = 0; i < SlotNum; i++)
                {
                    // Prefab has variables set so no need to save reference! (?)
                    Instantiate(InvenSlotPrefab, InvenSlotParent.transform);
                }
            }

            invenMngr.activeContainer = this;
            invenMngr.Container = Items;
            invenMngr.RefreshContainerInvenUI();
            invenMngr.containerOverview.SetActive(true);

            open = true;
        }
    }

    public void CloseContainer ()
    {
        invenMngr.containerOverview.SetActive(false);
        open = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerControl playerControl) && !locked)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OpenContainer();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerControl playerControl) && !locked)
        {
            if (open)
            {
                CloseContainer();
            }
        }
    }
}