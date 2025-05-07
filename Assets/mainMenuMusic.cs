using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainMenuMusic : MonoBehaviour

{
    public AudioClip[] musicClips;           // Assign in Inspector
    public AudioSource audioSource;          // Assign or auto-get
    public bool loopPlaylist = true;         // Should the playlist loop?

    private int currentTrackIndex = 0;

    void Awake()
    {
        // Ensure only one instance persists
        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (musicClips.Length > 0)
        {
            PlayTrack(currentTrackIndex);
        }
    }

    void Update()
    {
        if (!audioSource.isPlaying && musicClips.Length > 0)
        {
            NextTrack();
        }
    }

    private void PlayTrack(int index)
    {
        if (index >= 0 && index < musicClips.Length)
        {
            audioSource.clip = musicClips[index];
            audioSource.Play();
        }
    }

    private void NextTrack()
    {
        currentTrackIndex++;

        if (currentTrackIndex >= musicClips.Length)
        {
            if (loopPlaylist)
                currentTrackIndex = 0;
            else
                return;
        }

        PlayTrack(currentTrackIndex);
    }
}


