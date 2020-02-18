using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightClickMenu : MonoBehaviour
{
    public InventoryManager invenMgr;
    public BuildingSystem buildSystem;
    public IngameMenu ingameMenu;
    public string source;
    public int index;

    void Update()
    {
        // Check if the mouse is outside the menu area
        if (!RectTransformUtility.RectangleContainsScreenPoint(this.gameObject.transform as RectTransform, Input.mousePosition))
        {
            gameObject.SetActive(false);   
        }
    }

    public void Drop ()
    {
        int q = invenMgr.GetInvenByString<InvenItem[]>(source)[index].Quantity;
        invenMgr.DropItem(index, source, q);
    }

    public void Use()
    {
        Item item = invenMgr.GetInvenByString<InvenItem[]>(source)[index].Item;
        if (item is Building building)
        {
            ingameMenu.HideAllUI();
            buildSystem.StartBuild(source, index, building);
        }
    }
}
