#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 21:16:31
// Email:             836045613@qq.com

#endregion


using Common;

namespace Client.LegoUI
{
    public class LegoProgressbarBinder : ILegoBinder
    {
        public void Binding(object control, object model, LegoRectTransformMeta rectMeta)
        {
            var progressbar = control as YuLegoProgressbar;
            var rxModel = model as YuLegoProgressbarRxModel;
            rxModel.LocControl = progressbar;

            // 设置可交互状态
            progressbar.Interactable = rxModel.LastInteractableValue;

            progressbar.Progress = rxModel.Progress.Value;
            rxModel.Progress.BindingControl(progressbar, progressbar.ReceiveProgressChange);

            progressbar.gameObject.SetActive(rectMeta.IsDefaultActive);
        }

        public void UnBinding(object model)
        {
            var rxModel = model.As<YuLegoProgressbarRxModel>();
            rxModel.Progress.UnBinding();
        }
    }
}