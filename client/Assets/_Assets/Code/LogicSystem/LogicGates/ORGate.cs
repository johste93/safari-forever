using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF.LogicSystem.v2;

public class ORGate : MonoBehaviour
{
    public InputNode inputNode;
    public OutputNode outputNode;

	private int lastFrameOfEmission;

	private bool HasEmittedThisFrame()
	{
		return lastFrameOfEmission == Time.frameCount;
	}

    private void EvaluateConnections(int poweredLinksThisFrame)
    {
		if(HasEmittedThisFrame())
			return;

		if(poweredLinksThisFrame > 0)
			lastFrameOfEmission = Time.frameCount;

        outputNode.EmitPower(poweredLinksThisFrame > 0);
    }

	private void On_StartedEvaluating(int poweredLinksSoFar)
	{
		if(HasEmittedThisFrame())
			return;

		if(poweredLinksSoFar > 0)
		{
			lastFrameOfEmission = Time.frameCount;
			outputNode.EmitPower(true);
		}       
	}
    
    private void OnEnable()
    {
		inputNode.On_StartedEvaluating += On_StartedEvaluating;
        inputNode.On_DoneEvaluating += EvaluateConnections;
    }

    private void Unsubscribe()
    {
		inputNode.On_StartedEvaluating -= On_StartedEvaluating;
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
