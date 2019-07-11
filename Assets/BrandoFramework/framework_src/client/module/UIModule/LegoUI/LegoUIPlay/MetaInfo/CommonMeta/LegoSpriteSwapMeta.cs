#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com

#endregion

using UnityEngine.UI;

namespace Client.LegoUI
{
    [System.Serializable]
    public class LegoSpriteSwapMeta
    {
        /// <summary>
        /// 所有精灵属性项都为空。
        /// 不进行反序列化操作。
        /// </summary>
        public bool NoSprite;

        public string TargetGraphic;
        public string HighlightedSprite;
        public string PressedSprite;
        public string DisabledSprite;

        public static LegoSpriteSwapMeta create(Selectable selectable)
        {
            var meta = new LegoSpriteSwapMeta();
            var spriteState = selectable.spriteState;
            if (selectable.targetGraphic == null && spriteState.highlightedSprite == null
                                                 && spriteState.pressedSprite == null &&
                                                 spriteState.disabledSprite == null
            )
            {
                meta.NoSprite = true;
            }
            else
            {
                meta.TargetGraphic = selectable.targetGraphic == null ? null : selectable.targetGraphic.name;
                meta.HighlightedSprite =
                    spriteState.highlightedSprite == null ? null : spriteState.highlightedSprite.name;
                meta.PressedSprite = spriteState.pressedSprite == null ? null : spriteState.pressedSprite.name;
                meta.DisabledSprite = spriteState.pressedSprite == null ? null : spriteState.pressedSprite.name;
            }

            return meta;
        }
    }
}