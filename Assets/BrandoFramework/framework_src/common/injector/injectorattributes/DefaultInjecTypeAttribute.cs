#region Head

// Author:            Yu
// CreateDate:        2018/8/20 15:46:11
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System;

namespace Common
{
    [AttributeUsage(AttributeTargets.Interface| AttributeTargets.Class)]
    public class DefaultInjecTypeAttribute : Attribute
    {
        public Type SourceType { get; }

        public DefaultInjecTypeAttribute(Type type)
        {
            SourceType = type;
        }
    }
}