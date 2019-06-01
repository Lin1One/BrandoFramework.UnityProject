#region Head

// Author:            Yu
// CreateDate:        2018/10/24 20:38:53
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.GamePlaying.AI
{
    [Serializable]
    public class BehaviorTreeBaseNode
    {
        /// <summary>
        /// 当前状态
        /// </summary>
        [HorizontalGroup]
        [LabelText("状态")]
        [LabelWidth(40)]
        public AIBehaviorState CurState;


        [HorizontalGroup]
        [LabelText("描述")]
        [LabelWidth(40)]
        public string StateStr;

        [LabelText("子节点")]
        public List<BehaviorTreeBaseNode> childNodes = new List<BehaviorTreeBaseNode>();

        public void BindBehaviour(YuAIBehaviorBase behaviour)
        {
            CurState = behaviour.CurState;
            behaviour.OnStateChange = SetState;
        }

        private void SetState(AIBehaviorState state)
        {
            if (CurState != state)
            {
                CurState = state;
                stateCount = 0;
            }
            else
            {
                stateCount = (stateCount + 1 ) % 3;
            }
            switch (CurState)
            {
                case AIBehaviorState.Success:
                    StateStr = Success + dot[stateCount];
                    break;
                case AIBehaviorState.Failure:
                    StateStr = Failure + dot[stateCount];
                    break;

            }
        }


        #region 文本描述
        private int stateCount;

        private const string Invalid = "初始状态";   //初始状态
        private const string Success = "通过";    //成功
        private const string Failure = "失败";    //失败
        private const string Running = "正在运行";    //运行
        private const string Aborted = "终止";    //终止

        private string[] dot = {".","..","..." };

        #endregion
    }
}

