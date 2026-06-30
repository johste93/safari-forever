using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSlime : MonoBehaviour
{
    public GameObject top;
    public GameObject bottom;
    public GameObject left;
    public GameObject right;

    public void Enable(Direction4 direction)
    {
        top.SetActive(direction == Direction4.Up);
        bottom.SetActive(direction == Direction4.Down);
        left.SetActive(direction == Direction4.Left);
        right.SetActive(direction == Direction4.Right);
    }

    public void Disable()
    {
        top.SetActive(false);
        bottom.SetActive(false);
        left.SetActive(false);
        right.SetActive(false);
    }
}
