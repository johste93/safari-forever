using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
	public class PhysicsHelper : MonoBehaviour {

		public static float CalculateSpeedFromHeight(float height, float gravity)
		{
			return Mathf.Sqrt(height * 2f * Mathf.Abs(gravity));
		}
	}
}
