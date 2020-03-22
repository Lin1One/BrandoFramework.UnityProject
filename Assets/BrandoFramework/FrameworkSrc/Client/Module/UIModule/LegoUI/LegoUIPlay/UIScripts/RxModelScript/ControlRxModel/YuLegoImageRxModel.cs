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
    public interface IYuLegoImageRxModel : IYuLegoControlRxModel
    {
        ILegoRxStruct<string> SpriteId { get; set; }
    }

    [System.Serializable]
    public class YuLegoImageRxModel : IYuLegoImageRxModel
    {
        public ILegoRxStruct<string> SpriteId { get; set; } = YuLegoRxStrcutPool.GetRxStr();

        public void Release()
        {
            YuLegoRxStrcutPool.RestoreRxStr(SpriteId);
        }

        public void SetGoActive(bool state)
        {
            SpriteId.SetGoActive(state);
        }

        public void Copy(object target)
        {
            IYuLegoImageRxModel imageRx = (IYuLegoImageRxModel)target;
            SpriteId.Value = imageRx.SpriteId.Value;
        }

        public void InitFromSerializeField()
        {
            if (spriteId == null)
            {
                spriteId = new LegoSpriteString();
            }

            SpriteId.Value = spriteId?.SpriteId;

#if DEBUG
            // 绑定内部可序列化字段的响应事件
            spriteId?.Binding(ReceiveSpriteId);
#endif

        }


        void ReceiveSpriteId(string spId)
        {
            SpriteId.Value = spId;
        }

        public ILegoControl LocControl { get; set; }

        #region 可序列化字段

        [SerializeField]
        [ShowInInspector]
        [LabelText("图片精灵资源Id")]
        private LegoSpriteString spriteId;

        #endregion
    }
}