#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/1 11:44:51
// Email:             836045613@qq.com

#endregion

using Client.Core;
using Common;

namespace Client.Assets.Editor
{
    [Singleton]
    //[DefaultInjecType(typeof(IBundlePathHelper))]
    public class BundleMapFilePathHelper
    {
        private readonly ProjectInfo _appSetting = ProjectInfoDati.GetActualInstance();

        /// <summary>
        /// ��ȡ AssetId To Bundle ӳ����Ϣ�ļ�
        /// </summary>
        /// <returns></returns>
        public string GetAppBundleLoadInfoPath()
        {
            //var path = _appSetting.Helper.FinalSandboxDir + $"{typeof(AppBundleLoadInfo).Name}.bytes";
            //return path;
            return null;
        }

        /// <summary>
        /// ��ȡ Bundle To DependBundle ӳ����Ϣ�ļ�
        /// </summary>
        /// <returns></returns>
        public string GetAppBundleDependInfoPath()
        {
            //var path = _appSetting.Helper.LocalHttpRootDir + $"{typeof(AppBundleDependInfo).Name}.bytes";
            //return path;
            return null;
        }
    }


}