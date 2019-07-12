#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高视图控件交互行为类型。
    /// </summary>
    public enum LegoInteractableType : byte
    {
        None,
        
        OnMove,
        OnPointerDown,
        OnPointerUp,
        OnPointerEnter,
        OnPointerExit,
        OnSelect,
        OnDeselect,
        OnPointerClick,
        OnSubmit,
        OnDrag,
        OnBeginDrag,
        OnEndDrag,

        #region Slider | ProgressBar | InputField

        OnValueChanged,

        #endregion
    }
}