

//namespace Common.DataStruct
//{
//    /// <summary>
//    /// 游戏设置数据模型。
//    /// </summary>
//    [Singleton]
//    [System.Serializable]
//    public class YuGameOptionRxModel : IRelease
//    {
//        public RxFloat MusicVolume = (YuRxFloat) YuRxStrcutPool.GetRxFloat();
//        public RxBool MusicOpenState = (YuRxBool) YuRxStrcutPool.GetRxBool();
//        public YuRxFloat SoundVolume = (YuRxFloat) YuRxStrcutPool.GetRxFloat();
//        public YuRxBool SoundOpenState = (YuRxBool) YuRxStrcutPool.GetRxBool();
//        public YuRxFloat VoiceVolume = (YuRxFloat) YuRxStrcutPool.GetRxFloat();
//        public YuRxBool VoiceOpenState = (YuRxBool) YuRxStrcutPool.GetRxBool();

//        public virtual void Release()
//        {
//            YuRxStrcutPool.RestoreRxFloat(MusicVolume);
//            YuRxStrcutPool.RestoreRxFloat(SoundVolume);
//            YuRxStrcutPool.RestoreRxFloat(VoiceVolume);

//            YuRxStrcutPool.RestoreRxBool(MusicOpenState);
//            YuRxStrcutPool.RestoreRxBool(SoundOpenState);
//            YuRxStrcutPool.RestoreRxBool(VoiceOpenState);
//        }
//    }
//}