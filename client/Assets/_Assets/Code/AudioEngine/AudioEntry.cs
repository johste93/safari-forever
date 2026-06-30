using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioEntry
{
    [HideInInspector]public AudioClip randomClip
    {
        get{
            if(pool.Count == 0)
                Refill();

            int index = Random.Range(0, pool.Count);
            AudioClip randomClip = clips[index];
            pool.RemoveAt(index);
            return randomClip;
        }
    }
    private List<AudioClip> pool = new List<AudioClip>();

    public AudioClip[] clips;
    public float defaultVolume = 1f;
    public float defaultPitch = 1f;
    public bool loop = false;

    private void Refill()
    {
        if(clips.Length == 0)
        {
            Debug.LogError("No clips have been added to entry!");
        }
        pool = new List<AudioClip>(clips);
    }
}
