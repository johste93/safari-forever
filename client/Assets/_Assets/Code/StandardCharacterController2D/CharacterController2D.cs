using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StandardCharacterController2D;

namespace StandardCharacterController2D
{
    public abstract class CharacterController2D : MonoBehaviour, ISuspendable
    {
        public Rigidbody2D Rigidbody2D;
        public BoxCollider2D BoxCollider2D;
        
        public IInputContoller InputController;             //This is the component AI or Player will be interfacing with.
        public IMotionController MotionController;          //Contains all the 
        public IStateController StateController;             //Controllers the characthers current state!
        public ICollisionController CollisionController;    //this is the component that senses the world around the character and stops it from moving through walls and other items.

        public abstract void Intitalize();

        protected abstract void FixedUpdate();

        public abstract void Suspend(bool suspend);
        public abstract void On_SuspensionEvent(bool suspend);

        public abstract float GetMovementSpeed();
    }
}