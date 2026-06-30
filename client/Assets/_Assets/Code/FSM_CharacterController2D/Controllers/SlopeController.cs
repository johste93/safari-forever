using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class SlopeController
    {
        public FSM_CharacterController characterController;

        public void SetReferenceToCharacter(FSM_CharacterController characterController)
        {
            this.characterController = characterController;
        }

        public void HandleSlopes()
        {
            Vector2 deltaMovement = characterController.motion.combinedVelocity * Time.fixedDeltaTime * SaveManager.currentSave.gameSpeed;
            Vector2 point = (Vector2)characterController.rigidBody2D.position + characterController.boxCollider2D.offset + deltaMovement;
            Collider2D slopeCollider = Physics2D.OverlapBox(point, characterController.boxCollider2D.size, 0, characterController.properties.whatIsSlope);
            bool insideSlope = slopeCollider != null;
            characterController.motion.standingOnSlope = insideSlope;

            if(characterController.motion.standingOnSlope)
            {
                //characterController.collisionInfo.below = true; //probably has no effect on outcome as we reset it before doing calculations.

                characterController.motion.slopeDirection = slopeCollider.GetComponentInParent<Slope>().slopeDirection;
                float adjustment = 0f;
                bool hitRoof = false;
                do{
                    adjustment += 0.01f;
                    
                    deltaMovement = characterController.motion.combinedVelocity * Time.fixedDeltaTime * SaveManager.currentSave.gameSpeed + new Vector2(0, adjustment);
                    point = (Vector2)characterController.rigidBody2D.position + characterController.boxCollider2D.offset + deltaMovement;
                    insideSlope = Physics2D.OverlapBox(point, characterController.boxCollider2D.size, 0, characterController.properties.whatIsSlope) != null;
                    CollisionInfo collisionInfo = new CollisionInfo();
                    hitRoof = VerticalCollisionHandling(deltaMovement, ref collisionInfo) == CollisionOutcome.Collided;
                }
                while(insideSlope && !hitRoof);

                if(hitRoof)
                {
                    characterController.motion.runningDirection *= -1;
                    characterController.motion.runningVelocity = characterController.properties.movementSpeed * characterController.motion.runningDirection;
                }
                   

                characterController.rigidBody2D.position += new Vector2(0, adjustment);
                characterController.motion.rawVelocity.y = -8f;
            }
        }   

        private RaycastInfo raycastInfo;
        private CollisionOutcome VerticalCollisionHandling(Vector2 deltaMovement, ref CollisionInfo collisionInfo)
		{
            raycastInfo.UpdateRaycastOrigins(characterController.transform, characterController.boxCollider2D);
			raycastInfo.CalculateRaySpacing(characterController.transform, characterController.boxCollider2D);

			float rayDir = Mathf.Sign(deltaMovement.y);
            if(rayDir == -1)
				return CollisionOutcome.NoCollision;

			float rayLenght = Mathf.Abs(deltaMovement.y) + RaycastInfo.skinWidth;

			for( int i = 0; i < raycastInfo.verticalRayCount; i++)
			{
				Vector2 rayOrigin = (rayDir == -1) ? raycastInfo.bottomLeft : raycastInfo.topLeft;
				rayOrigin += (Vector2)characterController.transform.right * (raycastInfo.verticalRaySpacing * i + deltaMovement.x);

				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, (Vector2)characterController.transform.up * rayDir, rayLenght, characterController.properties.whatIsPlatform);
				if(hit)
				{
					Debug.DrawRay(rayOrigin, (Vector2)characterController.transform.up * rayDir * rayLenght, Color.cyan);
					if(hit.collider.tag == "OneWayPlatform")
					{
						if(rayDir == 1 || hit.distance == 0)
						{
							continue;	
						}
					}	

					collisionInfo.below = rayDir == -1;
					collisionInfo.above = rayDir == 1;

					return CollisionOutcome.Collided;
				}
				else
					Debug.DrawRay(rayOrigin, (Vector2)characterController.transform.up * rayDir * rayLenght, Color.gray);
			}

			return CollisionOutcome.NoCollision;
		}

        
    }
}