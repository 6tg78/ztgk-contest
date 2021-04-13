using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{

    [SerializeField] private List<AudioClip> musicClips = new List<AudioClip>();
    private AudioSource musicSource = default;
    [SerializeField] private AudioClip introTrack;

    private bool playingMusic = false;

    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        StartCoroutine(PlayMusic());
    }
 

    private AudioClip GetMusic()
    {
        int index = Random.Range(0, musicClips.Count);

        AudioClip clip = musicClips[index];
        return clip;
    }

    private IEnumerator PlayMusic()
    {
        AudioClip currentClip;
        bool firstTrackPlayed = false;

        while (true)
        {
            currentClip = GetMusic();
            while(currentClip == introTrack && !firstTrackPlayed)
            {
                currentClip = GetMusic(); // Getting new track if the first one played in gameplay is the one played in intro.
            }
            musicSource.clip = currentClip;
            musicSource.Play();
            yield return new WaitForSeconds(currentClip.length);
        }
    }
}
