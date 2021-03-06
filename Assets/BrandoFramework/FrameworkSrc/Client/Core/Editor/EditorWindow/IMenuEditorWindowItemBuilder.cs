using Sirenix.OdinInspector.Editor;
using System;

namespace Client.Core.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MenuItemBuilderDescAttribute : Attribute
    {
        public Type WindowType { get; private set; }

        /// <summary>
        /// 构建器的说明。
        /// </summary>
        public string Desc { get; private set; }

        public MenuItemBuilderDescAttribute(Type winwodType, string desc)
        {
            WindowType = winwodType;
            Desc = desc;
        }
    }

    /// <summary>
    /// 多菜单窗口窗口项构建器。
    /// 用于动态扩展一个多菜单窗口。
    /// </summary>
    public interface IMenuEditorWindowItemBuilder
    {
        void BuildMenuItem(OdinMenuTree tree);
    }
}