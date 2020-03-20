using YuU3dPlay;

namespace Client.LegoUI
{
    public interface IYuLegoRockerRxModel : IYuLegoControlRxModel
    {
        ILegoRxStruct<string> BgSpriteId { get; }

        ILegoRxStruct<string> RockerSpriteId { get; }
    }

    [System.Serializable]
    public class YuLegoRockerRxModel : IYuLegoRockerRxModel
    {
        public void Release()
        {
            YuLegoRxStrcutPool.RestoreRxStr(BgSpriteId);
            YuLegoRxStrcutPool.RestoreRxStr(RockerSpriteId);
        }

        public void SetGoActive(bool state) => LocControl.GameObject.SetActive(state);

        public void Copy(object target)
        {
            var rockerRx = (IYuLegoRockerRxModel)target;
            BgSpriteId.Value = rockerRx.BgSpriteId.Value;
            RockerSpriteId.Value = rockerRx.RockerSpriteId.Value;
        }

        public ILegoControl LocControl { get; set; }
        public void InitFromSerializeField()
        {
        }

        public ILegoRxStruct<string> BgSpriteId { get; }
            = YuLegoRxStrcutPool.GetRxStr();
        public ILegoRxStruct<string> RockerSpriteId { get; }
            = YuLegoRxStrcutPool.GetRxStr();
    }
}
