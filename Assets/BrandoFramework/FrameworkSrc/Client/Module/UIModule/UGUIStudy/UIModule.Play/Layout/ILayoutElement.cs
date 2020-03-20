namespace Client.UI
{
    /// <summary>
    /// ����Ԫ�ؽӿ�
    /// </summary>
    public interface ILayoutElement
    {
        // After this method is invoked, layout horizontal input properties should return up-to-date values.
        // Children will already have up-to-date layout horizontal inputs when this methods is called.
        //���ô˷����󣬲���ˮƽ��������Ӧ��������ֵ��
        //���ô˷���ʱ�������Ѿ��������µĲ���ˮƽ���롣
        void CalculateLayoutInputHorizontal();

        //���ô˷����󣬲��ִ�ֱ��������Ӧ��������ֵ��
        //���ô˷���ʱ�������Ѿ��������µĲ��ִ�ֱ���롣
        void CalculateLayoutInputVertical();

        // Layout horizontal inputs ˮƽ����
        float minWidth { get; }
        float preferredWidth { get; }
        float flexibleWidth { get; }
        // Layout vertical inputs ��ֱ����
        float minHeight { get; }
        float preferredHeight { get; }
        float flexibleHeight { get; }

        /// <summary>
        /// �������ȼ�
        /// </summary>
        int layoutPriority { get; }
    }

    /// <summary>
    /// ���ֿؼ��ӿ�
    /// </summary>
    public interface ILayoutController
    {
        void SetLayoutHorizontal();
        void SetLayoutVertical();
    }

    /// <summary>
    /// �����飬�������������� Rect
    /// </summary>
    // An ILayoutGroup component should drive the RectTransforms of its children.
    public interface ILayoutGroup : ILayoutController
    {
    }

    /// <summary>
    /// �޸����� Rect
    /// </summary>
    // An ILayoutSelfController component should drive its own RectTransform.
    public interface ILayoutSelfController : ILayoutController
    {
    }

    /// <summary>
    /// ���Բ��ֽӿ�
    /// </summary>
    // An ILayoutIgnorer component is ignored by the auto-layout system.
    public interface ILayoutIgnorer
    {
        bool ignoreLayout { get; }
    }
}
