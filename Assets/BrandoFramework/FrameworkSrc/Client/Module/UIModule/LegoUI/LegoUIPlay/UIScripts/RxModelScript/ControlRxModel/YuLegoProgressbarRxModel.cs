#region Head

// Author:            Yu
// CreateDate:        2018/9/1 16:33:37
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Sirenix.OdinInspector;
using UnityEngine;

namespace Client.LegoUI
{
    public interface IYuLegoProgressbarRxModel : IYuLegoControlRxModel
    {
        ILegoRxStruct<float> Progress { get; set; }
    }

    [System.Serializable]
    public class YuLegoProgressbarRxModel : YuAbsLegoInteractableRxModel, IYuLegoProgressbarRxModel
    {
        public ILegoRxStruct<float> Progress { get; set; } = YuLegoRxStrcutPool.GetRxFloat();

        public void Release()
        {
            YuLegoRxStrcutPool.RestoreRxFloat(Progress);
        }

        public void SetGoActive(bool state)
        {
            Progress.SetGoActive(state);
        }

        public void Copy(object target)
        {
            IYuLegoProgressbarRxModel progressRx = (IYuLegoProgressbarRxModel)target;
            Progress.Value = progressRx.Progress.Value;
        }

        public void InitFromSerializeField()
        {
            Progress.Value = progress;
        }

        #region 可序列化字段

        [SerializeField]
#if DEBUG
        [ShowInInspector]
        [LabelText("进度条当前值")]
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