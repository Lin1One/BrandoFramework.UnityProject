#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion

using Common.ScriptCreate;

namespace Client.DataTable.Editor
{
    public class YuExcel_ListFloatFieldHandler
        : IYuExcelFieldHandler
    {
        public ScriptType ScriptType { get; }

        public ExcelFieldType FieldType
        {
            get { return ExcelFieldType.FloatArray; }
        }

        public string GetCodeStr(string languageType, ExcelFieldInfo fieldInfo)
        {
            var codeStr = string.Empty;

            switch (languageType)
            {
                case "Csharp":
                    codeStr = $"{fieldInfo.EnglishName.ToLower()} " +
                              $"= cells[{fieldInfo.Index}].ToListFloat('{fieldInfo.ArraySplit}');";
                    break;
                case "TypeScript":
                    codeStr = $"entity.{fieldInfo.EnglishName} " +
                              $"= parseListFloat(cells[{fieldInfo.Index}]);";
                    break;
            }

            return codeStr;
        }
    }
}