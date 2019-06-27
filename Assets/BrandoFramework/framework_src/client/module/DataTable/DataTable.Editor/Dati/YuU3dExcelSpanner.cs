#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion

using System;
using client_common;
using Common.PrefsData;
using Sirenix.OdinInspector;

namespace Client.DataTable.Editor
{
    [Serializable]
    [DatiInEditor]
    [YuDatiDesc(YuDatiSaveType.Single, typeof(YuU3dExcelSpannerDati), "Excel工具")]
    public class YuU3dExcelSpannerDati : GenericSingleDati<YuU3dExcelSpanner,YuU3dExcelSpannerDati>
    {
    }

    [Serializable]
    public class YuU3dExcelSpanner
    {
        [BoxGroup("全局配置")]
        [HideLabel]
        public YuU3dExcelGlobalSetting GlobalSetting;
        
        //[BoxGroup("Excel脚本导出")] [LabelText("导出脚本类型")]
        //public YuScriptType scriptType;

        [BoxGroup("Excel脚本导出")]
        [HideLabel]
        public YuU3dExcelScriptExportSetting ScriptExportSetting;

        [BoxGroup("Excel数据导出")]
        [HideLabel]
        public YuU3dExcelDataExportSetting DataExportSetting;
    }

    #region Excel脚本导出

    [Serializable]
    public class YuU3dExcelScriptExportSetting
    {
        [BoxGroup("任意Excel导出")]
        [HideLabel]
        public YuU3dExcelScriptExport ScriptExport;

        [BoxGroup("应用内Excel导出")]
        [HideLabel]
        public YuU3dExcelScriptAppExport ScriptAppExport;
    }

    [Serializable]
    public class YuU3dExcelScriptExport
    {
        [LabelText("目标Excel文件")] [FilePath] public string TaregetExcel;

        [LabelText("导出目录")] [FolderPath] public string ExportDir;

        [Button("导出Excel脚本")]
        private void ExportScript()
        {
            Injector.Instance.Get<YuU3dExcelEntityScriptCreator>().CreateScript(TaregetExcel/*,YuU3dAppSettingDati.CurrentActual*/);
            Injector.Instance.Get<YuU3dExcelCsharpInterfaceScriptCreator>().CreateScript(TaregetExcel/*, YuU3dAppSettingDati.CurrentActual*/);
        }
    }

    [Serializable]
    public class YuU3dExcelScriptAppExport
    {
    }

    #endregion

    #region 数据导出

    [Serializable]
    public class YuU3dExcelDataExportSetting
    {
        [BoxGroup("任意Excel数据导出")] [HideLabel] public YuU3dExcelDataExport DataExport;

        [BoxGroup("应用内Excel数据导出")] [HideLabel] public YuU3dExcelAppDataExportSetting AppDataExportSetting;
    }

    [Serializable]
    public class YuU3dExcelDataExport
    {
    }

    public class YuU3dExcelAppDataExportSetting
    {
    }

    #endregion
}