using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    ItemsDatabase itemDB;
    public InventoryManager invenMngr;
    CraftingDatabase craftingDB;

    [Header("Info Panel")]
    public GameObject infoPanel;
    public Image infoImage;
    public Text infoTitle;
    public Text infoText;
    public Button craftOne;
    public Button craftFive;
    public Button craftAll;
    public GameObject noInfoPanel;
    [Header("Scroll Panel")]
    public GameObject ScrollContent;
    public ToggleGroup toggleGroup;
    public GameObject craftPrefab;
    public List<GameObject> ContentButtons = new List<GameObject>();
    [Header("Filters")]
    public InputField search;

    private CraftRecipe activeRecipe;

    void Awake()
	{
        itemDB = ItemsDatabase.itemsDatabase;
		craftingDB = CraftingDatabase.craftingDatabase;
    }

    private void Start()
    {
        RefreshRecipeButtons();
    }

    // Delete all displayed recipes and rebuild the list
    public void RefreshRecipeButtons()
    {
        bool searchFilter = false;
        if (search.text != "")
        {
            searchFilter = true;
        }
        int count = ScrollContent.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            ContentButtons.Add(ScrollContent.transform.GetChild(i).gameObject);
        }

        ClearRecipeButtons();

        // Loop through all crafting recipes and add a UI element for each one
        foreach (CraftRecipe craftRecipe in craftingDB.craftRecipes)
        {
            string searchStr = search.text.ToLower();
            if (searchFilter && !craftRecipe.Fuzzy.Contains(searchStr))
            {
                continue;
            }

            GameObject prefab = Instantiate(craftPrefab);
            prefab.transform.SetParent(ScrollContent.transform, false);
            SetRecipeButton(prefab, craftRecipe);

            ContentButtons.Add(prefab);
        }
    }
    // Resets the ScrollContent list and deletes existing recipe buttons
    void ClearRecipeButtons ()
    {
        int count = ScrollContent.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            Destroy(ScrollContent.transform.GetChild(i).gameObject);
        }
        ContentButtons.Clear();
        ContentButtons.RemoveAll(obj => obj == null);
    }
    // Sets the recipe button image, title, and text
    void SetRecipeButton(GameObject obj, CraftRecipe recipe)
    {
        // Set the image to the output item sprite
        Image[] images = obj.GetComponentsInChildren<Image>();
        images[2].sprite = itemDB.GetItem(recipe.OutputID).Sprite;
        // Set the title to the recipe title
        Text[] texts = obj.GetComponentsInChildren<Text>();
        texts[0].text = recipe.Title;
        // Check if character inventory conatina all the required items
        bool haveItems = true;
        foreach (CraftInput input in recipe.Inputs)
        {
            if (!invenMngr.HaveItems(input.ID, input.Quantity))
            {
                haveItems = false;
                break;
            }
        }
        if (haveItems)
        {
            texts[1].text = "Sufficient Resources";
        }
        else
        {
            texts[1].text = "Insufficient Resources";
        }
        // Assign the toggle to the toggle group of all recipes
        Toggle toggle = obj.GetComponent<Toggle>();
        toggle.group = toggleGroup;
        toggle.onValueChanged.AddListener(delegate {
            RefreshUI();
        });
        obj.GetComponent<RecipeToggle>().recipe = recipe;
    }
    // Just refreshes the text on all recipe buttons
    public void UpdateRecipeButtons ()
    {
        foreach (GameObject obj in ContentButtons)
        {
            CraftRecipe recipe = obj.GetComponent<RecipeToggle>().recipe;
            Text[] texts = obj.GetComponentsInChildren<Text>();
            // Check if character inventory conatins all the required items
            bool haveItems = true;
            foreach (CraftInput input in recipe.Inputs)
            {
                if (!invenMngr.HaveItems(input.ID, input.Quantity))
                {
                    haveItems = false;
                    break;
                }
            }
            if (haveItems)
            {
                texts[1].text = "Sufficient Resources";
            }
            else
            {
                texts[1].text = "Insufficient Resources";
            }
        }
    }
    // Update the crafting info panel
    public void RefreshUI ()
    {
        CraftRecipe recipe = null;
        foreach (GameObject child in ContentButtons)
        {
            if (child.GetComponent<Toggle>().isOn)
            {
                recipe = child.GetComponent<RecipeToggle>().recipe;
                break;
            }
        }
        if (recipe != null)
        {
            activeRecipe = recipe;

            infoPanel.SetActive(true);
            infoImage.color = Color.white;
            infoImage.sprite = itemDB.GetItem(recipe.OutputID).Sprite;

            // Create title text
            StringBuilder builder = new StringBuilder();
            builder.Append(recipe.Title).Append(" x").Append(recipe.OutputQuantity);
            infoTitle.text = builder.ToString();

            // Create info text from CraftRecipe object
            builder = new StringBuilder();
            foreach (CraftInput input in recipe.Inputs)
            {
                builder.Append("x").Append(input.Quantity).Append(" ").Append(itemDB.GetItem(input.ID).Title).AppendLine();
            }
            builder.Append("Crafting time: ").Append(recipe.CraftTime);
            infoText.text = builder.ToString();

            // Enable or disable crafting buttons
            bool haveOne = true;
            bool haveFive = true;
            foreach (CraftInput input in recipe.Inputs)
            {
                if (!invenMngr.HaveItems(input.ID, input.Quantity))
                {
                    haveOne = false;
                }
                if (!invenMngr.HaveItems(input.ID, input.Quantity * 5))
                {
                    haveFive = false;
                }
            }
            craftOne.interactable = haveOne;
            craftFive.interactable = haveFive;
            craftAll.interactable = haveOne;

            noInfoPanel.SetActive(false);
        }
        else
        {
            infoPanel.SetActive(false);
            noInfoPanel.SetActive(true);
        }
    }
    // Remove input items from the character inventory and add the output
    public void Craft(int amount)
    {
        // First: If input amount is 0, find the maximum amount possible
        if (amount == 0)
        {
            bool plusOne = true;
            int maxAmount = 0;
            while (plusOne)
            {
                maxAmount++;
                foreach (CraftInput input in activeRecipe.Inputs)
                {
                    if (!invenMngr.HaveItems(input.ID, input.Quantity * maxAmount))
                    {
                        maxAmount--;
                        plusOne = false;
                        break;
                    }
                }
            }
            amount = maxAmount;
        }
        // Second: check that there are enough resources for the amount requested to craft ( 1 or 5 or maxAmount )
        foreach (CraftInput input in activeRecipe.Inputs)
        {
            if (!invenMngr.HaveItems(input.ID, input.Quantity * amount))
                return;
        }
        // Remove inputs from the inventory and add the output
        foreach (CraftInput input in activeRecipe.Inputs)
        {
            bool remove = invenMngr.RemoveFromInventory(input.ID, input.Quantity * amount, false);
        }
        for (int a = 0; a < activeRecipe.OutputQuantity * amount; a++)
        {
            invenMngr.AddToInventory(activeRecipe.OutputID);
        }

        UpdateRecipeButtons();
    }
}
