#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/1 11:44:51
// Email:             836045613@qq.com

#endregion

using Client.Core;
using Common;

namespace Client.Assets
{
    public interface IBundlePathHelper
    {
    }

    [Singleton]
    //[DefaultInjecType(typeof(IBundlePathHelper))]
    public class BundlePathHelper : IBundlePathHelper
    {
        public BundlePathHelper(ProjectInfo appSetting) => _appSetting = appSetting;

        public BundlePathHelper()
        {
        }

        private readonly ProjectInfo _appSetting = ProjectInfoDati.GetActualInstance();

        ////public string GetAppBundleLoadInfoPath()
        ////{
        ////    var path = _appSetting.Helper.FinalSandboxDir + $"{typeof(AppBundleLoadInfo).Name}.bytes";
        ////    return path;
        ////}

        ////public string GetAppBundleDependInfoPath()
        ////{
        ////    var path = _appSetting.Helper.LocalHttpRootDir + $"{typeof(AppBundleDependInfo).Name}.bytes";
        ////    return path;
        ////}
    }


}