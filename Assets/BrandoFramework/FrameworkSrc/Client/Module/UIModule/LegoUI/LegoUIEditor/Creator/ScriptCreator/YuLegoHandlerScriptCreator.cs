#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/13 14:44:21
// Email:             836045613@qq.com

#endregion

using System.Linq;

namespace Client.LegoUI.Editor
{
    public class YuLegoHandlerScriptCreator : YuAbsLegoScriptCreator
    {
        private readonly string handlerFullName;
        private readonly string lowerId;
        private readonly string controlType;

        private YuLegoHandlerScriptCreator(LegoUIMeta uiMeta,
            string handlerFullName, string controlId)
            : base(uiMeta)
        {
            this.handlerFullName = handlerFullName;
            controlType = "IYuLego" + controlId.Split('_').First();
            lowerId = controlId.ToLower();
        }

        public static void CreateHandlerScript(LegoUIMeta uiMeta,
            string handlerFullName, string controlId)
        {
            var creator = new YuLegoHandlerScriptCreator(uiMeta, handlerFullName, controlId);
            creator.CreateScript();
        }

        private void CreateScript()
        {
            Appender.Clean();
            Appender.AppendPrecomplie(
               // U3DAppSetting.LocAppId + "Play"
            );

            Appender.AppendLine();

            Appender.AppCsNoteHeader();
            Appender.AppendUsingNamespace(
                "UnityEngine",
                "Client.LegoUI",
                "YuCommon",
                "YuU3dPlay"
            );

           // Appender.AppendLine($"namespace {U3DAppSetting.LocAppId}Play");
            Appender.AppendLine("{");
            Appender.ToRight();
            Appender.AppendLine($"public class {handlerFullName} : {GetBaseClassId()}");
            Appender.AppendLine("{");
            Appender.ToRight();
            AppendField();
            Appender.AppendLine();
            AppendExecute();

            Appender.AppendCsFooter();
            Appender.AppendLine("#endif");
            ////var path = UiMeta.LegoHandlerScriptPath(U3DAppSetting, handlerFullName);
            ////YuIOUtility.TryWriteAllText(path, Appender.ToString());
        }

        private string GetBaseClassId()
        {
            var scriptCreateSetting = YuU3dAppLegoUISettingDati.CurrentActual?.ScriptSetting;

            if (scriptCreateSetting == null)
            {
                return BASE_HANDLER_TYPE;
            }

            return scriptCreateSetting.LegoActionScriptId;
        }

        private void AppendField()
        {
            Appender.AppendLine($"private {controlType} {lowerId};");
//            Appender.AppendLineAtStart("#if UNITY_EDITOR");
//            Appender.AppendLine("private IYuLogAppender LogAppender => YuU3dAppUtility.Injector.Get<IYuLogModule>()");
//            Appender.AppendLine($"    .GetLogAppender(\"{YuU3dAppSettingDati.CurrentActual.CurrentDeveloper.Name}\");");
//            Appender.AppendLineAtStart("#endif");
        }

        private const string BASE_HANDLER_TYPE = "IYuLegoActionHandler";

        private void AppendExecute()
        {
            Appender.AppendCsComment("该方法处理当前交互的业务逻辑。");
            var overrideStr = GetBaseClassId() == BASE_HANDLER_TYPE ? null : "override";
            Appender.AppendLine($"public {overrideStr} void Execute(object legoControl)");
            Appender.AppendLine("{");
            Appender.ToRight();
            Appender.AppendLine($"if ({lowerId} == null)");
            Appender.AppendLine("{");
            Appender.ToRight();
            Appender.AppendLine($"{lowerId} = ({controlType}) legoControl;");
            Appender.ToLeft();
            Appender.AppendLine("}");
            Appender.AppendLine();
//            Appender.AppendLineAtStart("#if UNITY_EDITOR");
//            Appender.AppendLine($"LogAppender.AppendInfo(\"{handlerFullName} is invoked!\",\"UI\");");
//            Appender.AppendLineAtStart("#endif");
            Appender.ToLeft();
            Appender.AppendLine("}");
            Appender.AppendLine();
        }
    }
}