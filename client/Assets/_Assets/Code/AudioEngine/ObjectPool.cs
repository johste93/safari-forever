using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private GameObject prefab;
    public List<GameObject> objects = new List<GameObject>();

    public GameObject GetFirstInactive(GameObject prefab)
    {
        foreach(GameObject obj in objects)
        {
            if(obj == null)
                continue;

            if(obj.activeInHierarchy)
                continue;
            
            obj.SetActive(true);
            return obj;
        }

        GameObject newGameobject = MonoBehaviour.Instantiate(prefab);
        objects.Add(newGameobject);
        return newGameobject;
    }
}
