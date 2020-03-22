#region Head

// Author:            Yu
// CreateDate:        2018/8/28 10:43:06
// Email:             836045613@qq.com

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
    public interface IYuLegoToggleRxModel : IYuLegoControlRxModel, IYuLegoInteractableRxModel
    {
        ILegoRxStruct<bool> IsOn { get; }
        ILegoRxStruct<string> BackgroundSpriteId { get; }
        ILegoRxStruct<string> CheckmarkSpriteId { get; }
        ILegoRxStruct<string> TextContent { get; }
    }

    public class YuLegoToggleRxModel : YuAbsLegoInteractableRxModel, IYuLegoToggleRxModel
    {
        public ILegoRxStruct<bool> IsOn { get; } = YuLegoRxStrcutPool.GetRxBool();
        public ILegoRxStruct<string> BackgroundSpriteId { get; } = YuLegoRxStrcutPool.GetRxStr();
        public ILegoRxStruct<string> CheckmarkSpriteId { get; } = YuLegoRxStrcutPool.GetRxStr();
        public ILegoRxStruct<string> TextContent { get; } = YuLegoRxStrcutPool.GetRxStr();

        public void Release()
        {
            YuLegoRxStrcutPool.RestoreRxBool(IsOn);
            YuLegoRxStrcutPool.RestoreRxStr(BackgroundSpriteId);
            YuLegoRxStrcutPool.RestoreRxStr(CheckmarkSpriteId);
            YuLegoRxStrcutPool.RestoreRxStr(TextContent);
        }

        public void SetGoActive(bool state)
        {
            IsOn.SetGoActive(state);
            BackgroundSpriteId.SetGoActive(state);
            CheckmarkSpriteId.SetGoActive(state);
            TextContent.SetGoActive(state);
        }

        public void Copy(object target)
        {
            IYuLegoToggleRxModel rx = (IYuLegoToggleRxModel)target;
            IsOn.Value = rx.IsOn.Value;
            BackgroundSpriteId.Value = rx.BackgroundSpriteId.Value;
            CheckmarkSpriteId.Value = rx.CheckmarkSpriteId.Value;
            TextContent.Value = rx.TextContent.Value;
        }

        public void InitFromSerializeField()
        {
            IsOn.Value = isOn;
            BackgroundSpriteId.Value = backgroundSpriteId.SpriteId;

#if DEBUG
            // 绑定内部可序列化字段的响应事件
            backgroundSpriteId.Binding(s => BackgroundSpriteId.Value = s);
#endif
        }

        #region 可序列化字段

        [SerializeField]
#if DEBUG
        [ShowInInspector]
        [LabelText("开关选择状态")]
        [OnValueChanged("PushToIsOn")]
#endif
        private bool isOn;

#if DEBUG
        private void PushToIsOn()
        {
            IsOn.Value = isOn;
        }
#endif

        [SerializeField]
#if DEBUG
        [ShowInInspector]
        [LabelText("背景图片精灵资源Id")]
#endif
        private LegoSpriteString backgroundSpriteId;

        #endregion
    }
}