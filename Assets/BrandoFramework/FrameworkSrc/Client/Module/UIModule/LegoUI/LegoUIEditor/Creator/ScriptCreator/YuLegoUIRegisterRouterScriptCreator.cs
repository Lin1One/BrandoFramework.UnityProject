#region Head

// Author:            Yu
// CreateDate:        2018/9/29 14:29:01
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion


namespace Client.LegoUI.Editor
{
    public class YuLegoUIRegisterRouterScriptCreator
    {
        ////private readonly YuStringAppender appender
        ////    = new YuStringAppender();

        ////private readonly Dictionary<string, Type> viewTypeDict;
        ////private readonly Dictionary<string, Type> componentTypeDict;
        ////private readonly Dictionary<string, Type> scrollViewRxModelTypeDict;

        ////public YuLegoUIRegisterRouterScriptCreator(YuU3dAppSetting u3DApp)
        ////{
        ////    u3DAppSetting = u3DApp;
        ////    var appAsm = YuUnityIOUtility.GetUnityAssembly(u3DApp.PlayAsmId);
        ////    if (appAsm == null)
        ////    {
        ////        throw new Exception($"目标应用{u3DApp.LocAppId}程序集加载失败！");
        ////    }

        ////    viewTypeDict = YuReflectUtility.GetTypeDictionary<ILegoView>(appAsm);
        ////    componentTypeDict = YuReflectUtility.GetTypeDictionary<ILegoComponent>(appAsm);
        ////    scrollViewRxModelTypeDict = YuReflectUtility.GetTypeDictionary<IYuLegoScrollViewRxModel>(appAsm);
        ////}

        ////private readonly YuU3dAppSetting u3DAppSetting;
        ////private string ClassName => $"{u3DAppSetting.LocAppId}_RegisterUIRouter";

        ////public void CreateScript()
        ////{
        ////    appender.AppendLine($"#if {u3DAppSetting.LocAppId}Play");
        ////    AppendScriptHead();
        ////    appender.AppendSingleComment("该脚本由框架自动生成，请勿做任何修改！");
        ////    AppendUsingNamespace();
        ////    AppendClassHead();
        ////    AppendField();
        ////    AppendEntryPoint();
        ////    AppendCreateViewFunctions();
        ////    AppendRegisterView();
        ////    AppendCreateComponentFunctions();
        ////    AppendReigsterComponent();
        ////    AppendCreateRxModel();
        ////    AppendRegisterRxModel();
        ////    appender.AppendCsFooter();
        ////    appender.AppendLine("#endif");
        ////    var path = u3DAppSetting.Helper.CsPlayDir + ClassName + ".cs";
        ////    YuIOUtility.WriteAllText(path, appender.ToString());
        ////}

        ////private void AppendScriptHead()
        ////{
        ////    appender.AppCsNoteHeader();
        ////}

        ////private void AppendUsingNamespace()
        ////{
        ////    appender.AppendUsingNamespace(
        ////        u3DAppSetting.LocAppId + "Play",
        ////        "YuLegoUIPlay",
        ////        "YuPlay"
        ////    );
        ////}

        ////private void AppendClassHead()
        ////{
        ////    appender.AppendLine($"namespace {u3DAppSetting.LocAppId}Play");
        ////    appender.AppendLine("{");
        ////    appender.ToRight();
        ////    appender.AppendLine($"public static class {u3DAppSetting.LocAppId}_RegisterUIRouter");
        ////    appender.AppendLine("{");
        ////    appender.ToRight();
        ////}

        ////private void AppendField()
        ////{
        ////    appender.AppendLine("private static YuLegoRouter legoRouter;");
        ////    appender.AppendLine();
        ////}

        ////private void AppendEntryPoint()
        ////{
        ////    appender.AppendLine("public static void RegisterUIRouter()");
        ////    appender.AppendLine("{");
        ////    appender.ToRight();
        ////    appender.AppendLine($"legoRouter = YuU3dAppUtility.Injector.Get<YuLegoRouter>(\"{u3DAppSetting.LocAppId}/LegoRouter\");");
        ////    appender.AppendLine("ReigsterComponent();");
        ////    appender.AppendLine("ReigsterView();");
        ////    appender.AppendLine("ReigsterRxModel();");
        ////    appender.ToLeft();
        ////    appender.AppendLine();
        ////    appender.AppendLine("}");
        ////    appender.AppendLine();
        ////}

        ////private void AppendRegisterView()
        ////{
        ////    appender.AppendLine("private static void ReigsterView()");
        ////    appender.AppendLine("{");
        ////    appender.ToRight();
        ////    foreach (var kv in viewTypeDict)
        ////    {
        ////        var lastId = kv.Key.Split('_').Last();
        ////        appender.AppendLine($"legoRouter.MapViewFunc(AssetId_{u3DAppSetting.LocAppId}_LegoMeta.LegoView_{lastId},");
        ////        appender.AppendLine($"    Create_{kv.Key});");
        ////    }

        ////    appender.ToLeft();
        ////    appender.AppendLine();
        ////    appender.AppendLine("}");
        ////    appender.AppendLine();
        ////    appender.AppendLine("#endregion");
        ////    appender.AppendLine();
        ////}

        ////private void AppendCreateViewFunctions()
        ////{
        ////    appender.AppendLine("#region View");
        ////    appender.AppendLine();

        ////    foreach (var kv in viewTypeDict)
        ////    {
        ////        appender.AppendLine($"private static IYuLegoView Create_{kv.Key}()");
        ////        appender.AppendLine($"    => new {kv.Key}();");
        ////    }
        ////}

        ////private void AppendReigsterComponent()
        ////{
        ////    appender.AppendLine("private static void ReigsterComponent()");
        ////    appender.AppendLine("{");
        ////    appender.ToRight();
        ////    foreach (var kv in componentTypeDict)
        ////    {
        ////        var lastId = kv.Key.Split('_').Last();
        ////        appender.AppendLine("legoRouter.MapComponentFunc(AssetId_" +
        ////                            $"{u3DAppSetting.LocAppId}_LegoMeta.LegoComponent_{lastId},");
        ////        appender.AppendLine($"    Create_{kv.Key});");
        ////    }

        ////    appender.ToLeft();
        ////    appender.AppendLine();
        ////    appender.AppendLine("}");
        ////    appender.AppendLine();
        ////    appender.AppendLine("#endregion");
        ////    appender.AppendLine();
        ////}

        ////private void AppendCreateComponentFunctions()
        ////{
        ////    appender.AppendLine("#region Component");
        ////    appender.AppendLine();

        ////    foreach (var kv in componentTypeDict)
        ////    {
        ////        appender.AppendLine($"private static IYuLegoComponent Create_{kv.Key}()");
        ////        appender.AppendLine($"    => new {kv.Key}();");
        ////    }

        ////    appender.AppendLine();
        ////}

        ////private void AppendCreateRxModel()
        ////{
        ////    appender.AppendLine("#region RxModel");
        ////    appender.AppendLine();

        ////    foreach (var kv in viewTypeDict)
        ////    {
        ////        appender.AppendLine($"private static IYuLegoUIRxModel Create_{kv.Key}_RxModel()");
        ////        appender.AppendLine($"    => new {kv.Key}_RxModel();");
        ////    }

        ////    appender.AppendLine();

        ////    foreach (var kv in componentTypeDict)
        ////    {
        ////        appender.AppendLine($"private static IYuLegoUIRxModel Create_{kv.Key}_RxModel()");
        ////        appender.AppendLine($"    => new {kv.Key}_RxModel();");
        ////    }

        ////    appender.AppendLine();
        ////}

        ////private void AppendRegisterRxModel()
        ////{
        ////    appender.AppendLine("private static void ReigsterRxModel()");
        ////    appender.AppendLine("{");
        ////    appender.ToRight();
        ////    foreach (var kv in viewTypeDict)
        ////    {
        ////        var lastId = kv.Key.Split('_').Last();
        ////        appender.AppendLine("legoRouter.MapRxModelFunc(AssetId_" +
        ////                            $"{u3DAppSetting.LocAppId}_LegoMeta.LegoView_{lastId},");
        ////        appender.AppendLine($"    Create_{kv.Key}_RxModel);");
        ////    }

        ////    foreach (var kv in componentTypeDict)
        ////    {
        ////        var lastId = kv.Key.Split('_').Last();
        ////        appender.AppendLine("legoRouter.MapRxModelFunc(AssetId_" +
        ////                            $"{u3DAppSetting.LocAppId}_LegoMeta.LegoComponent_{lastId},");
        ////        appender.AppendLine($"    Create_{kv.Key}_RxModel);");
        ////    }

        ////    appender.ToLeft();
        ////    appender.AppendLine();
        ////    appender.AppendLine("}");
        ////    appender.AppendLine();
        ////    appender.AppendLine("#endregion");
        ////    appender.AppendLine();
        ////}

        ////private void AppendCreateScrollRxModel()
        ////{
        ////    appender.AppendLine("#region ScrollView RxModel");
        ////    appender.AppendLine();

        ////    foreach (var kv in scrollViewRxModelTypeDict)
        ////    {
        ////        appender.AppendLine($"private static IYuLegoScrollViewRxModel Create_{kv.Key}_RxModel()");
        ////        appender.AppendLine($"    => new {kv.Key}_RxModel();");
        ////    }

        ////    appender.AppendLine();
        ////}

        ////private void AppendRegisterScrollRxModel()
        ////{
        ////    appender.AppendLine("private static void ReigsterScrollViewRxModel()");
        ////    appender.AppendLine("{");
        ////    appender.ToRight();
        ////    foreach (var kv in scrollViewRxModelTypeDict)
        ////    {
        ////        var rxModelId = GetScrollViewRxModelId(kv.Value.Name);
        ////        appender.AppendLine($"legoRouter.MapScrollViewRxModelFunc({rxModelId},");
        ////        appender.AppendLine($"    Create_{kv.Key}_RxModel);");
        ////    }

        ////    appender.ToLeft();
        ////    appender.AppendLine();
        ////    appender.AppendLine("}");
        ////    appender.AppendLine();
        ////    appender.AppendLine("#endregion");
        ////    appender.AppendLine();
        ////}

        ////private string GetScrollViewRxModelId(string typeName)
        ////{
        ////    // 去掉应用的头部长度
        ////    var app = YuU3dAppSettingDati.CurrentActual;
        ////    var id = typeName.Substring(app.LocAppId.Length, typeName.Length - app.LocAppId.Length);
        ////    id = id.Substring(0, id.Length - 13);
        ////    return id;
        ////}
    }
}