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

using System;
using System.Collections.Generic;

namespace Client.GamePlaying.AI
{
    /// <summary>
    /// AI行为基类
    /// </summary>
    [Serializable]
    public abstract class YuAIBehaviorBase
    {
        public virtual string BehaviourDes { get; }

        private AIBehaviorState curState;
        /// <summary>
        /// 当前状态
        /// </summary>
        public AIBehaviorState CurState
        {
            get { return curState; }
            set
            {
                    curState = value;
                    OnStateChange?.Invoke(curState);
            }
        }

        public Action<AIBehaviorState> OnStateChange;

        public YuAIBehaviorBase()
        {
            curState = AIBehaviorState.Invalid;
        }

        protected abstract void Enter();

        protected abstract AIBehaviorState Update();

        protected abstract void Exit();

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public AIBehaviorState Tick()
        {
            if(CurState != AIBehaviorState.Running)
            {
                Enter();
            }
            CurState = Update();
            if(CurState != AIBehaviorState.Running)
            {
                Exit();
            }
            return CurState;
        }

        /// <summary>
        /// 重置，重新设为初始状态
        /// </summary>
        public void Reset()
        {
            curState = AIBehaviorState.Invalid;
        }

        /// <summary>
        /// 手动停止行为的运行，并设置状态为终止
        /// </summary>
        public void StopRunning()
        {
            if (curState == AIBehaviorState.Running)
            {
                CurState = AIBehaviorState.Aborted;
                Exit();
            }
        }

        /// <summary>
        /// 如果此行为是保护子行为的，则添加一个子行为
        /// (只能有一个子行为，则覆盖)
        /// </summary>
        /// <param name="child"></param>
        public abstract void AddChild(YuAIBehaviorBase child);

        /// <summary>
        /// 如果此行为是保护子行为的，则添加一个子行为
        /// (只能有一个子行为，则覆盖)
        /// </summary>
        /// <param name="child"></param>
        public abstract List<YuAIBehaviorBase> GetChildren();

        /// <summary>
        /// 是否已结束（成功、失败、终止）
        /// </summary>
        /// <returns></returns>
        public bool IsTerminate()
        {
            if(curState == AIBehaviorState.Success||
               curState == AIBehaviorState.Failure ||
               curState == AIBehaviorState.Aborted)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public abstract void Release();

    }
}

