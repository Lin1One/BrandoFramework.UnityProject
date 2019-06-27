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
        //private static YuU3dAppSetting CurrentU3DApp => YuU3dAppSettingDati.CurrentActual;

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
        public ExcelFieldType FieldType { get; }

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

        //private readonly YuU3dAppSetting u3DAppSetting;

        public ExcelFieldInfo
        (
            string englishName,
            string chineseName,
            ExcelFieldType fieldType,
            int index,
            //YuU3dAppSetting u3DAppSetting,
            string sheetClassId,
            string classDesc = null
        )
        {
            EnglishName = englishName;
            ChineseName = chineseName;
            FieldType = fieldType;
            Index = index;
            ////this.u3DAppSetting = u3DAppSetting;
            SheetClassId = sheetClassId;
            FieldClassDesc = classDesc;
        }

        public string CsType
        {
            get
            {
                switch (FieldType)
                {
                    case ExcelFieldType.Int:
                        return "int";
                    case ExcelFieldType.Float:
                        return "float";
                    case ExcelFieldType.String:
                        return "string";
                    case ExcelFieldType.IntArray:
                        return "List<int>";
                    case ExcelFieldType.FloatArray:
                        return "List<float>";
                    case ExcelFieldType.StringArray:
                        return "List<string>";
                    case ExcelFieldType.Byte:
                        return "byte";
                    case ExcelFieldType.Short:
                        return "short";
                    case ExcelFieldType.Long:
                        return "long";
                    case ExcelFieldType.Enum:
                        break;
                    case ExcelFieldType.ByteArray:
                        return "List<byte>";
                    case ExcelFieldType.ShortArray:
                        return "List<short>";
                    case ExcelFieldType.LongArray:
                        return "List<long>";
                    case ExcelFieldType.ParamsPropertyClass:
                    //return $"{CurrentU3DApp.LocAppId}_ExcelWithinClass_{EnglishName}";
                        return $"ExcelWithinClass_{EnglishName}";
                    case ExcelFieldType.Bool:
                        return "bool";
                    case ExcelFieldType.SimpleObj:
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
                    case ExcelFieldType.Int:
                        return "number";
                    case ExcelFieldType.Float:
                        return "number";
                    case ExcelFieldType.String:
                        return "string";
                    case ExcelFieldType.IntArray:
                        return "number[]";
                    case ExcelFieldType.FloatArray:
                        return "number[]";
                    case ExcelFieldType.StringArray:
                        return "string[]";
                    default:
                        return "";
                }
            }
        }
    }
}