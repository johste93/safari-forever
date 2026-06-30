using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LogicGate : MonoBehaviour
{
    public LevelEntity entity;
    public GameObject[] gates;
    public TextMeshPro[] textMeshes;

    private void Awake()
    {
        entity.gizmo.On_Tap += UpdateGate;
    }

    private void Start()
    {
        UpdateGate();
    }

    public void UpdateGate()
    {
		if(!entity.serializableData.gizmoIndex.HasValue)
			entity.serializableData.gizmoIndex = 0;
			
		//Debug.Log(entity.serializableData.gizmoIndex);
        for(int i = 0; i < gates.Length; i++)
            gates[i].gameObject.SetActive(entity.serializableData.gizmoIndex == i);

        foreach(TextMeshPro textMesh in textMeshes)
            textMesh.text = ((OptionGizmo)entity.gizmo).GetValue();
    }

    private void Unsubscribe()
    {
        entity.gizmo.On_Tap -= UpdateGate;
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }
}
