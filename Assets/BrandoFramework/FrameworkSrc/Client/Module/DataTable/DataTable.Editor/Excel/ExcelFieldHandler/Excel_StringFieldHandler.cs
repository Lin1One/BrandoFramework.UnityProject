#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion

using Client.ScriptCreate;

namespace Client.DataTable.Editor
{
    public class Excel_StringFieldHandler
        : IExcelFieldHandler
    {
        public ScriptType ScriptType { get; }

        public FieldTypeEnum FieldType
        {
            get { return FieldTypeEnum.String; }
        }

        public string GetCodeStr(string languageType, ExcelFieldInfo fieldInfo)
        {
            var codeStr = string.Empty;

            switch (languageType)
            {
                case "Csharp":
                    codeStr = $"{fieldInfo.EnglishName.ToLower()} " +
                              $"= cells[{fieldInfo.Index}];";
                    break;
                case "TypeScript":
                    codeStr = $"entity.{fieldInfo.EnglishName}" +
                              $" = cells[{fieldInfo.Index}];";
                    break;
            }

            return codeStr;
        }
    }
}