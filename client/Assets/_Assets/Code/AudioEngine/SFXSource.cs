using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXSource : MonoBehaviour
{
    public AudioSource audioSource;

    public void Reset()
    {
        audioSource.clip = null;
        audioSource.volume = 1f;
        audioSource.pitch = 1f;
        audioSource.loop = false;
        audioSource.mute = false;
    }
}
