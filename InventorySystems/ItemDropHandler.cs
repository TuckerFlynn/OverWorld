using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDropHandler : MonoBehaviour, IDropHandler
{
    private InventoryManager invenMgr;

    public string receiver = "Inventory";
    public int index;

    void Start()
    {
        invenMgr = FindObjectOfType<InventoryManager>();
        if (receiver == "Inventory" || receiver == "Container")
        {
            Transform parent = transform.parent;
            int childCount = parent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                if (this.transform == parent.GetChild(i))
                {
                    index = i;
                }
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            ItemDragHandler itemDragHandler = eventData.pointerDrag.GetComponent<ItemDragHandler>();
            //InvenItem[] inven = invenMgr.GetInvenByString<InvenItem[]>(itemDragHandler.source);
            //Debug.Log("Dropping " + inven[itemDragHandler.index].Item + " from " + itemDragHandler.source + " slot " + itemDragHandler.index);

            itemDragHandler.ForceEndDrag();

            invenMgr.MoveItemsInternal(itemDragHandler.index, itemDragHandler.source, index, receiver);
        }
    }
}