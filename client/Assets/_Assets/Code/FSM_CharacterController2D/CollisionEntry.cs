using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public struct CollisionEntry
    {
        public IInteractable obj;
        public FSM_CharacterController controller;
        public Direction4 direction;

        public CollisionEntry(IInteractable obj, FSM_CharacterController controller, Direction4 direction)
        {
            this.obj = obj;
            this.controller = controller;
            this.direction = direction;
        }
    }
}
