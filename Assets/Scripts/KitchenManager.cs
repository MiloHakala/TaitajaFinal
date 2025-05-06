using UnityEngine;

public class KitchenManager : MonoBehaviour
{
    [Header("Toggle Station Availability")]
    public bool hasFryingPan = true;
    public bool hasKnife = true;

    [Header("Station GameObjects")]
    public GameObject fryingPanObject;
    public GameObject knifeObject;

    [Header("Station Scripts / Transforms")]
    public FryingPan fryingPan; // Should be on the same GameObject as fryingPanObject
    public Transform choppingLocation; // Point where choppable items are spawned

    private void Awake()
    {
        // Activate/deactivate stations based on booleans
        if (fryingPanObject != null)
            fryingPanObject.SetActive(hasFryingPan);

        if (knifeObject != null)
            knifeObject.SetActive(hasKnife);
    }

    public void UpdateKitchenStations()
    {
        if (fryingPanObject != null)
            fryingPanObject.SetActive(hasFryingPan);

        if (knifeObject != null)
            knifeObject.SetActive(hasKnife);
    }

    public FryingPan GetFryingPan()
    {
        return hasFryingPan ? fryingPan : null;
    }

    public Transform GetChoppingLocation()
    {
        return hasKnife ? choppingLocation : null;
    }
}
