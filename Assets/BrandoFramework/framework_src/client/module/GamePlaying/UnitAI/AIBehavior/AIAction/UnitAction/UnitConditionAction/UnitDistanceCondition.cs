#region Head

// Author:            Yu
// CreateDate:        2018/10/24 21:38:25
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System.Collections.Generic;
using UnityEngine;

namespace Client.GamePlaying.AI
{
    /// <summary>
    /// AI行为条件基类
    /// </summary>
    public class UnitDistanceCondition: YuAIConditionBase
    {
        public Vector2 targetPosition2D;
        public override void AddChild(YuAIBehaviorBase child)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning("这是一个AI条件，不具备子节点：YuAIActionPatrol");
#endif
        }

        public UnitDistanceCondition() { }

        public override void Init(object param)
        {
            targetPosition2D = (Vector2)param;
        }


        protected override AIBehaviorState OnUpdate()
        {
            var position2d = new Vector2(unitEntity.U3DData.Trans.position.x, unitEntity.U3DData.Trans.position.z);
            var distance = Vector2.Distance(position2d, targetPosition2D);
            AIBehaviorState state;
            if (distance < 5)
            {
                 state = AIBehaviorState.Success;
            }
            else
            {
                state = AIBehaviorState.Failure;            }
            
            return state;
        }

        protected override void Enter()
        {
            
        }

        protected override void Exit()
        {
            
        }

        public override void Release()
        {
            
        }
    }
}
