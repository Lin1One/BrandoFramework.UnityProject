#region Head

// Author:            LinYuzhou
// CreateDate:        2018/10/25 14:11:28
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
    /// AI行为，复合行为，选择器
    /// 依次执行子行为，直到一个成功、或是全失败完则结束
    /// </summary>
    public class YuAISelector : YuAICompositeBase
    {
        protected int m_curChildIndex;

        protected override void Enter()
        {
            m_curChildIndex = 0;
        }

        protected override void Exit()
        {

        }

        protected override AIBehaviorState Update()
        {
            while (true)
            {
                //执行全部完成则返回失败
                if (m_curChildIndex >= childendBehaviors.Count)
                    return AIBehaviorState.Failure;

                AIBehaviorState state = childendBehaviors[m_curChildIndex].Tick();
                //如果一个子行为未失败，则返回相应状态
                if (state != AIBehaviorState.Failure)
                {
                    return state;
                }
                //失败了则执行下一个
                m_curChildIndex++;
            }
        }
    }
}

