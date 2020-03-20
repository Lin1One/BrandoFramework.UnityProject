#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/11 22:06:31
// Email:             836045613@qq.com

#endregion

using System;
using UnityEngine;

namespace Common.DataStruct
{
    /// <summary>
    /// 响应数据模型抽象泛型实现。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public abstract class AbsRxStruct<T> : IRxStruct<T>
    {
        [SerializeField]
        protected T m_Value;

        public T Value
        {
            get { return m_Value; }
            set
            {
                m_Value = value;
                OnValueChanged?.Invoke(m_Value);
            }
        }

        protected abstract bool CheckChange(T newValue);
        protected Action<T> OnValueChanged;

        public virtual void ReceiveControlChange(T newValue)
        {
            if (!CheckChange(newValue))
            {
                return;
            }

            m_Value = newValue;
        }

        public void Watch(Action<T> onValueChange)
        {
            OnValueChanged = onValueChange;
        }

        public virtual void Reset()
        {
            OnValueChanged = null;
        }
    }
}