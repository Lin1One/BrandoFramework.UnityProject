#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using Anonym.Isometric;
using Anonym.Util;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Input = UnityEngine.Input;

namespace Client.InputModule
{
    public class IsoInputProcesser : InputProcesserBase
    {
        /// <summary>
        /// 对角线
        /// </summary>
        private bool bUseDiagonalKey = true;

        /// <summary>
        /// 八方向
        /// </summary>
        private bool b8DirectionalMovement = true;

        /// <summary>
        /// 使用按键输入
        /// </summary>
        private bool UseKey = true;

        private System.Action<InGameDirection> Do;
        private System.Func<KeyCode, bool> GetKeyMethod;

        public void SetTarget()
        {
            ////if (target.bContinuousMovement)
            ////{
            ////    Do = ContinuousMove;
            ////    GetKeyMethod = Input.GetKey;
            ////}
            ////else
            ////{
            ////    Do = EnQueueTo;
            ////    GetKeyMethod = Input.GetKeyDown;
            ////}
        }


        public override void Process()
        {
            bool bShifted = Input.GetKey(KeyCode.LeftShift);
            if (UseKey)
            {
                if (b8DirectionalMovement)
                {
                    bool bSelected = false;
                    if (bUseDiagonalKey)
                    {
                        if (keyMacro(InGameDirection.LT_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.Keypad7, KeyCode.Q) ||
                            keyMacro(InGameDirection.RD_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.Keypad3, KeyCode.C) ||
                            keyMacro(InGameDirection.RT_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.Keypad9, KeyCode.E) ||
                            keyMacro(InGameDirection.LD_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.Keypad1, KeyCode.Z))
                            bSelected = true;
                    }
                    else
                    {
                        if (keyMacro(InGameDirection.LT_Move, bShifted, Enumerable.All<KeyCode>, GetKeyMethod, Do, KeyCode.LeftArrow, KeyCode.UpArrow) ||
                            keyMacro(InGameDirection.RD_Move, bShifted, Enumerable.All<KeyCode>, GetKeyMethod, Do, KeyCode.RightArrow, KeyCode.DownArrow) ||
                            keyMacro(InGameDirection.RT_Move, bShifted, Enumerable.All<KeyCode>, GetKeyMethod, Do, KeyCode.RightArrow, KeyCode.UpArrow) ||
                            keyMacro(InGameDirection.LD_Move, bShifted, Enumerable.All<KeyCode>, GetKeyMethod, Do, KeyCode.LeftArrow, KeyCode.DownArrow) ||
                            keyMacro(InGameDirection.LT_Move, bShifted, Enumerable.All<KeyCode>, GetKeyMethod, Do, KeyCode.A, KeyCode.W) ||
                            keyMacro(InGameDirection.RD_Move, bShifted, Enumerable.All<KeyCode>, GetKeyMethod, Do, KeyCode.D, KeyCode.S) ||
                            keyMacro(InGameDirection.RT_Move, bShifted, Enumerable.All<KeyCode>, GetKeyMethod, Do, KeyCode.D, KeyCode.W) ||
                            keyMacro(InGameDirection.LD_Move, bShifted, Enumerable.All<KeyCode>, GetKeyMethod, Do, KeyCode.A, KeyCode.S))
                            bSelected = true;
                    }

                    if (bSelected == false)
                    {
                        keyMacro(InGameDirection.Left_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.Keypad4, KeyCode.LeftArrow, KeyCode.A);
                        keyMacro(InGameDirection.Right_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.Keypad6, KeyCode.RightArrow, KeyCode.D);
                        keyMacro(InGameDirection.Top_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.Keypad8, KeyCode.UpArrow, KeyCode.W);
                        keyMacro(InGameDirection.Down_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.Keypad2, KeyCode.DownArrow, KeyCode.X, KeyCode.S);
                    }
                }
                else
                {
                    keyMacro(InGameDirection.LT_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.LeftArrow, KeyCode.A);
                    keyMacro(InGameDirection.RD_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.RightArrow, KeyCode.D);
                    keyMacro(InGameDirection.RT_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.UpArrow, KeyCode.W);
                    keyMacro(InGameDirection.LD_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.DownArrow, KeyCode.S);
                }
            }

        }

        private bool keyMacro(InGameDirection direction, bool bShift,
            System.Func<IEnumerable<KeyCode>, System.Func<KeyCode, bool>, bool> action,
            System.Func<KeyCode, bool> subAction,
            System.Action<InGameDirection> Do,
            params KeyCode[] codes)
        {
            if (bShift) // Rotate
                direction = (InGameDirection)(-1 * (int)direction);

            if (action(codes, subAction))
            {
                Do(direction);
                return true;
            }
            return false;
        }

        void ContinuousMove(InGameDirection direction)
        {
            if (direction < 0)
                direction = (InGameDirection)(-1 * (int)direction);
            Vector3 vMovement = IsometricMovement.HorizontalVector(direction);
            //target.DirectTranslate(vMovement * Time.deltaTime);
        }
    }
}

