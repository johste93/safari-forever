using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StandardCharacterController2D;
using StandardCharacterController2D.Spikeball;

namespace StandardCharacterController2D.Spikeball.V1
{
    public class Spikeball_CharacterController2D : CharacterController2D
    {
        public float MovementSpeed;
        public float MaxFallSpeed;
        public float Gravity;

        private bool isSuspended = true;
        
        
        private void Awake()
        {
            Intitalize();
        }

        public override void Intitalize()
        {
            InputController = null;
            MotionController = new MotionController(this);
            StateController = new StateController(this);
            CollisionController = new CollisionController(this);
        }

        protected override void FixedUpdate()
        {
            if(isSuspended)
                return;
                
            StateController.Act();

            MotionController.UpdateRigidbody();

            StateController.CheckTransitions();
        }

        public override float GetMovementSpeed()
        {
            return MovementSpeed;
        }

        public override void Suspend(bool suspend)
        {
            isSuspended = !GameMaster.instance.IsPlaying() || suspend;
        }

        public override void On_SuspensionEvent(bool suspend)
        {
            //if trying to unsuspend.
            if(suspend == false)
            {
                //Check if we are in play mode
                if(GameMaster.instance.IsPlaying())
                    Suspend(false);
            }
            else
                Suspend(true);
        }

        private void OnEnable()
        {
            SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
        }

        private void Unsubscribe()
        {
            SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        public bool IsSuspended()
        {
            return isSuspended;
        }
    }
}