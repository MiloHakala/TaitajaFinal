using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int money = 0;
    public TextMeshProUGUI moneyText;
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

    void Start()
    {
        UpdateMoneyUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            AddMoney(100);
    }
    public void SetSelectedCard(RecipeCard card)
    {
        selectedCard = card;

    }
    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyUI();
    }

    public void SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateMoneyUI();
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

    void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = "$" + money.ToString();
    }
}
