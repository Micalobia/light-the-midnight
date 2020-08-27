using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainChar_sounds : MonoBehaviour
{
    public float soundToPlay = -1.0f; //this with designate which sound to play. -1 means don't play any sound. This is a float because then the animation window can access it. 
    public AudioClip[] audioClip; //this holds the sounds

    AudioSource audio;//for holding the audio source

    void Start()
    {
        audio = gameObject.GetComponent<AudioSource>();//put this in start. This gets the audio source. }
    }
    void Update()
    { 
        if (soundToPlay > -1.0f) //if the sound is greater than the value for not playing a sound 
        {
            PlaySound((int)soundToPlay, 1);
            soundToPlay = -1.0f;
            PlaySound((int)soundToPlay, 2);
            soundToPlay = -1.0f;
            PlaySound((int)soundToPlay, 3);
            soundToPlay = -1.0f;
            PlaySound((int)soundToPlay, 4);
            soundToPlay = -1.0f;

            void PlaySound(int clip, float volumeScale)
            {
                audio.PlayOneShot(audioClip[clip], volumeScale);//play the sound with the designated clip and volume scale }
            }
        }
     }
}