using UnityEngine;

namespace Client.UI
{
    public interface IClipper
    {
        void PerformClipping();
    }

    /// <summary>
    /// 可被 Clip 图形接口（通过放置在 2DMask 下，设置 clip 方法来实现遮罩）
    /// </summary>
    public interface IClippable
    {
        void RecalculateClipping();
        RectTransform rectTransform { get; }
        void Cull(Rect clipRect, bool validRect);
        void SetClipRect(Rect value, bool validRect);
    }
}
