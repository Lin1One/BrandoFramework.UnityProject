using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Anonym.Util
{
    using Isometric;

    public enum InGameDirection
    {
        BaseField = 0,
        No_Move = 0,

        Right_Move = 1,                     //右
        Right_Rotate = -1 * Right_Move,

        RD_Move = 2,                        //右下
        DR_Move = RD_Move,
        RD_Rotate = -1 * RD_Move,

        Down_Move = 3,                      //下
        Down_Rotate = -1 * Down_Move,

        LD_Move = 4,                        //左下
        DL_Move = LD_Move,
        LD_Rotate = -1 * LD_Move,

        Left_Move = 5,                      //左
        Left_Rotate = -1 * Left_Move,

        LT_Move = 6,                        //左上
        TL_Move = LT_Move,
        LT_Rotate = -1 * LT_Move,
            
        Top_Move = 7,                       //上
        Top_Rotate = -1 * Top_Move,

        RT_Move = 8,                        //右上
        TR_Move = RT_Move,
        RT_Rotate = -1 * RT_Move,

        Dash = 9,                           //冲刺

        Jump_Move = 101,                    //跳跃

        OppositeDir = 4,                    //相反方向
        OutField = 100,                     
        ParentField = 102,
        None = 1000,
    }

}