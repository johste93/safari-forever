using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
	public class CollisionController
	{   
		private FSM_CharacterController characterController;

		private RaycastInfo raycastInfo;

		private CollisionInfo previousCollisionInfo;

		public delegate void CollisionEvent();
		public CollisionEvent On_BecameGrounded;

		public void SetReferenceToCharacter(FSM_CharacterController characterController)
		{
			this.characterController = characterController;
		}

		public void UpdateCollisions()
		{
			previousCollisionInfo = characterController.collisionInfo;
			characterController.collisionInfo.Reset();

			CollisionInfo velocityCollisionInfo = new CollisionInfo();

			characterController.motion.touchingSlime = false;
			
			//Prepare Raycasting
			raycastInfo.UpdateRaycastOrigins(characterController.transform, characterController.boxCollider2D);
			raycastInfo.CalculateRaySpacing(characterController.transform, characterController.boxCollider2D);

			Vector2 deltaMovement = characterController.motion.combinedVelocity * Time.fixedDeltaTime * SaveManager.currentSave.gameSpeed;
			Vector2 unAlteredDeltaMovement = deltaMovement;

			CheckIfTouchingSlime(deltaMovement);

			switch(HorizontalCollisionHandling(ref deltaMovement, ref velocityCollisionInfo))
			{
				case CollisionOutcome.Collided:
					//characterController.motion.rawVelocity.x = 0f;

					if(unAlteredDeltaMovement.x > 0)
						characterController.motion.rawVelocity.x = Mathf.Min(0f, characterController.motion.rawVelocity.x);
					else
						characterController.motion.rawVelocity.x = Mathf.Max(0f, characterController.motion.rawVelocity.x);		

				break;
			}
			
			switch(VerticalCollisionHandling(ref deltaMovement, ref velocityCollisionInfo))
			{
				case CollisionOutcome.Collided:

					if(unAlteredDeltaMovement.y > 0)
						characterController.motion.rawVelocity.y = Mathf.Min(0f, characterController.motion.rawVelocity.y);
					else
						characterController.motion.rawVelocity.y = Mathf.Max(0f, characterController.motion.rawVelocity.y);		

				break;
				case CollisionOutcome.RedoCollisionHandling:
					UpdateCollisions();
					return;
			}

			characterController.motion.distanceToGround = GetDistanceToGround(deltaMovement);

			//Manage collision info
			characterController.collisionInfo = velocityCollisionInfo;
			CollisionInfo maintainedCollisions = MaintainCollisions();
			characterController.collisionInfo = CollisionInfo.Combine(characterController.collisionInfo, maintainedCollisions);

			characterController.motion.clampedVelocity = deltaMovement / Time.fixedDeltaTime;

			HandleOnBecameGroundedEvent();
		}

		private void CheckIfTouchingSlime(Vector2 deltaMovement)
		{
			//Horizontal
			float horizontalRayLenght = Mathf.Abs(deltaMovement.x) + RaycastInfo.skinWidth;

			if(Mathf.Approximately(deltaMovement.x, 0f))
				horizontalRayLenght += RaycastInfo.skinWidth;

			float horizontalRayDir = Mathf.Sign(deltaMovement.x);
			for( int i = 0; i < raycastInfo.horizontalRayCount; i++)
			{
				Vector2 rayOrigin = (horizontalRayDir == -1) ? raycastInfo.bottomLeft : raycastInfo.bottomRight;
				rayOrigin += (Vector2)characterController.transform.up * (raycastInfo.horizontalRaySpacing * i);

				RaycastHit2D hit = CustomRaycast(rayOrigin, (Vector2)characterController.transform.right * horizontalRayDir, horizontalRayLenght, characterController.properties.whatIsPlatform, deltaMovement, horizontalRayDir);
				if(hit)
				{
					Debug.DrawRay(rayOrigin, (Vector2)characterController.transform.right * horizontalRayDir * horizontalRayLenght, Color.cyan);
					if(hit.collider.CompareTag("Slime"))
					{
						characterController.motion.touchingSlime = true;
					}
				}
				else
					Debug.DrawRay(rayOrigin, (Vector2)characterController.transform.right * horizontalRayDir * horizontalRayLenght, Color.blue);
			}	

			//Vertical
			float verticalRayDir = -1;
			if(Mathf.Abs(deltaMovement.y) > 0)
				verticalRayDir = Mathf.Sign(deltaMovement.y);

			float verticalRayLenght = Mathf.Abs(deltaMovement.y) + RaycastInfo.skinWidth;

			for( int i = 0; i < raycastInfo.verticalRayCount; i++)
			{
				Vector2 rayOrigin = (verticalRayDir == -1) ? raycastInfo.bottomLeft : raycastInfo.topLeft;
				rayOrigin += (Vector2)characterController.transform.right * (raycastInfo.verticalRaySpacing * i + deltaMovement.x);

				RaycastHit2D hit = CustomRaycast(rayOrigin, (Vector2)characterController.transform.up * verticalRayDir, verticalRayLenght, characterController.properties.whatIsPlatform, deltaMovement, verticalRayDir);
				if(hit)
				{
					if(hit.collider.CompareTag("Slime"))
					{
						Debug.DrawRay(rayOrigin, (Vector2)characterController.transform.up * verticalRayDir * verticalRayLenght, Color.cyan);
						characterController.motion.touchingSlime = true;
					}
				}
				else
					Debug.DrawRay(rayOrigin, (Vector2)characterController.transform.up * verticalRayDir * verticalRayLenght, Color.blue);
			}
		}

		private CollisionInfo MaintainCollisions()
		{
			CollisionInfo collisionInfo = new CollisionInfo();

			if(previousCollisionInfo.right)
			{
				Vector2 vector = (Vector2)characterController.transform.right * RaycastInfo.skinWidth;
				HorizontalCollisionHandling(ref vector, ref collisionInfo);
			}

			if(previousCollisionInfo.left)
			{
				Vector2 vector = -(Vector2)characterController.transform.right * RaycastInfo.skinWidth;
				HorizontalCollisionHandling(ref vector, ref collisionInfo);
			}

			if(previousCollisionInfo.above)
			{
				Vector2 vector = (Vector2)characterController.transform.up * RaycastInfo.skinWidth;
				VerticalCollisionHandling(ref vector, ref collisionInfo);
			}

			if(previousCollisionInfo.below)
			{
				Vector2 vector = -(Vector2)characterController.transform.up * RaycastInfo.skinWidth;
				VerticalCollisionHandling(ref vector, ref collisionInfo);
			}

			
			return collisionInfo;
		}

		private CollisionOutcome HorizontalCollisionHandling(ref Vector2 deltaMovement, ref CollisionInfo collisionInfo)
		{		
			float rayDir = Mathf.Sign(deltaMovement.x);
			if(Mathf.Approximately(Mathf.Abs(deltaMovement.x), Mathf.Epsilon))
			{
				//If we are not moving in any direction. Raycast in both.
				CollisionResult leftResult = HorizontalCollisionDetection(-1, deltaMovement);
				CollisionResult rightResult = HorizontalCollisionDetection(1, deltaMovement);

				deltaMovement.x = 0f;

				collisionInfo.left = leftResult.collisionOutcome == CollisionOutcome.Collided;
				collisionInfo.right = rightResult.collisionOutcome == CollisionOutcome.Collided;

				return leftResult.collisionOutcome == CollisionOutcome.Collided || rightResult.collisionOutcome == CollisionOutcome.Collided ? CollisionOutcome.Collided : CollisionOutcome.NoCollision;
			}
			else
			{
				CollisionResult result = HorizontalCollisionDetection(rayDir, deltaMovement);
				
				if(result.collisionOutcome == CollisionOutcome.Collided)
					deltaMovement.x = result.distanceToCollision;

				if(rayDir < 0 )
					collisionInfo.left = result.collisionOutcome == CollisionOutcome.Collided;
				else
					collisionInfo.right = result.collisionOutcome == CollisionOutcome.Collided;

				return result.collisionOutcome;
			}
		}

		private CollisionResult HorizontalCollisionDetection(float rayDir, Vector2 deltaMovement)
		{
			float rayLenght = Mathf.Abs(deltaMovement.x) + RaycastInfo.skinWidth;

			if(Mathf.Approximately(deltaMovement.x, 0f))
				rayLenght += RaycastInfo.skinWidth;

			for( int i = 0; i < raycastInfo.horizontalRayCount; i++)
			{
				Vector2 rayOrigin = (rayDir == -1) ? raycastInfo.bottomLeft : raycastInfo.bottomRight;
				rayOrigin += (Vector2)characterController.transform.up * (raycastInfo.horizontalRaySpacing * i);

				RaycastHit2D hit = CustomRaycast(rayOrigin, (Vector2)characterController.transform.right * rayDir, rayLenght, characterController.properties.whatIsPlatform, deltaMovement, rayDir);
				if(hit)
				{
					Debug.DrawRay(rayOrigin, (Vector2)characterController.transform.right * rayDir * rayLenght, Color.red);

					ICollidableTrigger collidableTrigger = hit.collider.gameObject.GetComponent<ICollidableTrigger>();
					if(collidableTrigger != null)
					{
						collidableTrigger.OnCollisionTrigger(characterController, rayDir < 0 ? Direction4.Left : Direction4.Right);
					}

					float distanceToCollision = MathIsHard.Round((hit.distance - RaycastInfo.skinWidth) * rayDir, 6);

					return new CollisionResult(CollisionOutcome.Collided, distanceToCollision, hit);
				}
				else
					Debug.DrawRay(rayOrigin, (Vector2)characterController.transform.right * rayDir * rayLenght, Color.yellow);
			}	

			return new CollisionResult(CollisionOutcome.NoCollision, 0f, null);
		}

		private CollisionOutcome VerticalCollisionHandling(ref Vector2 deltaMovement, ref CollisionInfo collisionInfo)
		{
			float rayDir = -1;
			if(Mathf.Abs(deltaMovement.y) > 0)
				rayDir = Mathf.Sign(deltaMovement.y);

			float rayLenght = Mathf.Abs(deltaMovement.y) + RaycastInfo.skinWidth;

			for( int i = 0; i < raycastInfo.verticalRayCount; i++)
			{
				Vector2 rayOrigin = (rayDir == -1) ? raycastInfo.bottomLeft : raycastInfo.topLeft;
				rayOrigin += (Vector2)characterController.transform.right * (raycastInfo.verticalRaySpacing * i + deltaMovement.x);

				RaycastHit2D hit = CustomRaycast(rayOrigin, (Vector2)characterController.transform.up * rayDir, rayLenght, characterController.properties.whatIsPlatform, deltaMovement, rayDir);
				if(hit)
				{
					deltaMovement.y = MathIsHard.Round((hit.distance - RaycastInfo.skinWidth) * rayDir, 6);

					collisionInfo.below = rayDir == -1;
					collisionInfo.above = rayDir == 1;

					bool newBelowCollision = collisionInfo.below && !previousCollisionInfo.below;
					if(newBelowCollision)
					{	
						IStompable stompable = hit.collider.gameObject.GetComponent<IStompable>();
						if(stompable != null)
						{
							stompable.OnStomped(characterController.character);
							return CollisionOutcome.RedoCollisionHandling;
						}
					}

					bool newAboveCollision = collisionInfo.above && !previousCollisionInfo.above;
					if(newAboveCollision)
					{
						IHeadSmashable headSmashable = hit.collider.gameObject.GetComponent<IHeadSmashable>();
						if(headSmashable != null)
						{
							if(characterController.stateController.currentState.Equals(State.Jumping) || characterController.stateController.currentState.Equals(State.JumpingOnJumpPad) || characterController.stateController.currentState.Equals(State.WallSliding))
							{
								headSmashable.OnHeadSmashed(characterController.character);
								return CollisionOutcome.RedoCollisionHandling;
							}
						}
					}

					//If CollidableEvent
					if(newBelowCollision || newAboveCollision)
					{
						ICollidableTrigger collidableTrigger = hit.collider.gameObject.GetComponent<ICollidableTrigger>();
						if(collidableTrigger != null)
						{
							collidableTrigger.OnCollisionTrigger(characterController, newBelowCollision ? Direction4.Down : Direction4.Up);
						}
					}

					return CollisionOutcome.Collided;
				}
				//else
					//Debug.DrawRay(rayOrigin, (Vector2)characterController.transform.up * rayDir * rayLenght, Color.yellow);
			}

			return CollisionOutcome.NoCollision;
		}

		private void HandleOnBecameGroundedEvent()
		{
			if(characterController.collisionInfo.below && !previousCollisionInfo.below)
			{
				if(On_BecameGrounded != null)
					On_BecameGrounded();
			}
		}

		private float GetDistanceToGround(Vector2 deltaMovement)
		{
			float rayDir = -1;
			float rayLenght = Mathf.Infinity;

			float shortestDistance = rayLenght;

			for( int i = 0; i < raycastInfo.verticalRayCount; i++)
			{
				Vector2 rayOrigin = (rayDir == -1) ? raycastInfo.bottomLeft : raycastInfo.topLeft;
				rayOrigin += (Vector2)characterController.transform.right * (raycastInfo.verticalRaySpacing * i + deltaMovement.x);

				RaycastHit2D hit = CustomRaycast(rayOrigin, (Vector2)characterController.transform.up * rayDir, rayLenght, characterController.properties.whatIsPlatform, deltaMovement, rayDir);
				if(hit)
				{
					if(hit.distance < shortestDistance)
						shortestDistance = hit.distance;
				}
				else
					Debug.DrawRay(rayOrigin, (Vector2)characterController.transform.up * rayDir * rayLenght, Color.grey);
			}

			return shortestDistance;
		}

		private struct CollisionResult
		{
			public CollisionResult(CollisionOutcome outcome, float distanceToCollision, RaycastHit2D? hit)
			{
				this.collisionOutcome = outcome;
				this.distanceToCollision = distanceToCollision;
				this.hit = hit;
			}

			public CollisionOutcome collisionOutcome;
			public float distanceToCollision;
			public RaycastHit2D? hit;
		}

		private RaycastHit2D CustomRaycast(Vector2 rayOrigin, Vector2 direction, float distance, int layermask, Vector2 deltaMovement, float rayDir)
		{
			Direction4 direction4 =	direction.ToDirection();
			
			RaycastHit2D[] raycastHits = Physics2D.RaycastAll(rayOrigin, direction, distance, layermask);
			Debug.DrawRay(rayOrigin, direction * distance, Color.grey);

			//Filter
			foreach(RaycastHit2D hit in raycastHits)
			{
				if(direction4 == Direction4.Up || direction4 == Direction4.Down)
				{					
					if(hit.collider.tag == "OneWayGateNorth" || hit.collider.tag == "OneWayPlatform")
					{
						if(rayDir == 1 || hit.distance == 0)
						{
							continue;	
						}
					}

					if(hit.collider.tag == "OneWayGateSouth")
					{
						if(rayDir == -1 || hit.distance == 0)
						{
							continue;	
						}
					}
				}

				if(direction4 == Direction4.Left || direction4 == Direction4.Right)
				{
					if(hit.collider.tag == "OneWayGateEast")
					{
						if(rayDir == 1 || hit.distance == 0)
						{
							continue;	
						}
					}

					if(hit.collider.tag == "OneWayGateWest")
					{
						if(rayDir == -1 || hit.distance == 0)
						{
							continue;	
						}
					}
					
					if(hit.distance == 0) //Solves bug where platform moving through passenger would cause jittering in the x axis.
					{
						continue;
					}
				}

				return hit;
			}

			//Nothing was hit.
			return new RaycastHit2D();

			//return Physics2D.Raycast(rayOrigin, direction, distance, layermask);
			/*
			
			//int hitCount = Physics2D.RaycastNonAlloc(rayOrigin, direction, raycastHits, distance, layermask);

			
			
			//Filter
			foreach(RaycastHit2D hit in raycastHits)
			{
				Slope slope = hit.collider.GetComponent<Slope>();
				if(slope != null)
				{
					if(slope.realSlope)
						if(IgnoreCollider(slope, deltaMovement))
							continue;
				}

				return hit;
			}

			//Nothing was hit.
			return new RaycastHit2D();
			*/
		}

		/*
		private bool IgnoreCollider(Slope slope)
		{
			if(slope.slopeDirection > 0)
			{
				//Pointing Left
				return raycastInfo.bottomLeft.y > slope.bottomLeft.y + RaycastInfo.skinWidth && raycastInfo.topRight.x < slope.topRight.x - RaycastInfo.skinWidth;
			}
			else
			{
				//Pointing Right
				return raycastInfo.bottomLeft.y > slope.bottomLeft.y + RaycastInfo.skinWidth && raycastInfo.bottomLeft.x > slope.bottomLeft.x + RaycastInfo.skinWidth;
			}
		}
		*/

		/*
		private bool IgnoreCollider(Slope slope, Vector2 deltaMovement)
		{
			if(slope.slopeDirection > 0)
			{
				//Pointing Left
				return raycastInfo.bottomLeft.y + Mathf.Max(0f, deltaMovement.y) > slope.bottomLeft.y && raycastInfo.topRight.x + deltaMovement.x < slope.topRight.x;
			}
			else
			{
				//Pointing Right
				return raycastInfo.bottomLeft.y + Mathf.Max(0f, deltaMovement.y) > slope.bottomLeft.y && raycastInfo.bottomLeft.x + deltaMovement.x > slope.bottomLeft.x;
			}
		}
		*/
	}
}