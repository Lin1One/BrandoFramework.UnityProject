#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com

#endregion

using UnityEngine.UI;

namespace Client.LegoUI
{
    [System.Serializable]
    public class UvRect
    {
        public float X;
        public float Y;
        public float W;
        public float H;

        public void Init(RawImage image)
        {
            X = image.uvRect.x;
            Y = image.uvRect.y;
            W = image.uvRect.width;
            H = image.uvRect.height;
        }
    }

    [System.Serializable]
    public class LegoRawImageMeta
    {
        public string TextureId;
        public LegoColorMeta LegoColor;
        public string Material;
        public bool RaycastTarget;

        public static LegoRawImageMeta Create(YuLegoRawImage rawImage)
        {
            var meta = new LegoRawImageMeta
            {
                TextureId = rawImage.texture == null ? null : rawImage.texture.name,
                Material = rawImage.material == null ? null : rawImage.material.name,
                RaycastTarget = rawImage.raycastTarget,
                LegoColor = LegoColorMeta.Create(rawImage.color)
            };

            return meta;
        }
    }
}