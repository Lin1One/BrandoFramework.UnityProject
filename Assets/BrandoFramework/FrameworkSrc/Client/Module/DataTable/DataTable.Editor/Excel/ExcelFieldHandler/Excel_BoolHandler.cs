#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion



using Client.ScriptCreate;

namespace Client.DataTable.Editor
{
    public class Excel_BoolHandler : IExcelFieldHandler
    {
        public ScriptType ScriptType { get; }
        public FieldTypeEnum FieldType => FieldTypeEnum.Bool;

        public string GetCodeStr(string languageType, ExcelFieldInfo fieldInfo)
        {
            var codeStr = string.Empty;

            switch (languageType)
            {
                case "Csharp":
                    codeStr = $"{fieldInfo.EnglishName.ToLower()} " +
                              $"= cells[{fieldInfo.Index}].ToBool();";
                    break;
                case "TypeScript":
                    break;
            }

            return codeStr;
        }
    }
}