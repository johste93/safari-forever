using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class Running : IState
    {   
        public State stateEnum{
            get{
                return State.Running;
            }
        }
        public FSM_CharacterController characterController{get; set;}
        public void  SetReferenceToCharacter(FSM_CharacterController characterController)
        {
            this.characterController = characterController;
        }

        public void OnEnter(State previousState)
        {     
            characterController.audio.jumpPitch = 1f;
            characterController.audio.clipIndex = 0;

            characterController.motion.timeSpentFluttering = 0f;
            characterController.motion.runningVelocity = characterController.properties.movementSpeed * characterController.motion.runningDirection;
        }

        public void OnExit(State newState)
        {
        }

        public void Act()
        {
            if( characterController.motion.runningDirection == 1 ? characterController.collisionInfo.right : characterController.collisionInfo.left)
            {
                characterController.motion.runningDirection *= -1;
                characterController.motion.runningVelocity = characterController.properties.movementSpeed * characterController.motion.runningDirection;
                //Audio.Play(SFX.instance.character.turn);
            }
        }

        private void GlueToGround()
        {
            LayerMask layerMask = characterController.properties.whatIsPlatform;
            layerMask.CombineLayerMask(characterController.properties.whatIsSlope);
            RaycastHit2D hit = Physics2D.Raycast(characterController.rigidBody2D.position, Vector2.down, 1f, layerMask);

            if(hit.collider != null)
            {
                characterController.rigidBody2D.position += new Vector2(0, -hit.distance);
            }
        }

        public bool CheckTransitions()
        {
            return 
                Transition_RunningToJumping.ConditionsMet(characterController)
                || Transition_RunningToFalling.ConditionsMet(characterController);
        }
        
        public bool Equals(State state)
        {
            return state == stateEnum;
        }
    }
}