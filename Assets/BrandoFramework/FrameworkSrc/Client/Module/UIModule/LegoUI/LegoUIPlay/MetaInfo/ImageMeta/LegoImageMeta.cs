#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/11 22:16:32
// Email:             836045613@qq.com

#endregion

namespace Client.LegoUI
{
    [System.Serializable]
    public class LegoImageMeta
    {
        public string SpriteId;
        public LegoColorMeta LegoColor;
        public string Material;
        public bool RaycastTarget;
        public YuLegoImage.Type ImageType;
        public YuLegoImage.FillMethod FillMethod;
        public int FillOriginType;
        public float FillAmount;
        public bool Clockwise;
        public bool PreserveAspect;
        public bool FillCenter;
        public float AlphaHitTestMinimumThreshold;

        public static LegoImageMeta Create(YuLegoImage image)
        {
            var meta = new LegoImageMeta
            {
                SpriteId = image.sprite == null ? null : image.sprite.name,
                LegoColor = LegoColorMeta.Create(image.color),
                Material = image.material == null ? null : image.material.name,
                RaycastTarget = image.raycastTarget,
                ImageType = image.type,
                FillMethod = image.fillMethod,
                FillOriginType = image.fillOrigin,
                FillAmount = image.fillAmount,
                Clockwise = image.fillClockwise,
                PreserveAspect = image.preserveAspect,
                
            };

            return meta;
        }
    }
}