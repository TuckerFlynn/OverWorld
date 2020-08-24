using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvenSortType : MonoBehaviour
{
    InvenManager2 invenMngr;

    /// <summary>
    /// sortType = 1: name; sortType = 2: type;
    /// </summary>
    public int sortType = 1;
    private int sortTypeMax = 2;
    public Text txt;

    private void Start()
    {
        invenMngr = InvenManager2.invenManager2;
    }

    public void NextSortType ()
    {
        sortType++;
        if (sortType > sortTypeMax)
        {
            sortType = 1;
        }

        switch (sortType)
        {
            case 1:
                txt.text = "sort by name";
                break;
            case 2:
                txt.text = "sort by type";
                break;
            default:
                break;
        }
    }

    // Sort the item depending on the current sorting type
    public void SortInven()
    {
        if (sortType == 1)
        {
            // Loop through inventory and combine all partial stacks


            Array.Sort(invenMngr.Inventory);
        }
        else
        {
            Debug.Log("Sorting by type has not been implemented");
        }

        invenMngr.RefreshMainInvenUI();
    }
}
