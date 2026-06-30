using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StandardCharacterController2D.Spikeball.V1
{
    public class SlopeController : ISlopeController
    {
        public Spikeball_CharacterController2D controller2D;

        private LayerMask whatIsSlope;
        private LayerMask whatIsPlatform;

        private bool isStandingOnSlope;
        private int slopeDirection;

        private bool debug = false;

        public SlopeController(Spikeball_CharacterController2D controller2D)
        {
            this.controller2D = controller2D;
            whatIsSlope = LayerMask.GetMask("Slope");
            whatIsPlatform = LayerMask.GetMask("Default");
        }

        public void HandleSlopes()
        {
            Vector2 deltaMovement = controller2D.MotionController.GetCombinedVelocity() * Time.fixedDeltaTime * SaveManager.currentSave.gameSpeed;
            Vector2 point = (Vector2)controller2D.Rigidbody2D.position + controller2D.BoxCollider2D.offset + deltaMovement;
            Collider2D slopeCollider = Physics2D.OverlapBox(point, controller2D.BoxCollider2D.size, 0, whatIsSlope);
            bool insideSlope = slopeCollider != null;
            isStandingOnSlope = insideSlope;

            if(isStandingOnSlope)
            {
                //characterController.collisionInfo.below = true; //probably has no effect on outcome as we reset it before doing calculations.

                slopeDirection = slopeCollider.GetComponentInParent<Slope>().slopeDirection;
                float adjustment = 0f;
                bool hitRoof = false;
                do{
                    adjustment += 0.01f;
                    
                    deltaMovement = controller2D.MotionController.GetCombinedVelocity() * Time.fixedDeltaTime * SaveManager.currentSave.gameSpeed + new Vector2(0, adjustment);
                    point = (Vector2)controller2D.Rigidbody2D.position + controller2D.BoxCollider2D.offset + deltaMovement;
                    insideSlope = Physics2D.OverlapBox(point, controller2D.BoxCollider2D.size, 0, whatIsSlope) != null;
                    CollisionInfo collisionInfo = new CollisionInfo();
                    hitRoof = VerticalCollisionHandling(deltaMovement, ref collisionInfo);
                }
                while(insideSlope && !hitRoof);

                MotionController motionController = (MotionController)controller2D.MotionController;

                if(hitRoof)
                {    
                    motionController.MovingDirection *= -1;
                    motionController.SetInternalVelocityX(controller2D.MovementSpeed * motionController.MovingDirection);
                }
                   
                controller2D.Rigidbody2D.position += new Vector2(0, adjustment);
                motionController.SetInternalVelocityY(-8f);
            }
        }

        private RaycastInfo raycastInfo;
        private bool VerticalCollisionHandling(Vector2 deltaMovement, ref CollisionInfo collisionInfo)
		{
            raycastInfo.UpdateRaycastOrigins(controller2D.transform, controller2D.BoxCollider2D);
			raycastInfo.CalculateRaySpacing(controller2D.transform, controller2D.BoxCollider2D);

			float rayDir = Mathf.Sign(deltaMovement.y);
            if(rayDir == -1)
				return false;

			float rayLenght = Mathf.Abs(deltaMovement.y) + RaycastInfo.skinWidth;

			for( int i = 0; i < raycastInfo.verticalRayCount; i++)
			{
				Vector2 rayOrigin = (rayDir == -1) ? raycastInfo.bottomLeft : raycastInfo.topLeft;
				rayOrigin += (Vector2)controller2D.transform.right * (raycastInfo.verticalRaySpacing * i + deltaMovement.x);

				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, (Vector2)controller2D.transform.up * rayDir, rayLenght, whatIsPlatform);
				if(hit)
				{
                    if(debug) 
                        Debug.DrawRay(rayOrigin, (Vector2)controller2D.transform.up * rayDir * rayLenght, Color.cyan);

					if(hit.collider.tag == "OneWayPlatform")
					{
						if(rayDir == 1 || hit.distance == 0)
						{
							continue;	
						}
					}	

					collisionInfo.below = rayDir == -1;
					collisionInfo.above = rayDir == 1;

					return true;
				}
				else
					if(debug) 
                        Debug.DrawRay(rayOrigin, (Vector2)controller2D.transform.up * rayDir * rayLenght, Color.gray);
			}

			return false;
		}

        public bool IsStandingOnSlope()
        {
            return isStandingOnSlope;
        }
    }
}
