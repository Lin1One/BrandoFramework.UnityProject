#region Head

// Author:            Yu
// CreateDate:        2018/8/28 10:43:06
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Sirenix.OdinInspector;
using UnityEngine;
using YuU3dPlay;

namespace Client.LegoUI
{
    public interface IYuLegoRawImageRxModel : IYuLegoControlRxModel
    {
        ILegoRxStruct<string> TextureId { get; }
    }

    [System.Serializable]
    public class YuLegoRawImageRxModel : IYuLegoRawImageRxModel
    {
        public ILegoRxStruct<string> TextureId { get; } = YuLegoRxStrcutPool.GetRxStr();

        public void Release()
        {
            YuLegoRxStrcutPool.RestoreRxStr(TextureId);
        }

        public void SetGoActive(bool state)
        {
            TextureId.SetGoActive(state);
        }

        public void Copy(object target)
        {
            IYuLegoRawImageRxModel rx = (IYuLegoRawImageRxModel)target;
            TextureId.Value = rx.TextureId.Value;
        }

        public void InitFromSerializeField()
        {
            if (textureId != null && !string.IsNullOrEmpty(textureId.TextureId))
            {
                TextureId.Value = textureId.TextureId;
            }

#if DEBUG
            // 绑定内部可序列化字段的响应事件
            textureId.Binding(s => TextureId.Value = s);
#endif
        }

        public ILegoControl LocControl { get; set; }

        #region 可序列化字段

        [SerializeField]
#if DEBUG
        [ShowInInspector]
        [LabelText("图片精灵资源Id")]
#endif
        private LegoTextureString textureId = new LegoTextureString();

        #endregion
    }
}