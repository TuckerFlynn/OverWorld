using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager craftingManager;

    ItemsDatabase itemDB;
    InvenManager2 invenMngr;
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
    public List<int> availableStations = new List<int>();

    private CraftRecipe activeRecipe;

    void Awake()
	{
        if (craftingManager == null)
            craftingManager = this;
        else if (craftingManager != this)
            Destroy(this);
    }

    private void Start()
    {
        itemDB = ItemsDatabase.itemsDatabase;
        craftingDB = CraftingDatabase.craftingDatabase;
        invenMngr = InvenManager2.invenManager2;

        availableStations.Add(0);

        RefreshRecipeButtons();
    }

    // ------- UI SETUP --------

    // Delete all displayed recipes and rebuild the list
    public void RefreshRecipeButtons()
    {
        bool searchFilter = false;
        if (!string.IsNullOrWhiteSpace(search.text))
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
            // Split the search input by seperate words
            string searchStr = search.text.ToLower();
            string[] searchTerms = searchStr.Split(' ');
            // Loop through each term and check if the recipe title and/or keywords contain all terms
            bool success = true;
            bool exact = false;
            for (int i = 0; i < searchTerms.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(searchTerms[i]))
                    continue;
                // If any of the input terms aren't found in the title or fuzzy keywords, the search is not a success
                if (craftRecipe.Fuzzy.IndexOf(searchTerms[i], System.StringComparison.OrdinalIgnoreCase) < 0
                    && craftRecipe.Title.IndexOf(searchTerms[i], System.StringComparison.OrdinalIgnoreCase) < 0)
                {
                    success = false;
                }
                // If the search input contains an exact item name, that recipe will be moved to the top of the list
                if (searchStr.Contains(craftRecipe.Title.ToLower()))
                {
                    exact = true;
                }

                if (!success)
                    break;
            }
            // If there is a search input and a recipe has no matches, don't add a button for it
            if (searchFilter && !success)
            {
                continue;
            }
            // Skip a recipe if the required crafting station is not available
            if (!availableStations.Contains(craftRecipe.Station))
            {
                Debug.Log(string.Format("Unable to craft {0}; not close enough to crafting station {1}", craftRecipe.Title, craftRecipe.Station));
                continue;
            }

            GameObject prefab = Instantiate(craftPrefab);
            prefab.transform.SetParent(ScrollContent.transform, false);
            SetRecipeButton(prefab, craftRecipe);

            if (searchFilter && exact)
            {
                ContentButtons.Add(prefab);
                prefab.transform.SetAsFirstSibling();
            }
            else
            {
                ContentButtons.Add(prefab);
            }
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
            images[3].transform.GetChild(0).gameObject.SetActive(true);
            images[3].transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            images[3].transform.GetChild(0).gameObject.SetActive(false);
            images[3].transform.GetChild(1).gameObject.SetActive(true);
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
            Image[] images = obj.GetComponentsInChildren<Image>();
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
            // Display checkmark or X to indicate if the player has enough resources to make an item
            if (haveItems)
            {
                images[3].transform.GetChild(0).gameObject.SetActive(true);
                images[3].transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                images[3].transform.GetChild(0).gameObject.SetActive(false);
                images[3].transform.GetChild(1).gameObject.SetActive(true);
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

    // ----- THE IMPORTANT ONE -----

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
            bool remove = invenMngr.RemoveItemById(input.ID, input.Quantity * amount, "Inventory");
        }
        invenMngr.AddItem(activeRecipe.OutputID, amount, "Inventory");

        UpdateRecipeButtons();
        RefreshUI();
        invenMngr.RefreshMainInvenUI();
    }

    // ------- UTILITIES -------
    public void UpdateAvailableStation (bool add, int ID)
    {
        if (add)
            availableStations.Add(ID);
        else
            availableStations.Remove(ID);

        RefreshRecipeButtons();
        RefreshUI();
    }
}