using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public interface ICollidableTrigger
    {
        void OnCollisionTrigger(FSM_CharacterController controller, Direction4 direction);
    }
}
