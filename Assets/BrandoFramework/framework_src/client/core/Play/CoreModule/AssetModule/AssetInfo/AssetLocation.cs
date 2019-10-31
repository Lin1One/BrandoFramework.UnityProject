
#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 19:58:56
// Email:                 836045613@qq.com

#endregion

using System;

namespace Client.Core
{
    /// <summary>
    /// 资源位置。
    /// 决定资源采用何种加载方式加载。
    /// 以资源的相对目录路径作为判断依据，优先级从上往下递增。
    /// 即越靠后的资源位置会覆盖之前的资源位置。
    /// 例如，一个音乐资源 music_mian,同时在项目的Resources和HotUpdate下存在，则资源位置为AssetBundle。
    /// 1. Resources。
    /// 2. StreamingAssets
    /// 3. HotUpdate
    /// </summary>
    [Serializable]
    public enum AssetLocation : byte
    {
        /// <summary>
        /// 无需热更新也无需动态加载的基础资源。
        /// </summary>
        Resources,

        /// <summary>
        /// 位于应用根目录下的StreamingAssets目录。
        /// 用于存放无需热更新但需要动态加载的资源。
        /// </summary>
        StreamingAssets,

        /// <summary>
        /// 沙盒热更新AssetBundle目录
        /// 在编辑器下每个应用都会在Unity项目的Assets/HotUpdate/目录下有一个单独的子目录
        /// 应用需要热更新的AssetBundle文件约定存放于此目录。
        /// 该目录用于模拟远程Http服务器，运行时会将资源从该目录下载到沙盒下
        /// 最终从沙盒下同路径加载资源
        /// </summary>
        HotUpdate,
        
        /// <summary>
        /// 分包资源目录
        /// 应用根目录下的 AssetPackage 目录
        /// </summary>
        SubPackage,

        /// <summary>
        /// 其他未知位置
        /// </summary>
        Other
    }
}

