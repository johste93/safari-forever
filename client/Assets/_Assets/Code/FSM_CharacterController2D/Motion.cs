using UnityEngine;
using System.Collections;
using System;

namespace FSM_CharacterController2D
{
    [Serializable]
    public class Motion
    {   
        /// <summary>
        /// Our raw velocity before doing collision detection
        /// </summary>
        public Vector2 rawVelocity;
        /// <summary>
        /// Our velocity limited by collision detection
        /// </summary>
        public Vector2 clampedVelocity;
        /// <summary>
        /// Constant force applied by conveyorbelts
        /// </summary>
        public Vector2 conveyorSpeed;
        /// <summary>
        /// Constant force applied by running.
        /// </summary>
        public float runningVelocity = 0;
        /// <summary>
        /// Equal to rawVelocity + runningVelocity + conveyorVelocity
        /// </summary>
        /// <value></value>
        public Vector2 combinedVelocity
        {
            get{
                return rawVelocity + conveyorSpeed + new Vector2(runningVelocity, 0);
            }
        }


        /// <summary>
        /// The direction we are running in represented by an Int. Left == -1, Right == 1
        /// </summary>
        public int runningDirection = 1;
        
        public bool standingOnSlope = false;
        public int slopeDirection;

        public bool touchingSlime = false;

        public float distanceToGround = Mathf.Infinity;

        public bool isBeingTransported = false;


        //Jumping
        /// <summary>
        /// Used to allow the player some 'wiggleroom' when making inputs, ensuring a more responsive feeling.
        /// </summary>
        public float timeOfLastJumpAttempt;
        /// <summary>
        /// Is used to manipulate player jump height.
        /// </summary>
        public float gravityScale = 1;


        //Fluttering
        /// <summary>
        /// Time since we started fluttering.
        /// </summary>
        public float timeSpentFluttering;

        public float peakFallingVelocity;



        public float frameLastGrounded;
        public float timeLastGrounded;
    }
}
