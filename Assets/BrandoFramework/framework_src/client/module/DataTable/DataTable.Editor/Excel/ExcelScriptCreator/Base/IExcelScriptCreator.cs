#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion


using Common.ScriptCreate;

namespace Client.DataTable.Editor
{
    public interface IExcelScriptCreator
    {
        ScriptType ScriptType { get; }

        void CreateScript(string excelPath, ExcelScriptExportSetting exportSetting);

        ////void CreateScript(string excelPath, YuU3dAppSetting appSetting = null);
    }
}

