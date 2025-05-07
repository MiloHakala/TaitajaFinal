// RecipeCard.cs
using UnityEngine;

public enum RecipeType
{
    //Tool,
    Seasoning,
    Cookable,
    Choppable,
    Knife,
    FryingPan,
}

[CreateAssetMenu(fileName = "NewRecipeCard", menuName = "Card/RecipeCard")]
public class RecipeCard : ScriptableObject
{
    public string recipeName;
    public Sprite cardImage;
    public RecipeType type;
    public string tag; // Example values: "Beef", "Soup", "Fish"

    [Header("Runtime Spawn")]
    public GameObject prefabToSpawn;
}
