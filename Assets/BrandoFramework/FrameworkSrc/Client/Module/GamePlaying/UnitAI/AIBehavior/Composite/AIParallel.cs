#region Head

// Author:            LinYuzhou
// CreateDate:        2018/10/25 14:35:48
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

namespace Client.GamePlaying.AI
{
    /// <summary>
    /// AI行为，复合行为，并行器
    /// 让多个行为并行(同一帧各依次执行一次)
    /// </summary>
    public class YuAIParallel : YuAICompositeBase
    {
        //成功、失败条件，是一个行为，还是全部行为
        public enum EPolicy    
        {
            RequireOne, //一个
            RequireAll, //全部
        }

        protected EPolicy m_successPolicy;
        protected EPolicy m_failurePolicy;

        /// <summary>
        /// 尽量避免条件为全部成功/全部失败
        /// </summary>
        /// <param name="success">成功条件，一个成立或全部成立</param>
        /// <param name="failure">失败条件，一个成立或全部成立</param>
        public YuAIParallel(EPolicy success,EPolicy failure)
        {
            m_successPolicy = success;
            m_failurePolicy = failure;
        }


        protected override void Enter()
        {
            
        }

        protected override void Exit()
        {
            //如果满足条件退出时，还有子行为在运行中，则应立即中止其行为
            for(int i=0;i< childendBehaviors.Count;i++)
            {
                if(childendBehaviors[i].CurState == AIBehaviorState.Running)
                {
                    childendBehaviors[i].StopRunning();
                }
            }
        }

        protected override AIBehaviorState Update()
        {
            int successCount = 0, failureCount = 0;
            int childernCount = childendBehaviors.Count;
            bool isTerminate = true;


            for (int i=0;i<childernCount;i++)
            {
                if (!childendBehaviors[i].IsTerminate())   //还未结束
                {
                    isTerminate = false;
                    childendBehaviors[i].Tick();
                }

                AIBehaviorState state = childendBehaviors[i].CurState;
                if (state == AIBehaviorState.Success)
                {
                    if(m_successPolicy == EPolicy.RequireOne)
                    {
                        childendBehaviors[i].Reset();
                        return AIBehaviorState.Success;
                    }
                    successCount++;
                }
                else if(state == AIBehaviorState.Failure)
                {
                    if(m_failurePolicy == EPolicy.RequireOne)
                    {
                        childendBehaviors[i].Reset();
                        return AIBehaviorState.Failure;
                    }
                    failureCount++;
                }
            }

            if(m_failurePolicy == EPolicy.RequireAll && 
                failureCount == childernCount)
            {
                for(int i=0;i<childernCount;i++)
                {
                    childendBehaviors[i].Reset();
                }
                return AIBehaviorState.Failure;
            }

            if(m_successPolicy == EPolicy.RequireAll &&
                successCount == childernCount)
            {
                for (int i = 0; i < childernCount; i++)
                {
                    childendBehaviors[i].Reset();
                }
                return AIBehaviorState.Success;
            }

            //已经结束所有的行为，但未判定是成功还是失败（条件是全部成功/全部失败）
            if (isTerminate) 
            {
                return AIBehaviorState.Aborted;
            }

            return AIBehaviorState.Running;
        }
    }
}

