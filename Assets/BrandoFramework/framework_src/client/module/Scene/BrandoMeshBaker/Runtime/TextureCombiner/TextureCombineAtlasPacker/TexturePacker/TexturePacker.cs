using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameWorld
{
    public abstract class TexturePacker
    {
        internal enum NodeType
        {
            Container,
            maxDim,
            regular
        }

        internal class AtalsAreaPixRect
        {
            public int x;
            public int y;
            public int w;
            public int h;

            public AtalsAreaPixRect() { }
            public AtalsAreaPixRect(int xx, int yy, int ww, int hh)
            {
                x = xx;
                y = yy;
                w = ww;
                h = hh;
            }

            public override string ToString()
            {
                return String.Format("x={0},y={1},w={2},h={3}", x, y, w, h);
            }
        }
        
        #region Comparer
        internal class ImgIDComparer : IComparer<ImageAreaInAtlas>
        {
            public int Compare(ImageAreaInAtlas x, ImageAreaInAtlas y)
            {
                if (x.imgId > y.imgId)
                    return 1;
                if (x.imgId == y.imgId)
                    return 0;
                return -1;
            }
        }

        internal class ImageHeightComparer : IComparer<ImageAreaInAtlas>
        {
            public int Compare(ImageAreaInAtlas x, ImageAreaInAtlas y)
            {
                if (x.h > y.h)
                    return -1;
                if (x.h == y.h)
                    return 0;
                return 1;
            }
        }

        internal class ImageWidthComparer : IComparer<ImageAreaInAtlas>
        {
            public int Compare(ImageAreaInAtlas x, ImageAreaInAtlas y)
            {
                if (x.w > y.w)
                    return -1;
                if (x.w == y.w)
                    return 0;
                return 1;
            }
        }

        internal class ImageAreaComparer : IComparer<ImageAreaInAtlas>
        {
            public int Compare(ImageAreaInAtlas x, ImageAreaInAtlas y)
            {
                int ax = x.w * x.h;
                int ay = y.w * y.h;
                if (ax > ay)
                    return -1;
                if (ax == ay)
                    return 0;
                return 1;
            }
        }

        #endregion

        public bool atlasMustBePowerOfTwo = true;

        public static int RoundToNearestPositivePowerOfTwo(int x)
        {
            int p = (int)Mathf.Pow(2, Mathf.RoundToInt(Mathf.Log(x) / Mathf.Log(2)));
            if (p == 0 || p == 1)
                p = 2;
            return p;
        }

        public static int CeilToNearestPowerOfTwo(int x)
        {
            int p = (int)Mathf.Pow(2, Mathf.Ceil(Mathf.Log(x) / Mathf.Log(2)));
            if (p == 0 || p == 1)
                p = 2;
            return p;
        }

        public abstract AtlasPackingResult[] GetRects(List<Vector2> imgWidthHeights, int maxDimensionX, int maxDimensionY, int padding);

        public abstract AtlasPackingResult[] GetRects(List<Vector2> imgWidthHeights, List<AtlasPadding> paddings, int maxDimensionX, int maxDimensionY, bool doMultiAtlas);

        /*
        Packed rects may exceed atlas size and require scaling
        When scaling want pixel perfect fit in atlas. Corners of rects should exactly align with pixel grid
        Padding should be subtracted from pixel perfect rect to create pixel perfect square 
        TODO this doesn't handle each rectangle having different padding
        */
        internal bool ScaleAtlasToFitMaxDim(Vector2 rootWH, List<ImageAreaInAtlas> images, int maxDimensionX, int maxDimensionY, AtlasPadding padding, int minImageSizeX, int minImageSizeY, int masterImageSizeX, int masterImageSizeY, 
            ref int outW, ref int outH, out float padX, out float padY, out int newMinSizeX, out int newMinSizeY)
        {
            newMinSizeX = minImageSizeX;
            newMinSizeY = minImageSizeY;
            bool redoPacking = false;

            // the atlas may be packed larger than the maxDimension. If so then the atlas needs to be scaled down to fit
            padX = (float)padding.leftRight / (float)outW; //padding needs to be pixel perfect in size
            if (rootWH.x > maxDimensionX)
            {
                padX = (float)padding.leftRight / (float)maxDimensionX;
                float scaleFactor = (float)maxDimensionX / (float)rootWH.x;
                Debug.LogWarning("Packing exceeded atlas width shrinking to " + scaleFactor);
                for (int i = 0; i < images.Count; i++)
                {
                    ImageAreaInAtlas im = images[i];
                    if (im.w * scaleFactor < masterImageSizeX)
                    { //check if small images will be rounded too small. If so need to redo packing forcing a larger min size
                        Debug.Log("Small images are being scaled to zero. Will need to redo packing with larger minTexSizeX.");
                        redoPacking = true;
                        newMinSizeX = Mathf.CeilToInt(minImageSizeX / scaleFactor);
                    }
                    int right = (int)((im.x + im.w) * scaleFactor);
                    im.x = (int)(scaleFactor * im.x);
                    im.w = right - im.x;
                }
                outW = maxDimensionX;
            }

            padY = (float)padding.topBottom / (float)outH;
            if (rootWH.y > maxDimensionY)
            {
                //float minSizeY = ((float)minImageSizeY + 1) / maxDimension;
                padY = (float)padding.topBottom / (float)maxDimensionY;
                float scaleFactor = (float)maxDimensionY / (float)rootWH.y;
                Debug.LogWarning("Packing exceeded atlas height shrinking to " + scaleFactor);
                for (int i = 0; i < images.Count; i++)
                {
                    ImageAreaInAtlas im = images[i];
                    if (im.h * scaleFactor < masterImageSizeY)
                    { //check if small images will be rounded too small. If so need to redo packing forcing a larger min size
                        Debug.Log("Small images are being scaled to zero. Will need to redo packing with larger minTexSizeY.");
                        redoPacking = true;
                        newMinSizeY = Mathf.CeilToInt(minImageSizeY / scaleFactor);
                    }
                    int bottom = (int)((im.y + im.h) * scaleFactor);
                    im.y = (int)(scaleFactor * im.y);
                    im.h = bottom - im.y;
                }
                outH = maxDimensionY;
            }
            return redoPacking;
        }

        //normalize atlases so that that rects are 0 to 1
        public void normalizeRects(AtlasPackingResult rr, AtlasPadding padding)
        {
            for (int i = 0; i < rr.rects.Length; i++)
            {
                rr.rects[i].x = (rr.rects[i].x + padding.leftRight) / rr.atlasX;
                rr.rects[i].y = (rr.rects[i].y + padding.topBottom) / rr.atlasY;
                rr.rects[i].width = (rr.rects[i].width - padding.leftRight * 2) / rr.atlasX;
                rr.rects[i].height = (rr.rects[i].height - padding.topBottom * 2) / rr.atlasY;
            }
        }
    }

}