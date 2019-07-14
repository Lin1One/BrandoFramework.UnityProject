using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Util
{
    // 
    public static class NonRenderingLayer
    {
		public static readonly string NonRenderingLayerName = "Non-Rendering";
        public static readonly string TemporaryRenderingLayerName = "Temporary-Rendering";

        const int InValidLayer = -1;
        const int InValidLayerMask = 0;

        static int _NonRenderingLayer = -1;
        static int _TemporaryRenderingLayer = -1;
        static bool isValidLayer_F { get { return !(_NonRenderingLayer < 0 || _NonRenderingLayer >= 32); } }
        static bool isValidLayer_T { get { return !(_TemporaryRenderingLayer < 0 || _TemporaryRenderingLayer >= 32); } }
        public static int F_Mask { get { return isValidLayer_F ? 1 << _NonRenderingLayer : InValidLayerMask; } }
        public static int T_Mask { get { return isValidLayer_T ? 1 << _TemporaryRenderingLayer : InValidLayerMask; } }

        public static int UpdateNonRenderingLayer()
        {
            if (!isValidLayer_F)
                _NonRenderingLayer = LayerMask.NameToLayer(NonRenderingLayerName);

            return isValidLayer_F ? _NonRenderingLayer : InValidLayer;
        }

        public static int UpdateTemporaryRenderingLayer()
        {
            if (!isValidLayer_T)
                _TemporaryRenderingLayer = LayerMask.NameToLayer(TemporaryRenderingLayerName);

            return isValidLayer_T ? _TemporaryRenderingLayer : InValidLayer;
        }

        public static void ApplyMask(Camera camera)
        {
            if (camera != null)
            {
                UpdateNonRenderingLayer();
                if (isValidLayer_F)
                {
                    camera.cullingMask &= ~F_Mask;
                }
                else
                {
                    Debug.LogError("To use this feature, you must be added to the project layer as \"Non - Rendering\". This should not render all cameras. However, physical relationships are possible.");
                }
            }
        }
    }

}