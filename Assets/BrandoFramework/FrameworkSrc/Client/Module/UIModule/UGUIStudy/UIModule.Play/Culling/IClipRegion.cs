using UnityEngine;

namespace Client.UI
{
    public interface IClipper
    {
        void PerformClipping();
    }

    /// <summary>
    /// �ɱ� Clip ͼ�νӿڣ�ͨ�������� 2DMask �£����� clip ������ʵ�����֣�
    /// </summary>
    public interface IClippable
    {
        void RecalculateClipping();
        RectTransform rectTransform { get; }
        void Cull(Rect clipRect, bool validRect);
        void SetClipRect(Rect value, bool validRect);
    }
}
