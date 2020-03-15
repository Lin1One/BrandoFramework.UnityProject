using Common.Setting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Client.Utility
{
    /// <summary>
    /// Unity环境IO工具。
    /// </summary>
    public static class YuUnityIOUtility
    {
        /// <summary>
        /// 传入完整路径计算目标文件相对于当前unity项目Assets目录的相对路径。
        /// 同时也是AssetBundle的Importer路径。
        /// </summary>
        /// <returns>The importer assetsPath.</returns>
        /// <param name="fullPath">Full assetsPath.</param>
        public static string GetAssetsPath(string fullPath)
        {
            if (fullPath.StartsWith("Assets/"))
            {
                return fullPath;
            }

            var path = "Assets" + fullPath.Replace(Application.dataPath,
                           "");
            return path;
        }

        public static string GetFullPath(string path)
        {
            var fullPath = Application.dataPath.Replace("Assets", "")
                           + path;
            return fullPath;
        }

        /// <summary>
        /// 获取给定相对路径（相对于unity的Assets目录）的unity完整路径。
        /// </summary>
        /// <param name="relativePath">相对路径（相对于unity的Assets目录）。</param>
        /// <returns></returns>
        public static string GetUnityFullPath(string relativePath)
        {
            var fullPath = Application.dataPath + "/" + relativePath;
            if (!File.Exists(fullPath) && !Directory.Exists(fullPath))
                throw new Exception("依据传入的相对路径所计算得出的最终路径无法访问，请检查传入路径！");

            return fullPath;
        }

        /// <summary>
        /// 获得当前unity项目的根目录。
        /// </summary>
        /// <returns></returns>
        public static string GetRootPath()
        {
            var assetsPath = Application.dataPath;
            var rootPath = assetsPath.Substring(0, assetsPath.Length - 6);
            return rootPath;
        }

        private static readonly Dictionary<string, Assembly>
            asmCache = new Dictionary<string, Assembly>();

        private static readonly HashSet<string> defaultAsmId
            = new HashSet<string>
            {
                "OdinEditor",
                "OdinPlay",
                "YuEditorAuto",
                "YuEditor",
                "YuPlay",
                "YuCommon",
                "YuLogEditor",
                "YuLogPlay"
            };


        /// <summary>
        /// 获得一个使用unity自定义程序集功能自动生成的程序集文件。
        /// </summary>
        /// <param name="asmId"></param>
        /// <returns></returns>
        public static Assembly GetUnityAssembly(string asmId)
        {
            if (asmCache.ContainsKey(asmId))
            {
                return asmCache[asmId];
            }
            string path;

#if UNITY_EDITOR
            path = Application.dataPath.Replace("Assets", "")
                       + "Library/ScriptAssemblies/" + asmId + ".dll";
            if (!File.Exists(path))
            {
                path = YuSetting.Instance.YuRootFullDir + "YuDLL/Editor/" + asmId + ".dll";
                if (!File.Exists(path))
                {
                    path = YuSetting.Instance.YuRootFullDir + "YuDLL/Play/" + asmId + ".dll";
                }
            }
            var asm = Assembly.LoadFile(path);
            return asm;

#elif UNITY_STANDALONE_WIN
            path = Application.dataPath
                       + "Managed/" + asmId + ".dll";
            Debug.Log("路径为： "+ path);
            var asm = Assembly.LoadFile(path);
            asmCache.Add(asmId, asm);
            return asm;


#elif UNITY_ANDROID
            path = Application.streamingAssetsPath 
                       +"/bin/Data/Managed/" + asmId + ".dll";

            var asm = Assembly.LoadFile(path);
            asmCache.Add(asmId, asm);
            return asm;
#endif
        }

    }
}