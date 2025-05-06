using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasoningPrefab : MonoBehaviour
{

    public enum Type
    {
        Salt,
        Pepper,

    }
    public GameObject ChoppedSprite;
    public GameObject wholeSprite;


    
    public Type currentType = Type.Salt;
    public AudioSource chopSound;
    public bool chop = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
