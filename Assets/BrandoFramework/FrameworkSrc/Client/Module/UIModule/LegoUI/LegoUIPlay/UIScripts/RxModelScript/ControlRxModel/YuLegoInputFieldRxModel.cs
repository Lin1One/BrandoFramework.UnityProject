#region Head

// Author:            Yu
// CreateDate:        2018/9/29 22:18:25
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
    public interface IYuLegoInputFieldRxModel : IYuLegoControlRxModel
    {
        ILegoRxStruct<string> BackgroundSpriteId { get; }

        /// <summary>
        /// 输入框提示文本。
        /// </summary>
        ILegoRxStruct<string> HolderContent { get; }

        /// <summary>
        /// 输入框正文。
        /// </summary>
        ILegoRxStruct<string> Content { get; }
    }

    [System.Serializable]
    public class YuLegoInputFieldRxModel : YuAbsLegoInteractableRxModel, IYuLegoInputFieldRxModel
    {
        public ILegoRxStruct<string> BackgroundSpriteId { get; } = YuLegoRxStrcutPool.GetRxStr();

        /// <summary>
        /// 输入框提示文本。
        /// </summary>
        public ILegoRxStruct<string> HolderContent { get; } = YuLegoRxStrcutPool.GetRxStr();

        /// <summary>
        /// 输入框正文。
        /// </summary>
        public ILegoRxStruct<string> Content { get; } = YuLegoRxStrcutPool.GetRxStr();

        public void Release()
        {
            YuLegoRxStrcutPool.RestoreRxStr(BackgroundSpriteId);
            YuLegoRxStrcutPool.RestoreRxStr(HolderContent);
            YuLegoRxStrcutPool.RestoreRxStr(Content);
        }

        public void SetGoActive(bool state)
        {
            BackgroundSpriteId.SetGoActive(state);
            HolderContent.SetGoActive(state);
            Content.SetGoActive(state);
        }

        public void Copy(object target)
        {
            IYuLegoInputFieldRxModel inputRx = (IYuLegoInputFieldRxModel)target;
            BackgroundSpriteId.Value = inputRx.BackgroundSpriteId.Value;
            HolderContent.Value = inputRx.HolderContent.Value;
            Content.Value = inputRx.Content.Value;
        }

        public void InitFromSerializeField()
        {
            BackgroundSpriteId.Value = backgroundSpriteId;
            HolderContent.Value = holderContent;
            Content.Value = content;
        }

        #region 可序列化字段

        [SerializeField]
        [ShowInInspector]
        [LabelText("背景图片精灵资源Id")]
        [OnValueChanged("PushToSpriteId")]
        private string backgroundSpriteId;

        private void PushToSpriteId()
        {
            BackgroundSpriteId.Value = backgroundSpriteId;
        }

        [SerializeField]
        [ShowInInspector]
        [LabelText("占位提示文本内容")]
        private string holderContent;

        [SerializeField]
        [ShowInInspector]
        [LabelText("输入框主体内容")]
        [OnValueChanged("PushToContent")]
        private string content;

        private void PushToContent()
        {
            Content.Value = content;
        }

        #endregion
    }
}