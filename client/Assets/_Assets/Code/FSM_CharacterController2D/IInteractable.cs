using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;

public interface IInteractable
{
    void NewCollision(CollisionEntry collisionInfo);
    void MaintainedCollision(CollisionEntry collisionInfo);
    void LostCollision(CollisionEntry collisionInfo);
}
