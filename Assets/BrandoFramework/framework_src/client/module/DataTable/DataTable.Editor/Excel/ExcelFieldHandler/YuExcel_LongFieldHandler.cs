#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion

using Common.ScriptCreate;

namespace Client.DataTable.Editor
{
    public class YuExcel_LongFieldHandler : IYuExcelFieldHandler
    {
        public ScriptType ScriptType { get; }
        public ExcelFieldType FieldType => ExcelFieldType.Long;
        public string GetCodeStr(string languageType, ExcelFieldInfo fieldInfo)
        {
            var codeStr = string.Empty;

            switch (languageType)
            {
                case "Csharp":
                    codeStr = $"{fieldInfo.EnglishName.ToLower()} " +
                              $"= Convert.ToInt64(cells[{fieldInfo.Index}]);";
                    break;
                case "TypeScript":
                    codeStr = $"entity.{fieldInfo.EnglishName} " +
                              $"= parseLong(cells[{fieldInfo.Index}]);";
                    break;
            }

            return codeStr;
        }
    }
}