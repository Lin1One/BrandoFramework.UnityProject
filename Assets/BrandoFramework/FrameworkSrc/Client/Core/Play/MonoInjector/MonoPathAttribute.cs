#region Head

// Author:            Yu
// CreateDate:        2018/8/21 9:50:57
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System;

namespace Client
{
    /// <summary>
    /// 继承自MonoBehavior的类可以通过该特性标明自己的挂载路径。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MonoPathAttribute : Attribute
    {
        /// <summary>
        /// mono游戏对象挂载的父路径。
        /// </summary>
        public string ParentPath { get; }

        /// <summary>
        /// mono游戏对象挂载时的自身路径即名字。
        /// </summary>
        public string SelfName { get; }

        public MonoPathAttribute(string parentPath, string selfName)
        {
            ParentPath = parentPath;
            SelfName = selfName;
        }
    }
}