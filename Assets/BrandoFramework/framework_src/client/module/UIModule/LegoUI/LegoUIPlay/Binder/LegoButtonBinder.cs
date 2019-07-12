#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com

#endregion

using Common.DataStruct;

namespace Client.LegoUI
{
    public class LegoButtonBinder : ILegoBinder
    {
        public void Binding(object control, object model, LegoRectTransformMeta rectMeta)
        {
            var button = control as YuLegoButton;
            var rxModel = model as YuLegoButtonRxModel;
            rxModel.LocControl = button;

            // 设置可交互状态
            button.Interactable = rxModel.LastInteractableValue;

            rxModel.BgSpriteId.BindingControl(button, button.ReceiveBgChange);
            rxModel.TextContent.BindingControl(button.SonText, button.ReceiveTextChange);

            button.GameObject.SetActive(rectMeta.IsDefaultActive);
        }

        public void UnBinding(object model)
        {
            var rxModel = model.As<YuLegoButtonRxModel>();
            rxModel.TextContent.UnBinding();
            rxModel.BgSpriteId.UnBinding();
        }
    }
}