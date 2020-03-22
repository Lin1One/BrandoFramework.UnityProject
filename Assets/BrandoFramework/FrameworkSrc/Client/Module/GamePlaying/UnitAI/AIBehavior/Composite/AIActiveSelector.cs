#region Head

// Author:            LinYuzhou
// CreateDate:        2018/10/25 19:59:38
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
    /// AI行为，复合行为，选择器，主动选择器
    /// </summary>
    public class YuAIActiveSelector : YuAISelector
    {

        protected override AIBehaviorState Update()
        {
            //保存当前行为节点
            int lastChildIndex = m_curChildIndex;
            base.Enter();       //重新从第一个（最高优先级）子行为开始运行:(m_curChildIndex = 0)
            AIBehaviorState stateResult = base.Update();
            //如果当前节点前面的节点能执行，或是当前节点失败，运行到了后面的节点，则终止当前节点的执行
            if( m_curChildIndex != lastChildIndex &&
                lastChildIndex > 0 && lastChildIndex < childendBehaviors.Count)    //安全判断
            {
                childendBehaviors[lastChildIndex].StopRunning();
            }

            return stateResult;
        }
    }
}

