#region Head

// Author:            Yu
// CreateDate:        2018/10/4 21:59:16
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
    public interface IYuLegoDropdownRxModel : IYuLegoControlRxModel
    {
        /// <summary>
        /// 记录下拉框当前所选择选项的索引值。
        /// </summary>
        ILegoRxStruct<int> SelectIndex { get; }
    }

    [System.Serializable]
    public class YuLegoDropdownRxModel : YuAbsLegoInteractableRxModel, IYuLegoDropdownRxModel
    {
        /// <summary>
        /// 记录下拉框当前所选择选项的索引值。
        /// </summary>
        public ILegoRxStruct<int> SelectIndex { get; } = YuLegoRxStrcutPool.GetRxInt();

        public void Release()
        {
            YuLegoRxStrcutPool.RestoreRxInt(SelectIndex);
        }

        public void SetGoActive(bool state)
        {
            SelectIndex.SetGoActive(state);
        }

        public void Copy(object target)
        {
            IYuLegoDropdownRxModel dropdownRx = (IYuLegoDropdownRxModel)target;
            SelectIndex.Value = dropdownRx.SelectIndex.Value;
        }

        public void InitFromSerializeField()
        {
            SelectIndex.Value = selectIndex;
        }

        #region 可序列化字段

        [SerializeField]
#if DEBUG
        [ShowInInspector]
        [LabelText("下拉框当前选项索引")]
        [OnValueChanged("PushToSelectIndex")]
#endif
        private int selectIndex;

#if DEBUG
        private void PushToSelectIndex()
        {
            SelectIndex.Value = selectIndex;
        }

#endif
        #endregion
    }
}