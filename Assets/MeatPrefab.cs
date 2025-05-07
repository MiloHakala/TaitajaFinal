using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeatPrefab : MonoBehaviour
{
    public enum MeatState
    {
        Raw,
        Medium,
        WellDone,
        Burnt
    }
    public enum MeatType
    {
        Chicken,
        Beef,
        //Pork,
        //Fish
    }
    public GameObject rawSprite;
    public GameObject mediumSprite;
    public GameObject wellDoneSprite;
    public GameObject burntSprite;

    public MeatState currentState = MeatState.Raw;
    public MeatType currentType = MeatType.Chicken;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == MeatState.Raw)
        {
            rawSprite.SetActive(true);
            mediumSprite.SetActive(false);
            wellDoneSprite.SetActive(false);
            burntSprite.SetActive(false);
        }
        else if (currentState == MeatState.Medium)
        {
            rawSprite.SetActive(false);
            mediumSprite.SetActive(true);
            wellDoneSprite.SetActive(false);
            burntSprite.SetActive(false);
        }
        else if (currentState == MeatState.WellDone)
        {
            rawSprite.SetActive(false);
            mediumSprite.SetActive(false);
            wellDoneSprite.SetActive(true);
            burntSprite.SetActive(false);
        }
        else if (currentState == MeatState.Burnt)
        {
            rawSprite.SetActive(false);
            mediumSprite.SetActive(false);
            wellDoneSprite.SetActive(false);
            burntSprite.SetActive(true);
        }
    }
}
