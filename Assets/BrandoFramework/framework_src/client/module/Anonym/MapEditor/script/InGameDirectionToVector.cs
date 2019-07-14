using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Anonym.Isometric
{
    using Util;
   
    public static class InGameDirectionToVector
    {
        public static int ToInt(this InGameDirection _dir)
        {
            return (int)_dir;
        }

        public static Vector3 ToVector3(this InGameDirection _dir)
        {
            switch(_dir)
            {
                case InGameDirection.Right_Move:
                    return Vector3.right + Vector3.forward;
                case InGameDirection.DR_Move:
                    return Vector3.right;
                case InGameDirection.Down_Move:
                    return Vector3.right + Vector3.back;
                case InGameDirection.DL_Move:
                    return Vector3.back ;
                case InGameDirection.Left_Move:
                    return Vector3.left + Vector3.back;
                case InGameDirection.TL_Move:
                    return Vector3.left;
                case InGameDirection.Top_Move:
                    return Vector3.left + Vector3.forward;
                case InGameDirection.TR_Move:
                    return Vector3.forward;
                case InGameDirection.Jump_Move:
                    return Vector3.up;
                default:
                    return Vector3.zero;
            }
        }
    }
}