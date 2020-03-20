using System.Collections.Generic;
using Common.PrefsData;
using Sirenix.OdinInspector;
using YuU3dPlay;

namespace Client.LegoUI
{
    /// <summary>
    /// 应用图片资源使用情况记录及配置。
    /// </summary>
    public class YuLegoGlobalPicRefSetting : YuAbsSingleSetting<YuU3dAppLegoUISetting>
    {
        [LabelText("所有应用图片资源使用数据 ")] public List<YuLegoAppPicRefSetting> AppPicRefs;

        public YuLegoAppPicRefSetting GetAppPicRef(string appId)
        {
            var appRef = AppPicRefs.Find(i => i.AppId == appId);
            return appRef;
        }
    }

    [System.Serializable]
    public class YuLegoAppPicRefSetting
    {
        [LabelText("应用Id")] public string AppId;

        [LabelText("使用中的图片资源Id列表")] public List<string> UsePicIds;
    }
}