#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/13 14:44:21
// Email:             836045613@qq.com

#endregion

using UnityEditor;
using UnityEngine;

namespace Client.Core.Editor
{
    /// <summary>
    /// 层次面板上下文扩展抽象基类。
    /// </summary>
    public abstract class AbsHierarchyItemChangeHandler
        : IHierarchyItemChangeHandler
    {
        public abstract bool SpecialCheck(GameObject go);

        public virtual void MakeDynamicMenu(GenericMenu menu)
        {
        }

        protected Event CurrentEvent
        {
            get { return Event.current; }
        }

        protected Vector2 MousePosition
        {
            get { return CurrentEvent.mousePosition; }
        }

        protected Rect MenuRect
        {
            get { return new Rect(MousePosition.x, MousePosition.y, 0, 0); }
        }

        /// <summary>
        /// 当前选择的层次面板窗口中的游戏对象。
        /// </summary>
        protected GameObject SelectGo
        {
            get { return Selection.activeGameObject; }
        }

        public abstract void MakeFixedMenu();


        //        public abstract void MakeContextMenu();
    }
}