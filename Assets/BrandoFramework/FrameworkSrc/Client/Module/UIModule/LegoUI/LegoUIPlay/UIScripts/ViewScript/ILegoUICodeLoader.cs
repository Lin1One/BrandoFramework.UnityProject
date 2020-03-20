using UnityEngine;

namespace Client.LegoUI
{
    public interface ILegoUICodeLoader
    {
        ILegoView GetView(RectTransform uiRect);

        ILegoComponent GetComponent(RectTransform uiRect);

        IViewLogic GetLogic(string uiRect);
    }
}