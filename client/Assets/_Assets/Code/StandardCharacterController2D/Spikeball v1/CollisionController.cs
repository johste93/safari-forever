using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StandardCharacterController2D;

namespace StandardCharacterController2D.Spikeball.V1
{
    public class CollisionController : ICollisionController
    {
        private CharacterController2D controller2D;
        private RaycastInfo raycastInfo;
        private BoxCollider2D boxCollider2D;
        public LayerMask whatIsPlatform = new LayerMask();

        private CollisionInfo collisionInfo;
        private CollisionInfo previousCollisionInfo;

        public delegate void CollisionEvent();
        public CollisionEvent On_CollisionEnterBelow;
        public CollisionEvent On_CollisionEnterAbove;
        public CollisionEvent On_CollisionEnterLeft;
        public CollisionEvent On_CollisionEnterRight;

        public CollisionEvent On_CollisionStayBelow;
        public CollisionEvent On_CollisionStayAbove;
        public CollisionEvent On_CollisionStayLeft;
        public CollisionEvent On_CollisionStayRight;

        public CollisionEvent On_CollisionExitBelow;
        public CollisionEvent On_CollisionExitAbove;
        public CollisionEvent On_CollisionExitLeft;
        public CollisionEvent On_CollisionExitRight;

        public CollisionController(CharacterController2D characterController2D)
		{
			this.controller2D = characterController2D;
            boxCollider2D = this.controller2D.GetComponent<BoxCollider2D>();
            whatIsPlatform = LayerMask.GetMask("Default");
            whatIsPlatform = whatIsPlatform.CombineLayerMask(LayerMask.GetMask("Cannon"));
			whatIsPlatform = whatIsPlatform.CombineLayerMask(LayerMask.GetMask("OneWayPlatform"));

        }

        public CollisionInfo GetCollisionInfo()
        {
            return collisionInfo;
        }

        public void UpdateCollisions()
		{
            //Prepare Raycasting
			raycastInfo.UpdateRaycastOrigins(controller2D.transform, boxCollider2D);
			raycastInfo.CalculateRaySpacing(controller2D.transform, boxCollider2D);

            Vector2 deltaMovement = controller2D.MotionController.GetCombinedVelocity() * Time.fixedDeltaTime * SaveManager.currentSave.gameSpeed;
            //Debug.Log(deltaMovement);

            CollisionInfo velocityCollisionInfo = new CollisionInfo();

            if(HorizontalCollisionHandling(ref deltaMovement, ref velocityCollisionInfo))
            {
                controller2D.MotionController.SetInternalVelocityX(0f);
            }

            if(VerticalCollisionHandling(ref deltaMovement, ref velocityCollisionInfo))
            {
                controller2D.MotionController.SetInternalVelocityY(0f);
            }

            collisionInfo = velocityCollisionInfo;
			CollisionInfo maintainedCollisions = MaintainCollisions();
			collisionInfo = CollisionInfo.Combine(collisionInfo, maintainedCollisions);

			controller2D.MotionController.SetClampedVelocity( deltaMovement / Time.fixedDeltaTime );

			HandleCollisionEvents();
        }

        private bool HorizontalCollisionHandling(ref Vector2 deltaMovement, ref CollisionInfo collisionInfo)
		{		
			float rayDir = Mathf.Sign(deltaMovement.x);			
			float rayLenght = Mathf.Abs(deltaMovement.x) + RaycastInfo.skinWidth;

            if(Mathf.Approximately(Mathf.Abs(deltaMovement.x), Mathf.Epsilon))
			{
				//When we are not moving we are raycasting in the (previous) moving direction.
				rayDir = ((StandardCharacterController2D.Spikeball.V1.MotionController) controller2D.MotionController).MovingDirection;
			}

			for( int i = 0; i < raycastInfo.horizontalRayCount; i++)
			{
				Vector2 rayOrigin = (rayDir == -1) ? raycastInfo.bottomLeft : raycastInfo.bottomRight;
				rayOrigin += (Vector2)controller2D.transform.up * (raycastInfo.horizontalRaySpacing * i);

				RaycastHit2D hit = CustomRaycast(rayOrigin, (Vector2)controller2D.transform.right * rayDir, rayLenght, whatIsPlatform, deltaMovement, rayDir);
				if(hit)
				{
					/*
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
					*/

					Debug.DrawRay(rayOrigin, (Vector2)controller2D.transform.right * rayDir * rayLenght, Color.blue);

					deltaMovement.x = MathIsHard.Round((hit.distance - RaycastInfo.skinWidth) * rayDir, 6);

					collisionInfo.left = rayDir == -1;
					collisionInfo.right = rayDir == 1;

					return true;
				}
				else
					Debug.DrawRay(rayOrigin, (Vector2)controller2D.transform.right * rayDir * rayLenght, Color.yellow);
			}

            return false;
		}

        private bool VerticalCollisionHandling(ref Vector2 deltaMovement, ref CollisionInfo collisionInfo)
		{
			float rayDir = Mathf.Sign(deltaMovement.y);
			float rayLenght = Mathf.Abs(deltaMovement.y) + RaycastInfo.skinWidth;

			for( int i = 0; i < raycastInfo.verticalRayCount; i++)
			{
				Vector2 rayOrigin = (rayDir == -1) ? raycastInfo.bottomLeft : raycastInfo.topLeft;
				rayOrigin += (Vector2)controller2D.transform.right * (raycastInfo.verticalRaySpacing * i + deltaMovement.x);

				RaycastHit2D hit = CustomRaycast(rayOrigin, (Vector2)controller2D.transform.up * rayDir, rayLenght, whatIsPlatform, deltaMovement, rayDir);
				if(hit)
				{
					/*
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
					*/

					deltaMovement.y = MathIsHard.Round((hit.distance - RaycastInfo.skinWidth) * rayDir, 6);

					collisionInfo.below = rayDir == -1;
					collisionInfo.above = rayDir == 1;

					return true;
				}
				else
					Debug.DrawRay(rayOrigin, (Vector2)controller2D.transform.up * rayDir * rayLenght, Color.yellow);
			}

			return false;
		}
    
        private CollisionInfo MaintainCollisions()
		{
			CollisionInfo collisionInfo = new CollisionInfo();

			if(previousCollisionInfo.right)
			{
				Vector2 vector = (Vector2)controller2D.transform.right * RaycastInfo.skinWidth;
				HorizontalCollisionHandling(ref vector, ref collisionInfo);
			}

			if(previousCollisionInfo.left)
			{
				Vector2 vector = -(Vector2)controller2D.transform.right * RaycastInfo.skinWidth;
				HorizontalCollisionHandling(ref vector, ref collisionInfo);
			}

			if(previousCollisionInfo.above)
			{
				Vector2 vector = (Vector2)controller2D.transform.up * RaycastInfo.skinWidth;
				VerticalCollisionHandling(ref vector, ref collisionInfo);
			}

			if(previousCollisionInfo.below)
			{
				Vector2 vector = -(Vector2)controller2D.transform.up * RaycastInfo.skinWidth;
				VerticalCollisionHandling(ref vector, ref collisionInfo);
			}
			
			return collisionInfo;
		}

		private RaycastHit2D[] raycastHit2Ds;
		private int hits = 0;
		private RaycastHit2D CustomRaycast(Vector2 rayOrigin, Vector2 direction, float distance, int layermask, Vector2 deltaMovement, float rayDir)
		{
			Direction4 direction4 =	direction.ToDirection();
			
			raycastHit2Ds = new RaycastHit2D[3];
			hits = Physics2D.RaycastNonAlloc(rayOrigin, direction, raycastHit2Ds, distance, layermask);
			Debug.DrawRay(rayOrigin, direction * distance, Color.grey);

			//Filter
			for(int i = 0; i < hits; i++)
			{
				if(direction4 == Direction4.Up || direction4 == Direction4.Down)
				{					
					if(raycastHit2Ds[i].collider.tag == "OneWayGateNorth" || raycastHit2Ds[i].collider.tag == "OneWayPlatform")
					{
						if(rayDir == 1 || raycastHit2Ds[i].distance == 0)
						{
							continue;	
						}
					}

					if(raycastHit2Ds[i].collider.tag == "OneWayGateSouth")
					{
						if(rayDir == -1 || raycastHit2Ds[i].distance == 0)
						{
							continue;	
						}
					}
				}

				if(direction4 == Direction4.Left || direction4 == Direction4.Right)
				{
					if(raycastHit2Ds[i].collider.tag == "OneWayGateEast")
					{
						if(rayDir == 1 || raycastHit2Ds[i].distance == 0)
						{
							continue;	
						}
					}

					if(raycastHit2Ds[i].collider.tag == "OneWayGateWest")
					{
						if(rayDir == -1 || raycastHit2Ds[i].distance == 0)
						{
							continue;	
						}
					}
					
					if(raycastHit2Ds[i].distance == 0) //Solves bug where platform moving through passenger would cause jittering in the x axis.
					{
						continue;
					}
				}

				return raycastHit2Ds[i];
			}

			//Nothing was hit.
			return new RaycastHit2D();
		}

        private void HandleCollisionEvents()
		{
			if(collisionInfo.below && !previousCollisionInfo.below)
				On_CollisionEnterBelow?.Invoke();

            if(collisionInfo.above && !previousCollisionInfo.above)
				On_CollisionEnterAbove?.Invoke();

            if(collisionInfo.left && !previousCollisionInfo.left)
				On_CollisionEnterLeft?.Invoke();

            if(collisionInfo.right && !previousCollisionInfo.right)
				On_CollisionEnterRight?.Invoke();

            

            if(!collisionInfo.below && previousCollisionInfo.below)
                On_CollisionExitBelow?.Invoke();

            if(!collisionInfo.above && previousCollisionInfo.above)
                On_CollisionExitAbove?.Invoke();

            if(!collisionInfo.left && previousCollisionInfo.left)
                On_CollisionExitLeft?.Invoke();

            if(!collisionInfo.right && previousCollisionInfo.right)
                On_CollisionExitRight?.Invoke();



            if(collisionInfo.below && previousCollisionInfo.below)
                On_CollisionStayBelow?.Invoke();

            if(collisionInfo.above && previousCollisionInfo.above)
                On_CollisionStayAbove?.Invoke();

            if(collisionInfo.left && previousCollisionInfo.left)
                On_CollisionStayLeft?.Invoke();

            if(collisionInfo.right && previousCollisionInfo.right)
                On_CollisionStayRight?.Invoke();
		}
    }
}
