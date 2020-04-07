using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private InventoryManager invenMgr;

    public string source = "Inventory";
    public int index;
    public Sprite DefaultSprite;

    private Vector3 originalPosition;
    private Transform originalParent;
    private RectTransform invenPanel;

    void Start()
    {
        invenMgr = FindObjectOfType<InventoryManager>();
        if (source == "Inventory")
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        gameObject.GetComponent<Image>().raycastTarget = false;
        originalParent = transform.parent;

        Transform topParent = transform;
        while ( !topParent.gameObject.name.Equals("InventoryOverview") )
        {
            topParent = topParent.parent;
        }
        invenPanel = topParent as RectTransform;
        transform.SetParent(topParent);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Returns the dragged item to its original position
        // This is required before maniupulating the inventory, even if the item is going to be removed from the inven
        transform.SetParent(originalParent);
        transform.SetAsFirstSibling();
        gameObject.GetComponent<Image>().raycastTarget = true;
        transform.position = originalPosition;

        // Check if the item has been dropped outside of the inventory window
        if (!RectTransformUtility.RectangleContainsScreenPoint(invenPanel, eventData.position))
        {
            int q = invenMgr.GetInvenByString<InvenItem[]>(source)[index].Quantity;
            if (Input.GetKey(KeyCode.LeftShift)) q = Mathf.FloorToInt(q / 2);

            Debug.Log("Dropped " + q + " of " + invenMgr.GetInvenByString<InvenItem[]>(source)[index].Item + " on the ground!");
            invenMgr.DropItem(index, source, q);
            invenMgr.UpdateCharPreview();
        }
    }

    public void ForceEndDrag()
    {
        transform.SetParent(originalParent);
        transform.SetAsFirstSibling();
        //gameObject.GetComponent<Image>().raycastTarget = true;
        transform.position = originalPosition;
    }
}
