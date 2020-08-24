using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvenSlot : MonoBehaviour
{
    public Image itemImage;
    public Text itemCount;

    public void UpdateSlot(Sprite sprite, string quantity)
    {
        itemImage.enabled = true;
        itemImage.raycastTarget = true;
        itemImage.sprite = sprite;

        itemCount.enabled = true;
        itemCount.text = quantity;
    }

    public void ClearSlot()
    {
        itemImage.enabled = false;
        itemImage.raycastTarget = false;

        itemCount.enabled = false;
        itemCount.text = "0";
    }
}
