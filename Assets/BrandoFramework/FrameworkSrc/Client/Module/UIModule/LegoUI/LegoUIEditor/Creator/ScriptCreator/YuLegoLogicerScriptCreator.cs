using Common.Utility;

namespace Client.LegoUI.Editor
{
    public class YuLegoLogicerScriptCreator : YuAbsLegoScriptCreator
    {
        private YuLegoLogicerScriptCreator(LegoUIMeta uiMeta)
            : base(uiMeta)
        {
        }

        public static void CreateLogicerScrpt(LegoUIMeta uiMeta)
        {
            var creator = new YuLegoLogicerScriptCreator(uiMeta);
            creator.CreateScript();
        }

        //private string LogicerScriptId => $"{U3DAppSetting.LocAppId}_{UiMeta.RootMeta.Name}_Logicer";

        private void CreateScript()
        {
            Appender.Clean();
            Appender.AppendPrecomplie(
                //U3DAppSetting.LocAppId + "Play"
            );

            Appender.AppendLine();

            Appender.AppCsNoteHeader();
            Appender.AppendUsingNamespace(
                "UnityEngine",
                "Client.LegoUI",
                "YuCommon",
                "YuU3dPlay"
            );

            //Appender.AppendLine($"namespace {U3DAppSetting.LocAppId}Play");
            Appender.AppendLine("{");
            Appender.ToRight();
            //Appender.AppendLine($"public class {LogicerScriptId} : {GetTargetScriptId()}");
            Appender.AppendLine("{");
            Appender.ToRight();
            Appender.AppendLine();
            AppendInit();

            Appender.AppendCsFooter();
            Appender.AppendLine("#endif");
            ////var path = UiMeta.LegoLogicerScriptPath(U3DAppSetting, LogicerScriptId);
            ////IOUtility.TryWriteAllText(path, Appender.ToString());
        }

        private string GetTargetScriptId()
        {
            var logicerScriptId = YuU3dAppLegoUISettingDati.CurrentActual?
                .ScriptSetting.LegoLogicerScriptId;
            
            if (string.IsNullOrEmpty(logicerScriptId))
            {
                return "YuAbsLegoLogicer";
            }

            return logicerScriptId;
        }

        private void AppendInit()
        {
            Appender.AppendLine("public override void Init(IYuLegoLogicContext context)");
            Appender.AppendLeftBracketsAndToRight();
            Appender.AppendLine("base.Init(context);");
            //Appender.AppendLine($"Debug.Log(\"{LogicerScriptId} is invoke!\");");
            Appender.AppendRightBracketsAndToLeft();
        }
    }
}