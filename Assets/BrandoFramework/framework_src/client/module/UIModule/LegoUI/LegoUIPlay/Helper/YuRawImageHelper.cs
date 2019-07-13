#if UNITY_EDITOR

#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using Common.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Client.LegoUI
{
    public class YuRawImageHelper : YuLegoComponentHelper
    {
        [LabelText("动态贴图资源Id")] public string rawImageSrc;

        ////private YuU3dAppSetting LocU3DApp => YuU3dAppSettingDati.CurrentActual;

        private void OnEnable()
        {
            var rawImage = GetComponent<YuLegoRawImage>();
            if (rawImage.mainTexture.name != "UnityWhite")
            {
                return;
            }

            var jpgFullPath = /*LocU3DApp.Helper.OriginRawImageDir + rawImageSrc+*/ ".jpg";
            var jpgAssetsPath = YuUnityIOUtility.GetAssetsPath(jpgFullPath);
            var texture2D = AssetDatabaseUtility.LoadAssetAtPath<Texture2D>(jpgAssetsPath);
            if (texture2D == null)
            {
                var pngFullPath = /*LocU3DApp.Helper.OriginRawImageDir + rawImageSrc + */".png";
                var pngAssetsPath = YuUnityIOUtility.GetAssetsPath(pngFullPath);
                texture2D = AssetDatabaseUtility.LoadAssetAtPath<Texture2D>(pngAssetsPath);
            }

            rawImage.texture = texture2D;
        }
    }
}

#endif