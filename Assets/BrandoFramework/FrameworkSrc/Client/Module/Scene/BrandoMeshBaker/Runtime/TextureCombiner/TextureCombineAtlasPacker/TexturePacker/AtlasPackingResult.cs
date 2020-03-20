using System;
using UnityEngine;

namespace GameWorld
{
    /// <summary>
    /// Atlas 排版结果
    /// </summary>
    [Serializable]
    public class AtlasPackingResult
    {
        public int atlasX;
        public int atlasY;
        public int usedW;
        public int usedH;
        public Rect[] rects;
        public AtlasPadding[] padding;
        public int[] srcImgIdxs;
        public object data;

        public AtlasPackingResult(AtlasPadding[] pds)
        {
            padding = pds;
        }

        public void CalcUsedWidthAndHeight()
        {
            Debug.Assert(rects != null);
            float maxW = 0;
            float maxH = 0;
            float paddingX = 0;
            float paddingY = 0;
            for (int i = 0; i < rects.Length; i++)
            {
                paddingX += padding[i].leftRight * 2f;
                paddingY += padding[i].topBottom * 2f;
                maxW = Mathf.Max(maxW, rects[i].x + rects[i].width);
                maxH = Mathf.Max(maxH, rects[i].y + rects[i].height);
            }
            usedW = Mathf.CeilToInt(maxW * atlasX + paddingX);
            usedH = Mathf.CeilToInt(maxH * atlasY + paddingY);
            if (usedW > atlasX) usedW = atlasX;
            if (usedH > atlasY) usedH = atlasY;
        }

        public override string ToString()
        {
            return string.Format("numRects: {0}, atlasX: {1} atlasY: {2} usedW: {3} usedH: {4}", rects.Length, atlasX, atlasY, usedW, usedH);
        }
    }
}
