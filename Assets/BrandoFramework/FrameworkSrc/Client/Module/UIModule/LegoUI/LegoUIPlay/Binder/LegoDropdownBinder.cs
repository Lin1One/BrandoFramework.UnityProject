#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 21:16:31
// Email:             836045613@qq.com


#endregion

namespace Client.LegoUI
{
    public class LegoDropdownBinder : ILegoBinder
    {
        public void Binding(object control, object model, LegoRectTransformMeta rectMeta)
        {
            var dropdown = control as YuLegoDropdown;
            var rxModel = model as YuLegoDropdownRxModel;
            rxModel.LocControl = dropdown;

            // 设置可交互状态
            dropdown.Interactable = rxModel.LastInteractableValue;

            rxModel.SelectIndex.BindingControl(dropdown, dropdown.ReceiveValueChange);
            dropdown.BindingPush(rxModel.SelectIndex.ReceiveControlChange);

            dropdown.GameObject.SetActive(rectMeta.IsDefaultActive);
        }

        public void UnBinding(object model)
        {
            var rxModel = model as YuLegoDropdownRxModel;

            rxModel.SelectIndex.UnBinding();
        }
    }
}