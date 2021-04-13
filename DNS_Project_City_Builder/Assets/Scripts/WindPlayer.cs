using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindPlayer : MonoBehaviour
{
    private AudioSource sound;

    private void Start()
    {
        sound = gameObject.GetComponent<AudioSource>();
        StartCoroutine(BeginPlaying());
    }

    private IEnumerator BeginPlaying()
    {
        while(true)
        {
            if(sound.clip != null)
            {
                sound.Play();
            }
            else
            {
                Debug.Log("Wind sound isn't assigned.");
            }
            yield return new WaitForSecondsRealtime(45.0f);
        }
    }
}
