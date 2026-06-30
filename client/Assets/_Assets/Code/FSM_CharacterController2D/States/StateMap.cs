using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FSM_CharacterController2D
{
    public class StateMap
    {
        public static IState LookUp(State state)
        {
            switch(state)
            {
                case State.Inactive:
                    return new Inactive();
                case State.Idle:
                    return new Idle();
                case State.Falling:
                    return new Falling();
                case State.Landing:
                    return new Landing();
                case State.FallingWithoutControl:
                    return new FallingWithoutControl();
                case State.FallingOfWall:
                    return new FallingOfWall();
                case State.SlidingAboveWall:
                    return new SlidingAboveWall();
                case State.Running:
                    return new Running();
                case State.Jumping:
                    return new Jumping();
                case State.WallJumping:
                    return new WallJumping();
                case State.Fluttering:
                    return new Fluttering();
                case State.WallSliding:
                    return new WallSliding();
                case State.Dead:
                    return new Dead();
                case State.JumpingOnJumpPad:
                    return new JumpingOnJumpPad();
                case State.Bouncing:
                    return new Bouncing();
                case State.Stopped:
                    return new Stopped();
                case State.StuckInSlime:
                    return new StuckInSlime();
                case State.StuckInSlimeOnWall:
                    return new StuckInSlimeOnWall();
                case State.StuckInSlimeOnRoof:
                    return new StuckInSlimeOnRoof();
                case State.InsideBubble:
                    return new InsideBubble();
            }

            Debug.LogError("State not found in StateMap!");
            return null;
        }
    }
}
