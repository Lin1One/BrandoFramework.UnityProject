using Common.DataStruct;
using System;


namespace Client.LegoUI
{
    public interface ILegoRxStruct<T> : IRxStruct<T>
    {
        void BindingControl(ILegoControl control, Action<T> onVChanged);

        void UnBinding();

        /// <summary>
        /// 控制数据模型对应的乐高UI控件是否显示。
        /// </summary>
        /// <param name="state"></param>
        void SetGoActive(bool state);

        #region 推送到检视面板

#if  UNITY_EDITOR

        /// <summary>
        /// 当响应式数据变更时将最新的值绘制到检视面板上。
        /// 仅在编辑器下有效。
        /// </summary>
        /// <param name="drawer"></param>
        void BindingInspectorDrawer(Action<T> drawer);

#endif

        #endregion
    }


    
}