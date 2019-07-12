#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 21:16:31
// Email:             836045613@qq.com

#endregion


using Common.DataStruct;

namespace Client.LegoUI
{
    public class LegoTButtonBinder : ILegoBinder
    {
        public void Binding(object control, object model, LegoRectTransformMeta rectMeta)
        {
            var tButton = control as YuLegoTButton;
            var rxModel = model as YuLegoTButtonRxModel;

            rxModel.LocControl = tButton;

            // 初始化赋值给控件
            tButton.SonText.Text = rxModel.TextContent.Value;

            // 设置可交互状态
            tButton.Interactable = rxModel.LastInteractableValue;

            rxModel.BgSpriteId.BindingControl(tButton, tButton.ReceiveBgChange);
            rxModel.IconSpriteId.BindingControl(tButton, tButton.ReceiveIconChange);
            rxModel.TextContent.BindingControl(tButton, tButton.ReceiveTextChange);

            // 设置按钮的默认激活
            tButton.GameObject.SetActive(rectMeta.IsDefaultActive);
        }

        public void UnBinding(object model)
        {
            var rxModel = model.As<YuLegoTButtonRxModel>();
            rxModel.BgSpriteId.UnBinding();
            rxModel.IconSpriteId.UnBinding();
            rxModel.TextContent.UnBinding();
        }
    }
}