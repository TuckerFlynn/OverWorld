using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDropHandler : MonoBehaviour, IDropHandler
{
    private InvenManager2 invenMgr;

    public string receiver = "Inventory";
    public int index;

    void Start()
    {
        invenMgr = InvenManager2.invenManager2;

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

            int q = invenMgr.GetInvenByString<InvenItem[]>(itemDragHandler.source)[itemDragHandler.index].Quantity;
            if (Input.GetKey(KeyCode.LeftShift))
                q = Mathf.FloorToInt(q / 2);

            invenMgr.MoveItems(itemDragHandler.source, itemDragHandler.index, receiver, index, q);
            if (invenMgr.inventoryOverview.activeSelf)
            {
                invenMgr.RefreshMainInvenUI();
                invenMgr.RefreshCharacterPreview();
            }
            else
            {
                invenMgr.RefreshContainerInvenUI();
            }
        }
    }
}