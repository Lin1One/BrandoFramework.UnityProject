using UnityEngine;

namespace Client.UI.EventSystem
{
    public class AxisEventData : BaseEventData
    {
        public Vector2 moveVector { get; set; }
        public MoveDirection moveDir { get; set; }

        public AxisEventData(BrandoEventSystem eventSystem)
            : base(eventSystem)
        {
            moveVector = Vector2.zero;
            moveDir = MoveDirection.None;
        }
    }
}
