using UnityEngine;

namespace Client.UI
{
    /// <summary>
    /// 画布元素接口
    /// </summary>
    public interface ICanvasElement
    {
        void Rebuild(CanvasUpdate executing);

        Transform transform { get; }

        void LayoutComplete();

        void GraphicUpdateComplete();

        // due to unity overriding null check
        // we need this as something may not be null
        // but may be destroyed
        bool IsDestroyed();
    }

}
