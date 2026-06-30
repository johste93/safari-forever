using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrefabId
{
    public string id{
        get{
            return prefab.GetComponent<LevelEntity>().id;
        }
    }
    public GameObject prefab;

    public PrefabId(GameObject prefab)
    {
        this.prefab = prefab;
    }
}
