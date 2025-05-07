using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelpickmusic : MonoBehaviour
{
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        GameObject music = GameObject.FindGameObjectWithTag("MainMenuMusicPlayer");
        if (music == null)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
