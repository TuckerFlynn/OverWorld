using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class MineBuilder : MonoBehaviour
{
    DungeonMaster dungeonMaster;
    InventoryManager invenMngr;
    ItemsDatabase itemsDB;
    [Header("UI ELEMENTS")]
    public Button UpgradeButton;
    public GameObject[] groups;
    public Text[] texts;
    public GameObject[] sprites;

    // Max level is automatically set based on the length of the resources array
    public int maxBuildLevel;

    int[][] Quantities = {  new int[]{   5,   2,   0 },
                            new int[]{  10,   4,   0 },
                            new int[]{  15,   6,   0 },
                            new int[]{  20,   8,   0 },
                            new int[]{  25,  10,   0 },
                            new int[]{  30,  12,   0 },
                            new int[]{  35,  14,   0 },
                            new int[]{  40,  16,   0 },
                            new int[]{  45,  18,   0 },
                            new int[]{  50,  20,   0 },
                            new int[]{  60,  25,   0 },
                            new int[]{  70,  30,   0 },
                            new int[]{  80,  35,   0 },
                            new int[]{  90,  40,   0 },
                            new int[]{ 100,  45,   0 },
                            new int[]{ 110,  50,   0 },
                            new int[]{ 120,  60,   0 },
                            new int[]{ 130,  70,   5 },
                            new int[]{ 140,  80,  10 },
                            new int[]{ 150, 100,  20 } }; // [3,2] -->{ {a,b}, {c,d}, {e,f} }

    int[][] Resources = {   new int[]{ 169, 166,   0 }          // Planks, Stones, Nails
                                                         };     // Stone Bricks, Planks, Lanterns
    // Use this for initialization
    void Start()
    {
        dungeonMaster = DungeonMaster.dungeonMaster;
        invenMngr = InventoryManager.inventoryManager;
        itemsDB = ItemsDatabase.itemsDatabase;

        maxBuildLevel = Resources.GetLength(0) * 19;
    }

    // Display the required resources for improving the mine
    public void WriteMineImprovementInfo()
    {
        // Disable the button if the player does not have enough resources OR the max level has been reached
        if (CheckForResources() && !dungeonMaster.activeConfig.SetSeed)
            UpgradeButton.interactable = true;
        else
            UpgradeButton.interactable = false;
        // Check that the UI is not trying to update display to a level that does not exist
        if (dungeonMaster.activeConfig.BuildLevel < maxBuildLevel)
        {

            InvenItem[] requiredItems = GetResources(1);
            // Disable the tertiary item display if there is none
            if (requiredItems[2].Quantity == 0)
                groups[2].SetActive(false);
            else
                groups[2].SetActive(true);
            // Set the text describing required items
            texts[0].text = string.Format("{0} x{1}", requiredItems[0].Item.Title, requiredItems[0].Quantity);
            texts[1].text = string.Format("{0} x{1}", requiredItems[1].Item.Title, requiredItems[1].Quantity);
            texts[2].text = string.Format("{0} x{1}", requiredItems[2].Item.Title, requiredItems[2].Quantity);

            for (int i = 0; i < 3; i++)
            {
                if (invenMngr.HaveItems(requiredItems[i].Item.ID, requiredItems[i].Quantity))
                {
                    sprites[i].transform.GetChild(0).gameObject.SetActive(true);
                    sprites[i].transform.GetChild(1).gameObject.SetActive(false);
                }
                else
                {
                    sprites[i].transform.GetChild(0).gameObject.SetActive(false);
                    sprites[i].transform.GetChild(1).gameObject.SetActive(true);
                }
            }
        }
        // Let the player know the max level has been reached
        else
        {
            texts[0].text = "maximum";
            texts[1].text = "improvement";
            texts[2].text = "reached";

            for (int i = 0; i < 3; i++)
            {
                sprites[i].transform.GetChild(0).gameObject.SetActive(false);
                sprites[i].transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    // Check the player inventory for the materials required to build the next mine level
    public bool CheckForResources () {
        int NextBuildLevel = dungeonMaster.activeConfig.BuildLevel + 1;

        if (NextBuildLevel > maxBuildLevel)
            return false;

        int[] Amount = Quantities[NextBuildLevel % 20];
        int[] Materials = Resources[Mathf.FloorToInt(NextBuildLevel / 20)];

        for (int i = 0; i < Amount.Length; i++)
        {
            if (!invenMngr.HaveItems(Materials[i], Amount[i]))
            {
                return false;
            }
        }
        return true;
    }
    // Get the required resources and amounts as an array of InvenItems
    public InvenItem[] GetResources (int adjust)
    {
        int NextBuildLevel = dungeonMaster.activeConfig.BuildLevel + adjust;
        InvenItem[] resources = new InvenItem[3];
        if (NextBuildLevel > maxBuildLevel)
            return resources;

        int[] Amount = Quantities[NextBuildLevel % 20];
        int[] Materials = Resources[Mathf.FloorToInt(NextBuildLevel / 20)];

        for (int i = 0; i < 3; i++)
        {
            resources[i] = new InvenItem
            {
                Item = itemsDB.GetItem(Materials[i]),
                Quantity = Amount[i]
            };
        }
        return resources;
    }
}
