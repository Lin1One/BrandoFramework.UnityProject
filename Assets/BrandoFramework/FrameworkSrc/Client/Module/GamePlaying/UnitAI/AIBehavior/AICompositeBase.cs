#region Head

// Author:            Yu
// CreateDate:        2018/10/25 11:13:26
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System.Collections.Generic;
using UnityEngine;

namespace Client.GamePlaying.AI
{
    /// <summary>
    /// AI行为，复合行为基类
    /// </summary>
    public abstract class YuAICompositeBase : YuAIBehaviorBase
    {
        protected List<YuAIBehaviorBase> childendBehaviors = 
            new List<YuAIBehaviorBase>();

        public override void AddChild(YuAIBehaviorBase child)
        {
            childendBehaviors.Add(child);
        }

        public override List<YuAIBehaviorBase> GetChildren()
        {
            return childendBehaviors;
        }

        /// <summary>
        /// 移除一个指定的子行为
        /// </summary>
        /// <param name="child"></param>
        public void RemoveChild(YuAIBehaviorBase child)
        {
            if (childendBehaviors.Contains(child))
            {
                childendBehaviors.Remove(child);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("错误，无法删除复合行为子行为，因为无法找到此行为：" +
                    child.GetType());
#endif
            }
        }

        /// <summary>
        /// 清空子行为列表，但不调用它们的Release
        /// </summary>
        public void ClearChildern()
        {
            childendBehaviors.Clear();
        }

        /// <summary>
        /// 施放自己，以及调用所有的子行为Release
        /// </summary>
        public override void Release()
        {
            for (int i = 0; i < childendBehaviors.Count; i++)
            {
                childendBehaviors[i].Release();
            }
            childendBehaviors.Clear();
        }
    }
}

