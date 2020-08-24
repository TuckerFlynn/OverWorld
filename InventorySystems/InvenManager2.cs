using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InvenManager2 : MonoBehaviour
{
    public static InvenManager2 invenManager2;

    ItemsDatabase itemsDB;
    CharacterManager charMngr;

    // Inventory size is set from the activeChar inventory size at loading
    public InvenItem[] Inventory;
    /// <summary>0=Legs, 1=Chest, 2=Head, 3=Mainhand, 4=Offhand, 5=Amulet, 6=Backpack </summary>
    public InvenItem[] Equipment = new InvenItem[7];
    // Reference to an external object containing an inventory
    public InvenItem[] Container;

    [Header("INVENTORY UI ELEMENTS")]
    public GameObject inventoryOverview;
    public GameObject containerOverview;
    [HideInInspector]
    public ContainerTile activeContainer;

    /// <summary>0=Body, 1=Legs, 2=Chest, 3=Hair, 4=Head, 5=Mainhand, 6=Offhand </summary>
    public Image[] characterImages;
    // Same order as Equipment array
    public Image[] equipImages;
    public GameObject MainInvenPanel;
    public GameObject ExternalInvenPanel;
    public GameObject ExternalCharInvenPanel;

    public GameObject tooltip;
    public GameObject rightClickMenu;

    [Header("PREFABS")]
    public GameObject mainInvenSlot;
    public GameObject containerInvenSlot;
    public GameObject droppedItem;

    [Header("OTHER")]
    public EventSystem eventSystem;
    public GraphicRaycaster mainInvenRaycaster;
    public GraphicRaycaster externalInvenRaycaster;

    // Equipment change actions!!
    public event Action OnMainhandChange;

    private void Awake()
    {
        if (invenManager2 == null)
            invenManager2 = this;
        else if (invenManager2 != this)
        {
            Debug.LogWarning("Duplicate InvenManager2 destroyed");
            Destroy(this);
        }
    }

    private void Start()
    {
        itemsDB = ItemsDatabase.itemsDatabase;
        charMngr = CharacterManager.characterManager;

        Inventory = new InvenItem[charMngr.activeChar.inventory.Length];
        LoadCharacterInventory();
    }

    private void Update()
    {
        tooltip.SetActive(false);
        if (inventoryOverview.activeSelf || containerOverview.activeSelf)
        {
            // Raycast under the mouse to check if it is hovering over an active inventory slot
            PointerEventData pointerEventData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };
            List<RaycastResult> results = new List<RaycastResult>();
            if (inventoryOverview.activeSelf)
                mainInvenRaycaster.Raycast(pointerEventData, results);
            else
                externalInvenRaycaster.Raycast(pointerEventData, results);

            // Loop through raycast results and check for actions
            foreach (RaycastResult result in results)
            {
                // Enable the right click menu
                if (Input.GetMouseButtonDown(1) || Input.GetMouseButton(1))
                {
                    if (result.gameObject.TryGetComponent<InvenTooltip>(out InvenTooltip tooltipScript))
                    {
                        rightClickMenu.SetActive(true);
                        rightClickMenu.transform.position = result.gameObject.transform.position + new Vector3(40.0f, -40.0f);
                        RightClickMenu script = rightClickMenu.GetComponent<RightClickMenu>();
                        script.source = tooltipScript.panel;
                        script.index = tooltipScript.index;
                    }
                }
                // Enable the tooltip
                else if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !rightClickMenu.activeSelf)
                {
                    if (result.gameObject.TryGetComponent<InvenTooltip>(out InvenTooltip tooltipScript))
                    {
                        Item item = GetInvenByString<InvenItem[]>(tooltipScript.panel)[tooltipScript.index].Item;
                        tooltipScript.UpdateTooltip(tooltip, item);
                    }
                }
            }
        }
    }

    // Load the player equipment and inventory from the character manager; converting from a BasicInvenItem to an InvemItem
    private void LoadCharacterInventory()
    {
        for (int i = 0; i < Equipment.Length; i++)
        {
            Equipment[i] = new InvenItem();
            if (charMngr.activeChar.equipment[i] > 0)
            {
                Equipment[i].Item = itemsDB.GetItem(charMngr.activeChar.equipment[i]);
                Equipment[i].Quantity = 1;
            }
        }

        for (int i = 0; i < Inventory.Length; i++)
        {
            Inventory[i] = new InvenItem();
            if (charMngr.activeChar.inventory[i].Quantity > 0)
            {
                Inventory[i].Item = itemsDB.GetItem(charMngr.activeChar.inventory[i].ID);
                Inventory[i].Quantity = charMngr.activeChar.inventory[i].Quantity;
            }
        }

        RefreshCharacterPreview();
        RefreshMainInvenUI();
    }
    // Update the large character preview in the inventory UI
    public void RefreshCharacterPreview()
    {
        // Body
        characterImages[0].sprite = charMngr.activeChar.bodySprite;
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
        // Hair
        characterImages[3].sprite = charMngr.activeChar.hairSprite;
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

        // Update activeChar equipment (non-UI) and refresh character
        charMngr.activeChar.equipment = ToIntArray(Equipment);
        charMngr.UpdateCharacter();
    }
    // Update equipment and inventory slot images and quantity text
    public void RefreshMainInvenUI()
    {
        // Equipment images are explicitly set with equipImages because they are children of different objects due to layout
        for (int i = 0; i < Equipment.Length; i++)
        {
            if (Equipment[i].Quantity > 0)
            {
                equipImages[i].sprite = Equipment[i].Item.Sprite;
                equipImages[i].raycastTarget = true;
            }
            else
            {
                equipImages[i].sprite = equipImages[i].gameObject.GetComponent<ItemDragHandler>().DefaultSprite;
                equipImages[i].raycastTarget = false;
            }
        }
        // Loop through inventory and enable/disable UI elements accordingly
        for (int i = 0; i < Inventory.Length; i++)
        {
            InvenSlot invenSlot = MainInvenPanel.transform.GetChild(i).GetComponent<InvenSlot>();
            if (Inventory[i].Quantity > 0)
            {
                invenSlot.UpdateSlot(Inventory[i].Item.Sprite, Inventory[i].Quantity.ToString());
            }
            else
            {
                invenSlot.ClearSlot();
            }
        }

        // Update activeChar inventory (non-UI)
        charMngr.activeChar.inventory = ToBasicInven(Inventory);
    }

    public void RefreshContainerInvenUI()
    {
        // Loop through inventory and enable/disable UI elements accordingly
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (ExternalCharInvenPanel.transform.GetChild(i).TryGetComponent<InvenSlot>(out InvenSlot invenSlot))
            {
                if (Inventory[i].Quantity > 0)
                {
                    invenSlot.UpdateSlot(Inventory[i].Item.Sprite, Inventory[i].Quantity.ToString());
                }
                else
                {
                    invenSlot.ClearSlot();
                }
            }
        }
        // Loop through container and enable/disable UI elements accordingly
        for (int i = 0; i < Container.Length; i++)
        {
            if (ExternalInvenPanel.transform.GetChild(i).TryGetComponent<InvenSlot>(out InvenSlot invenSlot))
            {

                if (Container[i].Quantity > 0)
                {
                    invenSlot.UpdateSlot(Container[i].Item.Sprite, Container[i].Quantity.ToString());
                }
                else
                {
                    invenSlot.ClearSlot();
                }
            }
        }

        charMngr.activeChar.inventory = ToBasicInven(Inventory);
        activeContainer.Items = Container;
    }

    // -------- MUY IMPORTANTE ---------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceIndex"></param>
        /// <param name="dest"></param>
        /// <param name="destIndex">Defaults to -1 to fill partial stacks and/or first available slot</param>
        /// <param name="quantity">Defaults to -1 to move to quantity in slot</param>
        /// <returns></returns>
    public bool MoveItems (string source, int sourceIndex, string dest, int destIndex = -1, int quantity = -1)
    {
        bool success = false;

        // COPIES OF SOURCE AND DEST, NOT JUST REFERENCE TO ACTUAL
        InvenItem[] tempSource = new InvenItem[GetInvenByString<InvenItem[]>(source).Length];
        InvenItem[] tempDest = new InvenItem[GetInvenByString<InvenItem[]>(dest).Length];
        for (int s = 0; s < tempSource.Length; s++)
            tempSource[s] = new InvenItem
            {
                Item = GetInvenByString<InvenItem[]>(source)[s].Item,
                Quantity = GetInvenByString<InvenItem[]>(source)[s].Quantity,
            };
        for (int d = 0; d < tempDest.Length; d++)
            tempDest[d] = new InvenItem
            {
               Item = GetInvenByString<InvenItem[]>(dest)[d].Item,
               Quantity = GetInvenByString<InvenItem[]>(dest)[d].Quantity
            };

        // Limit quantity to 1 if moving items into equipment slots
        if (dest == "Equipment")
            quantity = 1;

        // Move item from temp source --> dest
        if (AddItem(tempSource[sourceIndex].Item.ID, quantity, dest, destIndex))
        {
            if (destIndex == -1)
            {
                // Special case when quick moving items into equipment slots with an item already equipped;
                // Overrides destIndex = -1 with the actual index
                if (dest == "Equipment")
                {
                    for (int e = 0; e < Equipment.Length; e++)
                    {
                        if (tempSource[sourceIndex].Item.ID == Equipment[e].Item.ID)
                        {
                            destIndex = e;
                        }
                    }
                    if (destIndex != -1)
                    {
                        AddItem(tempDest[destIndex].Item.ID, quantity, source, -1);
                    }
                }
                // Remove item from source
                RemoveItemByIndex(quantity, source, sourceIndex);
                success = true;
            }
            else
            {
                // If destination is empty, just remove the original item
                if (tempDest[destIndex].Quantity == 0)
                {
                    RemoveItemByIndex(quantity, source, sourceIndex);
                    success = true;
                }
                // If an item is being moved onto the same, stack instead of switching
                else if (tempSource[sourceIndex].Item.ID == tempDest[destIndex].Item.ID && dest != "Equipment")
                {
                    RemoveItemByIndex(quantity, source, sourceIndex);
                    GetInvenByString<InvenItem[]>(dest)[destIndex].Quantity += tempDest[destIndex].Quantity;
                    // If the combined slots are more than one full stack, overflow the extras with destIndex = -1
                    if (GetInvenByString<InvenItem[]>(dest)[destIndex].Quantity > tempDest[destIndex].Item.Stack)
                    {
                        int surplus = GetInvenByString<InvenItem[]>(dest)[destIndex].Quantity - tempDest[destIndex].Item.Stack;
                        GetInvenByString<InvenItem[]>(dest)[destIndex].Quantity = tempDest[destIndex].Item.Stack;
                        AddItem(tempDest[destIndex].Item.ID, surplus, dest);
                    }
                    success = true;
                }
                // If not all items in the source slot are being moved, any items in destination slot must go somewhere else
                else if (tempSource[sourceIndex].Quantity > quantity)
                {
                    RemoveItemByIndex(quantity, source, sourceIndex);
                    success = AddItem(tempDest[destIndex].Item.ID, quantity, source, -1);
                }
                // Move item from temp dest --> source
                else if (AddItem(tempDest[destIndex].Item.ID, quantity, source, sourceIndex))
                {
                    success = true;
                }
                else
                {
                    // *** Possibly drop item on failure? ***
                    success = false;
                }
            }
            // --- UPDATE EQUIPMENT STATS ---

            // Trigger OnMainhandChange event
            if ((dest == "Equipment" && destIndex == 3) || (source == "Equipment" && sourceIndex == 3) && OnMainhandChange != null)
                OnMainhandChange();

            return success;
        }
        else
        {
            return success;
        }
    }

    public bool AddItem(int id, int quantity, string dest, int destIndex = -1)
    {
        bool success = false;

        // Equipment is a special case of only accepting appropriate items in specific slots
        if (dest == "Equipment")
        {
            if (destIndex == -1)
            {
                for (int e = 0; e < Equipment.Length; e++)
                {
                    if (e == itemsDB.GetItem(id).Equip)
                    {
                        Equipment[e].Item = itemsDB.GetItem(id);
                        Equipment[e].Quantity = 1;
                        return success = true;
                    }
                }
            }
            else if (destIndex == itemsDB.GetItem(id).Equip)
            {
                Equipment[destIndex].Item = itemsDB.GetItem(id);
                Equipment[destIndex].Quantity = 1;
                return success = true;
            }
            else
            {
                Debug.Log(string.Format("Cannot equip item ({0}) to the attempted slot.", itemsDB.GetItem(id).ToString()));
                return success;
            }
        }

        // If no destination index is provided, manager will try to stack or place in first empty slot
        if (destIndex == -1)
        {
            InvenItem[] invenRef = GetInvenByString<InvenItem[]>(dest);
            for (int i = 0; i < invenRef.Length; i++)
            {
                // Check if the dest inventory contains less than a full stack of the item being moved
                if (invenRef[i].Item.ID == id && invenRef[i].Quantity < invenRef[i].Item.Stack && quantity > 0)
                {
                    int fullStack = invenRef[i].Item.Stack - invenRef[i].Quantity;
                    if (fullStack > quantity)
                    {
                        invenRef[i].Quantity += quantity;
                        return success = true;
                    }
                    else
                    {
                        invenRef[i].Quantity = invenRef[i].Item.Stack;
                        quantity -= fullStack;
                    }
                }
            }
            // If all partial stacks have been filled but there is still a quantity remaining to add, try to fill an empty slot
            if (!success)
            {
                int stack = itemsDB.GetItem(id).Stack;
                for (int i = 0; i < invenRef.Length; i++)
                {
                    if (invenRef[i].Quantity == 0)
                    {
                        // If quantity remaining is less than one stack, add it all to one slot
                        if (quantity < stack)
                        {
                            invenRef[i] = new InvenItem
                            {
                                Item = itemsDB.GetItem(id),
                                Quantity = quantity
                            };
                            return success = true;
                        }
                        // Otherwise subtract a stack, and keep filling slots
                        else
                        {
                            invenRef[i] = new InvenItem
                            {
                                Item = itemsDB.GetItem(id),
                                Quantity = stack
                            };
                            quantity -= stack;
                        }
                    }
                }
            }
            // If this line is reached, all partial stacks and empty stacks have been filled and there are still items to add
            Debug.Log(string.Format("Unable to add {0} of item {1} to {2}", quantity, itemsDB.GetItem(id), dest));
            return success;
        }
        // Otherwise move the indicated quantity into the desired slot
        else
        {
            InvenItem[] invenRef = GetInvenByString<InvenItem[]>(dest);
            invenRef[destIndex] = new InvenItem
            {
                Item = itemsDB.GetItem(id),
                Quantity = quantity
            };
            return success = true;
        }
    }

    public bool RemoveItemByIndex(int quantity, string source, int sourceIndex)
    {
        bool success = false;

        InvenItem[] invenRef = GetInvenByString<InvenItem[]>(source);
        if (invenRef[sourceIndex].Quantity > quantity)
        {
            invenRef[sourceIndex].Quantity -= quantity;
            return success = true;
        }
        else if (invenRef[sourceIndex].Quantity == quantity)
        {
            invenRef[sourceIndex] = new InvenItem();
            return success = true;
        }
        else
        {
            return success;
        }
    }

    public bool RemoveItemById(int id, int quantity, string source)
    {
        bool success = false;

        InvenItem[] invenRef = GetInvenByString<InvenItem[]>(source);
        // Reverse loop through inventory, removing items until the input quantity is removed
        for (int i = invenRef.Length - 1; i >= 0; i--)
        {
            if (invenRef[i].Item.ID == id)
            {
                // If all quantity to be removed is less than a stack
                if (invenRef[i].Quantity > quantity)
                {
                    invenRef[i].Quantity -= quantity;
                    return success = true;
                }
                // ... equal to one stack
                else if (invenRef[i].Quantity == quantity)
                {
                    invenRef[i] = new InvenItem();
                    return success = true;
                }
                else if (invenRef[i].Quantity < quantity)
                {
                    quantity -= invenRef[i].Quantity;
                    invenRef[i] = new InvenItem();
                }
            }
        }
        // If there is still a quantity to remove but the entire inven has been checked
        if (quantity > 0)
        {
            // ** Should not be possible! Use HaveItems before removing to check sufficient quantity
            return success;
        }

        return success;
    }

    public void DropItem(int quantity, string source, int sourceIndex)
    {
        GameObject groundItem = Instantiate(droppedItem);

        InvenItem item = GetInvenByString<InvenItem[]>(source)[sourceIndex];
        // Make sure you can't drop more items than are in your inventory
        if (item.Quantity < quantity)
            quantity = item.Quantity;
        groundItem.GetComponent<SpriteRenderer>().sprite = item.Item.Sprite;
        groundItem.GetComponent<GroundItem>().ID = item.Item.ID;
        // Uses amount parameter instead of InvenItem quantity to allow dropping single items, half stacks, etc
        groundItem.GetComponent<GroundItem>().Quantity = quantity;
        groundItem.transform.position = charMngr.charObject.transform.position + new Vector3(0.0f, 1.5f);

        // Remove the item from the specified inven
        RemoveItemByIndex(quantity, source, sourceIndex);
        // *** Possibly add a drop item by ID? But wasn't used in previous invenMngr
    }

    /// <summary>
    /// Check if the inventory contains at least the specified amount of an item; checking for item id = 0 will always return true
    /// </summary>
    /// <param name="id">Item ID</param>
    /// <param name="quantity">Required amount</param>
    /// <returns></returns>
    public bool HaveItems(int id, int quantity)
    {
        bool success = false;
        if (id == 0)
            return true;
        int count = 0;
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i].Item.ID == id)
            {
                count += Inventory[i].Quantity;
            }
        }
        if (count >= quantity)
            success = true;
        return success;
    }

    public void CompressAllStacks()
    {

    }

    // ---- UTILITIES ----

    // Get a field from this class by string, ie. InvenItem[] inven = invenMngr.GetInvenByString<InvenItem[]>("Inventory");
    public T GetInvenByString<T>(string inven)
    {
        return (T)typeof(InvenManager2).GetField(inven).GetValue(this);
    }
    // Convert expanded inventories back into simplified form for saving as Json
    public BasicInvenItem[] ToBasicInven(InvenItem[] inven)
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
    private int[] ToIntArray(InvenItem[] inven)
    {
        int[] output = new int[inven.Length];
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = inven[i].Item.ID;
        }
        return output;
    }
}
