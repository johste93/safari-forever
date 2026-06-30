using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool<T>
{
    private T[] source;
    private List<T> pool;
    public Pool(T[] source)
    {
        this.source = source;

        if(source == null || source.Length == 0)
            Debug.LogError("Source array cannot be null or empty!");

        Refill();
    }

    public T Fish()
    {
        if(pool == null || pool.Count == 0)
            Refill();

        int randomIndex = Random.Range(0, pool.Count);
        T fish = pool[randomIndex];
        pool.RemoveAt(randomIndex);
        return fish;
    }

    public void Refill()
    {
        pool = new List<T>(source);
    }
}