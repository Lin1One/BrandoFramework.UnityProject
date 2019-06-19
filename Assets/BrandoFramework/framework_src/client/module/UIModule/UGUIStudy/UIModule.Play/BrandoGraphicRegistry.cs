using Client.UI.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client.UI
{
    /// <summary>
    /// Í¼Ïñ×¢²á´¦
    /// </summary>
    public class BrandoUIGraphicRegistry
    {
        private static BrandoUIGraphicRegistry s_Instance;
        public static BrandoUIGraphicRegistry instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new BrandoUIGraphicRegistry();
                return s_Instance;
            }
        }

        private readonly Dictionary<Canvas, IndexedSet<BrandoUIGraphic>> m_Graphics = 
            new Dictionary<Canvas, IndexedSet<BrandoUIGraphic>>();

        protected BrandoUIGraphicRegistry()
        {
            // This is needed for AOT on IOS. Without it the compile doesn't get the definition of the Dictionarys
#pragma warning disable 168
            Dictionary<BrandoUIGraphic, int> emptyGraphicDic;
            Dictionary<ICanvasElement, int> emptyElementDic;
#pragma warning restore 168
        }

        #region ×¢²á£¬×¢Ïú
        public static void RegisterGraphicForCanvas(Canvas c, BrandoUIGraphic graphic)
        {
            if (c == null)
                return;

            IndexedSet<BrandoUIGraphic> graphics;
            instance.m_Graphics.TryGetValue(c, out graphics);

            if (graphics != null)
            {
                graphics.Add(graphic);
                return;
            }

            graphics = new IndexedSet<BrandoUIGraphic>();
            graphics.Add(graphic);
            instance.m_Graphics.Add(c, graphics);
        }

        public static void UnregisterGraphicForCanvas(Canvas c, BrandoUIGraphic graphic)
        {
            if (c == null)
                return;

            IndexedSet<BrandoUIGraphic> graphics;
            if (instance.m_Graphics.TryGetValue(c, out graphics))
            {
                graphics.Remove(graphic);
            }
        }

        private static readonly List<BrandoUIGraphic> s_EmptyList = 
            new List<BrandoUIGraphic>();

        #endregion

        public static IList<BrandoUIGraphic> GetGraphicsForCanvas(Canvas canvas)
        {
            IndexedSet<BrandoUIGraphic> graphics;
            if (instance.m_Graphics.TryGetValue(canvas, out graphics))
                return graphics;

            return s_EmptyList;
        }
    }
}
