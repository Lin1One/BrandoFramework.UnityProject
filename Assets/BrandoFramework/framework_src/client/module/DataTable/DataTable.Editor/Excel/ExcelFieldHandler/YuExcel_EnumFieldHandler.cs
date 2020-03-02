#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion

using Common.ScriptCreate;

namespace Client.DataTable.Editor
{
    public class YuExcel_EnumFieldHandler
        : IYuExcelFieldHandler
    {
        public ScriptType ScriptType { get; }

        public FieldTypeEnum FieldType
        {
            get { return FieldTypeEnum.Enum; }
        }

        public string GetCodeStr(string languageType, ExcelFieldInfo fieldInfo)
        {
            // 首先创建或更新枚举脚本。
            //var enumCreator = new ExcelEnumScriptCreator(u3DAppSetting, fieldInfo);
            //enumCreator.CreateScript();

            var codeStr = string.Empty;

            switch (languageType)
            {
                case "Csharp":
                    ////codeStr = $"{fieldInfo.EnglishName.ToLower()} " +
                    ////          $"= cells[{fieldInfo.Index}].AsEnum<{enumCreator.EnumName}>();";
                    break;
                case "TypeScript":
                    codeStr = $"entity.{fieldInfo.EnglishName} " +
                              $"= parseInt(cells[{fieldInfo.Index}]);";
                    break;
            }

            return codeStr;
        }
    }
}