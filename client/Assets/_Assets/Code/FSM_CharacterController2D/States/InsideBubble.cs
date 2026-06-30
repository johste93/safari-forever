using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class InsideBubble : IState
    {   
        public State stateEnum{
            get{
                return State.InsideBubble;
            }
        }
        public FSM_CharacterController characterController{get; set;}
        public void  SetReferenceToCharacter(FSM_CharacterController characterController)
        {
            this.characterController = characterController;
        }

        public void OnEnter(State previousState)
        {      
            Audio.Play(SFX.instance.level.bubble.enter, Channel.Game);

            characterController.collisionInfo.below = true;
            characterController.transform.eulerAngles = Vector3.zero;
            characterController.boxCollider2D.enabled = false;
        }

        public void OnExit(State newState)
        { 
            characterController.transform.eulerAngles = Vector3.zero;
            characterController.boxCollider2D.enabled = true;

            characterController.motion.rawVelocity = Vector2.zero;
            characterController.motion.timeSpentFluttering = 0f;
            characterController.motion.timeOfLastJumpAttempt = 0f;

            Audio.Play(SFX.instance.level.bubble.pop, Channel.Game);
        }
        
        public void Act()
        {
        }

        public bool CheckTransitions()
        {
            return false;
        }
        
        public bool Equals(State state)
        {
            return state == stateEnum;
        }
    }
}