using System.Collections.Generic;
using UnityEngine;

public class RecipeSelectionDistributor : MonoBehaviour
{
    public static RecipeSelectionDistributor Instance { get; private set; }

    [Header("Outputs (populated at runtime)")]
    public List<RecipeCard> cookableRecipes = new List<RecipeCard>();
    public List<RecipeCard> choppableRecipes = new List<RecipeCard>();
    public List<RecipeCard> seasoningRecipes = new List<RecipeCard>();
    public List<RecipeCard> knifeTools = new List<RecipeCard>();
    public List<RecipeCard> fryingPanTools = new List<RecipeCard>();

    private void Awake()
    {
        // Singleton pattern
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
    /// It will clear and refill the categorized lists.
    /// </summary>
    public void DistributeSelections(List<RecipeCard> selectedCards)
    {
        cookableRecipes.Clear();
        choppableRecipes.Clear();
        seasoningRecipes.Clear();
        knifeTools.Clear();
        fryingPanTools.Clear();

        foreach (var card in selectedCards)
        {
            switch (card.type)
            {
                case RecipeType.Cookable:
                    cookableRecipes.Add(card);
                    break;

                case RecipeType.Choppable:
                    choppableRecipes.Add(card);
                    break;

                case RecipeType.Seasoning:
                    seasoningRecipes.Add(card);
                    break;

                case RecipeType.Knife:
                    knifeTools.Add(card);
                    break;

                case RecipeType.FryingPan:
                    fryingPanTools.Add(card);
                    break;
            }
        }

        Debug.Log($"Distributed:\n" +
                  $"- Cookables: {cookableRecipes.Count}\n" +
                  $"- Choppables: {choppableRecipes.Count}\n" +
                  $"- Seasonings: {seasoningRecipes.Count}\n" +
                  $"- Knives: {knifeTools.Count}\n" +
                  $"- Frying Pans: {fryingPanTools.Count}");
    }
}
