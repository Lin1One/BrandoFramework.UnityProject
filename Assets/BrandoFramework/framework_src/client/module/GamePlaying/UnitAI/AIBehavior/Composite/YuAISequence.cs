#region Head

// Author:            Yu
// CreateDate:        2018/10/25 11:36:31
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
    /// AI行为，复合行为，顺序器
    /// 依次执行子行为，直到一个失败、或是全执行完则结束
    /// </summary>
    public class YuAISequence : YuAICompositeBase
    {
        protected int curChildActionIndex;

        protected override void Enter()
        {
            curChildActionIndex = 0;
        }

        protected override void Exit()
        {

        }

        protected override AIBehaviorState Update()
        {
            while (curChildActionIndex < childendBehaviors.Count)
            {
                AIBehaviorState state = childendBehaviors[curChildActionIndex].Tick();
                //如果还未执行成功了就返回相应状态
                if (state != AIBehaviorState.Success)
                {
                    return state;
                }
                //执行成功了继续执行下一个行为
                curChildActionIndex++;
            }
            //执行全部完成则返回成功
            return AIBehaviorState.Success;
        }
    }
}

