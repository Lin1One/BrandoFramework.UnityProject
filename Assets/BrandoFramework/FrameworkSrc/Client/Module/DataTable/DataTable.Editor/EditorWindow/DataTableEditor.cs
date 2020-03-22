#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion

using Client.Core;
using Common;
using Client.ScriptCreate;
using Common.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Reflection;
using Client.Utility;

namespace Client.DataTable.Editor
{
    [Serializable]
    public class DataTableEditor
    {
        /// <summary>
        /// 脚本导出
        /// </summary>
        [TabGroup("Excel")]
        [HideLabel]
        public ExcelScriptExportSetting ScriptExportSetting;

        /// <summary>
        /// 数据导出
        /// </summary>
        [TabGroup("Excel")]
        [HideLabel]
        public YuU3dExcelDataExportSetting DataExportSetting;

        //public TableDataBoard<TestProject_ExcelEntity_Test> 
    }


    /// <summary>
    /// 脚本导出设置
    /// </summary>
    [Serializable]
    public class ExcelScriptExportSetting
    {
        [BoxGroup("读表规则")]
        [HideLabel]
        public DataTableReadRule readRule;

        [BoxGroup("Excel脚本导出")]
        [LabelText("导出脚本类型")]
        public ScriptType scriptType;

        [BoxGroup("Excel脚本导出")]
        [LabelText("序列化方式")]
        public SerializationType serializationType;

        [BoxGroup("Excel脚本导出")]
        [LabelText("目标Excel文件")]
        [FilePath]
        public string TaregetExcel;

        [BoxGroup("Excel脚本导出")]
        [LabelText("导出目录")]
        [FolderPath]
        public string ExportDir;

        [BoxGroup("Excel脚本导出")]
        [Button("导出Excel脚本")]
        private void ExportScript()
        {
            Injector.Instance.Get<ExcelCsharpInterfaceScriptCreator>().
                CreateScript(this);
            var scriptFileName = Injector.Instance.Get<ExcelCSharpEntityScriptCreator>().
                CreateScript(this);
            GlobalExcelPathMap.Instance.AddAppPathMap(ProjectInfoDati.GetActualInstance(),
                scriptFileName,
                TaregetExcel);
            GlobalExcelPathMap.Instance.Save();
            AssetDatabaseUtility.Refresh();
        }
    }

    /// <summary>
    /// 数据导出设置
    /// </summary>
    [Serializable]
    public class YuU3dExcelDataExportSetting
    {
        private const string DataFileExtention = ".bytes";

        [BoxGroup("Excel数据导出")]
        [LabelText("导出目录")]
        [FolderPath]
        public string ExportDir;

        [BoxGroup("Excel数据导出")]
        [LabelText("Excel脚本名称")]
        [InlineButton("ExportExcelData","导出数据")]
        public string excelDataClass;

        public void ExportExcelData()
        {
            var projectInfo = ProjectInfoDati.GetActualInstance();
            var fullClassName = projectInfo.ProjectRuntimeScriptDefines + "." + excelDataClass;
            var assemblyPath = projectInfo.ProjectRuntimeAssemblyPath;
            var targetAssembly = Assembly.LoadFile(assemblyPath);
            var excelDataClassType = targetAssembly.GetType(fullClassName);
            var excelFileToExport = GlobalExcelPathMap.GetFilename(excelDataClass, projectInfo.DevelopProjectName);
            ExportExcelDatas(excelDataClassType, excelFileToExport);
        }

        [BoxGroup("Excel数据导出")]
        [Button("导出所有Excel数据")]
        public void ExportAllExcelData()
        {
            var projectInfo = ProjectInfoDati.GetActualInstance();

            var assemblyPath = projectInfo.ProjectRuntimeAssemblyPath;
            Assembly targetAssembly = Assembly.LoadFile(assemblyPath);
            var excelDataClassType = targetAssembly.GetTypes();
            var types = targetAssembly.GetTypes();
            foreach (var type in types)
            {
                if((typeof(IExcelEntity).IsAssignableFrom(type)))
                {
                    var excelFileToExport = GlobalExcelPathMap.GetFilename(type.Name, projectInfo.DevelopProjectName);
                    ExportExcelDatas(type, excelFileToExport);
                }
            }
        }

        private void ExportExcelDatas(Type excelEntityType, string excelFilePath)
        {
            var dataRowStrs = ExcelUtilty.GetSheetStrs(excelFilePath);

            var initEntitysMethod = excelEntityType.GetMethod("InitEntitys");
            var instance = Activator.CreateInstance(excelEntityType);
            object[] param = new object[] { dataRowStrs };
            var result = initEntitysMethod?.Invoke(instance, param);
            var dataList = result.As<List<IExcelEntity>>();

            var DataBytes = Injector.Instance.Get<ProtobufSerializer>().Serialize(dataList);
            var exportFilePath = ExportDir + "/" + excelEntityType.Name + DataFileExtention;
            IOUtility.WriteAllBytes(exportFilePath, DataBytes, true);
        }

        private void ExportExcelDatas(IExcelEntity entity, string excelFilePath)
        {
            var dataRowStrs = ExcelUtilty.GetSheetStrs(excelFilePath);
            var dataList = entity.InitEntitys(dataRowStrs);
            var DataBytes = Injector.Instance.Get<ISerializer>().Serialize(dataList);
            var exportFilePath = ExportDir + "/" + entity.TypeName() + DataFileExtention;
            IOUtility.WriteAllBytes(exportFilePath, DataBytes, true);
        }
    }

    [Serializable]
    public class TableDataBoard<T> where T : class, IExcelEntity<T>
    {
        public List<T> tableDatas;
    }



}