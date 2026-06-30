using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StandardCharacterController2D
{
    public interface ICollisionController
    {
        void UpdateCollisions();
        CollisionInfo GetCollisionInfo();
    }
}
