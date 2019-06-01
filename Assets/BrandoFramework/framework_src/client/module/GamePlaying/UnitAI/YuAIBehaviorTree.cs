#region Head

// Author:            Yuzhou
// CreateDate:        2018/10/24 20:55:42
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

namespace Client.GamePlaying.AI
{
    public class YuAIBehaviorTree 
    {
        //起始行为
        private YuAIBehaviorBase m_rootBehavior;

        public YuAIBehaviorTree(YuAIBehaviorBase root)
        {
            m_rootBehavior = root;
        }

        public AIBehaviorState Tick()
        {
            if (m_rootBehavior != null)
            {
                //Todo 根据返回的状态改变行为
                return m_rootBehavior.Tick();
            }
            return AIBehaviorState.Aborted;
        }

        public bool HaveRoot
        {
            get { return m_rootBehavior != null; }
        }

        public void SetRoot(YuAIBehaviorBase root)
        {
            m_rootBehavior = root;
        }

        public void Release()
        {
            m_rootBehavior.Release();
        }
    }
}

