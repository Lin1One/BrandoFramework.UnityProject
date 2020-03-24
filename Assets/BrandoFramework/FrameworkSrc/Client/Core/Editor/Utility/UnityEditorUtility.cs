using Client.Utility;
using Common;
using Common.Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Client.Core.Editor
{
    /// <summary>
    /// 编辑器常用工具。
    /// </summary>
    public static class UnityEditorUtility
    {
        #region 路径

        /// <summary>
        /// 获得当前编辑器摸下所选择的路径列表。
        /// </summary>
        /// <returns>The select paths.</returns>
        public static List<string> GetSelectPaths()
        {
            var guids = Selection.assetGUIDs;
            var paths = guids.Select(AssetDatabase.GUIDToAssetPath).ToList();
            return paths;
        }

        /// <summary>
        /// 获取当前选中文件夹
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSelectDirs()
        {
            var guids = Selection.assetGUIDs;
            var paths = guids.Select(AssetDatabase.GUIDToAssetPath).ToList();
            var dirs = new List<string>();
            foreach (var path in paths)
            {
                if (!Directory.Exists(path))
                {
                    continue;
                }

                var dir = path.EnsureDirEnd();
                dirs.Add(dir);
            }
            return dirs;
        }

        /// <summary>
        /// 获取当前选中资源 Assets 路径
        /// </summary>
        /// <returns></returns>
        private static string GetSelectFileAssetsPath()
        {
            var go = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(go);
            return path;
        }

        /// <summary>
        /// 获取当前选中资源完整路径
        /// </summary>
        /// <returns></returns>
        public static string GetSelectFileFullPath()
        {
            return UnityIOUtility.GetFullPath(GetSelectFileAssetsPath());
        }

        /// <summary>
        /// 尝试获取当前在编辑器下选择的唯一的目录。
        /// 如果同时选中了多个则会返回空。
        /// </summary>
        /// <returns>The single select dir.</returns>
        public static string GetSelectDir()
        {
            var paths = GetSelectPaths();
            if (paths.Count > 1)
            {
                EditorUtility.DisplayDialog("错误", "不能同时选中多个目录执行该操作！", "知道了");
                return null;
            }

            if (paths.Count == 0)
            {
                EditorUtility.DisplayDialog("错误", "没有选中任何目录！", "知道了");
                return null;
            }

            var dirPath = paths.First().EnsureDirEnd();
            return dirPath;
        }

        #endregion

        #region Json


        #endregion

        #region 可序列化脚本

        /// <summary>
        /// 将可序列化脚本对象创建为资产文件。
        /// 如果目标路径上已有资产文件则将会删除。
        /// 该方法会确保目标路径存在。
        /// </summary>
        /// <param name="scriptObject"></param>
        /// <param name="path"></param>
        public static void CreateScriptAsset(Object scriptObject, string path)
        {
            IOUtility.EnsureDirExist(path);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            AssetDatabase.CreateAsset(scriptObject, path);
        }

        /// <summary>
        /// 将可序列化脚本对象创建为资产文件。
        /// 如果目标路径上已有资产文件则将会删除。
        /// 该方法会确保目标路径存在。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        public static void CreateScriptAsset(object obj, string path)
        {
            CreateScriptAsset((Object)obj, path);
        }

        #endregion

        #region 应用

        public static string GetLocAppIdAtSelectDir()
        {
            var firstDir = GetSelectDirs().First();
            firstDir = Application.dataPath.Replace("Assets", "") + firstDir;

            var appSettings = ProjectInfoDati.GetActualInstance();
            var appRootDir = appSettings.ProjectRootDir;
            if (firstDir.StartsWith(appRootDir))
            {
                return appSettings.DevelopProjectName;
            }
            return null;
        }

        /// <summary>
        /// 尝试获取指定目录所在的应用配置。
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static ProjectInfo TryGetLocProjectInfoAtDir(string dir)
        {
            var fullDir = UnityIOUtility.GetFullPath(dir);
            var appSetting = ProjectInfoDati.GetActualInstance();
            var appRootDir = appSetting.ProjectRootDir;
            if (fullDir.StartsWith(appRootDir))
            {
                return appSetting;
            }

            return null;
        }

        #endregion

        #region Unity编辑器API

        public static void RefreshAsset()
        {
            AssetDatabase.Refresh();
        }

        public static void DisplayError(string message)
        {
            EditorUtility.DisplayDialog("错误", message, "OK");
        }


        public static void DisplayTooptx(string message)
        {
            EditorUtility.DisplayDialog("通知", message, "OK");
        }

        #endregion
    }
}