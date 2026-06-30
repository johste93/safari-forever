using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StandardCharacterController2D 
{
    public interface IMotionController
    {
        void UpdateRigidbody();

        Vector2 GetInternalVelocity();
        Vector2 GetCombinedVelocity();

        void SetInternalVelocity(Vector2 unclampedVelocity);
        void SetInternalVelocityX(float x);
        void SetInternalVelocityY(float y);

        void SetClampedVelocity(Vector2 clampedVelocity);
        void SetClampedVelocityX(float x);
        void SetClampedVelocityY(float y);

        void SetConveyorVelocity(Vector2 conveyorVelocity);
    }
}