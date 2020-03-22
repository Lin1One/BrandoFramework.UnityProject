#region Head

// Author:            Yu
// CreateDate:        2018/9/6 11:44:00
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion


using Common;

namespace Client.LegoUI.Editor
{
    /// <summary>
    /// 乐高视图组件常量脚本创建器。
    /// </summary>
    public class YuLegoUIConstScriptCreator
    {
        ////private readonly YuStringAppender appender
        ////    = new YuStringAppender();

        ////private readonly LegoUIMeta uiMeta;
        ////private readonly YuU3dAppSetting u3DAppSetting;

        ////private YuLegoUIConstScriptCreator(LegoUIMeta uiMeta, YuU3dAppSetting u3DAppSetting)
        ////{
        ////    this.uiMeta = uiMeta;
        ////    this.u3DAppSetting = u3DAppSetting;
        ////}

        ////#region 静态调用

        ////public static void CreateScript(YuLegoUIMeta uiMeta, YuU3dAppSetting u3DAppSetting)
        ////{
        ////    var creator = new YuLegoUIConstScriptCreator(uiMeta, u3DAppSetting);
        ////    creator.CreateScript();
        ////}

        ////#endregion

        ////#region 实例API

        ////private void CreateScript()
        ////{
        ////    appender.Clean();
        ////    appender.AppendLine($"#if {u3DAppSetting.LocAppId}Play");
        ////    appender.AppendLine();
        ////    appender.AppendUsingNamespace(
        ////        "YuCommon"
        ////    );
        ////    AppendBaseHead();
        ////    AppendClassHead();
        ////    AppendField();
        ////    appender.AppendCsFooter();
        ////    appender.AppendLine("#endif");
        ////    YuIOUtility.WriteAllText(uiMeta.UIConstScriptPath(u3DAppSetting), appender.ToString());
        ////}

        ////private void AppendBaseHead()
        ////{
        ////    appender.AppendSingleComment("该脚本由框架自动生成，请勿做任何修改！！！");
        ////    appender.AppendLine();

        ////    appender.AppendLine($"namespace {u3DAppSetting.LocAppId}Play");
        ////    appender.AppendLine("{");
        ////    appender.ToRight();
        ////}

        ////private void AppendClassHead()
        ////{
        ////    appender.AppendLine("[YuSingleton]");
        ////    appender.AppendLine($"public class {uiMeta.UIConstScriptId(u3DAppSetting)}");
        ////    appender.AppendLine("{");
        ////    appender.ToRight();
        ////}

        ////private void AppendField()
        ////{
        ////    appender.AppendLine($"#region controlId");
        ////    appender.AppendLine();

        ////    foreach (var rectMeta in uiMeta.RectMetas)
        ////    {
        ////        appender.AppendLine($"public string {rectMeta.PropertyId} " + "{ get; } = " + $"\"{rectMeta.Name}\";");
        ////        appender.AppendLine();
        ////    }

        ////    appender.AppendLine();
        ////    appender.AppendLine("#endregion");
        ////}

        ////#endregion
    }
}