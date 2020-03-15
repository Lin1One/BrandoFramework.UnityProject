#region Head

// Author:            Yu
// CreateDate:        2018/9/29 21:47:35
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
    public interface IYuLegoSliderRxModel : IYuLegoControlRxModel
    {
        ILegoRxStruct<float> Progress { get; }
        ILegoRxStruct<byte> Direction { get; }
        ILegoRxStruct<float> MinValue { get; }
        ILegoRxStruct<float> MaxValue { get; }
        ILegoRxStruct<bool> IsWholeNumbers { get; }
    }

    public class YuLegoSliderRxModel : YuAbsLegoInteractableRxModel, IYuLegoSliderRxModel
    {
        public ILegoRxStruct<float> Progress { get; } = YuLegoRxStrcutPool.GetRxFloat();
        public ILegoRxStruct<byte> Direction { get; } = YuLegoRxStrcutPool.GetRxByte();
        public ILegoRxStruct<float> MinValue { get; }  = YuLegoRxStrcutPool.GetRxFloat();
        public ILegoRxStruct<float> MaxValue { get; } = YuLegoRxStrcutPool.GetRxFloat();
        public ILegoRxStruct<bool> IsWholeNumbers { get; } = YuLegoRxStrcutPool.GetRxBool();

        public void Release()
        {
            YuLegoRxStrcutPool.RestoreRxFloat(Progress);
            YuLegoRxStrcutPool.RestoreRxBool(IsWholeNumbers);
            YuLegoRxStrcutPool.RestoreRxByte(Direction);
            YuLegoRxStrcutPool.RestoreRxFloat(MinValue);
            YuLegoRxStrcutPool.RestoreRxFloat(MaxValue);
        }

        public void SetGoActive(bool state)
        {
            Progress.SetGoActive(state);
        }

        public void Copy(object target)
        {
            IYuLegoSliderRxModel rx = (IYuLegoSliderRxModel)target;
            Progress.Value = rx.Progress.Value;
        }

        public void InitFromSerializeField()
        {
            Progress.Value = progress;
        }

        #region 可序列化字段

        [SerializeField]
#if DEBUG
        [ShowInInspector]
        [LabelText("滑动器当前值")]
        [Range(0, 1)]
        [OnValueChanged("PushToProgress")]
#endif

        private float progress;

#if DEBUG
        private void PushToProgress()
        {
            Progress.Value = progress;
        }

#endif

        #endregion
    }
}