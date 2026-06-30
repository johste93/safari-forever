using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class InputController
    {
        private FSM_CharacterController characterController;

        public InputController(FSM_CharacterController characterController)
        {
            this.characterController = characterController;
            characterController.inputInfo = new InputInfo();
        }

        public void RegisterInput()
        {
            characterController.inputInfo = new InputInfo();

            if(CharacterInput.On_TouchStart(0))
            {
                if (TransitionHole.instance.IsClosed())
                    return;

                characterController.motion.timeOfLastJumpAttempt = Time.time;
                characterController.inputInfo.onButtonDownFrame = Time.frameCount;
            }
                
            if(CharacterInput.On_TouchMaintained(0))
            {
                if (TransitionHole.instance.IsClosed())
                    return;

                characterController.inputInfo.onButtonPressedFrame = Time.frameCount;
            }
            
            if(CharacterInput.On_TouchEnd(0))
            {
                if (TransitionHole.instance.IsClosed())
                    return;

                characterController.inputInfo.onButtonUpFrame = Time.frameCount;
            }
        }

        public void ConsumeInput()
        {
            characterController.motion.timeOfLastJumpAttempt = 0f;
            characterController.inputInfo.Consume();
        }
    }
}