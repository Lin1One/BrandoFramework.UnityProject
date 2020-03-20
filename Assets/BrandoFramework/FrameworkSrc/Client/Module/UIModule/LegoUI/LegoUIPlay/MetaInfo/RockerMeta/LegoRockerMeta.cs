
using Sirenix.OdinInspector;

namespace Client.LegoUI
{
    /// <summary>
    /// 摇杆元数据。
    /// </summary>
    [System.Serializable]
    public class LegoRockerMeta
    {
        [LabelText("摇杆背景图片元数据")]
        public LegoImageMeta BgImageMeta;

        [LabelText("摇杆控制杆图片元数据")]
        public LegoImageMeta RockerImageMeta;

        public static LegoRockerMeta Create(YuLegoRocker rocker)
        {
            var meta = new LegoRockerMeta
            {
                BgImageMeta = LegoImageMeta.Create(rocker.BgImage),
                RockerImageMeta = LegoImageMeta.Create(rocker.RockerImage)
            };

            return meta;
        }
    }
}
