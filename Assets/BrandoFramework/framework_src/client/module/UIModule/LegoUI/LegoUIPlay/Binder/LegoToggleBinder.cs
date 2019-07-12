#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 21:16:31
// Email:             836045613@qq.com

#endregion


using Common.DataStruct;

namespace Client.LegoUI
{
    public class LegoToggleBinder : ILegoBinder
    {
        public void Binding(object control, object model, LegoRectTransformMeta rectMeta)
        {
            var toggle = control as YuLegoToggle;
            var rxModel = model as YuLegoToggleRxModel;
            rxModel.LocControl = toggle;

            //toggle.isOn = rxModel.IsOn.Value; // 初始化赋值

            // 设置可交互状态
            toggle.Interactable = rxModel.LastInteractableValue;

            rxModel.IsOn.BindingControl(toggle, toggle.ReceiveIsOnChange);
            toggle.BindingPush(rxModel.IsOn.ReceiveControlChange);

            // 背景、checkmark、文本
            rxModel.BackgroundSpriteId.BindingControl(toggle, toggle.ReveiveBackgroundChange);
            rxModel.CheckmarkSpriteId.BindingControl(toggle, toggle.ReveiveCheckmarkChange);
            rxModel.TextContent.BindingControl(toggle, toggle.ReveiveTextContentChange);

            toggle.gameObject.SetActive(rectMeta.IsDefaultActive);
        }

        public void UnBinding(object model)
        {
            var rxModel = model.As<YuLegoToggleRxModel>();
            rxModel.IsOn.UnBinding();
            rxModel.TextContent.UnBinding();
            rxModel.BackgroundSpriteId.UnBinding();
            rxModel.CheckmarkSpriteId.UnBinding();
        }
    }
}