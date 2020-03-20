#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/1 11:44:51
// Email:             836045613@qq.com


#endregion

using System;

namespace Client.Assets.Editor
{
    [Serializable]
    public enum AssetBundleBuildType
    {
        /// <summary>
        /// 目录下的每个资源单独打包。
        /// </summary>
        EveryBuild,

        /// <summary>
        /// 目录下的所有资源打成一个包。
        /// 不包含所有子目录。
        /// </summary>
        BuildAtDirSelf,

        /// <summary>
        /// 目录及所有子目录下的所有资源打成一个包。
        /// 使用该方式打包前会自动清理掉所有子目录可能存在的打包配置。
        /// </summary>
        BuildAtDirTree,

        /// <summary>
        /// 目录下的资源依据给定的包大小打包。
        /// 解释：将目录下的资源按照默认的文件排序，依次取出放入当前的bunle包中，
        /// 如果包大小未超出则继续去除文件放入，若超出则中止放入该文件。
        /// 将当前包中的文件修改为统一的bundle名。
        /// 进行下一个bundle包设置。
        /// </summary>
        BuildAtSize,

        /// <summary>
        /// 基于包文件配置列表打包。
        /// </summary>
        BuildAtList,
    }
}