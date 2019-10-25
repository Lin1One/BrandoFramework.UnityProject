//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using System.Threading.Tasks;
//using Sirenix.OdinInspector;
//using UnityEngine;
//
//using YuU3dPlay;
//
//
//#region Head
//
//// Author:                liuruoyu1981
//// CreateDate:            5/16/2019 1:11:28 PM
//// Email:                 liuruoyu1981@gmail.com
//
//#endregion
//
//
//namespace YuU3dPlay
//{
//    [YuSingleton]
//    [Serializable]
//    [DefaultInjecType(typeof(IBundleLoadInfoHelper))]
//#if UNITY_EDITOR
//    public class BundleLoadInfoHelper : MonoBehaviour, IBundleLoadInfoHelper
//#else
//    public class BundleLoadInfoHelper : IBundleLoadInfoHelper
//#endif
//    {
//        [YuInject] private readonly BundlePathHelper _pathHelper;
//
//        #region 可视化支持
//
//        [LabelText("子包加载数据列表")] [ReadOnly] public List<BundleSubLoadInfo> bundleSubLoadInfos;
//
//#if UNITY_EDITOR
//
//        private void HiddenConstruct(object injector)
//        {
//            Task.Run(() =>
//            {
//                InitLoadInfo();
//                InitVisualSupport();
//            });
//        }
//
//        private void InitVisualSupport()
//        {
//            bundleSubLoadInfos = new List<BundleSubLoadInfo>();
//
//            foreach (var kv in _loadInfo.AllSubPackages)
//            {
//                var subLoaInfo = kv.Value;
//                subLoaInfo.InitVisualSupport();
//                bundleSubLoadInfos.Add(subLoaInfo);
//            }
//        }
//
//#endif
//
//        #endregion
//
//        #region 运行时加载数据
//
//        private AppBundleLoadInfo _loadInfo;
//
//        private AppBundleLoadInfo LoadInfo
//        {
//            get
//            {
//                if (_loadInfo != null)
//                {
//                    return _loadInfo;
//                }
//
//                InitLoadInfo();
//
//                return _loadInfo;
//            }
//        }
//
//        private void InitLoadInfo()
//        {
//            var path = _pathHelper.GetAppBundleLoadInfoPath();
//
//            _loadInfo = !File.Exists(path)
//                ? new AppBundleLoadInfo()
//                : YuSerializeUtility.Get<AppBundleLoadInfo>(path);
//        }
//
//        #endregion
//
//        #region ID查询
//
//        public string GetBundleId(string assetId) => LoadInfo.GetBundleId(assetId);
//
//        public string GetAssetSubId(string assetId) => LoadInfo.GetAssetSubId(assetId);
//
//        public string GetBundleSubId(string bundleId) => LoadInfo.GetBundleSubId(bundleId);
//
//        #endregion
//
//        #region 路径查询
//
//        private readonly StringBuilder _sb = new StringBuilder();
//        private YuAppHelper AppPath => _appSetting.Helper;
//
//        private readonly YuU3dAppSetting _appSetting = U3dGlobal.CurrentApp;
//
//        public string GetBundlePath(string bundleId)
//        {
//            var subId = LoadInfo.GetBundleSubId(bundleId);
//            var path = _sb.AppendAndReturn(AppPath.FinalAssetBundleDir, subId, "/", bundleId);
//            if (File.Exists(path))
//            {
//                return path;
//            }
//
//            // 如果沙盒下不存在则尝试读取stream目录，因为有可能是内置的分包内资源。
//            path = _sb.AppendAndReturn(AppPath.FinalStreamingAssetsDir, subId, "/", bundleId);
//            return path;
//        }
//
//        public string DirectGetBundlePath(string assetId)
//        {
//            var bundleId = GetBundleId(assetId);
//            var path = GetBundlePath(bundleId);
//            return path;
//        }
//
//        #endregion
//    }
//}