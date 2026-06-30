using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class FSM_CharacterController : MonoBehaviour, ISuspendable
    {
        public Character character;
        public InputController inputController;
        public StateController stateController;
        public MovementController movementController;
        public Transform centerPivot;

        public Properties properties;
        public Motion motion;
        public new AudioProperties audio;
        public CollisionInfo collisionInfo;
        public InputInfo inputInfo;

        public Rigidbody2D rigidBody2D;
        public BoxCollider2D boxCollider2D;

        private bool isSuspended;

        private void Awake()
        {
            character = GetComponent<Character>();
            inputController = new InputController(this);
            movementController = new MovementController(this);
            motion = new Motion();
            audio = new AudioProperties();

            stateController = new StateController(this); //Has to come last.
            
            //stateController.SetState(properties.defaultState);
            stateController.currentState = StateMap.LookUp(properties.defaultState);
            stateController.currentState.SetReferenceToCharacter(this);    
        }

        private void Update()
        {
            inputController.RegisterInput();
        }

        private void FixedUpdate()
        {
            if(isSuspended)
                return;

            stateController.UpdateState();

            if(stateController.currentState.Equals(State.Inactive))
            {
                stateController.CheckTransitions();
                return;
            }            

            stateController.Act();

            movementController.UpdateRigidbody();

            stateController.CheckTransitions();
        }

        public void Suspend(bool suspend)
        {
            isSuspended = suspend || motion.isBeingTransported;
        }

        public void On_SuspensionEvent(bool suspend)
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
    }
}
