using System.Collections.Generic;
using UnityEngine;

public class RecipeSelectionDistributor : MonoBehaviour
{
    public static RecipeSelectionDistributor Instance { get; private set; }

    [Header("Outputs (populated at runtime)")]
    public List<RecipeCard> toolRecipes = new List<RecipeCard>();
    public List<RecipeCard> seasoningRecipes = new List<RecipeCard>();
    public List<RecipeCard> foodRecipes = new List<RecipeCard>();

    private void Awake()
    {
        // Singleton pattern: keep one instance alive
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Call this once you have your 5 selected RecipeCard objects.
    /// It will clear and then refill the three lists.
    /// </summary>
    public void DistributeSelections(List<RecipeCard> selectedCards)
    {
        toolRecipes.Clear();
        seasoningRecipes.Clear();
        foodRecipes.Clear();

        foreach (var card in selectedCards)
        {
            switch (card.type)
            {
                case RecipeType.Tool:
                    toolRecipes.Add(card);
                    break;
                case RecipeType.Seasoning:
                    seasoningRecipes.Add(card);
                    break;
                case RecipeType.Food:
                    foodRecipes.Add(card);
                    break;
            }
        }

        Debug.Log($"Distributed: {toolRecipes.Count} tools, " +
                  $"{seasoningRecipes.Count} seasonings, " +
                  $"{foodRecipes.Count} foods.");
    }
}
