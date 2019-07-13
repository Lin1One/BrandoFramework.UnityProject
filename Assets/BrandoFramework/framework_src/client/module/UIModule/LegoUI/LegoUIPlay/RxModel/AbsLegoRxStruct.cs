using Common.DataStruct;
using System;


namespace Client.LegoUI
{
    /// <summary>
    /// 乐高UI响应式数据模型。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbsLegoRxStruct<T> : AbsRxStruct<T>, ILegoRxStruct<T>
    {
        private ILegoControl m_Control;

        public virtual void BindingControl(ILegoControl control, Action<T> onVChanged)
        {
            m_Control = control;
            OnValueChanged = onVChanged;
            OnValueChanged?.Invoke(m_Value);
        }

        public void UnBinding()
        {
            m_Control = null;
            OnValueChanged = null;
        }

        /// <summary>
        /// 控制数据模型对应的乐高UI控件是否显示。
        /// </summary>
        /// <param name="state"></param>
        public void SetGoActive(bool state)
        {
            m_Control?.RectTransform.gameObject.SetActive(state);
        }

        public override void ReceiveControlChange(T newValue)
        {
            base.ReceiveControlChange(newValue);
#if UNITY_EDITOR

            inspectorDrawer?.Invoke(m_Value);

#endif
        }

        #region 推送到检视面板

#if UNITY_EDITOR

        private Action<T> inspectorDrawer;

        /// <summary>
        /// 当响应式数据变更时将最新的值绘制到检视面板上。
        /// 仅在编辑器下有效。
        /// </summary>
        /// <param name="drawer"></param>
        public void BindingInspectorDrawer(Action<T> drawer)
        {
            if (inspectorDrawer != null)
            {
                return;
            }

            inspectorDrawer = drawer;
        }
#endif

        #endregion
    }
}