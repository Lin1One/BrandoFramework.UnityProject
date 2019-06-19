#if UNITY_EDITOR
using Client.UI.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI
{
    public static class BrandoGraphicRebuildTracker
    {
        static IList<BrandoUIGraphic> m_Tracked = new IndexedSet<BrandoUIGraphic>();
        static bool s_Initialized;

        public static void TrackGraphic(BrandoUIGraphic g)
        {
            if (!s_Initialized)
            {
                CanvasRenderer.onRequestRebuild += OnRebuildRequested;
                s_Initialized = true;
            }

            m_Tracked.Add(g);
        }

        public static void UnTrackGraphic(BrandoUIGraphic g)
        {
            m_Tracked.Remove(g);
        }

        static void OnRebuildRequested()
        {
            StencilMaterial.ClearAll();
            for (int i = 0; i < m_Tracked.Count; i++)
            {
                m_Tracked[i].OnRebuildRequested();
            }
        }
    }
}
#endif // if UNITY_EDITOR
