#region Head

// Author:            Chengkefu
// CreateDate:        2019/04/25 16:20:00
// Email:             chengkefu0730@live.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Client.GamePlaying.AI;
using Common.DataStruct;
using System;
using UnityEngine;

namespace Client.GamePlaying.Unit
{
    /// <summary>
    /// AI控制组件接口
    /// </summary>
    public interface IYuUnitAIControl : IUnitComponent
    {
        ///// <summary>
        ///// 关注的目标
        ///// </summary>
        //UnitEntityBase TargetUnit
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 移动到某处
        ///// </summary>
        ///// <param name="coord">服务器二维数组坐标</param>
        ///// <param name="callBack">寻路回调，成功到达返回true，获取道路失败或是中途中止，返回false</param>
        ///// <param name="isPath">true：使用寻路方式移动   false：强制直线移动</param>
        //void MoveTo(Point2 coord, Action<bool> callBack = null, bool isPath = true, float stopDis = 0.0f);

        ///// <summary>
        ///// 移动到某处
        ///// </summary>
        ///// <param name="coord">服务器二维数组坐标</param>
        ///// <param name="callBack">寻路回调，成功到达返回true，获取道路失败或是中途中止，返回false</param>
        ///// <param name="isPath">true：使用寻路方式移动   false：强制直线移动</param>
        //void MoveTo(Vector2 pos, Action<bool> callBack = null, bool isPath = true, float stopDis = 0.0f);

        /// <summary>
        /// 设置当前行为数，（原有行为数将被结束）
        /// </summary>
        /// <param name="tree"></param>
        void ResetBehaviorTree(YuAIBehaviorTree tree);

        //用于切换AI行为，记录的参数
        object BehaviorParam { get; set; }

        bool CheckBehaviorTreeMatch(YuAIBehaviorTree tree);
    }
}

