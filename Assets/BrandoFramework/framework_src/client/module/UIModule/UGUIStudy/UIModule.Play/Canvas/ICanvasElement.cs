using UnityEngine;

namespace Client.UI
{
    /// <summary>
    /// ����Ԫ�ؽӿ�
    /// </summary>
    public interface ICanvasElement
    {
        void Rebuild(CanvasUpdateStep executing);

        Transform transform { get; }

        void LayoutComplete();

        void GraphicUpdateComplete();

        // due to unity overriding null check
        // we need this as something may not be null
        // but may be destroyed
        bool IsDestroyed();
    }

}
