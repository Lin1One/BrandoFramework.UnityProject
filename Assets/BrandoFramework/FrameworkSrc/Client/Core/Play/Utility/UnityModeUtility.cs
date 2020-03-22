using System.IO;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Client.Utility
{
    /// <summary>
    /// unity工具，提供公用的常用API。
    /// </summary>
    public static class UnityModeUtility
    {
        /// <summary>
        /// 当前是否处于编辑器下。
        /// </summary>
        /// <value><c>true</c> if is editor mode; otherwise, <c>false</c>.</value>
        public static bool IsEditorMode => Application.platform == RuntimePlatform.LinuxEditor
                                           || Application.platform == RuntimePlatform.OSXEditor
                                           || Application.platform == RuntimePlatform.WindowsEditor;

        /// <summary>
        /// 当前是否处于可用的设备上。
        /// </summary>
        /// <value><c>true</c> if is player; otherwise, <c>false</c>.</value>
        public static bool IsPlayer => Application.platform == RuntimePlatform.Android
                                       || Application.platform == RuntimePlatform.IPhonePlayer
                                       || Application.platform == RuntimePlatform.OSXPlayer
                                       || Application.platform == RuntimePlatform.LinuxPlayer
                                       || Application.platform == RuntimePlatform.WindowsPlayer;

        public static bool HasYuSrc =>
            Directory.Exists(Application.dataPath + "/_Yu/YuSrc/");

        private static Regex assetCheckRegex;

        public static Regex AssetCheckRegex
        {
            get
            {
                if (assetCheckRegex != null)
                {
                    return assetCheckRegex;
                }

                assetCheckRegex = new Regex("^[a-zA-Z][a-zA-Z0-9@_]*$");
                return assetCheckRegex;
            }
        }
    }
}