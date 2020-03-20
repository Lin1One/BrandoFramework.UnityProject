#region Head

// Author:            Yu
// CreateDate:        2018/9/6 16:35:09
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System.Collections.Generic;
using System.Linq;


namespace Client.LegoUI.Editor
{
    public class YuLegoContainerScriptCreator : YuAbsLegoScriptCreator
    {
        //#region 静态调用

        ////public static void CreateScript(YuLegoUIMeta uiMeta, YuU3dAppSetting u3DAppSetting)
        ////{
        ////    var creator = new YuLegoContainerScriptCreator(uiMeta, u3DAppSetting);
        ////    creator.CreateScript();
        ////}

        ////private void CreateScript()
        ////{
        ////    CreateGetOperateControl();
        ////    CreateRegisterHandler();
        ////}

        ////#endregion

        ////#region Common

        ////private string ParentClass =>
        ////    UiMeta.RootMeta.Name.StartsWith("LegoView") ? "YuAbsLegoView" : "YuAbsLegoComponent";

        ////private void AppendCommonHead()
        ////{
        ////    Appender.AppCsNoteHeader();
        ////    Appender.AppendUsingNamespace(
        ////        "UnityEngine",
        ////        "Client.LegoUI",
        ////        "YuU3dPlay"
        ////    );

        ////    Appender.AppendSingleComment("该脚本由框架自动生成，请勿做任何修改！！！");
        ////    Appender.AppendLine();
        ////    Appender.AppendLine($"namespace {U3DAppSetting.LocAppId}Play");
        ////    Appender.AppendLine("{");
        ////    Appender.ToRight();
        ////    Appender.AppendLine($"public partial class {U3DAppSetting.LocAppId}_{UiMeta.RootMeta.TypeId} : {ParentClass}");
        ////    Appender.AppendLine("{");
        ////    Appender.ToRight();
        ////}

        ////#endregion

        ////#region GetOperateControl

        ////private void CreateGetOperateControl()
        ////{
        ////    Appender.Clean();
        ////    Appender.AppendPrecomplie(
        ////        U3DAppSetting.LocAppId + "Play"
        ////    );

        ////    Appender.AppendLine();
        ////    AppendCommonHead();
        ////    UiMeta.AppendVCost(Appender, U3DAppSetting);
        ////    AppendDefineControlFiled();
        ////    AppendGetControl();
        ////    Appender.AppendCsFooter();
        ////    Appender.AppendLine("#endif");
        ////    YuIOUtility.WriteAllText(UiMeta.GetOperateControlScriptPath(U3DAppSetting), Appender.ToString());
        ////}

        ////private class ControlInfo
        ////{
        ////    public YuLegoUIType UiType;
        ////    public string Id;

        ////    public ControlInfo(YuLegoUIType type, string id)
        ////    {
        ////        UiType = type;
        ////        Id = id;
        ////    }
        ////}

        ////private readonly Dictionary<YuLegoUIType, List<ControlInfo>> controlInfos
        ////    = new Dictionary<YuLegoUIType, List<ControlInfo>>();

        ////private string porpertyStr = "{ get; private set;}";

        ////private void AppendDefineControlFiled()
        ////{
        ////    var length = UiMeta.ElementTypes.Count;

        ////    for (var i = 0; i < length; i++)
        ////    {
        ////        var et = UiMeta.ElementTypes[i];
        ////        var rect = UiMeta.RectMetas[i];
        ////        if (!controlInfos.ContainsKey(et))
        ////        {
        ////            controlInfos.Add(et, new List<ControlInfo>());
        ////        }

        ////        controlInfos[et].Add(new ControlInfo(et, rect.PropertyId));
        ////    }

        ////    foreach (var kv in controlInfos)
        ////    {
        ////        Appender.AppendLine($"#region {kv.Key}");
        ////        Appender.AppendLine();

        ////        foreach (var controlInfo in kv.Value)
        ////        {
        ////            var lowerId = controlInfo.Id.ToLower();
        ////            var getMethodStr = string.Empty;
        ////            var controlType = string.Empty;

        ////            switch (controlInfo.UiType)
        ////            {
        ////                case YuLegoUIType.Component:
        ////                    controlType = "IYuLegoUI";
        ////                    getMethodStr = "GetSonComponent";
        ////                    break;
        ////                case YuLegoUIType.Container:
        ////                    controlType = "RectTransform";
        ////                    getMethodStr = "GetContainer";
        ////                    break;
        ////                default:
        ////                    controlType = "IYuLego" + controlInfo.UiType.ToString();
        ////                    getMethodStr = $"GetControlAndConstruct<{controlType}>";
        ////                    break;
        ////            }

        ////            Appender.AppendLine($"private {controlType} {lowerId};");
        ////            Appender.AppendLine($"public {controlType} {controlInfo.Id}");
        ////            Appender.AppendLine("{");
        ////            Appender.ToRight();
        ////            Appender.AppendLine("get");
        ////            Appender.AppendLine("{");
        ////            Appender.ToRight();
        ////            Appender.AppendLine($"if ({lowerId} != null)");
        ////            Appender.AppendLine("{");
        ////            Appender.ToRight();
        ////            Appender.AppendLine($"return {lowerId};");
        ////            Appender.ToLeft();
        ////            Appender.AppendLine("}");
        ////            Appender.AppendLine();

        ////            Appender.AppendLine($"{lowerId} = {getMethodStr}(vConst.{controlInfo.Id});");
        ////            Appender.AppendLine($"return {lowerId};");
        ////            Appender.ToLeft();
        ////            Appender.AppendLine("}");
        ////            Appender.ToLeft();
        ////            Appender.AppendLine("}");
        ////            Appender.AppendLine();
        ////        }

        ////        Appender.AppendLine();
        ////        Appender.AppendLine("#endregion");
        ////        Appender.AppendLine();
        ////    }

        ////    Appender.AppendLine();
        ////}

        ////private void AppendGetControl()
        ////{
        ////    Appender.AppendLine($"protected override void GetOperateControl()");
        ////    Appender.AppendLine("{");
        ////    Appender.ToRight();

        ////    //foreach (var kv in controlInfos)
        ////    //{
        ////    //    Appender.AppendLine($"#region {kv.Key}");
        ////    //    Appender.AppendLine();

        ////    //    foreach (var controlInfo in kv.Value)
        ////    //    {
        ////    //        if (controlInfo.UiType == YuLegoUIType.Component)
        ////    //        {
        ////    //            continue;
        ////    //        }
        ////    //        else if (controlInfo.UiType == YuLegoUIType.Container)
        ////    //        {
        ////    //            Appender.AppendLine($"{controlInfo.Id} " +
        ////    //            $"= GetContainer(vConst.{controlInfo.Id});");
        ////    //        }
        ////    //        else
        ////    //        {
        ////    //            Appender.AppendLine($"{controlInfo.Id} " +
        ////    //             $"= GetControlAndConstruct<YuLego{controlInfo.UiType}>" +
        ////    //             $"(vConst.{controlInfo.Id});");
        ////    //        }
        ////    //    }

        ////    //    Appender.AppendLine();
        ////    //    Appender.AppendLine("#endregion");
        ////    //    Appender.AppendLine();
        ////    //}

        ////    Appender.ToLeft();
        ////    Appender.AppendLine("}");
        ////    Appender.AppendLine();
        ////}

        ////#endregion

        ////#region RegisterHandler

        ////private void CreateRegisterHandler()
        ////{
        ////    Appender.Clean();
        ////    Appender.AppendPrecomplie(
        ////        U3DAppSetting.LocAppId + "Play"
        ////    );

        ////    Appender.AppendLine();
        ////    AppendCommonHead();
        ////    AppendRegisterHandler();
        ////    Appender.AppendCsFooter();
        ////    Appender.AppendLine("#endif");
        ////    YuIOUtility.WriteAllText(UiMeta.RegisterHandlerScriptPath(U3DAppSetting), Appender.ToString());
        ////}

        ////private readonly HashSet<YuLegoUIType> noInteractableTypes
        ////    = new HashSet<YuLegoUIType>
        ////    {
        ////        YuLegoUIType.Text,
        ////        YuLegoUIType.Image,
        ////        YuLegoUIType.RawImage
        ////    };

        ////private void AppendRegisterHandler()
        ////{
        ////    Appender.AppendLine("protected override void RegisterHandler()");
        ////    Appender.AppendLine("{");
        ////    Appender.ToRight();

        ////    foreach (var kv in controlInfos)
        ////    {
        ////        if (noInteractableTypes.Contains(kv.Key))
        ////        {
        ////            continue;
        ////        }

        ////        Appender.AppendLine($"#region {kv.Key}");
        ////        Appender.AppendLine();

        ////        foreach (var info in kv.Value)
        ////        {
        ////            var legoType = info.UiType;
        ////            var interactableType = string.Empty;

        ////            switch (legoType)
        ////            {
        ////                case YuLegoUIType.Button:
        ////                case YuLegoUIType.TButton:
        ////                case YuLegoUIType.PlaneToggle:
        ////                    interactableType = "YuLegoInteractableType.OnPointerClick";
        ////                    break;
        ////                case YuLegoUIType.InputField:
        ////                    interactableType = "YuLegoInteractableType.OnValueChanged";
        ////                    break;
        ////                case YuLegoUIType.Slider:
        ////                    interactableType = "YuLegoInteractableType.OnValueChanged";
        ////                    break;
        ////                case YuLegoUIType.Progressbar:
        ////                    interactableType = "YuLegoInteractableType.OnValueChanged";
        ////                    break;
        ////                case YuLegoUIType.Toggle:
        ////                    interactableType = "YuLegoInteractableType.OnValueChanged";
        ////                    break;
        ////                case YuLegoUIType.Tab:
        ////                    break;
        ////                case YuLegoUIType.Dropdown:
        ////                    break;
        ////                case YuLegoUIType.Rocker:
        ////                    break;
        ////                case YuLegoUIType.Grid:
        ////                    break;
        ////                case YuLegoUIType.ScrollView:
        ////                    break;
        ////            }

        ////            if (!string.IsNullOrEmpty(interactableType))
        ////            {
        ////                var handlerFullId = $"{U3DAppSetting.LocAppId}_{UiMeta.RootMeta.TypeId}_{info.Id}" +
        ////                                    $"_{interactableType.Split('.').Last()}";
        ////                YuLegoHandlerScriptCreator.CreateHandlerScript(UiMeta, U3DAppSetting, handlerFullId, info.Id);
        ////                Appender.AppendLine($"{info.Id}.RegisterHandler({interactableType},");
        ////                Appender.AppendLine($"    YuU3dAppUtility.Injector.Get<{handlerFullId}>());");
        ////            }
        ////        }

        ////        Appender.AppendLine();
        ////        Appender.AppendLine("#endregion");
        ////        Appender.AppendLine();
        ////    }

        ////    Appender.ToLeft();
        ////    Appender.AppendLine("}");
        ////}

        ////#endregion

        ////private YuLegoContainerScriptCreator(LegoUIMeta uiMeta, YuU3dAppSetting u3DAppSetting) : base(uiMeta, u3DAppSetting)
        ////{
        public YuLegoContainerScriptCreator(LegoUIMeta uiMeta) : base(uiMeta)
        {
        }
    }

}