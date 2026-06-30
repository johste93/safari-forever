using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public enum State
    {
        Inactive,
        Idle,
        Falling,
        FallingWithoutControl,
        FallingOfWall,
        SlidingAboveWall,
        Running,
        Jumping,
        WallJumping,
        Fluttering,
        WallSliding,
        JumpingOnJumpPad,
        Landing,
        Dead,
        Bouncing,
        Stopped,
        StuckInSlime,
        StuckInSlimeOnWall,
        StuckInSlimeOnRoof,
        InsideBubble
    }
}