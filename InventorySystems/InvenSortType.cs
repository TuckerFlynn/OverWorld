using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvenSortType : MonoBehaviour
{
    /// <summary>
    /// sortType = 1: name; sortType = 2: type;
    /// </summary>
    public int sortType = 1;
    private int sortTypeMax = 2;
    public Text txt;

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
}
