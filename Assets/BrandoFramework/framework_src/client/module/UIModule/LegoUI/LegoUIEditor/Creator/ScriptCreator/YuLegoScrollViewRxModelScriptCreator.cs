#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/13 14:44:21
// Email:             836045613@qq.com

#endregion

using Common;
using System.Linq;

namespace Client.LegoUI.Editor
{
    public class YuLegoScrollViewRxModelScriptCreator
    {
        ////private readonly YuStringAppender appender
        ////    = new YuStringAppender();

        ////private readonly LegoRectTransformMeta rectMeta;
        //////private readonly YuU3dAppSetting u3DAppSetting;
        ////private readonly string componentType;
        ////private readonly string interfaceType;
        ////private string ClassId => $"{u3DAppSetting.LocAppId}_Lego{rectMeta.PropertyId}_RxModel";

        ////private YuLegoScrollViewRxModelScriptCreator(YuLegoRectTransformMeta rtMeta, YuU3dAppSetting u3DAppSetting)
        ////{
        ////    rectMeta = rtMeta;
        ////    this.u3DAppSetting = u3DAppSetting;
        ////    var useComponentId = rectMeta.Name.Contains('@')
        ////        ? rectMeta.Name.Split('@').Last().Split('=').Last()
        ////        : rectMeta.Name.Split('_').Last();
        ////    componentType = $"{u3DAppSetting.LocAppId}_LegoComponent_{useComponentId}_RxModel";
        ////    interfaceType = $"I{u3DAppSetting.LocAppId}_LegoComponent_{useComponentId}_RxModel";
        ////}

        ////private void CreateScript()
        ////{
        ////    appender.Clean();

        ////    appender.AppendPrecomplie(
        ////        u3DAppSetting.LocAppId + "Play"
        ////    );

        ////    appender.AppendLine();

        ////    appender.AppCsNoteHeader();
        ////    appender.AppendUsingNamespace(
        ////        "Client.LegoUI"
        ////    );

        ////    appender.AppendSingleComment("该脚本由框架自动生成，请勿做任何修改！！！");
        ////    appender.AppendLine();

        ////    appender.AppendLine($"namespace {u3DAppSetting.LocAppId}Play");
        ////    appender.AppendLine("{");
        ////    appender.ToRight();
        ////    appender.AppendLine("[System.Serializable]");
        ////    appender.AppendLine($"public partial class {ClassId}");
        ////    appender.AppendLine($"    : YuAbsLegoScrollViewRxModel<{componentType}>");
        ////    appender.AppendLine("{");
        ////    appender.ToRight();
        ////    appender.AppendCsFooter();
        ////    appender.AppendLine();
        ////    appender.AppendLine("#endif");
        ////    var developerId = YuU3dAppSettingDati.CurrentActual.CurrentDeveloper.Name;
        ////    var path = u3DAppSetting.Helper.CsLegoUIDir + developerId + "/" +
        ////               "ScrollView/" + ClassId + ".cs";
        ////    YuIOUtility.WriteAllText(path, appender.ToString());
        ////}

        ////public static void CreateScript(YuLegoRectTransformMeta rectMeta, YuU3dAppSetting u3DApp)
        ////{
        ////    var creator = new YuLegoScrollViewRxModelScriptCreator(rectMeta, u3DApp);
        ////    creator.CreateScript();
        ////}
    }
}