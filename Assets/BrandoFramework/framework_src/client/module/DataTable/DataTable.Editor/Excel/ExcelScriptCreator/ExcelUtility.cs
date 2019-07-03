#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion

using Common.Config;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

namespace Client.DataTable.Editor
{
    [InitializeOnLoad]
    public static class ExcelUtility
    {
        #region Excel脚本Id

        public static string EntityScriptName
        (
            ProjectInfo u3DAppSetting,
            ExcelSheetInfo sheetInfo
        )
        {
            var scriptName = $"{u3DAppSetting.DevelopProjectName}_ExcelEntity_{sheetInfo.EnglishId}";
            return scriptName;
        }

        public static string EntityInterfaceName
        (
            ProjectInfo u3DAppSetting,
            ExcelSheetInfo sheetInfo
        )
        {
            var scriptName = $"{u3DAppSetting.DevelopProjectName}_IExcelEntity_{sheetInfo.EnglishId}";
            return scriptName;
        }

        public static string GetWithinClassId(ExcelFieldInfo fieldInfo)
            //=> $"{YuU3dAppSettingDati.CurrentActual.LocAppId}_ExcelWithinClass_{fieldInfo.EnglishName}";
              => $"ExcelWithinClass_{fieldInfo.EnglishName}";

        #endregion

        #region Excel字段类型

        public static string GetFieldTypeStr(ExcelFieldType fieldType)
        {
            string typeStr = string.Empty;

            switch (fieldType)
            {
                case ExcelFieldType.String:
                    typeStr = "string";
                    break;
                case ExcelFieldType.Byte:
                    typeStr = "byte";
                    break;
                case ExcelFieldType.Short:
                    typeStr = "short";
                    break;
                case ExcelFieldType.Int:
                    typeStr = "int";
                    break;
                case ExcelFieldType.Long:
                    typeStr = "long";
                    break;
                case ExcelFieldType.Float:
                    typeStr = "float";
                    break;
                case ExcelFieldType.Enum:

                    break;
                case ExcelFieldType.StringArray:
                    typeStr = "List<string>";
                    break;
                case ExcelFieldType.ByteArray:
                    typeStr = "List<byte>";
                    break;
                case ExcelFieldType.ShortArray:
                    typeStr = "List<short>";
                    break;
                case ExcelFieldType.IntArray:
                    typeStr = "List<int>";
                    break;
                case ExcelFieldType.FloatArray:
                    typeStr = "List<float>";
                    break;
                case ExcelFieldType.LongArray:
                    typeStr = "List<long>";
                    break;
                case ExcelFieldType.ParamsPropertyClass:
                    break;
                case ExcelFieldType.Ignore:
                    break;
                case ExcelFieldType.Bool:
                    typeStr = "bool";
                    break;
                case ExcelFieldType.SimpleObj:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fieldType), fieldType, null);
            }

            return typeStr;
        }

        #endregion

        #region 辅助方法

        public static ISheet GetFirstSheet(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var workBoot = new XSSFWorkbook(fs);
                var sheet = workBoot.GetSheetAt(0);
                return sheet;
            }
        }

        #endregion

        #region Excel源文件转换

        /// <summary>
        /// 将Excel工作簿转换为解析数据实体所需的数据源字符串列表形式。
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSheetStrs(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var workBoot = new XSSFWorkbook(fs);
                var sheet = workBoot.GetSheetAt(0);
                var strList = new List<string>();
                var sb = new StringBuilder(1000);
                var rowLength = sheet.PhysicalNumberOfRows;
                var cellLength = sheet.GetRow(1).PhysicalNumberOfCells; // 字段中文描述行


                for (var rowIndex = 4; rowIndex < rowLength; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);

                    if (row.GetCell(0) == null ||
                        row.GetCell(0).ToString().StartsWith("Ignore"))
                    {
                        //Debug.Log($"Excel数据表{path}中在第{rowIndex}行发现忽略行，将跳过该行数据生成！");
                        continue;
                    }

                    sb.Clear();

                    for (var colIndex = 0; colIndex < cellLength; colIndex++)
                    {
                        try
                        {
                            // 如果目标单元格为空则填入默认的“0”字符串。
                            if (row.GetCell(colIndex) == null || row.GetCell(colIndex).ToString() == "")
                            {
                                ////sb.Append("0" + YuExcelConstant.SeparatorStr);
                                sb.Append("0"  /*YuExcelConstant.SeparatorStr*/);
                            }
                            else
                            {
                                var cellStr = row.GetCell(colIndex).ToString();
                                ////sb.Append(cellStr + YuExcelConstant.SeparatorStr);
                                sb.Append(cellStr /*+ YuExcelConstant.SeparatorStr*/);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"在解析数据表{path}的第{rowIndex}行第{colIndex}列时发生异常，" +
                      $"异常信息为{ex.Message}！");
                        }
                    }

                    // 移除最后添加的分割字符串，默认为[__]。
                    var lineStr = sb.ToString();
                    var cutedLineStr = lineStr.Substring(0,
                        lineStr.Length/* - YuExcelConstant.SeparatorStr.Length*/);
                    strList.Add(cutedLineStr);
                }

                return strList;
            }
        }


        #region 编辑器桥接注入

        static ExcelUtility()
        {
            //YuEditorAPIInvoker.GetSheetStrs = GetSheetStrs;
        }

        #endregion
    }

    #endregion
}
