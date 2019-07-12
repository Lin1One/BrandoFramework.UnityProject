#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 21:16:31
// Email:             836045613@qq.com

#endregion


using Common.DataStruct;

namespace Client.LegoUI
{
    public class LegoTextBinder : ILegoBinder
    {
        public void Binding(object control, object model, LegoRectTransformMeta rectMeta)
        {
            var text = control as YuLegoText;
            var rxModel = model as YuLegoTextRxModel;
            rxModel.LocControl = text;

            rxModel.Content.BindingControl(text, text.ReceiveTextChange);
            rxModel.ColorStr.BindingControl(text, text.ReceiveColorChange);

            text.gameObject.SetActive(rectMeta.IsDefaultActive);
        }

        public void UnBinding(object model)
        {
            var rxModel = model.As<YuLegoTextRxModel>();
            rxModel.Content.UnBinding();
        }
    }
}