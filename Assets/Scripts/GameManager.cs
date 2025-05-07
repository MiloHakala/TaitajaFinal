using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public RecipeCard selectedCard;
    public List<RecipeCard> recipeCards = new List<RecipeCard>(); // Selected cards
    public List<RecipeCard> allRecipeCards = new List<RecipeCard>(); // All available cards

    public int playerScore;
    public string currentLevel;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetSelectedCard(RecipeCard card)
    {
        selectedCard = card;

    }
}
