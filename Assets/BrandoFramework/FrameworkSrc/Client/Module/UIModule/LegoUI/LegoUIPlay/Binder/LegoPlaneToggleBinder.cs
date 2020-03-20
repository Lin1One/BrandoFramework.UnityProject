#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 21:16:31
// Email:             836045613@qq.com


#endregion


using Common;

namespace Client.LegoUI
{
    public class LegoPlaneToggleBinder : ILegoBinder
    {
        public void Binding(object control, object model, LegoRectTransformMeta rectMeta)
        {
            var toggle = control as YuLegoPlaneToggle;
            var rxModel = model as YuLegoPlaneToggleRxModel;
            rxModel.LocControl = toggle;

            toggle.IsOn = rxModel.IsOn.Value; // 初始化赋值

            // 设置可交互状态
            toggle.Interactable = rxModel.LastInteractableValue;

            rxModel.IsOn.BindingControl(toggle, toggle.ReceiveIsOnChange);
            toggle.BindingPush(rxModel.IsOn.ReceiveControlChange);

            // 背景、checkmark、文本
            rxModel.FrontSpriteId.BindingControl(toggle, toggle.ReveiveFrontSpriteChange);
            rxModel.BackgroundSpriteId.BindingControl(toggle, toggle.ReveiveBackgroundSpriteChange);

            toggle.GameObject.SetActive(rectMeta.IsDefaultActive);
        }

        public void UnBinding(object model)
        {
            var rxModel = model.As<YuLegoPlaneToggleRxModel>();
            rxModel.FrontSpriteId.UnBinding();
            rxModel.BackgroundSpriteId.UnBinding();
        }
    }
}