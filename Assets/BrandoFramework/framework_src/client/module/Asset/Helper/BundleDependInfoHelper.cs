 

using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;


#region Head

// Author:                liuruoyu1981
// CreateDate:            2019/5/16 13:55:20
// Email:                 liuruoyu1981@gmail.com

#endregion


namespace Client.Assets
{
    //    [YuSingleton]
    //    [Serializable]
    //    [DefaultInjecType(typeof(IBundleDependInfoHelper))]
    //#if UNITY_EDITOR
    public class BundleDependInfoHelper : MonoBehaviour, IBundleDependInfoHelper
    //#else
    //public class BundleDependInfoHelper : IBundleDependInfoHelper
    //#endif
    {
        //        #region 可视化支持

        //#if UNITY_EDITOR

        //        [HideLabel] public AppBundleDependInfo VisualDependInfo;

        //        private void HiddenConstruct(object injector)
        //        {
        //            Task.Run(() =>
        //            {
        //                InitDependInfo();
        //                AppDependInfo.InitVisualSupport();
        //                VisualDependInfo = AppDependInfo;
        //            });
        //        }

        //#endif

        //        #endregion

        //        #region 基础

        //        //[YuInject] private readonly BundlePathHelper _pathHelper;

        //        private AppBundleDependInfo _appDependInfo;

        //        private AppBundleDependInfo AppDependInfo
        //        {
        //            get
        //            {
        //                if (_appDependInfo != null)
        //                {
        //                    return _appDependInfo;
        //                }

        //                InitDependInfo();
        //                return _appDependInfo;
        //            }
        //        }

        //        private void InitDependInfo()
        //        {
        //            var path = _pathHelper.GetAppBundleDependInfoPath();
        //            _appDependInfo = YuSerializeUtility.Get<AppBundleDependInfo>(path);
        //        }

        //        public BundleDependInfo GetDependInfo(string bundleId) => AppDependInfo.GetDependInfo(bundleId);

        //        #endregion

    }
}
