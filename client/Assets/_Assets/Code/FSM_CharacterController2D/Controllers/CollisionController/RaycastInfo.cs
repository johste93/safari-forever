using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
	public struct RaycastInfo
	{
		public const float skinWidth = 0.015f;
		public const float dstBetweenRays = 0.20f;

		public int horizontalRayCount;
		public int verticalRayCount;
		public float horizontalRaySpacing;
		public float verticalRaySpacing;

		public Vector3 topLeft, topRight;
		public Vector3 bottomLeft, bottomRight;

		public void UpdateRaycastOrigins(Transform transform, BoxCollider2D boxCollider2D)
		{
			Bounds bounds = boxCollider2D.bounds;
			Vector2 center = bounds.center;

			float sizeX = (boxCollider2D.size.x/2f) -skinWidth;
			float sizeY = (boxCollider2D.size.y/2f) -skinWidth;

			bottomLeft = center + (sizeX * -(Vector2)transform.right) + (sizeY * -(Vector2)transform.up);
			bottomRight = center + (sizeX * (Vector2)transform.right) + (sizeY * -(Vector2)transform.up);
			topLeft = center + (sizeX * -(Vector2)transform.right) + (sizeY * (Vector2)transform.up);
			topRight = center + (sizeX * (Vector2)transform.right) + (sizeY * (Vector2)transform.up);
		}

		public void CalculateRaySpacing(Transform transform, BoxCollider2D boxCollider2D) 
		{	
			Vector2 center = transform.TransformPoint(boxCollider2D.offset);

			float boundsWidth = Vector2.Distance(bottomLeft, bottomRight);
			float boundsHeight = Vector2.Distance(bottomLeft, topLeft);

			horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
			verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);

			horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
			verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

			horizontalRaySpacing = boundsHeight / (horizontalRayCount - 1);
			verticalRaySpacing = boundsWidth / (verticalRayCount - 1);
		}
	}
}
