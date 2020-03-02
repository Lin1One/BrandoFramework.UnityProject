#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion

using Client.Core;
using Common;
using Common.ScriptCreate;
using Common.Utility;
using NPOI.XSSF.UserModel;
using System;
using System.IO;

namespace Client.DataTable.Editor
{
    public abstract class ExcelScriptCreatorBase : IExcelScriptCreator, IDisposable
    {
        private const string IGNORE_STR = "Ignore";
        private const string CLASS_PARAMS_PROPERTYOBJ = "Class=ParamsPropertyObj";
        private const string CLASS_SIMPLEOBJ = "Class=SimpleObj";
        private const string ARRAY_STR = "Array";

        protected ProjectInfo projectInfo => ProjectInfoDati.GetActualInstance();

        protected ExcelScriptExportSetting exportSetting;
        protected ExcelSheetInfo SheetInfo { get; private set; }
        public abstract ScriptType ScriptType { get; }
        public abstract string ScriptName { get; }
        protected YuStringAppender Appender { get; } = new YuStringAppender();

        public string CreateScript(ExcelScriptExportSetting exportSetting)
        {
            this.exportSetting = exportSetting;
            InitSheetInfo(exportSetting);

            if (projectInfo != null && !string.IsNullOrEmpty(projectInfo.ProjectScriptingDefines))
            {
                Appender.AppendPrecomplie(projectInfo.ProjectScriptingDefines.Split(','));
            }

            AppendScript();

            if (projectInfo != null && !string.IsNullOrEmpty(projectInfo.ProjectScriptingDefines))
            {
                Appender.AppendLine("#endif");
            }

            var content = Appender.ToString();
            IOUtility.WriteAllText($"{exportSetting.ExportDir}/{ScriptName}.cs", content);
            Dispose();
            return ScriptName;
        }

        /// <summary>
        /// 获取工作簿，读取表头信息，
        /// </summary>
        /// <param name="Excel文件路径"></param>
        private void InitSheetInfo(ExcelScriptExportSetting exportSetting)
        {
            string excelFilePath = exportSetting.TaregetExcel;
            var readRule = exportSetting.readRule;

            using (var fs = new FileStream(excelFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var workBook = new XSSFWorkbook(fs);
                var firstSheet = workBook.GetSheetAt(0); // 约定只有第一个工作簿为有效导出源
                SheetInfo = new ExcelSheetInfo(firstSheet);
                var chineseCommentRow = firstSheet.GetRow(readRule.ChineseCommentIndex);
                var englishCommentRow = firstSheet.GetRow(readRule.EnglishCommentIndex);
                var fieldDefineRow = firstSheet.GetRow(readRule.StrcutDefineIndex);

                //字段数
                var fieldCount = chineseCommentRow.PhysicalNumberOfCells;

                for (var index = 0; index < fieldCount; index++)
                {
                    try
                    {
                        var chineseComment = chineseCommentRow.GetCell(index).ToString();
                        var englishComment = englishCommentRow.GetCell(index).ToString();
                        var fieldDefineStr = fieldDefineRow.GetCell(index).ToString();
                        //字段类型
                        var fieldType = GetFieldDefine(fieldDefineStr);
                        //字段备注
                        var fieldClassDesc = TryGetClassFieldDesc(fieldDefineStr);
                        //分隔符
                        var arraySplit = TryGetArraySplit(fieldDefineStr);

                        var fieldInfo = new ExcelFieldInfo(
                                englishComment,
                                chineseComment,
                                fieldType,
                                arraySplit,
                                index,
                                SheetInfo.EnglishId,
                                fieldClassDesc);
                        SheetInfo.AddFieldInfo(fieldInfo);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"在解析Excel文件{excelFilePath}的第{index}列时发生异常," +
                                            $"异常信息为{e.Message}！");
                    }
                }
            }
        }

        /// <summary>
        /// 获取数组分隔符
        /// </summary>
        /// <param name="fieldDefineStr"></param>
        /// <returns></returns>
        private char TryGetArraySplit(string fieldDefineStr)
        {
            if (fieldDefineStr.Contains(ARRAY_STR))
            {
                var array = fieldDefineStr.Split('=');
                if (array.Length == 1)
                {
                    return ';';
                }
                return fieldDefineStr[fieldDefineStr.Length - 1];
            }
            return ';';
        }

        private string TryGetClassFieldDesc(string typeStr)
        {
            if (typeStr.StartsWith(CLASS_PARAMS_PROPERTYOBJ))
            {
                var result = typeStr.Substring(24, typeStr.Length - 24);
                return result;
            }

            if (typeStr.StartsWith(CLASS_SIMPLEOBJ))
            {
                var result = typeStr.Substring(16, typeStr.Length - 16);
                return result;
            }

            return null;
        }

        /// <summary>
        /// 获取字段类型
        /// </summary>
        private FieldTypeEnum GetFieldDefine(string fieldDefineStr)
        {
            if (string.IsNullOrEmpty(fieldDefineStr) || fieldDefineStr == IGNORE_STR)
            {
                return FieldTypeEnum.Ignore;
            }

            if (fieldDefineStr.StartsWith(CLASS_PARAMS_PROPERTYOBJ))
            {
                return FieldTypeEnum.ParamsPropertyClass;
            }

            if (fieldDefineStr.StartsWith(CLASS_SIMPLEOBJ))
            {
                return FieldTypeEnum.SimpleObj;
            }

            if (fieldDefineStr.Contains(ARRAY_STR))
            {
                var arrayTypeStr = fieldDefineStr.Split('=')[0];
                return arrayTypeStr.AsEnum<FieldTypeEnum>();
            }

            var filedType = fieldDefineStr.AsEnum<FieldTypeEnum>();
            return filedType;
        }

        /// <summary>
        /// 编写具体脚本内容
        /// </summary>
        protected abstract void AppendScript();

        public virtual void Dispose() => Appender.Dispose();


    }
}