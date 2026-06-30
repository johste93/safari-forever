using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public enum CollisionOutcome{
        NoCollision,
        Collided,
        RedoCollisionHandling
    }
}