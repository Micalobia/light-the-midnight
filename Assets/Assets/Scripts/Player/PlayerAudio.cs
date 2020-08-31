using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] stepclips;

    [SerializeField]
    private AudioClip jumpclip;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void jumpNoise()
    {
        audioSource.PlayOneShot(jumpclip);
    }

    private void Step()
    {
        AudioClip clip = GetRandomClip();
        audioSource.PlayOneShot(clip);
    }

    private AudioClip GetRandomClip()
    {
        return stepclips[UnityEngine.Random.Range(0, stepclips.Length)];
    }


}
