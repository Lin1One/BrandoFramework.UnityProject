using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Client.UI.EventSystem
{
    /// <summary>
    /// ͼ�����߷�����--UGUI ������Ϊ������
    /// </summary>
    [AddComponentMenu("Event/Graphic Raycaster")]
    [RequireComponent(typeof(Canvas))]
    public class BrandoGraphicRaycaster : BaseRaycaster
    {
        protected const int kNoEventMaskSet = -1;

        /// <summary>
        /// ������߶�������
        /// </summary>
        public enum BlockingObjects
        {
            None = 0,
            TwoD = 1,
            ThreeD = 2,
            All = 3,
        }

        /// <summary>
        /// �������ȼ�
        /// </summary>
        public override int sortOrderPriority
        {
            get
            {
                // We need to return the sorting order here as distance will all be 0 for overlay.
                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    return canvas.sortingOrder;
                }
                return base.sortOrderPriority;
            }
        }

        /// <summary>
        /// ��Ⱦ˳�����ȼ�
        /// </summary>
        public override int renderOrderPriority
        {
            get
            {
                // We need to return the sorting order here as distance will all be 0 for overlay.
                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    return canvas.renderOrder;
                }
                return base.renderOrderPriority;
            }
        }

        //���Է����ͼ��
        [FormerlySerializedAs("ignoreReversedGraphics")]
        [SerializeField]
        private bool m_IgnoreReversedGraphics = true;
        public bool ignoreReversedGraphics
        {
            get
            {
                return m_IgnoreReversedGraphics;
            }
            set
            {
                m_IgnoreReversedGraphics = value;
            }
        }

        [FormerlySerializedAs("blockingObjects")]
        [SerializeField]
        private BlockingObjects m_BlockingObjects = BlockingObjects.None;
        public BlockingObjects blockingObjects
        {
            get
            {
                return m_BlockingObjects;
            }
            set
            {
                m_BlockingObjects = value;
            }
        }

        /// <summary>
        /// �����ڵ� Mask
        /// </summary>
        [SerializeField]
        protected LayerMask m_BlockingMask = kNoEventMaskSet;

        protected BrandoGraphicRaycaster()
        {}

        private Canvas m_Canvas;
        private Canvas canvas
        {
            get
            {
                if (m_Canvas != null)
                    return m_Canvas;

                m_Canvas = GetComponent<Canvas>();
                return m_Canvas;
            }
        }

        //���߽��
        [NonSerialized]
        private List<BrandoUIGraphic> m_RaycastResults 
            = new List<BrandoUIGraphic>();

        #region Raycast ��������
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="resultAppendList"></param>
        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (canvas == null)
                return;

            // Convert to view space
            Vector2 pos;
            if (eventCamera == null)
            {
                pos = new Vector2(eventData.position.x / Screen.width, eventData.position.y / Screen.height);
            }
            else
            {
                pos = eventCamera.ScreenToViewportPoint(eventData.position);
            }

            // If it's outside the camera's viewport, do nothing ָ��λ������⣬��������
            if (pos.x < 0f || pos.x > 1f || pos.y < 0f || pos.y > 1f)
            {
                return;
            }


            float hitDistance = float.MaxValue;
            Ray ray = new Ray();

            if (eventCamera != null)
            {
                ray = eventCamera.ScreenPointToRay(eventData.position);
            }

            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay &&
                blockingObjects != BlockingObjects.None)
            {
                float dist = 100.0f;
                if (eventCamera != null)
                {
                    dist = eventCamera.farClipPlane - eventCamera.nearClipPlane;
                }

                //���߾���
                if (blockingObjects == BlockingObjects.ThreeD || blockingObjects == BlockingObjects.All)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, dist, m_BlockingMask))
                    {
                        hitDistance = hit.distance;
                    }
                }
                if (blockingObjects == BlockingObjects.TwoD || blockingObjects == BlockingObjects.All)
                {
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, dist, m_BlockingMask);
                    if (hit.collider != null)
                    {
                        hitDistance = hit.fraction * dist;
                    }
                }
            }
            m_RaycastResults.Clear();
            Raycast(canvas, eventCamera, eventData.position, m_RaycastResults);

            //��������������������Ч��ͼ��
            for (var index = 0; index < m_RaycastResults.Count; index++)
            {
                var go = m_RaycastResults[index].gameObject;
                bool appendGraphic = true;

                //�Ƿ���Է���
                if (ignoreReversedGraphics)
                {
                    if (eventCamera == null)
                    {
                        // If we dont have a camera we know that we should always be facing forward
                        var dir = go.transform.rotation * Vector3.forward;
                        appendGraphic = Vector3.Dot(Vector3.forward, dir) > 0;
                    }
                    else
                    {
                        // If we have a camera compare the direction against the cameras forward.
                        var cameraFoward = eventCamera.transform.rotation * Vector3.forward;
                        var dir = go.transform.rotation * Vector3.forward;
                        appendGraphic = Vector3.Dot(cameraFoward, dir) > 0;
                    }
                }

                if (appendGraphic)
                {
                    float distance = 0;

                    if (eventCamera == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                        distance = 0;
                    else
                    {
                        Transform trans = go.transform;
                        Vector3 transForward = trans.forward;
                        distance = (Vector3.Dot(transForward, trans.position - ray.origin) 
                            / Vector3.Dot(transForward, ray.direction));

                        // Check to see if the go is behind the camera.
                        if (distance < 0)
                            continue;
                    }

                    if (distance >= hitDistance)
                        continue;

                    //�������߽����Ϣ
                    var castResult = new RaycastResult
                    {
                        gameObject = go,
                        module = this,
                        distance = distance,
                        screenPosition = eventData.position,
                        index = resultAppendList.Count,
                        depth = m_RaycastResults[index].depth,
                        sortingLayer =  canvas.sortingLayerID,
                        sortingOrder = canvas.sortingOrder
                    };
                    resultAppendList.Add(castResult);
                }
            }
        }

        private static void Raycast(Canvas canvas, 
            Camera eventCamera, 
            Vector2 pointerPosition, 
            List<BrandoUIGraphic> results)
        {
            // Necessary for the event system
            // ��ȡ�ѵǼǵĸ� Canvas �µ�ͼ�����
            var foundGraphics = BrandoUIGraphicRegistry.GetGraphicsForCanvas(canvas);
            for (int i = 0; i < foundGraphics.Count; ++i)
            {
                BrandoUIGraphic graphic = foundGraphics[i];

                // -1 means it hasn't been processed by the canvas, which means it isn't actually drawn
                // ���Ϊ -1 ��ʾû�л���
                if (graphic.depth == -1 || !graphic.raycastTarget)
                {
                    continue;
                }

                // Point �Ƿ�λ��ͼ��� Rect ��
                if (!RectTransformUtility.RectangleContainsScreenPoint(
                    graphic.rectTransform,
                    pointerPosition,
                    eventCamera))
                    continue;

                //ͼ�񴥷�����
                if (graphic.Raycast(pointerPosition, eventCamera))
                {
                    s_SortedGraphics.Add(graphic);
                }
            }

            s_SortedGraphics.Sort((g1, g2) => g2.depth.CompareTo(g1.depth));
            for (int i = 0; i < s_SortedGraphics.Count; ++i)
                results.Add(s_SortedGraphics[i]);
            s_SortedGraphics.Clear();
        }

        #endregion

        public override Camera eventCamera
        {
            get
            {
                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay
                    || (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null))
                    return null;

                return canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
            }
        }

        /// <summary>
        /// Perform a raycast into the screen and collect all graphics underneath it.
        /// </summary>
        [NonSerialized] static readonly List<BrandoUIGraphic> s_SortedGraphics = new List<BrandoUIGraphic>();

    }
}
