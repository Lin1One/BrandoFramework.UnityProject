namespace Client.UI
{
    /// <summary>
    /// 布局元素接口
    /// </summary>
    public interface ILayoutElement
    {
        // After this method is invoked, layout horizontal input properties should return up-to-date values.
        // Children will already have up-to-date layout horizontal inputs when this methods is called.
        //调用此方法后，布局水平输入属性应返回最新值。
        //调用此方法时，子项已经具有最新的布局水平输入。
        void CalculateLayoutInputHorizontal();

        //调用此方法后，布局垂直输入属性应返回最新值。
        //调用此方法时，子项已经具有最新的布局垂直输入。
        void CalculateLayoutInputVertical();

        // Layout horizontal inputs 水平布局
        float minWidth { get; }
        float preferredWidth { get; }
        float flexibleWidth { get; }
        // Layout vertical inputs 垂直布局
        float minHeight { get; }
        float preferredHeight { get; }
        float flexibleHeight { get; }

        /// <summary>
        /// 布局优先级
        /// </summary>
        int layoutPriority { get; }
    }

    /// <summary>
    /// 布局控件接口
    /// </summary>
    public interface ILayoutController
    {
        void SetLayoutHorizontal();
        void SetLayoutVertical();
    }

    /// <summary>
    /// 布局组，管理自身子物体 Rect
    /// </summary>
    // An ILayoutGroup component should drive the RectTransforms of its children.
    public interface ILayoutGroup : ILayoutController
    {
    }

    /// <summary>
    /// 修改自身 Rect
    /// </summary>
    // An ILayoutSelfController component should drive its own RectTransform.
    public interface ILayoutSelfController : ILayoutController
    {
    }

    /// <summary>
    /// 忽略布局接口
    /// </summary>
    // An ILayoutIgnorer component is ignored by the auto-layout system.
    public interface ILayoutIgnorer
    {
        bool ignoreLayout { get; }
    }
}
