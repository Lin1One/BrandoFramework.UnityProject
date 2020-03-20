#region Head

// Author:            Yu
// CreateDate:        2018/8/28 10:44:41
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System;

namespace Common.DataStruct
{
    /// <summary>
    /// 响应式基础数据结构接口。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRxStruct<T> : IReset
    {
        T Value { get; set; }

        /// <summary>
        /// 绑定模型。
        /// 数据会从控件推送到模型。
        /// </summary>
        /// <param name="newValue"></param>
        void ReceiveControlChange(T newValue);

        void Watch(Action<T> onValueChange);
    }
}