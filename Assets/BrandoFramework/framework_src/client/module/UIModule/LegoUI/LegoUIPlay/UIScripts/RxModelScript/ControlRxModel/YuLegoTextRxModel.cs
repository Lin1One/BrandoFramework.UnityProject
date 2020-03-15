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
    public interface IYuLegoTextRxModel : IYuLegoControlRxModel
    {
        ILegoRxStruct<string> Content { get; }
        ILegoRxStruct<string> ColorStr { get; }
    }

    [System.Serializable]
    public class YuLegoTextRxModel : IYuLegoTextRxModel
    {
        public ILegoRxStruct<string> Content { get; } = YuLegoRxStrcutPool.GetRxStr();

        /// <summary>
        /// 文本的颜色属性字符串（十六进制字符串）。
        /// </summary>
        public ILegoRxStruct<string> ColorStr { get; } = YuLegoRxStrcutPool.GetRxStr();

        public void Release()
        {
            YuLegoRxStrcutPool.RestoreRxStr(Content);
            YuLegoRxStrcutPool.RestoreRxStr(ColorStr);
        }

        public void SetGoActive(bool state)
        {
            Content.SetGoActive(state);
        }

        public void Copy(object target)
        {
            IYuLegoTextRxModel rx = (IYuLegoTextRxModel)target;
            Content.Value = rx.Content.Value;
            ColorStr.Value = rx.ColorStr.Value;
        }

        public void InitFromSerializeField()
        {
////#if DEBUG
////            if (content == null)
////            {
////                content = new YuIl8nString();
////            }

////#endif

////            Content.Value = content.CurrentContent;

////#if DEBUG
////            // 绑定内部可序列化字段的响应事件
////            content.Binding(s => Content.Value = s);
////#endif
        }

        public ILegoControl LocControl { get; set; }

        #region 可序列化字段     

////        [SerializeField]
////#if DEBUG
////        [ShowInInspector]
////        [LabelText("文本内容")]
////#endif
////        ///private YuIl8nString content = new YuIl8nString();

        #endregion
    }
}