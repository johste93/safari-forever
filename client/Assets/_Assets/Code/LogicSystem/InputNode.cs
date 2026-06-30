using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SF.LogicSystem.v2
{
    public class InputNode : Node
    {
        private int lastFrameOfPower;
        private int frameEnteredPlayMode;

        private int linkUpdateCount;
        private int poweredLinksThisFrame;

        public delegate void EvaluationEvent(int poweredLinksThisFrame);
        public EvaluationEvent On_DoneEvaluating;
		public EvaluationEvent On_StartedEvaluating;

        public bool HasConnections()
        {
            return links.Count > 0;
        }

        public void Power(bool emit)
        {
            linkUpdateCount++;

            if(emit)
            {
                poweredLinksThisFrame++;
            }

			if(poweredLinksThisFrame > 0)
			{
				On_StartedEvaluating?.Invoke(poweredLinksThisFrame);
			}

            if(linkUpdateCount == links.Count)
            {
                EvaluateConnections();
            }
        }

        private void EvaluateConnections()
        {
            bool isPowered = poweredLinksThisFrame > 0;

            if(isPowered)
            {
                lastFrameOfPower = Time.frameCount;
            }

            On_DoneEvaluating?.Invoke(poweredLinksThisFrame);
        }

        private void LateUpdate()
        {
            linkUpdateCount = 0;
            poweredLinksThisFrame = 0;
        }

        public bool IsPowered()
        {
            return lastFrameOfPower == Time.frameCount;
        }

        private void On_LevelReset(bool manual)
        {
            lastFrameOfPower = 0;
        }

		protected override void OnEnable()
		{
			base.OnEnable();
            GameMaster.On_LevelReset += On_LevelReset;
		}

		protected override void Unsubscribe()
		{
			base.Unsubscribe();
            GameMaster.On_LevelReset -= On_LevelReset;
		}	

    }
}
