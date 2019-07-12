
using UnityEngine.EventSystems;

namespace Client.LegoUI
{
    public interface IYuLegoRocker : ILegoControl, IDragHandler,
        IEndDragHandler
    {
    }
}
