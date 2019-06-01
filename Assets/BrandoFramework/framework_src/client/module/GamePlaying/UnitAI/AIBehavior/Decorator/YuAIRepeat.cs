#region Head

// Author:            Yu
// CreateDate:        2018/10/25 8:58:28
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

namespace Client.GamePlaying.AI
{
    /// <summary>
    /// AI行为，装饰器，重复执行
    /// </summary>
    public class YuAIRepeat : YuAIDecoratorBase
    {
        protected int repeatTime;    //重复次数(0为无限重复)
        protected int m_count;

        public YuAIRepeat(int limitCount)
        {
            repeatTime = limitCount;
        }

        protected override void Enter()
        {
            m_count = 0;
        }

        protected override void Exit()
        {

        }

        protected override AIBehaviorState Update()
        {
            child.Tick();
            if (child.CurState == AIBehaviorState.Running)        //执行中，继续执行
            {
                return AIBehaviorState.Running;
            }
            if (child.CurState == AIBehaviorState.Failure)     //失败则返回失败
            {
                return AIBehaviorState.Failure;
            }
            if (repeatTime > 0 && ++m_count >= repeatTime)                  //执行完成指定次数返回成功
            {
                return AIBehaviorState.Success;
            }
            //执行完毕一次，但未达到指定次数，重置子行为状态，并继续执行
            child.Reset();
            return AIBehaviorState.Running;
        }
    }
}

