#region Head

// Author:            Yu
// CreateDate:        2018/10/13 18:53:47
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
    public interface IYuLegoPlaneToggleRxModel : IYuLegoControlRxModel
    {
        ILegoRxStruct<bool> IsOn { get; }
        ILegoRxStruct<string> FrontSpriteId { get; }
        ILegoRxStruct<string> BackgroundSpriteId { get; }
    }

    [System.Serializable]
    public class YuLegoPlaneToggleRxModel : YuAbsLegoInteractableRxModel, IYuLegoPlaneToggleRxModel
    {
        public ILegoRxStruct<bool> IsOn { get; } = YuLegoRxStrcutPool.GetRxBool();
        public ILegoRxStruct<string> FrontSpriteId { get; } = YuLegoRxStrcutPool.GetRxStr();
        public ILegoRxStruct<string> BackgroundSpriteId { get; } = YuLegoRxStrcutPool.GetRxStr();

        public void Release()
        {
            YuLegoRxStrcutPool.RestoreRxBool(IsOn);
            YuLegoRxStrcutPool.RestoreRxStr(FrontSpriteId);
            YuLegoRxStrcutPool.RestoreRxStr(BackgroundSpriteId);
        }

        public void SetGoActive(bool state)
        {
            IsOn.SetGoActive(state);
            FrontSpriteId.SetGoActive(state);
            BackgroundSpriteId.SetGoActive(state);
        }

        public void Copy(object target)
        {
            IYuLegoPlaneToggleRxModel toggleRx = (IYuLegoPlaneToggleRxModel)target;
            IsOn.Value = toggleRx.IsOn.Value;
            FrontSpriteId.Value = toggleRx.FrontSpriteId.Value;
        }

        public void InitFromSerializeField()
        {
            IsOn.Value = isOn;
            FrontSpriteId.Value = backgroundSpriteId.SpriteId;

#if DEBUG
            // 绑定内部可序列化字段的响应事件
            backgroundSpriteId.Binding(s => FrontSpriteId.Value = s);
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
        [ShowInInspector]
        [LabelText("背景图片精灵资源Id")]
        private LegoSpriteString backgroundSpriteId;

        #endregion
    }
}