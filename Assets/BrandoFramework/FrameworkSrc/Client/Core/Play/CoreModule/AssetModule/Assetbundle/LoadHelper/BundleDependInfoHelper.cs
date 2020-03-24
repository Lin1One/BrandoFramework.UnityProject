 

using System;
using System.Threading.Tasks;
using Common;
using Common.Utility;
using Sirenix.OdinInspector;
using UnityEngine;


#region Head

// Author:                LinYuzhou
// CreateDate:            2019/5/16 13:55:20
// Email:                 836045613@qq.com

#endregion


namespace Client.Core
{
    public interface IBundleDependInfoHelper
    {
        BundleDependInfo GetDependInfo(string bundleId);
    }

    [Singleton]
    [Serializable]
    //[DefaultInjecType(typeof(IBundleDependInfoHelper))]
#if UNITY_EDITOR
    public class BundleDependInfoHelper : MonoBehaviour, IBundleDependInfoHelper
#else
    public class BundleDependInfoHelper : IBundleDependInfoHelper
#endif
    {
        #region 可视化支持

#if UNITY_EDITOR

        [HideLabel] public ProjectBundleDependInfo VisualDependInfo;

        private void HiddenConstruct(object injector)
        {
            Task.Run(() =>
            {
                InitDependInfo();
                AppDependInfo.InitVisualSupport();
                VisualDependInfo = AppDependInfo;
            });
        }

#endif

        #endregion

        #region 基础

        //[YuInject]
        private readonly BundlePathHelper _pathHelper;

        private ProjectBundleDependInfo _appDependInfo;

        private ProjectBundleDependInfo AppDependInfo
        {
            get
            {
                if (_appDependInfo != null)
                {
                    return _appDependInfo;
                }

                InitDependInfo();
                return _appDependInfo;
            }
        }

        private void InitDependInfo()
        {
            var path = _pathHelper.GetAppBundleDependInfoPath();
            _appDependInfo = SerializeUtility.DeSerialize<ProjectBundleDependInfo>(path);
        }

        public BundleDependInfo GetDependInfo(string bundleId) => AppDependInfo.GetDependInfo(bundleId);

        public void SetBundleDependInfo(byte[] bytes)
        {
            _appDependInfo = SerializeUtility.DeSerialize<ProjectBundleDependInfo>(bytes);
        }

        #endregion
    }
}
