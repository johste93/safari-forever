using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF.LogicSystem.v2;

public class NOTGate : MonoBehaviour
{
    public InputNode inputNode;
    public OutputNode outputNode;

    private void EvaluateConnections(int poweredLinksThisFrame)
    {
        outputNode.EmitPower(
            inputNode.links.Count > 0
            && poweredLinksThisFrame == 0 
        );
    }
    
    private void OnEnable()
    {
        inputNode.On_DoneEvaluating += EvaluateConnections;
    }

    private void Unsubscribe()
    {
        inputNode.On_DoneEvaluating -= EvaluateConnections;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
}
