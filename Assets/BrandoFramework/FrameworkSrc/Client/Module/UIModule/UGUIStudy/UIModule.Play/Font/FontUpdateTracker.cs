
using System.Collections.Generic;
using UnityEngine;

namespace Client.UI
{
    /// <summary>
    /// 字体更新跟踪器
    /// </summary>
    public static class FontUpdateTracker
    {
        static Dictionary<Font, List<BrandoUIText>> m_Tracked = 
            new Dictionary<Font, List<BrandoUIText>>();

        /// <summary>
        /// Font 资源重建调用。FontTexture 有更新
        /// </summary>
        /// <param name="f"></param>
        private static void RebuildForFont(Font f)
        {
            List<BrandoUIText> texts;
            m_Tracked.TryGetValue(f, out texts);
            if (texts == null)
                return;

            for (var i = 0; i < texts.Count; i++)
                texts[i].FontTextureChanged();
        }

        public static void TrackText(BrandoUIText t)
        {
            if (t.font == null)
                return;

            List<BrandoUIText> exists;
            m_Tracked.TryGetValue(t.font, out exists);
            if (exists == null)
            {
                // The textureRebuilt event is global for all fonts, 
                // so we add our delegate the first time we register *any* Text
                if (m_Tracked.Count == 0)
                    Font.textureRebuilt += RebuildForFont;

                exists = new List<BrandoUIText>();
                m_Tracked.Add(t.font, exists);
            }

            if (!exists.Contains(t))
                exists.Add(t);
        }
        public static void UntrackText(BrandoUIText t)
        {
            if (t.font == null)
                return;

            List<BrandoUIText> texts;
            m_Tracked.TryGetValue(t.font, out texts);

            if (texts == null)
                return;

            texts.Remove(t);

            if (texts.Count == 0)
            {
                m_Tracked.Remove(t.font);

                // There is a global textureRebuilt event for all fonts,
                // so once the last Text reference goes away, remove our delegate
                if (m_Tracked.Count == 0)
                    Font.textureRebuilt -= RebuildForFont;
            }
        }
    }
}
