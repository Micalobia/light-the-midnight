using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAudio : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] noiseclips;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void Noise()
    {
        AudioClip clip = GetRandomClip();
        audioSource.PlayOneShot(clip);
    }

    private AudioClip GetRandomClip()
    {
        return noiseclips[UnityEngine.Random.Range(0, noiseclips.Length)];
    }


}
