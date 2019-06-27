#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion

using Common.ScriptCreate;

namespace Client.DataTable.Editor
{
    public class YuExcel_ListStringFieldHandler
        : IYuExcelFieldHandler
    {
        public ScriptType ScriptType { get; }

        public ExcelFieldType FieldType
        {
            get { return ExcelFieldType.StringArray; }
        }

        public string GetCodeStr(string languageType, ExcelFieldInfo fieldInfo)
        {
            var codeStr = string.Empty;

            //if (languageType == YuExcelConstant.CsLanguageType)
            //{
            //    codeStr = $"{fieldInfo.EnglishName.ToLower()} " +
            //              $"= cells[{fieldInfo.Index}].Split('{fieldInfo.ArraySplit}').ToList();";
            //}

            //if (languageType == YuExcelConstant.TsLanguageType)
            //{
            //    codeStr = $"entity.{fieldInfo.EnglishName} " +
            //              $"= cells[{fieldInfo.Index}].split('_');";
            //}

            return codeStr;
        }
    }
}