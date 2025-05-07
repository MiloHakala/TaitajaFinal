using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public RecipeCard selectedCard;
    public List<RecipeCard> recipeCards = new List<RecipeCard>(); // Selected cards
    public List<RecipeCard> allRecipeCards = new List<RecipeCard>(); // All available cards

    public int playerScore;
    public string currentLevel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure singleton
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes
        }
    }

    public void SetSelectedCard(RecipeCard card)
    {
        selectedCard = card;
        
    }



    
}
