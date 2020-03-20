#region Head

// Author:            ChengKeFu
// CreateDate:        2018/10/25 16:54:30
// Email:             chengkefu0730@live.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

namespace Client.GamePlaying.AI
{
    /// <summary>
    /// AI行为，复合行为，监视器
    /// 
    /// Todo 
    /// </summary>
    public class YuAIMonitor : YuAIParallel
    {
        public YuAIMonitor(EPolicy succss, EPolicy failure)
            :base(succss, failure)
        {

        }

        //条件
        //private  bool Condition

        protected override AIBehaviorState Update()
        {
            AIBehaviorState state = base.Update();
            //Todo 加入条件测试，不满足则马上跳出

            return state;
        }
    }
}

