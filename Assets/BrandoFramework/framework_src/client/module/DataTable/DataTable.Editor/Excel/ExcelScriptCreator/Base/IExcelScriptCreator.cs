#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion


using Client.ScriptCreate;

namespace Client.DataTable.Editor
{
    public interface IExcelScriptCreator
    {
        ScriptType ScriptType { get; }

        string CreateScript(ExcelScriptExportSetting exportSetting);

        ////void CreateScript(string excelPath, YuU3dAppSetting appSetting = null);
    }
}

