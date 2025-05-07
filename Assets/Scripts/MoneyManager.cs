using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;
    public int money = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Add(100);
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public bool HasEnough(int amount)
    {
        return money >= amount;
    }

    public void Add(int amount)
    {
        money += amount;
    }

    public bool Spend(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            return true;
        }
        return false;
    }
}
