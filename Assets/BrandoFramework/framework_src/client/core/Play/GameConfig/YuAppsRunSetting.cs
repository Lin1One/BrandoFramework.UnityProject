using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable ConvertToConstant.Global
// ReSharper disable UnassignedField.Global

namespace Client
{
    /// <summary>
    /// 应用集合运行时设置。
    /// 该类用于提供所有App公用的基础设置项。
    /// 1. 模块实例名。
    /// 2. 第三方模块账号信息。
    /// 3. 各类模块器地址。
    /// </summary>
    [Serializable]
    public class YuAppsRunSetting
    {
        #region 网络配置


        [FoldoutGroup("网络配置")]
        [LabelText("Socket类型")]
        public YuSocketType SocketType;

        /// <summary>
        /// 心跳发送频率。
        /// </summary>
        [FoldoutGroup("网络配置")]
        [LabelText("心跳发送频率默认为5")]
        public int HeartFrequency = 5;

        #endregion

        #region 资源

        /// <summary>
        /// 是否从AssetBundle中加载资源。
        /// 为真则按照StreamingAssets、沙盒、热更Http模块器的路径获取资源。
        /// 为假则使用反射加载应用模块AssetDatabase目录下的资源。
        /// </summary>
        [FoldoutGroup("资源加载配置")]
        [LabelText("是否从AssetBundle加载资源")]
        public bool IsLoadFromAssetBundle;

        /// <summary>
        /// 热更新模块器地址，默认为本机python SimpleHTTPServer的默认地址。
        /// </summary>
        [FoldoutGroup("资源加载配置")]
        [LabelText("热更新服务器地址")]
        [Tooltip("默认为本机python SimpleHTTPServer的默认地址=>http://127.0.0.1:8000/")]
        public string HotUpdateHttpUrl = "http://127.0.0.1:8000/";

        /// <summary>
        /// AssetBundle同时加载所允许的最大任务数量。
        /// </summary>
        [FoldoutGroup("资源加载配置")]
        [LabelText("AssetBundle同时加载所允许的最大任务数量")]
        public int AssetBundleSameTimeLoadMax = 3;

        /// <summary>
        /// 图集同时加载所允许的最大任务数量。
        /// </summary>
        [FoldoutGroup("资源加载配置")]
        [LabelText("图集同时加载所允许的最大任务数量")]
        public int AtlasSameTimeLoadMax = 3;

        /// <summary>
        /// 资源加载器同时加载所允许的最大任务数量。
        /// </summary>
        [FoldoutGroup("资源加载配置")]
        [LabelText("资源加载器同时加载所允许的最大任务数量")]
        public int LoaderSameTimeLoadMax = 3;

        /// <summary>
        /// 热更资源开关，默认为触发。
        /// </summary>
        [FoldoutGroup("资源加载配置")]
        [LabelText("是否热更资源")]
        public bool IsAssetUpdate = true;

        /// <summary>
        /// 热更资源开关，默认为触发。
        /// </summary>
        [FoldoutGroup("资源加载配置")]
        [LabelText("热更资源从 CDN 测试地址下载")]
        public bool IsAssetUpdateFromTestDir = true;

        /// <summary>
        /// 以分包资源模式运行开关，默认为关。
        /// 资源仅读取项目目录的 AssetPackage 中的分目录
        /// </summary>
        [FoldoutGroup("资源加载配置")]
        [LabelText("以分包资源检查模式运行")]
        public bool IsSubpackage = false;

        [FoldoutGroup("资源加载配置")]
        [LabelText("分包资源忽略文件夹")]
        public List<string> SubpackageIgnoreDir;

        #endregion

    }
    [Serializable]
    public enum YuSocketType : byte
    {
        /// <summary>
        /// 原生Socket。
        /// </summary>
        OriginSocket,

        /// <summary>
        /// WebSocket。
        /// </summary>
        WebSocket
    }


}