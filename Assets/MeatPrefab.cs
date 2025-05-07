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
        Beef
    }

    public GameObject rawSprite;
    public GameObject mediumSprite;
    public GameObject wellDoneSprite;
    public GameObject burntSprite;

    public MeatState currentState = MeatState.Raw;
    public MeatType currentType = MeatType.Chicken;

    public float cookInterval = 5f; // Time between states
    private float cookTimer = 0f;
    private bool isCooking = false;

    void Start()
    {
        UpdateVisuals(); // show correct sprite at start
    }

    void Update()
    {
        if (!isCooking) return;

        cookTimer += Time.deltaTime;

        if (cookTimer >= cookInterval)
        {
            cookTimer = 0f;
            AdvanceCookState();
        }
    }

    void AdvanceCookState()
    {
        if (currentState < MeatState.Burnt)
        {
            currentState++;
            UpdateVisuals();
            Debug.Log($"{currentType} is now {currentState}");
        }
        else
        {
            isCooking = false; // Stop cooking after Burnt
        }
    }

    void UpdateVisuals()
    {
        rawSprite.SetActive(currentState == MeatState.Raw);
        mediumSprite.SetActive(currentState == MeatState.Medium);
        wellDoneSprite.SetActive(currentState == MeatState.WellDone);
        burntSprite.SetActive(currentState == MeatState.Burnt);
    }

    public void StartCooking()
    {
        isCooking = true;
        cookTimer = 0f;
    }

    public void StopCooking()
    {
        isCooking = false;
    }
}
