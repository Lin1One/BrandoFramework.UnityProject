#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion


using Common;

namespace Client.DataTable.Editor
{
    public abstract class AbsExcelWithinClassScriptCreator
    {
        protected readonly StringAppender appender
            = new StringAppender();

        //protected YuU3dAppSetting CurrentU3DApp => YuU3dAppSettingDati.CurrentActual;
        protected string ScriptId => ExcelUtilty.GetWithinClassId(FieldInfo);
        protected ExcelFieldInfo FieldInfo;

        protected string ScriptPath => /*CurrentU3DApp.Helper.CsExcelDir + */FieldInfo.SheetClassId + "/"
                                       + ScriptId + ".cs";

        /// <summary>
        /// 尝试获得已经存在的嵌套Excel数据表目标脚本的路径。
        /// </summary>
        protected string TryGetExistPath
        {
            get
            {
                //var app = YuU3dAppSettingDati.CurrentActual;
                //var asm = YuUnityIOUtility.GetUnityAssembly(app.PlayAsmId);
                //var types = asm.GetTypes().ToList();
                //var targetType = types.Find(t => t.Name == ScriptId);
                //if (targetType != null)
                //{
                //    var appExcelDir = app.Helper.CsExcelDir;
                //    var targetFileId = targetType.Name + ".cs";
                //    var excelCsPaths = YuIOUtility.GetPathDictionary(appExcelDir, s => s.EndsWith(".cs")).Values
                //        .ToList();
                //    var targetPath = excelCsPaths.Find(p => p.EndsWith(targetFileId));
                //    return targetPath;
                //}

                return null;
            }
        }
    }
}