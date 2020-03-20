#region Head

// Author:        Yu
// CreateDate:    2018/4/16 14:47:37
// Email:         Yu@gmail.com || 35490136@qq.com

#endregion

using System;

namespace Common
{
    /// <summary>
    /// 注入特性。
    /// 该特性用于标识需要被注入的字段或者属性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectAttribute : Attribute
    {
    }
}