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
    /// 不确定同时有效的属性数量的内嵌类脚本处理器。
    /// </summary>
    public class YuExcel_ParamsPropertyClassHandler : IYuExcelFieldHandler
    {
        public ScriptType ScriptType { get; }
        public FieldTypeEnum FieldType => FieldTypeEnum.ParamsPropertyClass;

        public string GetCodeStr(string languageType, ExcelFieldInfo fieldInfo)
        {
            var creator = new ScriptCreator(fieldInfo);
            creator.CreateScript();

            var codeStr = string.Empty;

            switch (languageType)
            {
                case "Csharp":
                    codeStr = $"{fieldInfo.EnglishName.ToLower()} " +
                              $"= {fieldInfo.CsType}.Create(cells[{fieldInfo.Index}]);";
                    break;
                case "TypeScript":
                    break;
            }

            return codeStr;
        }

        private class ScriptCreator : YuAbsExcelWithinClassScriptCreator
        {
            private readonly YuExcelSimpleObjInfo SimpleObjInfo;

            public ScriptCreator(ExcelFieldInfo fieldInfo)
            {
                FieldInfo = fieldInfo;
                SimpleObjInfo = new YuExcelSimpleObjInfo(fieldInfo.FieldClassDesc);
            }

            public void CreateScript()
            {
                appender.Clean();

                ////appender.AppendPrecomplie(CurrentU3DApp.LocAppId + "Play");
                appender.AppendUsingNamespace(
                    "System"
                );
                ////appender.AppendCsHeadComment();

                ////appender.AppendLine($"namespace {CurrentU3DApp.LocAppId}Play");
                appender.AppendLine();
                appender.AppendLine("{");
                appender.ToRight();

                appender.AppendLine("[Serializable]");
                ////if (YuSetting.Instance.IsUseProtobuf)
                ////{
                ////    appender.AppendLine("[ProtoBuf.ProtoContract]");
                ////}
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

            private string GetSimpleObjSwitchTypeStr(FieldTypeEnum fieldType)
            {
                string result = string.Empty;

                switch (fieldType)
                {
                    case FieldTypeEnum.String:
                        break;
                    case FieldTypeEnum.Byte:
                        break;
                    case FieldTypeEnum.Short:
                        break;
                    case FieldTypeEnum.Int:
                        result = "Convert.ToInt32(value)";
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
                        throw new ArgumentOutOfRangeException(nameof(fieldType), fieldType, null);
                }

                return result;
            }

            private void AppendFieldDefine()
            {
                var protobufMemberIndex = 1;
                var propertyIdMap = SimpleObjInfo.PropertyIdMap;
                var chineseIdMap = SimpleObjInfo.ChineseIdMap;

                foreach (var sourceId in SimpleObjInfo.SourceIds)
                {
                    var englishId = propertyIdMap.ContainsKey(sourceId)
                        ? propertyIdMap[sourceId]
                        : sourceId;
                    var chineseId = chineseIdMap.ContainsKey(sourceId)
                        ? chineseIdMap[sourceId]
                        : "无中文注释";
                    appender.AppendCsComment(chineseId);

                    ////if (YuSetting.Instance.IsUseProtobuf)
                    ////{
                    ////    appender.AppendLine($"[ProtoBuf.ProtoMember({protobufMemberIndex})]");
                    ////    protobufMemberIndex++;
                    ////}

                    var typeStr = ExcelUtility.GetFieldTypeStr(SimpleObjInfo.FieldType);
                    appender.AppendLine($"public {typeStr} {englishId};");
                    appender.AppendLine();
                }

                appender.AppendLine();
            }

            private void AppendCreateMethod()
            {
                appender.AppendLine($"public static {ScriptId} Create(string text)");
                appender.AppendLine("{");
                appender.ToRight();
                appender.AppendLine($"var instance = new {ScriptId}();");

                // 构建一个临时的字符串键值对字典用于存放字段和值的映射。
                appender.AppendLine($"var lines = text.Split('{SimpleObjInfo.FirstSplit}');");

                appender.AppendLine("foreach (var line in lines)");
                appender.AppendLine("{");
                appender.ToRight();

                appender.AppendLine($"var array = line.Split('{SimpleObjInfo.SecondSplit}');");
                appender.AppendLine("var propertyId = array[0];");
                appender.AppendLine("var value = array[1];");
                appender.AppendLine("switch (propertyId)");
                appender.AppendLine("{");
                appender.ToRight();

                foreach (var kv in SimpleObjInfo.PropertyIdMap)
                {
                    appender.AppendLine($"case \"{kv.Key}\":");
                    appender.AppendLine($"instance.{kv.Value} = {GetSimpleObjSwitchTypeStr(SimpleObjInfo.FieldType)};");
                    appender.AppendLine("break;");
                }

                appender.ToLeft();
                appender.AppendLine("}");
                appender.ToLeft();
                appender.AppendLine("}");
                appender.AppendLine();
                appender.AppendLine("return instance;");
                appender.ToLeft();
                appender.AppendLine("}");
            }
        }

        /// <summary>
        /// 简单对象表示法。
        /// 二级拆分符。
        /// 一种业务逻辑属性在一个Excel单元格内的不定序表示。
        /// </summary>
        private class YuExcelSimpleObjInfo
        {
            public char FirstSplit;
            public char SecondSplit;
            public FieldTypeEnum FieldType;

            /// <summary>
            /// 表中原有的字段英文Id。
            /// </summary>
            public List<string> SourceIds = new List<string>();

            /// <summary>
            /// 将原Excel中命名不符合C#规范的重新映射为规范的命名。
            /// </summary>
            public Dictionary<string, string> PropertyIdMap = new Dictionary<string, string>();

            /// <summary>
            /// 简单对象表示法中字段的中文注释映射字典。
            /// 字典中每个键值对的Value即是对应字段的中文注释。
            /// </summary>
            public Dictionary<string, string> ChineseIdMap = new Dictionary<string, string>();

            public YuExcelSimpleObjInfo(string text)
            {
                var array = text.Split('\n');
                // 第一行为两级拆分符和字段数据结构类型定义
                var firstLine = array[0];
                var firstArray = firstLine.Split('|');
                FirstSplit = firstArray[0][firstArray[0].Length - 1];
                SecondSplit = firstArray[1][firstArray[1].Length - 1];
                FieldType = firstArray[2].Split('=')[1].AsEnum<FieldTypeEnum>();

                // 第二行为字段名重映射，如果没有找到重映射则直接使用该字段在表中的原始值。
                var secondLine = array[1];
                var secondArray = secondLine.Split('|');
                foreach (var s in secondArray)
                {
                    var tmpArray = s.Split('=');
                    var k = tmpArray[0];
                    var v = tmpArray[1];
                    SourceIds.Add(k);
                    PropertyIdMap.Add(k, v);
                }

                // 第三行为字段名中文注释映射。
                var thirdLine = array[2];
                var thirdArray = thirdLine.Split('|');
                foreach (var s in thirdArray)
                {
                    var tmpArray = s.Split('=');
                    var k = tmpArray[0];
                    var v = tmpArray[1];
                    ChineseIdMap.Add(k, v);
                }
            }
        }
    }
}