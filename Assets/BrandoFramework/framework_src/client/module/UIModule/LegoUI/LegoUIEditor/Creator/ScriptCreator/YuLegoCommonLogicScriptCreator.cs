#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/13 14:44:21
// Email:             836045613@qq.com

#endregion


using Common.Utility;

namespace Client.LegoUI.Editor
{
    public class YuLegoCommonLogicScriptCreator : YuAbsLegoScriptCreator
    {
        private YuLegoCommonLogicScriptCreator(LegoUIMeta uiMeta)
            : base(uiMeta)
        {
        }

        //private string ClassId => $"{U3DAppSetting.LocAppId}_{UiMeta.RootMeta.TypeId}_CommonLogic";

        private void CreateScript()
        {
            Appender.Clean();
            Appender.AppendPrecomplie(
                //U3DAppSetting.LocAppId + "Play"
            );

            Appender.AppCsNoteHeader();
            Appender.AppendSingleComment("在这里实现UI组件或视图的公共逻辑！");
            //Appender.AppendLine($"namespace {U3DAppSetting.LocAppId}Play");
            Appender.AppendLine("{");
            Appender.ToRight();
            //Appender.AppendLine($"public static class {ClassId}");
            Appender.AppendLine("{");
            Appender.ToLeft();
            Appender.AppendCsFooter();
            Appender.AppendLine("#endif");
            //var path = UiMeta.UICommonLogicScriptPath(U3DAppSetting, ClassId);
            //IOUtility.TryWriteAllText(path, Appender.ToString());
        }

        #region 静态调用

        public static void CreateScript(LegoUIMeta uiMeta)
        {
            var creator = new YuLegoCommonLogicScriptCreator(uiMeta);
            creator.CreateScript();
        }

        #endregion
    }
}