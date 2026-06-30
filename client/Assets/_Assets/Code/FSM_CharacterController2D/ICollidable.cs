using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public interface ICollidable
    {
        void NewCollision(FSM_CharacterController controller);
        void MaintainedCollision(FSM_CharacterController controller);
        void LostCollision(FSM_CharacterController controller);
    }
}