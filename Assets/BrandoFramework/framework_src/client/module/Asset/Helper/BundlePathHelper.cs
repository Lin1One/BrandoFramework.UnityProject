#region Head

// Author:                liuruoyu1981
// CreateDate:            2019/5/14 17:49:57
// Email:                 liuruoyu1981@gmail.com

#endregion


using client_common;

namespace Client.Assets
{
    [Singleton]
    //[DefaultInjecType(typeof(IBundlePathHelper))]
    public class BundlePathHelper : IBundlePathHelper
    {
        //private readonly YuU3dAppSetting _appSetting = U3dGlobal.CurrentApp;

        //public string GetAppBundleDependInfoPath()
        //{
            //if (YuUnityUtility.IsEditorMode)
            //{
            //    var path = _appSetting.Helper.AssetBundleBuildDir + $"{typeof(AppBundleDependInfo).Name}.bytes";
            //    return path;
            //}
            //else
            //{
            //    var path = _appSetting.Helper.SandboxHotUpdateDir + $"{typeof(AppBundleDependInfo).Name}.bytes";
            //    return path;
            //}
        //}
    }
}