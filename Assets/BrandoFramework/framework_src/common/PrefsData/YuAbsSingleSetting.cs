using Common.Utility;
using Sirenix.OdinInspector;
using System;
using System.IO;
using UnityEngine;

namespace Common.PrefsData
{
    #region 可持久化保存对象特性类

    /// <summary>
    /// 是否为编辑器下专用配置。
    /// 被附加了该特殊的配置类将会从_Yu/EditorSetting加载。
    /// </summary>
    public class YuEditorSettingAttribute : Attribute
    {
    }

    public class YuSaveAbleDescAttribute : Attribute
    {
        /// <summary>
        /// 可保存对象的用途说明。
        /// </summary>
        public string Desc { get; }

        public YuSaveAbleDescAttribute(string desc)
        {
            Desc = desc;
        }
    }

    /// <summary>
    /// 基于日期命名持久化数据文件的持久化可保存对象特性。
    /// </summary>
    public class YuSaveAbleAtDate : Attribute
    {
    }

    #endregion

    #region 可持久化保存对象泛型基类

    public abstract class YuAbsSingleSetting<T>
        where T : class, new()
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                var isEditorSetting = typeof(T).HasAttribute<YuEditorSettingAttribute>();

                if (YuUnityUtility.IsEditorMode)
                {
                    if (isEditorSetting)
                    {
                        var editorPath = EditorSettingPath;

                        if (!File.Exists(editorPath))
                        {
                            IOUtility.EnsureDirExist(editorPath);
                            instance = new T();
                            var jsContent = JsonUtility.ToJson(instance);
                            IOUtility.WriteAllText(editorPath, jsContent);
                            return instance;
                        }

                        var jsonContent = File.ReadAllText(editorPath);
                        instance = JsonUtility.FromJson<T>(jsonContent);
                    }
                    else
                    {
                        var resourcesPath = ResourcesPath;

                        if (!File.Exists(resourcesPath))
                        {
                            IOUtility.EnsureDirExist(resourcesPath);
                            instance = new T();
                            var jsContent = JsonUtility.ToJson(instance);
                            IOUtility.WriteAllText(resourcesPath, jsContent);
                            return instance;
                        }

                        var resId = "Setting/" + typeof(T).Name;
                        var textAsset = Resources.Load<TextAsset>(resId);
                        instance = JsonUtility.FromJson<T>(textAsset.text) ?? new T();
                    }
                }
                else
                {
                    var resId = "Setting/" + typeof(T).Name;
                    var textAsset = Resources.Load<TextAsset>(resId);
                    instance = JsonUtility.FromJson<T>(textAsset.text);
                }

                return instance;
            }

            protected set => instance = value;
        }

        private static string EditorSettingPath
        {
            get
            {
                string path;
                var dateAttr = typeof(T).GetAttribute<YuSaveAbleAtDate>();
                if (dateAttr == null)
                {
                    path = Application.dataPath + "/_Yu/EditorSetting/" + typeof(T).Name + ".txt";
                }
                else
                {
                    path = Application.dataPath + $"/_Yu/EditorSetting/{typeof(T).Name}/"
                                                + $"{DateTime.Now.ToLongDateString()}.txt";
                }

                return path;
            }
        }

        private static string ResourcesPath
        {
            get
            {
                string path;
                var dateAttr = typeof(T).GetAttribute<YuSaveAbleAtDate>();
                if (dateAttr == null)
                {
                    path = Application.dataPath + "/_Yu/Resources/Setting/" + typeof(T).Name + ".txt";
                }
                else
                {
                    path = Application.dataPath + $"/_Yu/Resources/Setting/{typeof(T).Name}/"
                                                + $"{DateTime.Now.ToLongDateString()}.txt";
                }

                return path;
            }
        }

        [HorizontalGroup("底部按钮")]
        [GUIColor(0.6f, 0.8f, 1f)]
        [Button("重新载入本地设置", ButtonSizes.Medium)]
        private void Reload()
        {
            var isEditorSetting = typeof(T).HasAttribute<YuEditorSettingAttribute>();
            var loadPath = isEditorSetting ? EditorSettingPath : ResourcesPath;

            var jsonContent = File.ReadAllText(loadPath);
            var t = JsonUtility.FromJson<T>(jsonContent);
            instance = t;
        }

#if UNITY_EDITOR

        [HorizontalGroup("底部按钮")]
        [GUIColor(0.6f, 0.8f, 1f)]
        [Button("保存更新", ButtonSizes.Medium)]
        private void SaveAndShowDialog() => Save(true);

        public void Save(bool showDialog = false)
        {
            var jsContent = JsonUtility.ToJson(instance);

            var isEditorSetting = typeof(T).HasAttribute<YuEditorSettingAttribute>();
            var writePath = isEditorSetting ? EditorSettingPath : ResourcesPath;

            if (Application.isPlaying)
            {
                IOUtility.WriteAllText(writePath, jsContent);
            }
            else
            {
                //var formatedContent = YuEditorAPIInvoker.PrettifyJsonString(jsContent);
                //YuIOUtility.WriteAllText(writePath, formatedContent);
            }

            TryShowDialog(showDialog);
        }

        private void TryShowDialog(bool showDialog = false)
        {
            if (!showDialog)
            {
                return;
            }

            var descAttr = typeof(T).GetAttribute<YuSaveAbleDescAttribute>();
            var mesasge = descAttr == null
                ? $"可保存对象{typeof(T).Name}当前已成功保存！"
                : descAttr.Desc + "当前已成功保存！";

            //YuEditorAPIInvoker.DisplayTip(mesasge);
        }

#endif
    }

    #endregion

}