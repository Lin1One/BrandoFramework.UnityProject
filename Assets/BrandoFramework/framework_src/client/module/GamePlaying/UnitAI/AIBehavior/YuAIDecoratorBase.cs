#region Head

// Author:            Yu
// CreateDate:        2018/10/24 21:36:15
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
    /// AI行为装饰器基类
    /// </summary>
    public abstract class YuAIDecoratorBase : YuAIBehaviorBase
    {
        protected YuAIBehaviorBase child;

        public YuAIDecoratorBase()
        {

        }

        public override void AddChild(YuAIBehaviorBase child)
        {
            this.child = child;
        }

        public override void Release()
        {
            child.Release();
            child = null;
        }

    }
}
