using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightClickMenu : MonoBehaviour
{
    InventoryManager invenMngr;
    public BuildingSystem buildSystem;
    public IngameMenu ingameMenu;
    public string source;
    public int index;


    private void Start()
    {
        invenMngr = InventoryManager.inventoryManager;
    }

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
        int q = invenMngr.GetInvenByString<InvenItem[]>(source)[index].Quantity;
        invenMngr.DropItem(index, source, q);
    }

    public void Use()
    {
        Item item = invenMngr.GetInvenByString<InvenItem[]>(source)[index].Item;
        if (item is Building building)
        {
            ingameMenu.HideAllUI();
            buildSystem.StartBuild(source, index, building);
        }
        if (item is Consumable consumable)
        {
            // Remove item from inventory
            invenMngr.RemoveFromInventory(source, index);
            // Apply effects
            for (int i = 0; i < consumable.Effects.Count; i++)
            {
                StatusEffect effect = consumable.Effects[i];
                if (effect.Status == "hunger")
                {
                    if (effect.Discrete)
                    {
                        HungerManager.hungerManager.HungerDiscrete(effect.Effect);
                    }
                    else
                    {
                        HungerManager.hungerManager.HungerRate(effect.Effect, effect.Time);
                    }
                }
            }
        }
    }
}
