using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class CraftingDatabase : MonoBehaviour
{
    public static CraftingDatabase craftingDatabase;
    public List<CraftRecipe> craftRecipes = new List<CraftRecipe>();

    void Awake()
    {
        if (craftingDatabase == null)
        {
            //DontDestroyOnLoad(gameObject);
            craftingDatabase = this;

            string JsonIn = Resources.Load<TextAsset>("Json/recipes").text;
            CraftRecipe[] recipes = JsonConvert.DeserializeObject<CraftRecipe[]>(JsonIn);
            for (int i = 0; i < recipes.Length; i++)
            {
                craftRecipes.Add(recipes[i]);
            }
            Debug.Log("craftRecipes list built with " + craftRecipes.Count + " recipes.");
        }
        else if (craftingDatabase != this)
        {
            Destroy(this.gameObject);
        }
    }

    public CraftRecipe GetCraftRecipe(int id)
    {
        // This is iffy, may cause unpredicted results, but for now it prevents errors when trying to request item with ID=0
        if (id == 0)
        {
            return new CraftRecipe();
        }
        if (id > craftRecipes.Count)
        {
            Debug.Log("Recipe ID " + id + " outside of craftRecipes range");
            return new CraftRecipe();
        }

        return craftRecipes.Find(recipe => recipe.ID == id);
    }
}

[Serializable]
public class CraftRecipe
{
    public int ID { get; set; }
    public string Title { get; set; }
    public string Fuzzy { get; set; }
    public int OutputID { get; set; }
    public int OutputQuantity { get; set; }
    public List<CraftInput> Inputs { get; set; }
    public float CraftTime { get; set; }
    // Not used yet, but will be added
    //public int Skill { get; set; }
    //public float Experience { get; set; }

    public override string ToString()
    {
        return "CraftRecipe ID:" + ID;
    }
}
[Serializable]
public class CraftInput
{
    public int ID { get; set; }
    public int Quantity { get; set; }
}