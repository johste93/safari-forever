using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Track : ScriptableObject
{
    public Music key;
    public AudioClip normal;
    public AudioClip soft;
}
