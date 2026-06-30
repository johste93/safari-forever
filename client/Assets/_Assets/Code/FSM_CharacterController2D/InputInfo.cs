using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class InputInfo
    {
        /// <summary>
        /// Returns True during the frame the user pressed down the button
        /// </summary>
        /// <value></value>
        public bool onButtonDown
        {
            get{
                return Time.frameCount == _onButtonDownFrame+1;
            }
        }
        private int _onButtonDownFrame = -2;
        public int onButtonDownFrame{
            set{
                _consumed = false;
                _onButtonDownFrame = value;
            }
        }
        
        /// <summary>
        /// Returns True while the user holds down the button
        /// </summary>
        /// <value></value>
        public bool onButtonPressed
        {
            get{
                if(_consumed)
                    return false;

                return Time.frameCount == _onButtonPressedFrame+1;
            }
        }
        private int _onButtonPressedFrame = -2;
        public int onButtonPressedFrame{
            set{
                _onButtonPressedFrame = value;
            }
        }
        
        /// <summary>
        /// Returns True the first frame the user releases the button
        /// </summary>
        /// <value></value>
        public bool onButtonUp
        {
            get{
                if(_consumed)
                    return false;

                return Time.frameCount == _onButtonUpFrame+1;
            }
        }
        private int _onButtonUpFrame = -2;
        public int onButtonUpFrame{
            set{
                _onButtonUpFrame = value;
            }
        }

        private static bool _consumed = true;

        public void Reset()
        {
            _onButtonDownFrame = -2;
            _onButtonPressedFrame = -2;
            _onButtonUpFrame = -2;
        }

        public void Consume()
        {
            Reset();
            _consumed = true;
        }
    }
}
