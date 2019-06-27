#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion

using Common;
using Common.ScriptCreate;
using Sirenix.OdinInspector;
using System;

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
            Injector.Instance.Get<ExcelCSharpEntityScriptCreator>().
                CreateScript(TaregetExcel,this);
            Injector.Instance.Get<YuU3dExcelCsharpInterfaceScriptCreator>().
                CreateScript(TaregetExcel,this);
        }
    }

    /// <summary>
    /// 数据导出设置
    /// </summary>
    [Serializable]
    public class YuU3dExcelDataExportSetting
    {
        [BoxGroup("Excel数据导出")]
        [HideLabel]
        public YuU3dExcelDataExport DataExport;

        //[BoxGroup("应用内Excel数据导出")] [HideLabel] public YuU3dExcelAppDataExportSetting AppDataExportSetting;
    }

    [Serializable]
    public class YuU3dExcelDataExport
    {
    }

}