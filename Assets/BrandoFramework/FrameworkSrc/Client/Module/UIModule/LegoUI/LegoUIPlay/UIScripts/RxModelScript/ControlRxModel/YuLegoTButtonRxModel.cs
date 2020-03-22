#region Head

// Author:            Yu
// CreateDate:        2018/10/15 7:00:00
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Common;
using Sirenix.OdinInspector;
using UnityEngine;

using YuU3dPlay;

namespace Client.LegoUI
{
    public interface IYuLegoTButtonRxModel : IYuLegoButtonRxModel
    {
        /// <summary>
        /// 按钮下图片控件的精灵资源Id。
        /// </summary>
        ILegoRxStruct<string> IconSpriteId { get; set; }

        void SetIconActive(bool state);

        void SetBgActive(bool state);
    }

    [System.Serializable]
    public class YuLegoTButtonRxModel : YuLegoButtonRxModel, IYuLegoTButtonRxModel
    {
        public ILegoRxStruct<string> IconSpriteId { get; set; }
            = YuLegoRxStrcutPool.GetRxStr();

        public override void Release()
        {
            base.Release();

            YuLegoRxStrcutPool.RestoreRxStr(IconSpriteId);
        }

        public override void InitFromSerializeField()
        {
            base.InitFromSerializeField();

            if (iconSprite != null && !string.IsNullOrEmpty(iconSprite.SpriteId))
            {
                IconSpriteId.Value = iconSprite.SpriteId;
            }

#if DEBUG
            // 绑定内部可序列化字段的响应事件
            iconSprite.Binding(s => IconSpriteId.Value = s);
#endif
        }

        #region 可序列化字段     

        [SerializeField]
#if DEBUG
        [ShowInInspector]
        [LabelText("按钮背景图片")]
#endif
        private LegoSpriteString iconSprite = new LegoSpriteString();

        #endregion

        #region 子控件控制

        public void SetIconActive(bool state) => LocControl.As<YuLegoTButton>().IconImage.GameObject.SetActive(state);

        public void SetBgActive(bool state) => LocControl.As<YuLegoTButton>().BgImage.GameObject.SetActive(state);

        #endregion
    }
}