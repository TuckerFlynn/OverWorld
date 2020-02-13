using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingActive : MonoBehaviour
{
    public CraftingManager craftingManager;

    private void OnEnable()
    {
        craftingManager.UpdateRecipeButtons();
    }
}
