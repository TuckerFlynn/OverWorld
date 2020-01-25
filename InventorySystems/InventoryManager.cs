﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour
{
    CharacterManager charMngr;
    ItemsDatabase itemDB;

    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    private PointerEventData pointerEventData;

    public InvenItem[] Inventory = new InvenItem[66];
    /// <summary>
    /// By index: 0=Legs, 1=Chest, 2=Head, 3=Mainhand, 4=Offhand, 5=Amulet, 6=Backpack
    /// </summary>
    public InvenItem[] Equipment = new InvenItem[7];
    [Header("Inven UI elements")]
    public GameObject invenCanvas;
    public GameObject invenPanelHolder;
    private GameObject[] invenPanels;
    public GameObject tooltip;
    public Image[] equipImages;
    public Image[] characterImages;
    [Header("Dropped item prefab")]
    public GameObject itemPrefab;

    private void Start()
    {
        charMngr = CharacterManager.characterManager;
        itemDB = ItemsDatabase.itemsDatabase;

        // Get all inventory panels as array of gameobjects
        invenPanels = new GameObject[invenPanelHolder.transform.childCount];
        for (int i = 0; i < invenPanelHolder.transform.childCount; i++)
        {
            invenPanels[i] = invenPanelHolder.transform.GetChild(i).gameObject;
        }
        // Load the character inventory from the current active character, and setup the display window
        LoadInvenUI();
    }
    // Called via Update on the Inventory Canvas, not via Update on this script because it only needs to run while the inven is enabled
    public void InvenActiveUpdate()
    {
        // Set the tooltip position and text when the mouse is over an image with the InvenTooltip script attached
        tooltip.SetActive(false);
        pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            bool noMouse = !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2);
            // Display tooltip if an image under the mouse has the tooltip script and no mouse buttons are down
            if (result.gameObject.TryGetComponent<InvenTooltip>(out InvenTooltip tooltipScript) && noMouse)
            {
                tooltip.SetActive(true);
                tooltip.transform.position = result.gameObject.transform.position + tooltipScript.offset;
                if (tooltipScript.panel == "Equipment")
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append("<size=7><color='white'>");
                    builder.Append(Equipment[tooltipScript.index].Item.Title);
                    builder.Append("</color></size>").AppendLine();
                    builder.Append(Equipment[tooltipScript.index].Item.Description);
                    tooltip.GetComponentInChildren<Text>().text = builder.ToString();
                }
                else
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append("<size=7><color='white'>");
                    builder.Append(Inventory[tooltipScript.index].Item.Title);
                    builder.Append("</color></size>").AppendLine();
                    builder.Append(Inventory[tooltipScript.index].Item.Description);
                    tooltip.GetComponentInChildren<Text>().text = builder.ToString();
                }
            }
        }
    }

    // This function overwrites the ingame inventory arrays with the inventory arrays existing on file
    void LoadInvenUI()
    {
        // Load unchanging character sprites
        characterImages[0].sprite = charMngr.bodySprites[charMngr.activeChar.bodyIndex];
        characterImages[3].sprite = charMngr.hairSprites[charMngr.activeChar.hairIndex];
        // Load the array of active equipment and refresh the sprites
        for (int i = 0; i < Equipment.Length; i++)
        {
            Equipment[i] = new InvenItem();
            if (charMngr.activeChar.equipment[i] > 0)
            {
                Equipment[i].Item = itemDB.GetItem(charMngr.activeChar.equipment[i]);
                Equipment[i].Quantity = 1;
            }
        }

        // Load the array of inventory content and refresh the sprites
        for (int i = 0; i < Inventory.Length; i++)
        {
            Inventory[i] = new InvenItem();
            if (charMngr.activeChar.inventory[i].Quantity > 0)
            {
                Inventory[i].Item = itemDB.GetItem(charMngr.activeChar.inventory[i].ID);
                Inventory[i].Quantity = charMngr.activeChar.inventory[i].Quantity;
            }
        }
        // Refresh UI
        RefreshInvenUI();
        // Update character preview
        UpdateCharPreview();
    }

    void RefreshInvenUI()
    {
        for (int i = 0; i < Equipment.Length; i++)
        {
            if (Equipment[i].Quantity > 0)
            {
                equipImages[i].enabled = true;
                equipImages[i].sprite = Equipment[i].Item.Sprite;
            }
            else
            {
                equipImages[i].enabled = false;
            }
        }
        // Load the array of inventory content and refresh the sprites
        for (int i = 0; i < Inventory.Length; i++)
        {
            GameObject image = invenPanels[i].transform.GetChild(0).gameObject;
            GameObject text = image.transform.GetChild(0).gameObject;
            if (Inventory[i].Quantity > 0)
            {
                image.GetComponent<Image>().enabled = true;
                image.GetComponent<Image>().sprite = Inventory[i].Item.Sprite;
                text.GetComponent<Text>().enabled = true;
                text.GetComponent<Text>().text = Inventory[i].Quantity.ToString();
            }
            else
            {
                image.GetComponent<Image>().enabled = false;
                text.GetComponent<Text>().enabled = false;
            }
        }
        // Update activeChar inventories and character sprites (non-UI)
        charMngr.activeChar.inventory = ToBasicInven(Inventory);
        charMngr.activeChar.equipment = ToIntArray(Equipment);
        charMngr.UpdateCharacter();
    }
    // Refresh the character preview by checking the currently active equipment
    void UpdateCharPreview()
    {
        // Legs
        if (Equipment[0].Item != null && Equipment[0].Item.ID != 0)
        {
            characterImages[1].color = Color.white;
            characterImages[1].sprite = Equipment[0].Item.Sprite;
        }
        else
        {
            characterImages[1].color = Color.clear;
        }
        // Chest
        if (Equipment[1].Item != null && Equipment[1].Item.ID != 0)
        {
            characterImages[2].color = Color.white;
            characterImages[2].sprite = Equipment[1].Item.Sprite;
        }
        else
        {
            characterImages[2].color = Color.clear;
        }
        // Head
        if (Equipment[2].Item != null && Equipment[2].Item.ID != 0)
        {
            characterImages[4].color = Color.white;
            characterImages[4].sprite = Equipment[2].Item.Sprite;
        }
        else
        {
            characterImages[4].color = Color.clear;
        }
        // Mainhand
        if (Equipment[3].Item != null && Equipment[3].Item.ID != 0)
        {
            characterImages[5].color = Color.white;
            characterImages[5].sprite = Equipment[3].Item.Sprite;
        }
        else
        {
            characterImages[5].color = Color.clear;
        }
        // Offhand
        if (Equipment[4].Item != null && Equipment[4].Item.ID != 0)
        {
            characterImages[6].color = Color.white;
            characterImages[6].sprite = Equipment[4].Item.Sprite;
        }
        else
        {
            characterImages[6].color = Color.clear;
        }
    }
    // Handles all cases of moving items between the player inventory and equipment slots
    public bool MoveItemsInternal(int sourceIndex, string sourceStr, int destIndex, string destStr)
    {
        bool success = false;
        // There are three possible source/destination combinations ... (no way to move Equipment => Equipment)

        // Moving an item from the inventory
        if (sourceStr == "Inventory")
        {
            if (destStr == "Inventory")
            {
                // Move from inventory space into empty inventory space
                if (Inventory[destIndex].Quantity == 0)
                {
                    Inventory[destIndex] = Inventory[sourceIndex];
                    Inventory[sourceIndex] = new InvenItem();
                    success = true;
                }
                // Move from inventory space into occupied inventory space
                else
                {
                    // Check if items can be stacked
                    if (Inventory[sourceIndex].Item.ID == Inventory[destIndex].Item.ID)
                    {
                        Inventory[destIndex].Quantity += Inventory[sourceIndex].Quantity;
                        // If the total of the two stacks is more than a full stack, move as many as possible to the destination
                        if (Inventory[destIndex].Quantity > Inventory[destIndex].Item.Stack)
                        {
                            int diff = Inventory[destIndex].Quantity - Inventory[destIndex].Item.Stack;
                            Inventory[destIndex].Quantity = Inventory[destIndex].Item.Stack;
                            Inventory[sourceIndex].Quantity = diff;
                        }
                        // Otherwise fill the destination stack and empty the source
                        else
                        {
                            Inventory[sourceIndex] = new InvenItem();
                        }
                    }
                    else
                    {
                        InvenItem sourceItem = Inventory[sourceIndex];
                        InvenItem destItem = Inventory[destIndex];
                        Inventory[sourceIndex] = destItem;
                        Inventory[destIndex] = sourceItem;
                        success = true;
                    }
                }
            }
            // Moving to equipment
            else if (destStr == "Equipment")
            {
                // First check the the item is allowed to be equipped in the requested slot
                if (Inventory[sourceIndex].Item.Equip == destIndex)
                {
                    // Move from inventory to empty equipment space
                    if (Equipment[destIndex].Quantity == 0)
                    {
                        Equipment[destIndex] = Inventory[sourceIndex];
                        // Decrease inven quantity or remove from inven
                        if (Inventory[sourceIndex].Quantity > 1)
                            Inventory[sourceIndex].Quantity--;
                        else
                            Inventory[sourceIndex] = new InvenItem();
                        success = true;
                    }
                    // Move from inventory to occupied equipment space
                    else
                    {
                        int destID = Equipment[destIndex].Item.ID;
                        Equipment[destIndex] = Inventory[sourceIndex];
                        // Decrease inven quantity or remove from inven
                        if (Inventory[sourceIndex].Quantity > 1)
                            Inventory[sourceIndex].Quantity--;
                        else
                            Inventory[sourceIndex] = new InvenItem();
                        // Add replaced equipment back into inventory
                        bool addToInven = AddToInventory(destID, false);


                        // IF ADDTOINVEN IS FALSE, Inventory IS FULL SO DROP THE ITEM
                        if (!addToInven)
                        {
                            DropItem(destIndex, destStr, GetInvenByString<InvenItem[]>(destStr)[destIndex].Quantity, false);
                            Debug.Log("Inventory is full, dropping " + itemDB.ItemDatabase[destID]);
                            success = false;
                        }
                    }
                }
                else
                {
                    Debug.Log("Cannot equip item " + Inventory[sourceIndex].Item + " to slot " + destIndex);
                    success = false;
                }
            }
        }
        // Moving an item from equipment
        else if (sourceStr == "Equipment")
        {
            if (destStr == "Inventory")
            {
                // Move from equipment to empty inventory slot
                if (Inventory[destIndex].Quantity == 0)
                {
                    Inventory[destIndex] = Equipment[sourceIndex];
                    Equipment[sourceIndex] = new InvenItem();
                    success = true;
                }
                // Move from equipment to occupied slot
                else
                {
                    // If the destination slot contains an item that can be equipped in the source slot, swap items
                    if (sourceIndex == Inventory[destIndex].Item.Equip)
                    {
                        InvenItem sourceItem = Equipment[sourceIndex];
                        InvenItem destItem = Inventory[destIndex];
                        Inventory[destIndex] = sourceItem;
                        Equipment[sourceIndex] = destItem;
                        success = true;
                    }
                    // If the destination slot contains an item that cannot be equipped in the source slot, add the source item w/o changing the destination item
                    else
                    {
                        int sourceID = Equipment[sourceIndex].Item.ID;
                        bool addToInven = AddToInventory(sourceID, false);
                        // IF ADDTOINVEN IS FALSE, Inventory IS FULL SO DROP THE ITEM
                        if (!addToInven)
                        {
                            DropItem(sourceIndex, sourceStr, GetInvenByString<InvenItem[]>(sourceStr)[sourceIndex].Quantity, false);
                            Debug.Log("Inventory is full, dropping " + itemDB.ItemDatabase[sourceID]);
                            success = false;
                        }
                        Equipment[sourceIndex] = new InvenItem();
                    }
                }
            }
        }
        RefreshInvenUI();
        UpdateCharPreview();
        return success;
    }

    public bool AddToInventory(int id, bool refreshUI = true)
    {
        bool success = false;
        // Loop through inventory contents
        for (int i = 0; i < Inventory.Length; i++)
        {
            // First check if the item is already present in less than a full stack
            if (Inventory[i].Item.ID == id && Inventory[i].Quantity < Inventory[i].Item.Stack)
            {
                Inventory[i].Quantity++;
                success = true;
                break;
            }
        }
        // Loop through second time if the item is not present already
        if (!success)
        {
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i].Quantity == 0)
                {
                    Inventory[i] = new InvenItem
                    {
                        Item = itemDB.GetItem(id),
                        Quantity = 1
                    };
                    success = true;
                    break;
                }
            }
        }
        // Refresh the UI
        if (refreshUI) RefreshInvenUI();
        return success;
    }

    public void DropItem(int index, string source, int amount, bool refreshUI = true)
    {
        GameObject obj = Instantiate(itemPrefab);

        InvenItem item = GetInvenByString<InvenItem[]>(source)[index];
        // Make sure you can't drop more items than are in your inventory
        if (item.Quantity < amount) amount = item.Quantity;
        obj.GetComponent<SpriteRenderer>().sprite = item.Item.Sprite;
        obj.GetComponent<GroundItem>().ID = item.Item.ID;
        // Uses amount parameter instead of InvenItem quantity to allow dropping single items, half stacks, etc
        obj.GetComponent<GroundItem>().Quantity = amount;
        obj.transform.position = charMngr.charObject.transform.position + new Vector3(0.0f, 1.5f);

        if (item.Quantity == amount)
        {
            GetInvenByString<InvenItem[]>(source)[index] = new InvenItem();
        }
        else
        {
            GetInvenByString<InvenItem[]>(source)[index].Quantity -= amount;
        }

        if (refreshUI) RefreshInvenUI();
    }

    // Get a field from this class by string, ie. InvenItem[] inven = invenMngr.GetInvenByString<InvenItem[]>("Inventory");
    public T GetInvenByString<T> (string inven)
    {
        return (T)typeof(InventoryManager).GetField(inven).GetValue(this);
    }

    // Convert expanded inventories back into simplified form for saving as Json
    private BasicInvenItem[] ToBasicInven (InvenItem[] inven)
    {
        BasicInvenItem[] output = new BasicInvenItem[inven.Length];
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = new BasicInvenItem()
            {
                ID = inven[i].Item.ID,
                Quantity = inven[i].Quantity
            };
        }
        return output;
    }
    private int[] ToIntArray (InvenItem[] inven)
    {
        int[] output = new int[inven.Length];
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = inven[i].Item.ID;
        }
        return output;
    }
}

public class InvenItem
{
    public Item Item { get; set; }
    public int Quantity { get; set; }

    public InvenItem()
    {
        Item = new Item();
        Quantity = 0;
    }
}
/// <summary>
/// Simplified version of InvenItem that only contains ID and Quantity
/// </summary>
[Serializable]
public class BasicInvenItem
{
    public int ID { get; set; }
    public int Quantity { get; set; }
}