using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Client.UI.EventSystem
{
    /// <summary>
    /// Each touch event creates one of these containing all the relevant information.
    /// ÿ�������¼����ᴴ������һ���������������Ϣ���¼���
    /// </summary>
    public class PointerEventData : BaseEventData
    {
        public enum InputButton
        {
            Left = 0,
            Right = 1,
            Middle = 2
        }

        //���״̬
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
        /// ���ܵ�� Down ��������Ϸ����
        /// </summary>
        private GameObject m_PointerPress;
        /// <summary>
        /// ���
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
        /// �����ܵ�� Down ��������Ϸ����
        /// </summary>
        public GameObject lastPress { get; private set; }

        /// <summary>
        /// The object that the press happened on even if it can not handle the press event
        /// ����������޷�������������
        /// /// </summary>
        public GameObject rawPointerPress { get; set; }


        /// <summary>
        /// The object that received OnDrag
        /// ������ק����Ϸ����
        /// </summary>
        public GameObject pointerDrag { get; set; }

        /// <summary>
        /// ָ��ĵ�ǰ������Ϣ
        /// </summary>
        public RaycastResult pointerCurrentRaycast { get; set; }

        /// <summary>
        /// ָ��ĵ��������Ϣ
        /// </summary>
        public RaycastResult pointerPressRaycast { get; set; }

        public List<GameObject> hovered = new List<GameObject>();

        /// <summary>
        /// �Ƿ�ɵ��
        /// </summary>
        public bool eligibleForClick { get; set; }

        public int pointerId { get; set; }


        /// <summary>
        /// ��꣬������λ��
        /// </summary>
        public Vector2 position { get; set; }

        /// <summary>
        /// ��꣬������λ�þ���һ֡ƫ��
        /// </summary>
        public Vector2 delta { get; set; }

        /// <summary>
        /// ��꣬��������λ��
        /// </summary>
        public Vector2 pressPosition { get; set; }

        /// <summary>
        /// ��һ�ε���¼�����ʱ�䣨����˫����
        /// </summary>
        public float clickTime { get; set; }

        /// <summary>
        /// ���������2 Ϊ˫��
        /// </summary>
        // Number of clicks in a row. 2 for a double-click for example.
        public int clickCount { get; set; }

        /// <summary>
        /// �������
        /// </summary>
        public Vector2 scrollDelta { get; set; }

        /// <summary>
        /// �Ƿ�ʹ����ק��ֵ
        /// </summary>
        public bool useDragThreshold { get; set; }

        /// <summary>
        /// ������ק
        /// </summary>
        public bool dragging { get; set; }

        /// <summary>
        /// ��ť����
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
