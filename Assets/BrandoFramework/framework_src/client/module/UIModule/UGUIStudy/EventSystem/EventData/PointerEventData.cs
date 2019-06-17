using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Client.UI.EventSystem
{
    /// <summary>
    /// Each touch event creates one of these containing all the relevant information.
    /// 每个触摸事件都会创建其中一个包含所有相关信息的事件。
    /// </summary>
    public class PointerEventData : BaseEventData
    {
        public enum InputButton
        {
            Left = 0,
            Right = 1,
            Middle = 2
        }

        //点击状态
        public enum FramePressState
        {
            Pressed,
            Released,
            PressedAndReleased,
            NotChanged
        }

        public GameObject pointerEnter { get; set; }

        /// <summary>
        /// The object that received OnPointerDown
        /// 接受点击 Down 操作的游戏物体
        /// </summary>
        private GameObject m_PointerPress;
        /// <summary>
        /// 点击
        /// </summary>
        public GameObject pointerPress
        {
            get { return m_PointerPress; }
            set
            {
                if (m_PointerPress == value)
                    return;

                lastPress = m_PointerPress;
                m_PointerPress = value;
            }
        }

        /// <summary>
        /// The object last received OnPointerDown
        /// 最后接受点击 Down 操作的游戏物体
        /// </summary>
        public GameObject lastPress { get; private set; }

        /// <summary>
        /// The object that the press happened on even if it can not handle the press event
        /// 点击发生但无法处理点击的物体
        /// /// </summary>
        public GameObject rawPointerPress { get; set; }


        /// <summary>
        /// The object that received OnDrag
        /// 接受拖拽的游戏物体
        /// </summary>
        public GameObject pointerDrag { get; set; }

        /// <summary>
        /// 指针的当前射线信息
        /// </summary>
        public RaycastResult pointerCurrentRaycast { get; set; }

        /// <summary>
        /// 指针的点击射线信息
        /// </summary>
        public RaycastResult pointerPressRaycast { get; set; }

        public List<GameObject> hovered = new List<GameObject>();

        /// <summary>
        /// 是否可点击
        /// </summary>
        public bool eligibleForClick { get; set; }

        public int pointerId { get; set; }


        /// <summary>
        /// 鼠标，或触碰点位置
        /// </summary>
        public Vector2 position { get; set; }

        /// <summary>
        /// 鼠标，或触碰点位置距上一帧偏移
        /// </summary>
        public Vector2 delta { get; set; }

        /// <summary>
        /// 鼠标，或触碰点点击位置
        /// </summary>
        public Vector2 pressPosition { get; set; }

        /// <summary>
        /// 上一次点击事件发生时间（用于双击）
        /// </summary>
        public float clickTime { get; set; }

        /// <summary>
        /// 点击次数，2 为双击
        /// </summary>
        // Number of clicks in a row. 2 for a double-click for example.
        public int clickCount { get; set; }

        /// <summary>
        /// 滚动间距
        /// </summary>
        public Vector2 scrollDelta { get; set; }

        /// <summary>
        /// 是否使用拖拽阈值
        /// </summary>
        public bool useDragThreshold { get; set; }

        /// <summary>
        /// 正在拖拽
        /// </summary>
        public bool dragging { get; set; }

        /// <summary>
        /// 按钮类型
        /// </summary>
        public InputButton button { get; set; }

        public PointerEventData(BrandoEventSystem eventSystem) : base(eventSystem)
        {
            eligibleForClick = false;

            pointerId = -1;
            position = Vector2.zero; // Current position of the mouse or touch event
            delta = Vector2.zero; // Delta since last update
            pressPosition = Vector2.zero; // Delta since the event started being tracked
            clickTime = 0.0f; // The last time a click event was sent out (used for double-clicks)
            clickCount = 0; // Number of clicks in a row. 2 for a double-click for example.

            scrollDelta = Vector2.zero;
            useDragThreshold = true;
            dragging = false;
            button = InputButton.Left;
        }

        public bool IsPointerMoving()
        {
            return delta.sqrMagnitude > 0.0f;
        }

        public bool IsScrolling()
        {
            return scrollDelta.sqrMagnitude > 0.0f;
        }

        public Camera enterEventCamera
        {
            get { return pointerCurrentRaycast.module == null ? null : pointerCurrentRaycast.module.eventCamera; }
        }

        public Camera pressEventCamera
        {
            get { return pointerPressRaycast.module == null ? null : pointerPressRaycast.module.eventCamera; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<b>Position</b>: " + position);
            sb.AppendLine("<b>delta</b>: " + delta);
            sb.AppendLine("<b>eligibleForClick</b>: " + eligibleForClick);
            sb.AppendLine("<b>pointerEnter</b>: " + pointerEnter);
            sb.AppendLine("<b>pointerPress</b>: " + pointerPress);
            sb.AppendLine("<b>lastPointerPress</b>: " + lastPress);
            sb.AppendLine("<b>pointerDrag</b>: " + pointerDrag);
            sb.AppendLine("<b>Use Drag Threshold</b>: " + useDragThreshold);
            sb.AppendLine("<b>Current Rayast:</b>");
            sb.AppendLine(pointerCurrentRaycast.ToString());
            sb.AppendLine("<b>Press Rayast:</b>");
            sb.AppendLine(pointerPressRaycast.ToString());
            return sb.ToString();
        }
    }
}
