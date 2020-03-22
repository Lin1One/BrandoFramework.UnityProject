#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/13 14:44:21
// Email:             836045613@qq.com
#endregion

using Common;
using UnityEngine;


namespace Client.LegoUI.Editor
{
    public class YuLegoPipelineHandlerScriptCreator
    {
        protected readonly StringAppender Appender
            = new StringAppender();
        //private YuU3dAppSetting u3DAppSetting;
        private RectTransform uiRect;
        private UIPipelineType pipelineType;

        private YuLegoPipelineHandlerScriptCreator( RectTransform rect,
            UIPipelineType pipelineType)
        {
            //this.u3DAppSetting = u3DAppSetting;
            uiRect = rect;
            this.pipelineType = pipelineType;
        }

        private string ClassId => ""/*$"{u3DAppSetting.LocAppId}_{uiRect.name}_{pipelineType}_Handler"*/;

        private void CreateScript()
        {
            Appender.Clean();
            //Appender.AppendLine($"#if {u3DAppSetting.LocAppId}Play");
            Appender.AppCsNoteHeader();
            Appender.AppendUsingNamespace(
                "Client.LegoUI",
                "UnityEngine",
                "YuU3dPlay"
                );

            Appender.AppendSingleComment("在这里处理UI的生命周期！");
            //Appender.AppendLine($"namespace {u3DAppSetting.LocAppId}Play");
            Appender.AppendLine("{");
            Appender.ToRight();
            Appender.AppendLine($"public class {ClassId} : IYuLegoUIPipelineHandler");
            Appender.AppendLine("{");
            Appender.ToRight();
            Appender.AppendLine($"public string UiId => \"{uiRect.name}\";");
            Appender.AppendLine($"public YuUIPipelineType PipelineType => " +
                $"YuUIPipelineType.{pipelineType};");
            Appender.AppendLine();

            Appender.AppendLine($"public void Execute(IYuLegoUI legoUI)");
            Appender.AppendLine("{");
            Appender.ToRight();
            Appender.AppendLine("#if UNITY_EDITOR");
            Appender.AppendLine($"Debug.Log(\"{ClassId} is Invoked!\");");
            Appender.AppendLine("#endif");
            Appender.ToLeft();
            Appender.AppendLine("}");

            Appender.AppendCsFooter();
            Appender.AppendLine("#endif");
            //var developerId = YuU3dAppSettingDati.CurrentActual.CurrentDeveloper.Name;
            //var path = u3DAppSetting.Helper.CsLegoUIDir + developerId + "/"
            //    + GetUITypeDir() + "/" + uiRect.name + "/LogicCode/LogicAndPipeline/"
            //    + ClassId + ".cs";
            //var codeContent = Appender.ToString();
            //YuIOUtility.TryWriteAllText(path, codeContent);
        }

        private string GetUITypeDir()
        {
            if (uiRect.name.StartsWith("LegoView"))
            {
                return "LegoView";
            }

            return "LegoComponent";
        }

        #region 静态调用

        public static void CreateScript(  RectTransform rect,
            UIPipelineType pipelineType)
        {
            var creator = new YuLegoPipelineHandlerScriptCreator( rect, pipelineType);
            creator.CreateScript();
        }

        #endregion
    }
}