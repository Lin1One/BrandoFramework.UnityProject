#region Head

// Author:            Yu
// CreateDate:        2018/8/28 10:57:43
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using YuU3dPlay;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高UI控件响应式数据模型接口。
    /// </summary>
    public interface IYuLegoControlRxModel : IRelease
    {
        /// <summary>
        /// 设置数据模型对应的UI控件的激活状态。
        /// </summary>
        /// <param name="state"></param>
        void SetGoActive(bool state);

        /// <summary>
        /// 将给定的数据模型（和自身必须同一类型）的所有值都拷贝到自身。
        /// </summary>
        /// <param name="target"></param>
        void Copy(object target);

        /// <summary>
        /// 数据模型所绑定的控件。
        /// </summary>
        ILegoControl LocControl { get; set; }

        /// <summary>
        /// 从自身的可序列化字段中获取初始值。
        /// </summary>
        void InitFromSerializeField();
    }

    public interface IYuLegoInteractableRxModel
    {
        bool Interactable { get; set; }
    }

    /// <summary>
    /// 可控制控件显示状态数据模型接口（如有需要可整合至 IYuLegoControlRxModel 中）
    /// </summary>
    public interface IYuLegoChangeActivableRxModel
    {
        bool IsControlActive { get; set; }
    }
}