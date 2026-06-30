using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class StuckInSlimeOnRoof : IState
    {   
        public delegate void JumpEvent(FSM_CharacterController controller);
        public static JumpEvent OnPlayerJumped;

        public State stateEnum{
            get{
                return State.StuckInSlimeOnRoof;
            }
        }
        public FSM_CharacterController characterController{get; set;}
        public void  SetReferenceToCharacter(FSM_CharacterController characterController)
        {
            this.characterController = characterController;
        }

        public void OnEnter(State previousState)
        {      
            characterController.inputController.ConsumeInput();
            characterController.motion.runningVelocity = 0f;
            characterController.motion.timeSpentFluttering = 0f;
            characterController.motion.timeOfLastJumpAttempt = 0f;
            characterController.motion.rawVelocity = Vector2.zero;
            characterController.motion.conveyorSpeed = Vector2.zero;

            Vector2 wedge = Vector2.up * 0.25f;
            characterController.character.Wedge(wedge, characterController.motion.runningDirection * -5);
 
            BounceSlime(wedge);
        }

        private void BounceSlime(Vector2 wedge)
        {
            Vector2 halfsize =  (characterController.boxCollider2D.size/2f) + new Vector2(RaycastInfo.skinWidth, RaycastInfo.skinWidth) * 2f;
            Vector2 bottomCorner = ((Vector2)characterController.centerPivot.position + wedge) - halfsize;
            Vector3 topCorner  = ((Vector2)characterController.centerPivot.position + wedge) + halfsize;
            Collider2D[] colliders = Physics2D.OverlapAreaAll(bottomCorner, topCorner, characterController.properties.whatIsPlatform);

            foreach(Collider2D collider in colliders)
            {
                if(collider.CompareTag("Slime"))
                {
                    collider.GetComponent<Slime>().Bounce(characterController.centerPivot);
                }
            }
        }

        public void OnExit(State newState)
        {
            characterController.inputController.ConsumeInput();
            characterController.character.Wedge(Vector2.zero, 0);
        }
        
        public void Act()
        {
            
        }


        public bool CheckTransitions()
        {
            return Transition_StuckInSlimeOnRoofToFalling.ConditionsMet(characterController);
        }
        
        public bool Equals(State state)
        {
            return state == stateEnum;
        }
    }
}