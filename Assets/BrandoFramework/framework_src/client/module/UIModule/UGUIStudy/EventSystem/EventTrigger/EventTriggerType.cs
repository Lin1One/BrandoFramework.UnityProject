namespace Client.UI.EventSystem
{
    /// <summary>
    /// This class is capable of triggering one or more remote functions from a specified event.
    /// Usage: Attach it to an object with a collider, or to a GUI Graphic of your choice.
    /// NOTE: Doing this will make this object intercept ALL events, and no event bubbling will occur from this object!
    /// 从指定事件触发一个或多个方法
    /// 用法：附加到含有 Collider 的游戏物体，或选择的 UI 图形
    /// 注意：执行此操作将使此对象拦截所有事件，并且不会从此对象发生事件冒泡！
    /// </summary>

    public enum EventTriggerType
    {
        PointerEnter = 0,
        PointerExit = 1,
        PointerDown = 2,
        PointerUp = 3,
        PointerClick = 4,
        Drag = 5,
        Drop = 6,
        Scroll = 7,
        UpdateSelected = 8,
        Select = 9,
        Deselect = 10,
        Move = 11,
        InitializePotentialDrag = 12,
        BeginDrag = 13,
        EndDrag = 14,
        Submit = 15,
        Cancel = 16
    }
}
