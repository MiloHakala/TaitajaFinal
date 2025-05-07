using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class stove : MonoBehaviour
{

    public GameObject panPrefab;
    public GameObject Chicken;
    public GameObject beef;

    private GameObject currentMeatPrefab;
    public Transform panSpawnPoint;
    private FryingPan pan;
    private GameObject panObject;
    public float panCoockTime = 5f;

    public bool isPanOnStove = false;
    public bool sotveOnUse = false;

    public bool trigger1 = false;
    public GameObject plate;
    public PlateScript plateScript;

    public AudioSource audi;
    public AudioSource audi2;
    // Start is called before the first frame update
    void Start()
    {
        plate = GameObject.FindGameObjectWithTag("plate");
        plateScript = plate.GetComponent<PlateScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && !isPanOnStove)
        {
            spawnPan();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SpawnBeef();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            SpawnChicken();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            submitMeat();
        }


    }
    void OnMouseOver()
    {
        // Left click (submit meat to plate)
        if (Input.GetMouseButtonDown(0))
        {
            submitMeat();
        }

        // Right click (destroy meat only)
        if (Input.GetMouseButtonDown(1))
        {
            discardMeat();
        }
    }

    public void spawnPan()
    {
        if (!isPanOnStove)
        {
            panObject = Instantiate(panPrefab, panSpawnPoint.position, panSpawnPoint.rotation);
            isPanOnStove = true;
            pan = panObject.GetComponent<FryingPan>();
        }
        
    }

    public void SpawnChicken()
    {
        if (isPanOnStove && !sotveOnUse)
        {
            sotveOnUse = true;
            currentMeatPrefab = Instantiate(Chicken, panSpawnPoint);
            currentMeatPrefab.transform.localPosition = currentMeatPrefab.transform.localPosition + new Vector3(+0.7f, -0.55f, 0f); // Relative to pan
            pan.isFrying = true;
            MeatPrefab meat = currentMeatPrefab.GetComponent<MeatPrefab>();
            if (meat != null)
            {
                meat.StartCooking();
            }
            
        }
        if (!isPanOnStove)
        {
            Debug.Log("No pan on stove!");
        }
    }
    public void SpawnBeef()
    {
        if (isPanOnStove && !sotveOnUse)
        {
            sotveOnUse = true;
            currentMeatPrefab = Instantiate(beef,panSpawnPoint);
            currentMeatPrefab.transform.localPosition = currentMeatPrefab.transform.localPosition + new Vector3(+0.7f, -0.55f, 0f); // Relative to pan
            pan.isFrying = true;
            MeatPrefab meat = currentMeatPrefab.GetComponent<MeatPrefab>();
            if (meat != null)
            {
                meat.StartCooking();
            }
            
        }
        if (!isPanOnStove)
        {
            Debug.Log("No pan on stove!");
        }

    }
    public void submitMeat()
    {
        if (currentMeatPrefab != null && plate != null)
        {
            // Reparent the meat to the plate
            currentMeatPrefab.transform.SetParent(plate.transform);
            MeatPrefab meat = currentMeatPrefab.GetComponent<MeatPrefab>();
            meat.StopCooking();

            // Optionally reposition it relative to the plate
            currentMeatPrefab.transform.localPosition = new Vector3(-3.5f, 0.85f, 0); // adjust as needed

            // Stop frying
            pan.isFrying = false;

            // Add to plate's script list
            plateScript.foodOnPlate.Add(currentMeatPrefab);

            // Clean up pan
            Destroy(panObject);
            panObject = null;

            // Reset flags
            isPanOnStove = false;
            sotveOnUse = false;
            currentMeatPrefab = null;
            audi.Play();
        }
    }
    public void discardMeat()
    {
        if (currentMeatPrefab != null)
        {
            Destroy(currentMeatPrefab);
            currentMeatPrefab = null;

            // Stop frying
            pan.isFrying = false;

            // Reset stove usage flag
            sotveOnUse = false;

            Debug.Log("Meat discarded.");
            audi2.Play();
        }
    }

}
