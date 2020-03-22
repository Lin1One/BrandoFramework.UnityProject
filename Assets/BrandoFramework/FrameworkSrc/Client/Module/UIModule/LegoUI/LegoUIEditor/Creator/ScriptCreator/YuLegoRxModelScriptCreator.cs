using Common;
using System;
using UnityEngine;

namespace Client.LegoUI.Editor
{
    /// <summary>
    /// 乐高UI数据模型脚本创建器。
    /// </summary>
    public class YuLegoRxModelScriptCreator
    {
        private readonly StringAppender appender
            = new StringAppender();
        private readonly LegoUIMeta uiMeta;
        //private readonly YuU3dAppSetting u3DAppSetting;
        private readonly RectTransform uiRect;
        //private string ScriptId => uiMeta.UIRxModel_OriginId(u3DAppSetting);

        private YuLegoRxModelScriptCreator(LegoUIMeta uiMeta, RectTransform uiRect)
        {
            this.uiMeta = uiMeta;
            //this.u3DAppSetting = u3DAppSetting;
            this.uiRect = uiRect;
        }

        public static void CreateScript(LegoUIMeta uimeta, RectTransform uiRect)
        {
            var creator = new YuLegoRxModelScriptCreator(uimeta, uiRect);
            //creator.CreateInterface();
            creator.CreateScript();
        }

        private void CreateScript()
        {
            CreateOriginMain();
            //CreateOriginExtend();
        }

        private void CreateOriginMain()
        {
            appender.Clean();
            //appender.AppendLine($"#if {u3DAppSetting.LocAppId}Play");
            //AppendHead();
            //AppendVConst();
            //AppendRxModelPropertyDefine();
            //AppendInitRxModel();
            //appender.AppendCsFooter();
            //appender.AppendLine("#endif");
            //var content = appender.ToString();
            //YuIOUtility.WriteAllText(uiMeta.OriginModelScriptPath2(u3DAppSetting), content);
        }

        //private void AppendHead()
        //{
        //    appender.AppCsNoteHeader();
        //    appender.AppendUsingNamespace(
        //        "Sirenix.OdinInspector",
        //        "UnityEngine",
        //        "Client.LegoUI",
        //        "YuU3dPlay"
        //    );

        //    appender.AppendSingleComment("该脚本由框架自动生成，请勿做任何修改！！！");
        //    appender.AppendLine();

        //    //appender.AppendLine($"namespace {u3DAppSetting.LocAppId}Play");
        //    appender.AppendLine("{");
        //    appender.ToRight();
        //    appender.AppendLine("[System.Serializable]");

        //    //appender.AppendLine(
        //    //    $"public partial class {uiMeta.UIRxModel_OriginId(u3DAppSetting)} : YuLegoUIRxModel, " +
        //    //    $"{uiMeta.UIRxModel_InterfaceId(u3DAppSetting)}"
        //    //    );

        //    appender.AppendLine("{");
        //    appender.ToRight();
        //}

        //private void AppendVConst()
        //{
        //    appender.AppendLine($"private {uiMeta.UIConstScriptId(u3DAppSetting)} vConst");
        //    appender.AppendLine($"    => YuU3dAppUtility.Injector.Get<{uiMeta.UIConstScriptId(u3DAppSetting)}>();");
        //    appender.AppendLine();
        //}

        //private void AppendRxModelPropertyDefine()
        //{
        //    appender.AppendLine("#region RxModel Propertys");
        //    appender.AppendLine();

        //    var length = uiMeta.ElementTypes.Count;

        //    for (var i = 0; i < length; i++)
        //    {
        //        var eType = uiMeta.ElementTypes[i];
        //        var rectMeta = uiMeta.RectMetas[i];
        //        if (eType == YuLegoUIType.Component || eType == YuLegoUIType.Container
        //            || eType == YuLegoUIType.ScrollView)
        //        {
        //            continue;
        //        }

        //        appender.AppendLine($"[FoldoutGroup(\"{rectMeta.PropertyId.ToStartLower()}\")]");
        //        appender.AppendLine("[SerializeField]");
        //        appender.AppendLine($"private YuLego{eType}RxModel {rectMeta.PropertyId.ToStartLower()};");
        //        appender.AppendLine($"public IYuLego{eType}RxModel {rectMeta.PropertyId}_RxModel =>  {rectMeta.PropertyId.ToStartLower()};");
        //        appender.AppendLine();
        //    }

        //    appender.AppendLine("#endregion");
        //    appender.AppendLine();
        //}

        //private void AppendInitRxModel()
        //{
        //    appender.AppendLine("public override void InitRxModel()");
        //    appender.AppendLine("{");
        //    appender.ToRight();

        //    var length = uiMeta.ElementTypes.Count;

        //    for (var i = 0; i < length; i++)
        //    {
        //        var uiType = uiMeta.ElementTypes[i];
        //        var rectMeta = uiMeta.RectMetas[i];
        //        if (uiType == YuLegoUIType.Component || uiType == YuLegoUIType.Container || uiType == YuLegoUIType.ScrollView)
        //        {
        //            if (uiType == YuLegoUIType.ScrollView)
        //            {
        //                YuLegoScrollViewRxModelScriptCreator.CreateScript(rectMeta, u3DAppSetting);
        //            }

        //            continue;
        //        }

        //        var finalType = GetRxModelTypeStrMono(uiType, rectMeta);
        //        var fieldId = rectMeta.PropertyId.ToStartLower();

        //        appender.AppendLine(
        //            $"{fieldId} =" +
        //            $" AddControlRxModel<{finalType}>(vConst.{rectMeta.PropertyId});"
        //            );

        //        if (!rectMeta.IsDefaultActive) // 设置目标控件的默认显示状态
        //        {

        //        }

        //        AppendModelDefaultValueAtUiType(uiType, rectMeta, fieldId);
        //        appender.AppendLine();
        //    }

        //    appender.ToLeft();
        //    appender.AppendLine("}");
        //}

        //private void AppendModelDefaultValueAtUiType(YuLegoUIType type, YuLegoRectTransformMeta rectMeta
        //, string fieldId)
        //{
        //    var control = uiRect.Find(rectMeta.Name);
        //    switch (type)
        //    {
        //        case YuLegoUIType.None:
        //            break;
        //        case YuLegoUIType.Text:
        //            var text = control.GetComponent<YuLegoText>();
        //            appender.AppendLine($"{fieldId}.Content.Value = \"{text.Text}\";");
        //            break;
        //        case YuLegoUIType.Image:
        //            var image = control.GetComponent<YuLegoImage>();
        //            appender.AppendLine($"{fieldId}.SpriteId.Value = \"{image.sprite.name}\";");
        //            break;
        //        case YuLegoUIType.RawImage:
        //            var rawImage = control.GetComponent<YuLegoRawImage>();
        //            appender.AppendLine($"{fieldId}.TextureId.Value = \"{rawImage.texture.name}\";");
        //            break;
        //        case YuLegoUIType.Button:
        //            var button = control.GetComponent<YuLegoButton>();
        //            appender.AppendLine($"{fieldId}.BgSpriteId.Value = \"{button.BgImage.SpriteId}\";");
        //            appender.AppendLine($"{fieldId}.TextContent.Value = \"{button.ButtonContent.Text}\";");
        //            break;
        //        case YuLegoUIType.TButton:
        //            var tbutton = control.GetComponent<YuLegoTButton>();
        //            appender.AppendLine($"{fieldId}.BgSpriteId.Value = \"{tbutton.BgImage.SpriteId}\";");
        //            appender.AppendLine($"{fieldId}.TextContent.Value = \"{tbutton.ButtonContent.Text}\";");
        //            appender.AppendLine($"{fieldId}.IconSpriteId.Value = \"{tbutton.IconImage.SpriteId}\";");
        //            break;
        //        case YuLegoUIType.InputField:
        //            var input = control.GetComponent<YuLegoInputField>();
        //            appender.AppendLine($"{fieldId}.BackgroundSpriteId.Value = \"{input.BackGroundImage.SpriteId}\";");
        //            appender.AppendLine($"{fieldId}.HolderContent.Value = \"{input.PlaceHolder}\";");
        //            appender.AppendLine($"{fieldId}.Content.Value = \"{input.Text}\";");
        //            break;
        //        case YuLegoUIType.Slider:
        //            var slider = control.GetComponent<YuLegoSlider>();
        //            appender.AppendLine($"{fieldId}.Progress.Value = {slider.Progress}f;");
        //            appender.AppendLine($"{fieldId}.Direction.Value = {(byte)slider.direction};");
        //            appender.AppendLine($"{fieldId}.MinValue.Value = {slider.minValue}f;");
        //            appender.AppendLine($"{fieldId}.MaxValue.Value = {slider.maxValue}f;");
        //            appender.AppendLine($"{fieldId}.IsWholeNumbers.Value = {slider.wholeNumbers.ToString().ToLower()};");
        //            break;
        //        case YuLegoUIType.Progressbar:
        //            var progressbar = control.GetComponent<YuLegoProgressbar>();
        //            appender.AppendLine($"{fieldId}.Progress.Value = {progressbar.Progress}f;");
        //            break;
        //        case YuLegoUIType.Toggle:
        //            var toggle = control.GetComponent<YuLegoToggle>();
        //            appender.AppendLine($"{fieldId}.IsOn.Value = {toggle.IsOn.ToString().ToLower()};");
        //            appender.AppendLine($"{fieldId}.BackgroundSpriteId.Value = \"{toggle.BackgroundImage.sprite.name}\";");
        //            appender.AppendLine($"{fieldId}.CheckmarkSpriteId.Value = \"{toggle.CheckmarkImage.SpriteId}\";");
        //            appender.AppendLine($"{fieldId}.TextContent.Value = \"{toggle.transform.Find("Text").GetComponent<YuLegoText>().Text}\";");
        //            break;
        //        case YuLegoUIType.PlaneToggle:
        //            var planeToggle = control.GetComponent<YuLegoPlaneToggle>();
        //            appender.AppendLine($"{fieldId}.IsOn.Value = {planeToggle.IsOn.ToString().ToLower()};");
        //            appender.AppendLine($"{fieldId}.BackgroundSpriteId.Value = \"{planeToggle.FrontImage.SpriteId}\";");
        //            break;
        //        case YuLegoUIType.Dropdown:
        //            break;
        //        case YuLegoUIType.Rocker:
        //            break;
        //        case YuLegoUIType.ScrollView:
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(type), type, null);
        //    }
        //}

        //private string GetRxModelTypeStrMono(YuLegoUIType type, YuLegoRectTransformMeta rectMeta)
        //{
        //    return $"YuLego{type}RxModel";
        //}

        //private void AppendCommonHead()
        //{
        //    appender.AppCsNoteHeader();
        //    appender.AppendUsingNamespace(
        //        "Sirenix.OdinInspector",
        //        "UnityEngine",
        //        "Client.LegoUI",
        //        "YuU3dPlay"
        //    );

        //    appender.AppendSingleComment("该脚本由框架自动生成，请勿做任何修改！！！");
        //    appender.AppendLine();

        //    appender.AppendLine($"namespace {u3DAppSetting.LocAppId}Play");
        //    appender.AppendLine("{");
        //    appender.ToRight();
        //}

        //#region InterfaceScript

        //private void CreateInterface()
        //{
        //    CreateInterfaceMain();
        //    CreateInterfaceExtend();
        //}

        //private void CreateInterfaceMain()
        //{
        //    appender.Clean();
        //    appender.AppendLine($"#if {u3DAppSetting.LocAppId}Play");
        //    AppendCommonHead();

        //    appender.AppendLine($"public partial interface {uiMeta.UIRxModel_InterfaceId(u3DAppSetting)} : IYuLegoUIRxModel");
        //    appender.AppendLine("{");
        //    appender.ToRight();

        //    var length = uiMeta.ElementTypes.Count;

        //    for (var i = 0; i < length; i++)
        //    {
        //        var eType = uiMeta.ElementTypes[i];
        //        var rectMeta = uiMeta.RectMetas[i];
        //        if (eType == YuLegoUIType.Component || eType == YuLegoUIType.Container
        //            || eType == YuLegoUIType.ScrollView)
        //        {
        //            continue;
        //        }

        //        appender.AppendLine($"IYuLego{eType}RxModel {rectMeta.PropertyId}_RxModel " + "{ get; }");
        //        appender.AppendLine();
        //    }

        //    appender.AppendCsFooter();
        //    appender.AppendLine($"#endif");
        //    YuIOUtility.WriteAllText(uiMeta.InterfaceModelScriptPath(u3DAppSetting), appender.ToString());
        //}

        //private void CreateInterfaceExtend()
        //{
        //    appender.Clean();
        //    appender.AppendLine($"#if {u3DAppSetting.LocAppId}Play");
        //    AppendCommonHead();
        //    appender.AppendSingleComment("// 这是一个分部脚本，可以在这里扩展UI层次结构无法直接映射的业务逻辑所需的数据模型属性！");

        //    appender.AppendLine($"public partial interface {uiMeta.UIRxModel_InterfaceId(u3DAppSetting)} : IYuLegoUIRxModel");
        //    appender.AppendLine("{");

        //    appender.AppendCsFooter();
        //    appender.AppendLine("#endif");
        //    YuIOUtility.TryWriteAllText(uiMeta.InterfaceModelScriptPath_Extend(u3DAppSetting), appender.ToString());
        //}

        //private void CreateOriginExtend()
        //{
        //    appender.Clean();
        //    appender.AppendLine($"#if {u3DAppSetting.LocAppId}Play");
        //    AppendCommonHead();
        //    appender.AppendSingleComment("// 这是一个分部脚本，可以在这里扩展UI层次结构无法直接映射的业务逻辑所需的数据模型属性！");

        //    appender.AppendLine($"public partial class {uiMeta.UIRxModel_OriginId(u3DAppSetting)} : IYuLegoUIRxModel");
        //    appender.AppendLine("{");

        //    appender.AppendCsFooter();
        //    appender.AppendLine("#endif");
        //    YuIOUtility.TryWriteAllText(uiMeta.OriginModelScriptPath_Extend(u3DAppSetting), appender.ToString());
        //}

        //#endregion
    }
}
