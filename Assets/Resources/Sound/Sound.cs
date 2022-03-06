using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip sound;

    public void SoundClick()
    {
        audioSource.PlayOneShot(sound);
    }
}
