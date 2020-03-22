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
    public interface IYuLegoButtonRxModel : 
        IYuLegoControlRxModel,
        IYuLegoInteractableRxModel,
        IYuLegoChangeActivableRxModel
    {
        /// <summary>
        /// 按钮自身上图片控件的精灵资源Id。
        /// </summary>
        ILegoRxStruct<string> BgSpriteId { get; set; }

        /// <summary>
        /// 按钮下文本控件的内容字符串。
        /// </summary>
        ILegoRxStruct<string> TextContent { get; set; }
    }

    /// <summary>
    /// 乐高UI按钮的响应式数据模型。
    /// </summary>
    [System.Serializable]
    public class YuLegoButtonRxModel : YuAbsLegoInteractableRxModel, IYuLegoButtonRxModel
    {
        public ILegoRxStruct<string> BgSpriteId { get; set; }
            = YuLegoRxStrcutPool.GetRxStr();

        public ILegoRxStruct<string> TextContent { get; set; }
            = YuLegoRxStrcutPool.GetRxStr();

        private bool isControlActive = true;
        public bool IsControlActive
        {
            get
            {
                return isControlActive;
            }
            set
            {
                isControlActive = value;
            }
        }

        public void SetGoActive(bool state)
        {
            LocControl?.GameObject.SetActive(state);
        }

        public void Copy(object target)
        {
            var buttonRx = (IYuLegoButtonRxModel)target;
            BgSpriteId.Value = buttonRx.BgSpriteId.Value;
            TextContent.Value = buttonRx.TextContent.Value;
        }

        public virtual void Release()
        {
            YuLegoRxStrcutPool.RestoreRxStr(BgSpriteId);
            YuLegoRxStrcutPool.RestoreRxStr(TextContent);
        }

        public virtual void InitFromSerializeField()
        {
            ////if (buttonContent != null && !string.IsNullOrEmpty(buttonContent.CurrentContent))
            ////{
            ////    TextContent.Value = buttonContent.CurrentContent;
            ////}

            if (buttonSprite != null && !string.IsNullOrEmpty(buttonSprite.SpriteId))
            {
                BgSpriteId.Value = buttonSprite.SpriteId;
            }

#if DEBUG
            // 绑定内部可序列化字段的响应事件
            //buttonContent.Binding(ReceiveContent);
            buttonSprite.Binding(ReceiveBgSprite);
#endif
        }

        private void ReceiveContent(string content)
        {
            TextContent.Value = content;
        }

        private void ReceiveBgSprite(string spriteId)
        {
            BgSpriteId.Value = spriteId;
        }

        #region 可序列化字段     

        //[SerializeField]
        //[ShowInInspector]
        //[LabelText("按钮文本内容")]
        //private YuIl8nString buttonContent = new YuIl8nString();

        [SerializeField]
        [ShowInInspector]
        [LabelText("按钮背景图片")]
        private LegoSpriteString buttonSprite = new LegoSpriteString();
        #endregion
    }
}