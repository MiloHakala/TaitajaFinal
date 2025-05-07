using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlateScript : MonoBehaviour
{

    public List<GameObject> foodOnPlate;
    public LevelManager levelManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // build the plate contents list
            List<string> allFoodOnPlate = new List<string>();
            foreach (GameObject food in foodOnPlate)
            {
                string foodName;
                string tag = food.tag;
                if (tag == "chicken")
                {
                    MeatPrefab foodScript = food.GetComponent<MeatPrefab>();
                    foodName = "chicken(" + foodScript.currentState + ")";
                }
                else if (tag == "beef")
                {
                    MeatPrefab foodScript = food.GetComponent<MeatPrefab>();
                    foodName = "beef(" + foodScript.currentState + ")";
                }
                else
                {
                    foodName = tag;
                }

                allFoodOnPlate.Add(foodName);
            }

            // *** INSERT MATCH‑CHECK HERE ***
            Debug.Log("Food on plate: " + string.Join(", ", allFoodOnPlate));

            int match = levelManager.FindMatchingOrder(allFoodOnPlate);
            if (match >= 0)
            {
                Debug.Log($"✅ Plate matches order number {match + 1}");
            }
            else
            {
                Debug.Log("❌ Plate does NOT match any order; clearing plate contents.");
                allFoodOnPlate.Clear();
            }
            // *** END INSERT ***

        }
    }
}
