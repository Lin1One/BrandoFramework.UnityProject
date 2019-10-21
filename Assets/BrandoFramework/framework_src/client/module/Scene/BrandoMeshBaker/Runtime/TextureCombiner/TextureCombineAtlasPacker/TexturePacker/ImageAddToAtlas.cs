using UnityEngine;

namespace GameWorld
{
    public class ImageAreaInAtlas
    {
        public int imgId;
        public int w;
        public int h;
        public int x;
        public int y;

        public ImageAreaInAtlas(int id, int tw, int th, AtlasPadding padding, int minImageSizeX, int minImageSizeY)
        {
            imgId = id;
            w = Mathf.Max(tw + padding.leftRight * 2, minImageSizeX);
            h = Mathf.Max(th + padding.topBottom * 2, minImageSizeY);
        }

        public ImageAreaInAtlas(ImageAreaInAtlas im)
        {
            imgId = im.imgId;
            w = im.w;
            h = im.h;
            x = im.x;
            y = im.y;
        }
    }
}