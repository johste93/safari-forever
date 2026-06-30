using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
	[System.Serializable]
	public struct CollisionInfo
	{
		public bool left;
		public bool right;
		public bool below;
		public bool above;

		public void Reset()
		{
			left = right = below = above = false;
		}

		public override string ToString()
		{
			return 
				"CollisionInfo.left: " + left + "\n" +
				"CollisionInfo.right: " + right + "\n" +
				"CollisionInfo.below: " + below + "\n" +
				"CollisionInfo.above: " + above + "\n";
		}

		
		public static CollisionInfo Combine(CollisionInfo a, CollisionInfo b)
		{
			CollisionInfo result = new CollisionInfo();
			result.above = a.above || b.above;
			result.below = a.below || b.below;
			result.left = a.left || b.left;
			result.right = a.right || b.right;
			return result;
		}
	}
}