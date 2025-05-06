using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChoppabelPrefab : MonoBehaviour
{
    public enum ChoppedState
    {
        Whole,
        Chopped

    }
    public enum Type
    {
        Salad,
        
        //Pork,
        //Fish
    }
    public GameObject ChoppedSprite;
    public GameObject wholeSprite;


    public ChoppedState currentState = ChoppedState.Whole;
    public Type currentType = Type.Salad;
    public AudioSource chopSound;
    public bool chop = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == ChoppedState.Whole)
        {
            wholeSprite.SetActive(true);
            ChoppedSprite.SetActive(false);
        }
        else if (currentState == ChoppedState.Chopped)
        {
            wholeSprite.SetActive(false);
            ChoppedSprite.SetActive(true);
            
        }
        if (chop && currentState == ChoppedState.Whole)
        {
            Chop();
        }

    }

    public void Chop()
    {
        chop = false;
        if (chopSound != null)
        {
            chopSound.Play();
        }
        
        StartCoroutine(WaitForChopSound());

    }
    IEnumerator WaitForChopSound()
    {
        yield return new WaitForSeconds(chopSound.clip.length);
        currentState = ChoppedState.Chopped;
        //Debug.Log("Chopped");
    }
}
