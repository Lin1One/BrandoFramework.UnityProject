#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/13 14:44:21
// Email:             836045613@qq.com

#endregion

using UnityEditor;
using UnityEngine;

namespace Common.Editor
{
    /// <summary>
    /// 层次面板选择项改变事件处理器。
    /// </summary>
    public interface IHierarchyItemChangeHandler
    {
        /// <summary>
        /// 检测目标游戏对象是否可以被实例自身处理。
        /// 可以处理返回真，不可以处理返回假。
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        bool SpecialCheck(GameObject go);

        /// <summary>
        /// 生成动态菜单。
        /// </summary>
        void MakeDynamicMenu(GenericMenu menu);

        /// <summary>
        /// 生成固定菜单。
        /// </summary>
        /// <param name="go"></param>
        void MakeFixedMenu();
    }
}