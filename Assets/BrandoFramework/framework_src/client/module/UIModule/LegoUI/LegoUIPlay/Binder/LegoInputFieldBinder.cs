#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 21:16:31
// Email:             836045613@qq.com

#endregion



using Common;

namespace Client.LegoUI
{
    public class LegoInputFieldBinder : ILegoBinder
    {
        public void Binding(object control, object model, LegoRectTransformMeta rectMeta)
        {
            var inputField = control as YuLegoInputField;
            var rxModel = model as YuLegoInputFieldRxModel;
            rxModel.LocControl = inputField;

            // 设置可交互状态
            inputField.Interactable = rxModel.LastInteractableValue;

            // 双向绑定
            rxModel.Content.BindingControl(inputField, inputField.ReceiveContentChange);
            inputField.BindingPush(rxModel.Content.ReceiveControlChange);
            // 背景绑定
            rxModel.BackgroundSpriteId.BindingControl(inputField, inputField.ReceiveBackgroundChange);

            inputField.GameObject.SetActive(rectMeta.IsDefaultActive);
        }

        public void UnBinding(object model)
        {
            var inputModel = model.As<YuLegoInputFieldRxModel>();
            inputModel.BackgroundSpriteId.UnBinding();
        }
    }
}