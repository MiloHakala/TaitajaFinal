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
            List<string> allFoodOnPlate = new List<string>();
            foreach (GameObject food in foodOnPlate)
            {
                
                string foodName;
                string tag = food.tag;
                if (tag == "chicken")
                {
                    MeatPrefab foodScript = food.GetComponent<MeatPrefab>();
                    Debug.Log("chicken " + foodScript.currentState);
                    foodName = "chicken " + foodScript.currentState;
                }
                else if (tag == "beef")
                {
                    MeatPrefab foodScript = food.GetComponent<MeatPrefab>();
                    Debug.Log("beef " + foodScript.currentState);
                    foodName = "beef " + foodScript.currentState;
                }
                else
                {
                    Debug.Log(tag);
                    foodName = tag;
                }
                allFoodOnPlate.Add(foodName);
                print("Food on plate: " + string.Join(", ", allFoodOnPlate));
            }

        }
    }
}
