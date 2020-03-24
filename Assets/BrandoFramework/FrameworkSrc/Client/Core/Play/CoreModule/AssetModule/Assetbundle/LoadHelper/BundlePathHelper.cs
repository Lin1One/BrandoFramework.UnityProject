using Client.Utility;
using Common;
using UnityEngine;

#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 19:58:56
// Email:                 836045613@qq.com

#endregion


namespace Client.Core
{
    public interface IBundlePathHelper
    {

    }

    [Singleton]
    [DefaultInjecType(typeof(IBundlePathHelper))]
    public class BundlePathHelper : IBundlePathHelper
    {
        //private readonly YuU3dAppSetting _appSetting = U3dGlobal.CurrentApp;

        public string GetAppBundleDependInfoPath()
        {
            if (UnityModeUtility.IsEditorMode)
            {
                var path = /*_appSetting.Helper.LocalHttpRootDir +*/ $"{typeof(ProjectBundleDependInfo).Name}.bytes";
                return path;
            }
            else
            {
                var path = $"{Application.streamingAssetsPath}/AssetBundle/ProjectBundleDependInfo.bytes";
                return path;
            }
        }
    }
}