#region Head

// Author:            Yu
// CreateDate:        2018/10/24 21:32:21
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Client.GamePlaying.Unit;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.GamePlaying.AI
{
    /// <summary>
    /// 游戏实体AI行为
    /// </summary>
    [Serializable]
    public abstract class YuAIActionBase : YuAIBehaviorBase
    {
        //protected UnitEntityBase unit;    //行为主体

        public void SetRole(UnitEntityBase unitEntity)
        {
            //unit = unitEntity;
        }

        public abstract void Init(object param);

        public override void AddChild(YuAIBehaviorBase child)
        {
#if UNITY_EDITOR
            Debug.LogWarning("这是一个AI动作，不具备子节点：YuAIActionPatrol");
#endif
        }

        public override List<YuAIBehaviorBase> GetChildren()
        {
            return null;
        }
    }
}

