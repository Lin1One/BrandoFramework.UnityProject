#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Client.LegoUI
{
    /// <summary>
    /// 用于支持数据模型的精灵字符串。
    /// 其提供一个可视化的精灵选择器并用于赋值给内部的资源Id字符串。
    /// </summary>
    [Serializable]
    public class LegoSpriteString
    {
        [LabelText("精灵资源Id")]
        [ReadOnly]
        public string SpriteId;

#if DEBUG

        private Action<string> changeNotify;

        public void Binding(Action<string> callback)
        {
            changeNotify = callback;
        }

        [NonSerialized]
        [ShowInInspector]
        [LabelText("精灵")]
        [OnValueChanged("OnSprietChanged")]
        public Sprite Sprite;

        private void OnSprietChanged()
        {
            SpriteId = Sprite.name;
            changeNotify?.Invoke(Sprite.name);
        }
#endif
    }
}