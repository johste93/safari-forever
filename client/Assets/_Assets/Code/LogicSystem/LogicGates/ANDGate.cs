using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SF.LogicSystem.v2
{
    public class ANDGate : MonoBehaviour
    {
        public InputNode inputNode;
        public OutputNode outputNode;

        private void EvaluateConnections(int poweredLinksThisFrame)
        {
            outputNode.EmitPower(
                inputNode.links.Count > 0 
                && poweredLinksThisFrame == inputNode.links.Count
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
}
