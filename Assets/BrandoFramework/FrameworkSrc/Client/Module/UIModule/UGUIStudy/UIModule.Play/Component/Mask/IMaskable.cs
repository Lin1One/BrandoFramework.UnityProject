using System;

namespace Client.UI
{
    /// <summary>
    /// 可遮挡 UI 图形接口
    /// </summary>
    public interface IMaskable
    {
        void RecalculateMasking();
    }
}
