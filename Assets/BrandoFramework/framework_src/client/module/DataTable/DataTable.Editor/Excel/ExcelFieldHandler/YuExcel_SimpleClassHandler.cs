#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion

using Common;
using Common.ScriptCreate;
using Common.Utility;
using System;
using System.Collections.Generic;

namespace Client.DataTable.Editor
{
    /// <summary>
    /// 简单固定属性类型内嵌类脚本处理器。
    /// </summary>
    public class YuExcel_SimpleClassHandler : IYuExcelFieldHandler
    {
        public ScriptType ScriptType { get; }
        public FieldTypeEnum FieldType => FieldTypeEnum.SimpleObj;

        public string GetCodeStr(string languageType, ExcelFieldInfo fieldInfo)
        {
            var creator = new YuExcelInternalClassScriptCreator(fieldInfo);
            creator.CreateScript();

            var codeStr = string.Empty;

            switch (languageType)
            {
                case "Csharp":
                    codeStr = $"{fieldInfo.EnglishName.ToLower()} " +
                              $"= {fieldInfo.ScriptId}.Create(cells[{fieldInfo.Index}]);";
                    break;
                case "TypeScript":
                    break;
            }

            return codeStr;
        }

        private class YuExcelInternalClassScriptCreator : YuAbsExcelWithinClassScriptCreator
        {
            public YuExcelClassOperateInfo OperateInfo;

            public YuExcelInternalClassScriptCreator(ExcelFieldInfo fieldInfo)
            {
                FieldInfo = fieldInfo;
                OperateInfo = new YuExcelClassOperateInfo(fieldInfo.FieldClassDesc);
            }

            public void CreateScript()
            {
                //if (TryGetExistPath)
                //{
                //    Debug.Log($"目标Excel数据表脚本{ScriptId}已存在，创建取消！");
                //    return;
                //}

                appender.Clean();

                //appender.AppendPrecomplie(CurrentU3DApp.LocAppId + "Play");
                appender.AppendUsingNamespace(
                    "System",
                    "System.Collections.Generic",
                    "YuCommon"
                );
                //appender.AppendCsHeadComment();

                //appender.AppendLine($"namespace {CurrentU3DApp.LocAppId}Play");
                appender.AppendLine();
                appender.AppendLine("{");
                appender.ToRight();

                appender.AppendLine("[Serializable]");
                //if (YuSetting.Instance.IsUseProtobuf)
                //{
                //    appender.AppendLine("[ProtoBuf.ProtoContract]");
                //}

                appender.AppendLine($"public class {ScriptId}");
                appender.AppendLine("{");
                appender.ToRight();
                AppendFieldDefine();
                AppendCreateMethod();
                appender.AppendCsFooter();
                appender.AppendLine();
                appender.AppendLine("#endif");
                var content = appender.ToString();
                IOUtility.WriteAllText(ScriptPath, content);
            }

            private void AppendCreateMethod()
            {
                appender.AppendLine($"public static List<{ScriptId}> Create(string text)");
                appender.AppendLine("{");
                appender.ToRight();
                appender.AppendLine($"var list = new List<{ScriptId}>(1);");
                appender.AppendLine($"if (text == \"0\") return list;"); 
                appender.AppendLine($"var propertys = text.Split('{OperateInfo.FirstSplit}').RemoveNull();");
                appender.AppendLine("foreach (var s in propertys)");
                appender.AppendLine("{");
                appender.ToRight();
                appender.AppendLine($"var instance = new {ScriptId}();");
                appender.AppendLine($"var array = s.Split('{OperateInfo.SecondSplit}');");
                AppendCreateAtPropertyDefine();
                appender.ToLeft();
                appender.AppendLine("}");
                appender.AppendLine();
                appender.AppendLine("return list;");
                appender.ToLeft();
                appender.AppendLine("}");
            }

            private void AppendFieldDefine()
            {
                var protobufMemberIndex = 1;

                foreach (var operateInfo in OperateInfo.PropertyDefineInfos)
                {
                    appender.AppendCsComment(operateInfo.ChineseId);
                    var typeStr = ExcelUtility.GetFieldTypeStr(operateInfo.FieldType);
                    ////if (YuSetting.Instance.IsUseProtobuf)
                    ////{
                    ////    appender.AppendLine($"[ProtoBuf.ProtoMember({protobufMemberIndex})]");
                    ////    protobufMemberIndex++;
                    ////}
                    appender.AppendLine($"public {typeStr} {operateInfo.EnglishId};");
                    appender.AppendLine();
                }

                appender.AppendLine();
            }

            private void AppendCreateAtPropertyDefine()
            {
                int index = 0;

                foreach (var defineInfo in OperateInfo.PropertyDefineInfos)
                {
                    switch (defineInfo.FieldType)
                    {
                        case FieldTypeEnum.Ignore:
                            break;
                        case FieldTypeEnum.String:
                            appender.AppendLine($"instance.{defineInfo.EnglishId} = array[{index}];");
                            break;
                        case FieldTypeEnum.Bool:
                            break;
                        case FieldTypeEnum.Byte:
                            break;
                        case FieldTypeEnum.Short:
                            break;
                        case FieldTypeEnum.Int:
                            appender.AppendLine($"instance.{defineInfo.EnglishId} = Convert.ToInt32(array[{index}]);");
                            break;
                        case FieldTypeEnum.Long:
                            break;
                        case FieldTypeEnum.Float:
                            break;
                        case FieldTypeEnum.Enum:
                            break;
                        case FieldTypeEnum.StringArray:
                            break;
                        case FieldTypeEnum.ByteArray:
                            break;
                        case FieldTypeEnum.ShortArray:
                            break;
                        case FieldTypeEnum.IntArray:
                            break;
                        case FieldTypeEnum.FloatArray:
                            break;
                        case FieldTypeEnum.LongArray:
                            break;
                        case FieldTypeEnum.ParamsPropertyClass:
                            break;
                        case FieldTypeEnum.SimpleObj:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    index++;
                }

                appender.AppendLine("list.Add(instance);");
            }
        }

        public class PropertyDefineInfo
        {
            /// <summary>
            /// 字段数据类型。
            /// </summary>
            public FieldTypeEnum FieldType;

            /// <summary>
            /// 字段中文Id（注释）
            /// </summary>
            public string ChineseId;

            /// <summary>
            /// 字段英文Id（字段名）
            /// </summary>
            public string EnglishId;

            public PropertyDefineInfo(string text)
            {
                var array = text.Split('|');
                ChineseId = array[0].Split('=')[1];
                EnglishId = array[1].Split('=')[1];
                FieldType = array[2].Split('=')[1].AsEnum<FieldTypeEnum>();
            }
        }

        /// <summary>
        /// Excel嵌套类操作指令信息。
        /// </summary>
        public class YuExcelClassOperateInfo
        {
            public char FirstSplit;

            public char SecondSplit;

            public List<PropertyDefineInfo> PropertyDefineInfos
                = new List<PropertyDefineInfo>();


            public YuExcelClassOperateInfo(string text)
            {
                var array = text.Split('\n');
                // 第一行是拆分符号定义
                var firstLine = array[0];
                var firstArray = firstLine.Split('|');
                FirstSplit = firstArray[0][firstArray[0].Length - 1];
                SecondSplit = firstArray[1][firstArray[1].Length - 1];

                for (int index = 1; index < array.Length; index++)
                {
                    var line = array[index];
                    var propertyDefineInfo = new PropertyDefineInfo(line);
                    PropertyDefineInfos.Add(propertyDefineInfo);
                }
            }
        }
    }
}