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
    /// 用于支持数据模型的贴图字符串。
    /// 其提供一个可视化的贴图选择器并用于赋值给内部的资源Id字符串。
    /// </summary>
    [Serializable]
    public class LegoTextureString
    {
        [LabelText("贴图资源Id")]
        [ReadOnly]
        public string TextureId;

#if DEBUG

        private Action<string> changeNotify;

        public void Binding(Action<string> callback)
        {
            changeNotify = callback;
        }

        [NonSerialized]
        [ShowInInspector]
        [LabelText("贴图")]
        [OnValueChanged("OnTextureChanged")]
        public Texture2D Texture2D;

        private void OnTextureChanged()
        {
            TextureId = Texture2D.name;
            changeNotify?.Invoke(Texture2D.name);
        }
#endif
    }
}