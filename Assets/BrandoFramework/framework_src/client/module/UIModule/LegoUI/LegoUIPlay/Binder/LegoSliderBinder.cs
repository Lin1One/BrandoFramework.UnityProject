#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 21:16:31
// Email:             836045613@qq.com

#endregion


using Common.DataStruct;

namespace Client.LegoUI
{
    public class LegoSliderBinder : ILegoBinder
    {
        public void Binding(object control, object model, LegoRectTransformMeta rectMeta)
        {
            var slider = control as YuLegoSlider;
            var rxModel = model as YuLegoSliderRxModel;
            rxModel.LocControl = slider;


            // 设置可交互状态
            slider.Interactable = rxModel.LastInteractableValue;

            rxModel.Direction.BindingControl(slider, slider.ReceiveDirectionChange);
            rxModel.Progress.BindingControl(slider, slider.ReceiveValueChange);
            rxModel.MinValue.BindingControl(slider, slider.ReceiveMinValueChange);
            rxModel.MaxValue.BindingControl(slider, slider.ReceiveMaxValueChange);
            rxModel.IsWholeNumbers.BindingControl(slider, slider.ReceiveIsWholeNumbersChange);

            slider.BindingPush(rxModel.Progress.ReceiveControlChange);

            slider.gameObject.SetActive(rectMeta.IsDefaultActive);
        }

        public void UnBinding(object model)
        {
            var rxModel = model.As<YuLegoSliderRxModel>();
            rxModel.Progress.UnBinding();
        }
    }
}