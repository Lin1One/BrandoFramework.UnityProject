#region Head

// Author:            Yu
// CreateDate:        2018/8/21 19:43:32
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using UnityEngine.UI;

namespace Client.LegoUI
{
    [System.Serializable]
    public class LegoColorTintMeta
    {
        public LegoColorMeta NormalLegoColor;
        public LegoColorMeta HighlightedLegoColor;
        public LegoColorMeta PressedLegoColor;
        public LegoColorMeta DisabledLegoColor;
        public float ColorMultiplier;
        public float FadeDuration;

        public static LegoColorTintMeta Create(Selectable selectable)
        {
            var meta = new LegoColorTintMeta();
            var colorBlock = selectable.colors;
            meta.NormalLegoColor = LegoColorMeta.Create(colorBlock.normalColor);
            meta.HighlightedLegoColor = LegoColorMeta.Create(colorBlock.highlightedColor);
            meta.PressedLegoColor = LegoColorMeta.Create(colorBlock.pressedColor);
            meta.DisabledLegoColor = LegoColorMeta.Create(colorBlock.disabledColor);
            meta.ColorMultiplier = colorBlock.colorMultiplier;
            meta.FadeDuration = colorBlock.fadeDuration;

            return meta;
        }
    }
}