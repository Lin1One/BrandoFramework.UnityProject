#region Head

// Author:           Yu
// CreateDate:    2018/4/19 11:36:39
// Email:             Yu@gmail.com || 35490136@qq.com

#endregion

using System;

namespace Common
{
    /// <summary>
    /// 单例特性。
    /// 被附加该特性的类型在通过注入器获取实例时将始终返回同一个实例。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SingletonAttribute : Attribute
    {
    }
}