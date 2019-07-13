#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com

#endregion

using Common;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Client.LegoUI
{
    [Singleton]
    public class LegoBinder
    {
        private readonly Dictionary<Type, ILegoBinder> binderDict
            = new Dictionary<Type, ILegoBinder>();

        public LegoBinder()
        {
            InitBinders();
        }

        private void InitBinders()
        {
            binderDict.Add(typeof(YuLegoText), new LegoTextBinder());
            binderDict.Add(typeof(YuLegoButton), new LegoButtonBinder());
            binderDict.Add(typeof(YuLegoTButton), new LegoTButtonBinder());
            binderDict.Add(typeof(YuLegoImage), new LegoImageBinder());
            binderDict.Add(typeof(YuLegoRawImage), new LegoRawImageBinder());
            binderDict.Add(typeof(YuLegoToggle), new LegoToggleBinder());
            binderDict.Add(typeof(YuLegoPlaneToggle), new LegoPlaneToggleBinder());
            binderDict.Add(typeof(YuLegoSlider), new LegoSliderBinder());
            binderDict.Add(typeof(YuLegoProgressbar), new LegoProgressbarBinder());
            binderDict.Add(typeof(YuLegoInputField), new LegoInputFieldBinder());
            binderDict.Add(typeof(YuLegoDropdown), new LegoDropdownBinder());
        }

        public void Binding(ILegoUI ui, LegoUIMeta uiMeta, IYuLegoUIRxModel uiRxModel)
        {

            var rectMetas = uiMeta.RectMetas;
            var elementTypes = uiMeta.ElementTypes;
            var length = rectMetas.Count;

            for (var i = 0; i < length; i++)
            {
                var id = rectMetas[i].Name;
                var elementType = elementTypes[i];
                var rectMeta = rectMetas[i];

                try
                {
                    BingdngAtElementType(elementType, ui, id, uiRxModel, rectMeta);
                }
                catch (Exception e)
                {
                    Debug.LogError($"在绑定类型为{elementType}Id为{id}的目标控件时发生异常，异常信息为{e.Message + e.StackTrace}！");
                }
            }
        }

        private void TryUnBingdngAtElementType(LegoUIType elementType, ILegoUI ui, string id,
            IYuLegoUIRxModel uiRxModel)
        {

        }

        private void BingdngAtElementType(LegoUIType elementType, ILegoUI ui, string id,
            IYuLegoUIRxModel uiRxModel, LegoRectTransformMeta rectMeta)
        {
            switch (elementType)
            {
                case LegoUIType.Text:
                    BindingText(ui, id, uiRxModel, rectMeta);
                    break;
                case LegoUIType.Image:
                    BindingImage(ui, id, uiRxModel, rectMeta);
                    break;
                case LegoUIType.RawImage:
                    BindingRawImage(ui, id, uiRxModel, rectMeta);
                    break;
                case LegoUIType.Button:
                    BindingButton(ui, id, uiRxModel, rectMeta);
                    break;
                case LegoUIType.TButton:
                    BindingTButton(ui, id, uiRxModel, rectMeta);
                    break;
                case LegoUIType.InputField:
                    BindingInputField(ui, id, uiRxModel, rectMeta);
                    break;
                case LegoUIType.Slider:
                    BindingSlider(ui, id, uiRxModel, rectMeta);
                    break;
                case LegoUIType.Progressbar:
                    BindingProgressbar(ui, id, uiRxModel, rectMeta);
                    break;
                case LegoUIType.Toggle:
                    BindingToggle(ui, id, uiRxModel, rectMeta);
                    break;
                case LegoUIType.Tab:
                    break;
                case LegoUIType.Dropdown:
                    BindingDropdown(ui, id, uiRxModel, rectMeta);
                    break;
                case LegoUIType.Rocker:
                    break;
                case LegoUIType.Grid:
                    break;
                case LegoUIType.ScrollView:
                    break;
                case LegoUIType.None:
                    break;
                case LegoUIType.InlineText:
                    break;
                case LegoUIType.PlaneToggle:
                    BindingPlaneToggle(ui, id, uiRxModel, rectMeta);
                    break;
                case LegoUIType.Component:
                    break;
                case LegoUIType.Container:
                    break;
                case LegoUIType.View:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(elementType), elementType, null);
            }
        }

        #region 具象控件绑定

        private void BindingText(ILegoUI ui, string id, IYuLegoUIRxModel uiRxModel, LegoRectTransformMeta rectMeta)
        {
            var binder = binderDict[typeof(YuLegoText)];

            var oldModel = ui.RxModel.GetControlRxModel<YuLegoTextRxModel>(id);
            binder.UnBinding(oldModel);

            var text = ui.GetControl<YuLegoText>(id);
            var model = uiRxModel.GetControlRxModel<YuLegoTextRxModel>(id);
            binder.Binding(text, model, rectMeta);
        }

        private void BindingImage(ILegoUI ui, string id, IYuLegoUIRxModel uiRxModel, LegoRectTransformMeta rectMeta)
        {
            var binder = binderDict[typeof(YuLegoImage)];

            var oldModel = ui.RxModel.GetControlRxModel<YuLegoImageRxModel>(id);
            if (oldModel != null)
            {
                binder.UnBinding(oldModel);
            }
#if DEBUG
            else
            {
                string uiName = null;
                if (ui?.UIRect != null)
                {
                    uiName = ui.UIRect.name;
                }
                Debug.LogError("BindingImage错误，oldModel为null" + uiName + "  " + id);
            }
#endif

            var image = ui.GetControl<YuLegoImage>(id);
            var model = uiRxModel.GetControlRxModel<YuLegoImageRxModel>(id);
            if(model != null)
            {
                binder.Binding(image, model, rectMeta);
            }
#if DEBUG
            else
            {
                string uiName = null;
                if (ui?.UIRect != null)
                {
                    uiName = ui.UIRect.name;
                }
                Debug.LogError("BindingImage错误，Model为null" + uiName + "  " + id);
            }
#endif
        }

        private void BindingRawImage(ILegoUI ui, string id, IYuLegoUIRxModel uiRxModel, LegoRectTransformMeta rectMeta)
        {
            var binder = binderDict[typeof(YuLegoRawImage)];

            var oldModel = ui.RxModel.GetControlRxModel<YuLegoRawImageRxModel>(id);
            binder.UnBinding(oldModel);

            var rawImage = ui.GetControl<YuLegoRawImage>(id);
            var model = uiRxModel.GetControlRxModel<YuLegoRawImageRxModel>(id);

            binder.Binding(rawImage, model, rectMeta);
        }

        private void BindingButton(ILegoUI ui, string id, IYuLegoUIRxModel uiRxModel, LegoRectTransformMeta rectMeta)
        {
            var binder = binderDict[typeof(YuLegoButton)];

            var oldModel = ui.RxModel.GetControlRxModel<YuLegoButtonRxModel>(id);
            binder.UnBinding(oldModel);

            var button = ui.GetControl<YuLegoButton>(id);
            var model = uiRxModel.GetControlRxModel<YuLegoButtonRxModel>(id);
            binder.Binding(button, model, rectMeta);
        }

        private void BindingTButton(ILegoUI ui, string id, IYuLegoUIRxModel uiRxModel, LegoRectTransformMeta rectMeta)
        {
            var binder = binderDict[typeof(YuLegoTButton)];

            var oldModel = ui.RxModel.GetControlRxModel<YuLegoTButtonRxModel>(id);
            binder.UnBinding(oldModel);

            var button = ui.GetControl<YuLegoTButton>(id);
            var model = uiRxModel.GetControlRxModel<YuLegoTButtonRxModel>(id);

            binder.Binding(button, model, rectMeta);
        }

        private void BindingToggle(ILegoUI ui, string id, IYuLegoUIRxModel uiRxModel, LegoRectTransformMeta rectMeta)
        {
            var binder = binderDict[typeof(YuLegoToggle)];

            var oldModel = ui.RxModel.GetControlRxModel<YuLegoToggleRxModel>(id);
            binder.UnBinding(oldModel);

            var toggle = ui.GetControl<YuLegoToggle>(id);
            var model = uiRxModel.GetControlRxModel<YuLegoToggleRxModel>(id);

            binder.Binding(toggle, model, rectMeta);
        }

        private void BindingPlaneToggle(ILegoUI ui, string id, IYuLegoUIRxModel uiRxModel, LegoRectTransformMeta rectMeta)
        {
            var binder = binderDict[typeof(YuLegoPlaneToggle)];

            var oldModel = ui.RxModel.GetControlRxModel<YuLegoPlaneToggleRxModel>(id);
            binder.UnBinding(oldModel);

            var planeToggle = ui.GetControl<YuLegoPlaneToggle>(id);
            var model = uiRxModel.GetControlRxModel<YuLegoPlaneToggleRxModel>(id);

            binder.Binding(planeToggle, model, rectMeta);
        }

        private void BindingProgressbar(ILegoUI ui, string id, IYuLegoUIRxModel uiRxModel, LegoRectTransformMeta rectMeta)
        {
            var binder = binderDict[typeof(YuLegoProgressbar)];

            var oldModel = ui.RxModel.GetControlRxModel<YuLegoProgressbarRxModel>(id);
            binder.UnBinding(oldModel);

            var progressbar = ui.GetControl<YuLegoProgressbar>(id);
            var model = uiRxModel.GetControlRxModel<YuLegoProgressbarRxModel>(id);
            binder.Binding(progressbar, model, rectMeta);
        }

        private void BindingInputField(ILegoUI ui, string id, IYuLegoUIRxModel uiRxModel, LegoRectTransformMeta rectMeta)
        {
            var binder = binderDict[typeof(YuLegoInputField)];

            var oldModel = ui.RxModel.GetControlRxModel<YuLegoInputFieldRxModel>(id);
            binder.UnBinding(oldModel);

            var inputField = ui.GetControl<YuLegoInputField>(id);
            var model = uiRxModel.GetControlRxModel<YuLegoInputFieldRxModel>(id);

            binder.Binding(inputField, model, rectMeta);
        }

        private void BindingSlider(ILegoUI ui, string id, IYuLegoUIRxModel uiRxModel
            , LegoRectTransformMeta rectMeta)
        {
            var binder = binderDict[typeof(YuLegoSlider)];

            var oldModel = ui.RxModel.GetControlRxModel<YuLegoSliderRxModel>(id);
            binder.UnBinding(oldModel);

            var slider = ui.GetControl<YuLegoSlider>(id);
            var model = uiRxModel.GetControlRxModel<YuLegoSliderRxModel>(id);

            binder.Binding(slider, model, rectMeta);
        }

        private void BindingDropdown(ILegoUI ui, string id, IYuLegoUIRxModel uiRxModel
            , LegoRectTransformMeta rectMeta)
        {
            var binder = binderDict[typeof(YuLegoDropdown)];

            var oldModel = ui.RxModel.GetControlRxModel<YuLegoDropdownRxModel>(id);
            binder.UnBinding(oldModel);

            var dropdown = ui.GetControl<YuLegoDropdown>(id);
            var model = uiRxModel.GetControlRxModel<YuLegoDropdownRxModel>(id);

            binder.Binding(dropdown, model, rectMeta);
        }

        #endregion
    }
}