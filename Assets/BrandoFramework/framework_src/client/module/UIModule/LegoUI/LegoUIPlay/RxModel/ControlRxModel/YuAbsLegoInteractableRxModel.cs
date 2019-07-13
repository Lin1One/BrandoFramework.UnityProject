namespace Client.LegoUI
{
    public abstract class YuAbsLegoInteractableRxModel : IYuLegoInteractableRxModel
    {

        private IYuLegoInteractableControl interactableControl;
        public ILegoControl LocControl { get; set; }

        /// <summary>
        /// 上次所设置的可交互开关状态。
        /// 记录下来用于在数据模型控件绑定时对控件赋值。
        /// </summary>
        public bool LastInteractableValue { get; private set; } = true;

        public bool Interactable
        {
            get
            {
                if (interactableControl == null)
                {
                    interactableControl = LocControl as IYuLegoInteractableControl;
                }

                if (interactableControl != null)
                {
                    return interactableControl.Interactable;
                }

                return false;
            }
            set
            {
                LastInteractableValue = value;

                if (interactableControl == null)
                {
                    interactableControl = LocControl as IYuLegoInteractableControl;
                }

                if (interactableControl != null)
                {
                    interactableControl.Interactable = value;
                }
            }
        }
    }
}