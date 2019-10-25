#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion

using Common;
using NPOI.XSSF.UserModel;
using System;
using System.IO;
using UnityEditor;

namespace Client.DataTable.Editor
{
    /// <summary>
    /// Excel相关脚本创建器。
    /// </summary>
    public class YuExcelScriptCreator
    {
        //private static YuU3dAppSetting CurrentU3DApp => YuU3dAppSettingDati.CurrentActual;

        private const string LAST_EXCEL_PATH = "LAST_EXCEL_PATH";

        #region 编辑器菜单项

        [MenuItem("Yu/自动化工作流/导出Excel脚本 &p", false, 1)]
        private static void CreateExcelEntityScript()
        {
            var lastPath = EditorPrefs.GetString(LAST_EXCEL_PATH);

            if (!string.IsNullOrEmpty(lastPath))
            {
                HasLast(lastPath);
            }
            else
            {
                NotLast();
            }
        }

        private static void HasLast(string lastPath)
        {
            var path = EditorUtility.OpenFilePanel("Excel", lastPath, "xlsx");
            var yuExcelScriptCreator = new YuExcelScriptCreator();
            yuExcelScriptCreator.CreateScript(path);
            AssetDatabase.Refresh();
            return;
        }

        private static void NotLast()
        {
            //if (string.IsNullOrEmpty(CurrentU3DApp.AppExcelDir))
            //{
            //    EditorUtility.DisplayDialog(
            //        "错误",
            //        $"当前应用{CurrentU3DApp.LocAppId}的Excel源文件路径没有设置！",
            //        "OK"
            //    );

            //    return;
            //}

            //if (!Directory.Exists(CurrentU3DApp.AppExcelDir))
            //{
            //    EditorUtility.DisplayDialog(
            //        "错误",
            //        $"当前应用{CurrentU3DApp.LocAppId}的Excel源文件路径不是一个有效目录！",
            //        "OK"
            //    );

            //    return;
            //}

            //var path = EditorUtility.OpenFilePanel("Excel", CurrentU3DApp.AppExcelDir, "xlsx");
            //if (!path.EndsWith(".xlsx"))
            //{
            //    EditorUtility.DisplayDialog(
            //        "错误",
            //        "选择的不是一个有效的Excel文件！",
            //        "OK"
            //    );

            //    return;
           // }

            // 记录上次打开的Excel文件。
            //EditorPrefs.SetString(LAST_EXCEL_PATH, path);
            //var yuExcelScriptCreator = new YuExcelScriptCreator();
            //yuExcelScriptCreator.CreateScript(path);
            //AssetDatabase.Refresh();
        }

        #endregion

        #region 基础字段

        #endregion

        private string TryGetClassFieldDesc(string typeStr)
        {
            if (typeStr.StartsWith("Class=ParamsPropertyObj"))
            {
                var calssDesc = typeStr.Substring(24, typeStr.Length - 24);
                return calssDesc;
            }

            if (typeStr.StartsWith("Class=SimpleObj"))
            {
                var classDesc = typeStr.Substring(16, typeStr.Length - 16);
                return classDesc;
            }

            return null;
        }

        private ExcelFieldType GetFieldType(string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr) || typeStr == "Ignore")
            {
                return ExcelFieldType.Ignore;
            }

            if (typeStr.StartsWith("Class=ParamsPropertyObj"))
            {
                return ExcelFieldType.ParamsPropertyClass;
            }

            if (typeStr.StartsWith("Class=SimpleObj"))
            {
                return ExcelFieldType.SimpleObj;
            }

            if (typeStr.Contains("Array"))
            {
                var arrayTypeStr = typeStr.Split('=')[0];
                return arrayTypeStr.AsEnum<ExcelFieldType>();
            }

            var type = typeStr.AsEnum<ExcelFieldType>();
            return type;
        }

        private char TryGetArraySplit(string typeStr)
        {
            if (typeStr.Contains("Array"))
            {
                var temArray = typeStr.Split('=');
                if (temArray.Length == 1)
                {
                    return ';';
                }

                return typeStr[typeStr.Length - 1];
            }

            return ';';
        }

        public void CreateScript(string excelPath)
        {
            using (var fs = new FileStream(excelPath, FileMode.Open, FileAccess.ReadWrite))
            {
                var workBoot = new XSSFWorkbook(fs);
                var sheet = workBoot.GetSheetAt(0);
                var sheetInfo = new ExcelSheetInfo(sheet);
                var chineseCommentRow = sheet.GetRow(0);
                var englishCommentRow = sheet.GetRow(2);
                var strcutTypeRow = sheet.GetRow(3);

                var length = chineseCommentRow.PhysicalNumberOfCells;
                for (var index = 0; index < length; index++)
                {
                    try
                    {
                        var chineseComment = chineseCommentRow.GetCell(index).ToString();
                        var englishComment = englishCommentRow.GetCell(index).ToString();
                        var typeStr = strcutTypeRow.GetCell(index).ToString();
                        var strcutType = GetFieldType(typeStr);
                        var fieldClassDesc = TryGetClassFieldDesc(typeStr);

                        var fieldInfo = new ExcelFieldInfo
                        (
                            englishComment,
                            chineseComment,
                            strcutType,
                            index,
                            //CurrentU3DApp,
                            sheetInfo.EnglishId,
                            fieldClassDesc
                        )
                        { ArraySplit = TryGetArraySplit(typeStr) };

                        sheetInfo.AddFieldInfo(fieldInfo);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"在解析第{index}列时发生异常，异常信息为{e.StackTrace}！");
                    }
                }

                ////YExcelEntityInterfaceCreator.CreateScript(sheetInfo, CurrentU3DApp);
                ////YuExcelEntityScriptCreator.CreateScript(sheetInfo, CurrentU3DApp);
                ////YuGlobalExcelPathMap.Instance.AddAppPathMap(CurrentU3DApp,
                ////    YuExcelUtility.EntityScriptName(CurrentU3DApp, sheetInfo),
                ////    Path.GetFileNameWithoutExtension(excelPath));
                ////YuGlobalExcelPathMap.Instance.Save();
                AssetDatabase.Refresh();
            }
        }
    }
}