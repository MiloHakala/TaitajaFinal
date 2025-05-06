using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipeCard", menuName = "Card/RecipeCard")]
public class RecipeCard : ScriptableObject
{
    public string recipeName;
    public Sprite cardImage;
    // You can add ingredients, difficulty, etc.
}
