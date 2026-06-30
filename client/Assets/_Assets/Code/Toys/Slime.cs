using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    public BoxCollider2D boxCollider2D;
    public List<SlimeBubble> bubbles;

    public void Bounce(Transform target)
    {
        foreach(SlimeBubble bubble in bubbles)
        {
            bubble.Bounce(target);
        }
    }
}
