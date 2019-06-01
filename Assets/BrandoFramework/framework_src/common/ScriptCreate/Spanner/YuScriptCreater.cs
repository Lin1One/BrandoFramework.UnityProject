#region Head

// Author:            Yu
// CreateDate:        2019/1/22 15:05:37
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Common.DataStruct;
using Common.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Common.ScriptCreate
{
    [Serializable]
    public class YuScriptCreater
    {
        private static YuScriptCreater instance;
        public static YuScriptCreater Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new YuScriptCreater();
                }
                return instance;
            }
        }

        [FolderPath]
        [LabelText("目标目录")]
        public string TargetDir;

        [BoxGroup("开发者信息")]
        [LabelText("开发者ID")]
        public string DevelopId;

        [BoxGroup("开发者信息")]
        [LabelText("开发者邮箱")]
        public string DevelopEmail;

        [LabelText("脚本创建设置")]
        public List<YuScriptCreateSetting> ScriptCreateSettings;

        #region 创建脚本

        [Button("创建脚本", ButtonSizes.Medium)]
        private void CreateScript()
        {
            TargetDir = TargetDir.EnsureDirEnd();

            var appender = new YuStringAppender();
            var successfulSettings = new List<YuScriptCreateSetting>();

            foreach (var setting in ScriptCreateSettings)
            {
                if (string.IsNullOrEmpty(setting.ScriptId))
                {
                    //YuEditorAPIInvoker.DisplayTip("脚本名不能为空！");
                    return;
                }

                var scriptPath = TargetDir + setting.ScriptId + ".cs";
                if (File.Exists(scriptPath))
                {
                    Debug.Log($"目标脚本{scriptPath}已存在，创建取消！");
                    continue;
                }

                AppendNoteHead(appender, setting);
                AppendBody(appender, setting);


                var content = appender.ToString();
                YuIOUtility.WriteAllText(scriptPath, content);
                appender.Clean();
                successfulSettings.Add(setting);
            }

            foreach (var setting in successfulSettings)
            {
                ScriptCreateSettings.Remove(setting);
            }

            AssetDatabase.Refresh();
        }

        private void AppendNoteHead(YuStringAppender appender, YuScriptCreateSetting setting)
        {
            if (!string.IsNullOrEmpty(setting.PreComplie))
            {
                appender.AppendLine("#if " + setting.PreComplie);
                appender.AppendLine();
            }

            appender.AppendLine("#region Head");
            appender.AppendLine();
            appender.AppendLine($"// Author:            {DevelopId}");
            appender.AppendLine($"// Email:             {DevelopEmail}");
            appender.AppendLine();
            appender.AppendLine("#endregion");
            appender.AppendLine();
        }

        private void TryAppendAotuInterface(YuStringAppender appender, YuScriptCreateSetting setting)
        {
            if (!setting.IsAotuCreateInterface)
            {
                return;
            }

            var interfaceId = "I" + setting.ScriptId;
            appender.AppendLine($"public interface {interfaceId}" + "{}");
            appender.AppendLine();
        }

        private void AppendBody(YuStringAppender appender, YuScriptCreateSetting setting)
        {
            var nameSpace = setting.NameSpace;
            appender.AppendLine("namespace " + nameSpace);
            appender.AppendLine("{");
            appender.ToRight();
            var scriptId = setting.ScriptId;
            var inheritText = setting.IsAotuCreateInterface ? ": I" + scriptId : null;

            switch (setting.inherits)
            {
                case Inherits.Class:
                    TryAppendAotuInterface(appender, setting);
                    appender.AppendLine($"public class {scriptId} {inheritText}");
                    break;
                case Inherits.AbstractClass:
                    TryAppendAotuInterface(appender, setting);
                    appender.AppendLine($"public abstract class {scriptId} {inheritText}");
                    break;
                case Inherits.Enum:
                    appender.AppendLine($"public enum {scriptId} : byte");
                    break;
                case Inherits.Interface:
                    appender.AppendLine($"public interface {scriptId}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!string.IsNullOrEmpty(setting.Inherit)
                && setting.inherits != Inherits.Enum)
            {
                appender.Append("        : " + setting.Inherit);
            }

            appender.AppendLine("{");
            appender.AppendLine("}");
            appender.ToLeft();
            appender.AppendLine("}");
            appender.AppendLine();

            if (!string.IsNullOrEmpty(setting.PreComplie))
            {
                appender.AppendLine("#endif");
            }
        }

        #endregion


    }
}