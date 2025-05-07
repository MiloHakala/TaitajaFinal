using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppingBlock : MonoBehaviour
{

    public GameObject letuce;
    public bool isLettuceOnBlock = false;
    public bool isChopped = false;
    public GameObject CurrentLetuce;
    public GameObject plate;
    public PlateScript plateScript;
    public AudioSource audi;
    // Start is called before the first frame update
    void Start()
    {
        plate = GameObject.FindGameObjectWithTag("plate");
        plateScript = plate.GetComponent<PlateScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnLetuce();
        }
    }

    public void SpawnLetuce()
    {
        if (!isLettuceOnBlock)
        {
            CurrentLetuce = Instantiate(letuce, transform.position, Quaternion.identity);
            isLettuceOnBlock = true;
        }
    }
    void OnMouseOver()
    {
        if (isLettuceOnBlock)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (isChopped)
                {
                    submit();
                }
                else
                {
                    Chop();
                }
                
            }

            // Right click (destroy meat only)
            if (Input.GetMouseButtonDown(1))
            {
                Discard();
            }
        }
        // Left click (submit meat to plate)
        
    }
    void submit()
    {
        if (isLettuceOnBlock && isChopped)
        {
            CurrentLetuce.transform.SetParent(plate.transform);
            
            

            // Optionally reposition it relative to the plate
            CurrentLetuce.transform.localPosition = new Vector3(0.03f, 0.13f, 0); // adjust as needed


            // Add to plate's script list
            plateScript.foodOnPlate.Add(CurrentLetuce);
            isLettuceOnBlock = false;

            audi.Play();
            isChopped = false;




        }
    }
    void Discard()
    {
        if (isLettuceOnBlock)
        {
            Destroy(CurrentLetuce);
            isLettuceOnBlock = false;
        }
    }
    void Chop()
    {
        isChopped = true;
        ChoppabelPrefab choppable = CurrentLetuce.GetComponent<ChoppabelPrefab>();
        choppable.chop = true;
    }
}
