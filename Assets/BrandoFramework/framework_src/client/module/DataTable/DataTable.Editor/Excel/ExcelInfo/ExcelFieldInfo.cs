#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion

namespace Client.DataTable.Editor
{
    /// <summary>
    /// Excel 表格字段信息
    /// </summary>
    public class ExcelFieldInfo
    {
        /// <summary>
        /// 字段英文名。
        /// </summary>
        public string EnglishName { get; }

        /// <summary>
        /// 字段中文名。
        /// </summary>
        public string ChineseName { get; }

        /// <summary>
        /// 数据结构字符串。
        /// </summary>
        public FieldTypeEnum FieldType { get; }

        /// <summary>
        /// 字段嵌套类描述（指令）。
        /// </summary>
        public string FieldClassDesc { get; }

        /// <summary>
        /// 列索引。
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// 数组拆分符。
        /// </summary>
        public char ArraySplit { get; set; }

        /// <summary>
        /// 字段所属的工作簿类名。
        /// </summary>
        public string SheetClassId { get; }


        public ExcelFieldInfo
        (
            string englishName,
            string chineseName,
            FieldTypeEnum fieldType,
            char arraySplit,
            int index,
            string sheetClassId,
            string classDesc = null
        )
        {
            EnglishName = englishName;
            ChineseName = chineseName;
            FieldType = fieldType;
            ArraySplit = arraySplit;
            Index = index;
            SheetClassId = sheetClassId;
            FieldClassDesc = classDesc;
        }

        public string CsType
        {
            get
            {
                switch (FieldType)
                {
                    case FieldTypeEnum.Int:
                        return "int";
                    case FieldTypeEnum.Float:
                        return "float";
                    case FieldTypeEnum.String:
                        return "string";
                    case FieldTypeEnum.IntArray:
                        return "List<int>";
                    case FieldTypeEnum.FloatArray:
                        return "List<float>";
                    case FieldTypeEnum.StringArray:
                        return "List<string>";
                    case FieldTypeEnum.Byte:
                        return "byte";
                    case FieldTypeEnum.Short:
                        return "short";
                    case FieldTypeEnum.Long:
                        return "long";
                    case FieldTypeEnum.Enum:
                        break;
                    case FieldTypeEnum.ByteArray:
                        return "List<byte>";
                    case FieldTypeEnum.ShortArray:
                        return "List<short>";
                    case FieldTypeEnum.LongArray:
                        return "List<long>";
                    case FieldTypeEnum.ParamsPropertyClass:
                    //return $"{CurrentU3DApp.LocAppId}_ExcelWithinClass_{EnglishName}";
                        return $"ExcelWithinClass_{EnglishName}";
                    case FieldTypeEnum.Bool:
                        return "bool";
                    case FieldTypeEnum.SimpleObj:
                    //return $"List<{CurrentU3DApp.LocAppId}_ExcelWithinClass_{EnglishName}>";
                        return $"List<ExcelWithinClass_{EnglishName}>";
                    default:
                        return "";
                }

                return null;
            }
        }

        public string ScriptId => ////$"{CurrentU3DApp.LocAppId}_ExcelWithinClass_{EnglishName}";
             $"ExcelWithinClass_{EnglishName}";
        public string TsType
        {
            get
            {
                switch (FieldType)
                {
                    case FieldTypeEnum.Int:
                        return "number";
                    case FieldTypeEnum.Float:
                        return "number";
                    case FieldTypeEnum.String:
                        return "string";
                    case FieldTypeEnum.IntArray:
                        return "number[]";
                    case FieldTypeEnum.FloatArray:
                        return "number[]";
                    case FieldTypeEnum.StringArray:
                        return "string[]";
                    default:
                        return "";
                }
            }
        }
    }
}