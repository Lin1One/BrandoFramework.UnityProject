#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/13 14:44:21
// Email:             836045613@qq.com

#endregion

using Common;
using UnityEngine;


namespace Client.LegoUI.Editor
{
    public class YuLegoScrollViewPipelineHandlerScriptCreator
    {
        //protected readonly YuStringAppender Appender
        //    = new YuStringAppender();
        //private YuU3dAppSetting u3DAppSetting;
        //private RectTransform uiRect;
        //private string ScrollViewGoName;
        //private YuLegoScrollViewPipelineType pipelineType;

        //private YuLegoScrollViewPipelineHandlerScriptCreator(YuU3dAppSetting u3DAppSetting, RectTransform rect,
        //    YuLegoScrollViewPipelineType pipelineType)
        //{
        //    this.u3DAppSetting = u3DAppSetting;
        //    uiRect = rect;
        //    this.pipelineType = pipelineType;
        //    ScrollViewGoName = rect.name.Split('@')[0];
        //}

        //private string ClassId => $"{u3DAppSetting.LocAppId}_{ScrollViewGoName}_{pipelineType}_Handler";

        //private void CreateScript()
        //{
        //    Appender.Clean();
        //    Appender.AppendLine($"#if {u3DAppSetting.LocAppId}Play");
        //    Appender.AppCsNoteHeader();
        //    Appender.AppendUsingNamespace(
        //        "YuLegoUIPlay",
        //        "UnityEngine"
        //        );

        //    Appender.AppendSingleComment("在这里处理UI的生命周期！");
        //    Appender.AppendLine($"namespace {u3DAppSetting.LocAppId}Play");
        //    Appender.AppendLine("{");
        //    Appender.ToRight();
        //    Appender.AppendLine($"public class {ClassId} : IYuLegoScrollViewPipelineHandler");
        //    Appender.AppendLine("{");
        //    Appender.ToRight();
        //    Appender.AppendLine($"public string UiId => \"{ScrollViewGoName}\";");
        //    Appender.AppendLine($"public YuLegoScrollViewPipelineType PipelineType => " +
        //        $"YuLegoScrollViewPipelineType.{pipelineType};");
        //    Appender.AppendLine();

        //    Appender.AppendLine($"public void Execute(IYuLegoScrollView legoUI)");
        //    Appender.AppendLine("{");
        //    Appender.ToRight();
        //    Appender.AppendLine($"Debug.Log(\"{ClassId} is Invokeed!\");");
        //    Appender.ToLeft();
        //    Appender.AppendLine("}");

        //    Appender.AppendCsFooter();
        //    Appender.AppendLine("#endif");
        //    var developerId = YuU3dAppSettingDati.CurrentActual.CurrentDeveloper.Name;
        //    var path = u3DAppSetting.Helper.CsLegoUIDir + developerId + "/"
        //        + GetUITypeDir() + "/ScrollViewPipelineHandler/" + ScrollViewGoName + "/"
        //        + ClassId + ".cs";
        //    var codeContent = Appender.ToString();
        //    YuIOUtility.TryWriteAllText(path, codeContent);
        //}

        //private string GetUITypeDir()
        //{
        //    return "ScrollView";
        //}

        #region 静态调用

        //public static void CreateScript(YuU3dAppSetting u3DAppSetting, RectTransform rect,
        //    YuLegoScrollViewPipelineType pipelineType)
        //{
        //    var creator = new YuLegoScrollViewPipelineHandlerScriptCreator(u3DAppSetting, rect, pipelineType);
        //    creator.CreateScript();
        //}

        #endregion
    }
}