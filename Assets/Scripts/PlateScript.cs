using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlateScript : MonoBehaviour
{
    public List<GameObject> foodOnPlate;
    public LevelManager levelManager;
    public AudioSource audi;

    public int completedOrders = 0;
    public int ordersToComplete = 3;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Build the plate contents list
            List<string> allFoodOnPlate = new List<string>();
            foreach (GameObject food in foodOnPlate)
            {
                string foodName;
                string tag = food.tag;

                if (tag == "Chicken")
                {
                    MeatPrefab foodScript = food.GetComponent<MeatPrefab>();
                    foodName = "chicken(" + foodScript.currentState + ")";
                }
                else if (tag == "Beef")
                {
                    MeatPrefab foodScript = food.GetComponent<MeatPrefab>();
                    foodName = "beef(" + foodScript.currentState + ")";
                }
                else
                {
                    foodName = tag.ToLower();
                }

                allFoodOnPlate.Add(foodName);
            }

            Debug.Log("Food on plate: " + string.Join(", ", allFoodOnPlate));

            int match = levelManager.FindMatchingOrder(allFoodOnPlate);
            if (match >= 0)
            {
                Debug.Log($"✅ Plate matches order number {match + 1}");
                audi.Play();
                completedOrders++;

                if (completedOrders >= ordersToComplete)
                {
                    Debug.Log("All orders completed!");
                    levelManager.CompleteGame();
                }

                // Destroy and clear plate
                ClearPlate();
            }
            else
            {
                Debug.Log("❌ Plate does NOT match any order; clearing plate contents.");
                ClearPlate();
            }
        }
    }

    private void ClearPlate()
    {
        foreach (GameObject food in foodOnPlate)
        {
            if (food != null)
                Destroy(food);
        }

        foodOnPlate.Clear();
    }
}
