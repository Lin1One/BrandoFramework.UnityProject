
using Client.UI.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI
{
    public class BrandoCanvasUpdateRegistry
    {
        private static BrandoCanvasUpdateRegistry s_Instance;

        public static BrandoCanvasUpdateRegistry instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new BrandoCanvasUpdateRegistry();
                }
                return s_Instance;
            }
        }

        protected BrandoCanvasUpdateRegistry()
        {
            Canvas.willRenderCanvases += PerformUpdate;
        }

        //处理布局更新
        private bool m_PerformingLayoutUpdate;
        //处理图像更新
        private bool m_PerformingGraphicUpdate;

        public static bool IsRebuildingLayout()
        {
            return instance.m_PerformingLayoutUpdate;
        }

        public static bool IsRebuildingGraphics()
        {
            return instance.m_PerformingGraphicUpdate;
        }

        private readonly IndexedSet<ICanvasElement> m_LayoutRebuildQueue
            = new IndexedSet<ICanvasElement>();
        private readonly IndexedSet<ICanvasElement> m_GraphicRebuildQueue
            = new IndexedSet<ICanvasElement>();

        #region 注册注销方法

        public static void RegisterCanvasElementForLayoutRebuild(ICanvasElement element)
        {
            instance.InternalRegisterCanvasElementForLayoutRebuild(element);
        }

        public static bool TryRegisterCanvasElementForLayoutRebuild(ICanvasElement element)
        {
            return instance.InternalRegisterCanvasElementForLayoutRebuild(element);
        }

        /// <summary>
        /// 注册至 Layout 重建队列
        /// </summary>
        /// <param name="element"></param>
        private bool InternalRegisterCanvasElementForLayoutRebuild(ICanvasElement element)
        {
            if (m_LayoutRebuildQueue.Contains(element))
                return false;

            m_LayoutRebuildQueue.Add(element);
            return true;
        }

        public static void RegisterCanvasElementForGraphicRebuild(ICanvasElement element)
        {
            instance.InternalRegisterCanvasElementForGraphicRebuild(element);
        }

        public static bool TryRegisterCanvasElementForGraphicRebuild(ICanvasElement element)
        {
            return instance.InternalRegisterCanvasElementForGraphicRebuild(element);
        }

        /// <summary>
        /// 注册至 Graphic 重建队列
        /// </summary>
        /// <param name="element"></param>
        private bool InternalRegisterCanvasElementForGraphicRebuild(ICanvasElement element)
        {
            if (m_PerformingGraphicUpdate)
            {
                Debug.LogError(string.Format("Trying to add {0} for graphic rebuild while we are already inside a graphic rebuild loop. This is not supported.", element));
                return false;
            }

            if (m_GraphicRebuildQueue.Contains(element))
                return false;

            m_GraphicRebuildQueue.Add(element);
            return true;
        }


        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="element"></param>
        public static void UnRegisterCanvasElementForRebuild(ICanvasElement element)
        {
            instance.InternalUnRegisterCanvasElementForLayoutRebuild(element);
            instance.InternalUnRegisterCanvasElementForGraphicRebuild(element);
        }

        private void InternalUnRegisterCanvasElementForLayoutRebuild(ICanvasElement element)
        {
            if (m_PerformingLayoutUpdate)
            {
                Debug.LogError(string.Format("Trying to remove {0} from rebuild list while we are already inside a rebuild loop. This is not supported.", element));
                return;
            }

            element.LayoutComplete();
            instance.m_LayoutRebuildQueue.Remove(element);
        }

        private void InternalUnRegisterCanvasElementForGraphicRebuild(ICanvasElement element)
        {
            if (m_PerformingGraphicUpdate)
            {
                Debug.LogError(string.Format("Trying to remove {0} from rebuild list while we are already inside a rebuild loop. This is not supported.", element));
                return;
            }
            element.GraphicUpdateComplete();
            instance.m_GraphicRebuildQueue.Remove(element);
        }

        #endregion

        #region Canvas 更新
        /// <summary>
        /// Canvas 更新
        /// </summary>
        private void PerformUpdate()
        {
            CleanInvalidItems();

            //布局重建
            m_PerformingLayoutUpdate = true;

            m_LayoutRebuildQueue.Sort(s_SortLayoutFunction);
            for (int i = 0; i <= (int)CanvasUpdate.PostLayout; i++)
            {
                for (int j = 0; j < m_LayoutRebuildQueue.Count; j++)
                {
                    var rebuild = instance.m_LayoutRebuildQueue[j];
                    try
                    {
                        if (ObjectValidForUpdate(rebuild))
                            rebuild.Rebuild((CanvasUpdate)i);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e, rebuild.transform);
                    }
                }
            }

            for (int i = 0; i < m_LayoutRebuildQueue.Count; ++i)
                m_LayoutRebuildQueue[i].LayoutComplete();

            instance.m_LayoutRebuildQueue.Clear();
            m_PerformingLayoutUpdate = false;

            // now layout is complete do culling...
            ClipperRegistry.instance.Cull();

            //图像重建
            m_PerformingGraphicUpdate = true;
            for (var i = (int)CanvasUpdate.PreRender; i < (int)CanvasUpdate.MaxUpdateValue; i++)
            {
                for (var k = 0; k < instance.m_GraphicRebuildQueue.Count; k++)
                {
                    try
                    {
                        var element = instance.m_GraphicRebuildQueue[k];
                        if (ObjectValidForUpdate(element))
                            element.Rebuild((CanvasUpdate)i);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e, instance.m_GraphicRebuildQueue[k].transform);
                    }
                }
            }

            for (int i = 0; i < m_GraphicRebuildQueue.Count; ++i)
                m_GraphicRebuildQueue[i].LayoutComplete();

            instance.m_GraphicRebuildQueue.Clear();
            m_PerformingGraphicUpdate = false;
        }

        /// <summary>
        /// Object 是否有效
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private bool ObjectValidForUpdate(ICanvasElement element)
        {
            var valid = element != null;

            var isUnityObject = element is UnityEngine.Object;
            if (isUnityObject)
            {
                //Here we make use of the overloaded UnityEngine.Object == null, 
                //that checks if the native object is alive.
                valid = (element as UnityEngine.Object) != null;
            }
            return valid;
        }

        /// <summary>
        /// 清除无效 Item
        /// </summary>
        private void CleanInvalidItems()
        {
            // So MB's override the == operator for null equality, which checks
            // if they are destroyed. 
            // This is fine if you are looking at a concrete mb, 
            // but in this case we are looking at a list of ICanvasElement
            // this won't forward the == operator to the MB, but just check if the
            // interface is null. IsDestroyed will return if the backend is destroyed.

            for (int i = m_LayoutRebuildQueue.Count - 1; i >= 0; --i)
            {
                var item = m_LayoutRebuildQueue[i];
                if (item == null)
                {
                    m_LayoutRebuildQueue.RemoveAt(i);
                    continue;
                }

                if (item.IsDestroyed())
                {
                    m_LayoutRebuildQueue.RemoveAt(i);
                    item.LayoutComplete();
                }
            }

            for (int i = m_GraphicRebuildQueue.Count - 1; i >= 0; --i)
            {
                var item = m_GraphicRebuildQueue[i];
                if (item == null)
                {
                    m_GraphicRebuildQueue.RemoveAt(i);
                    continue;
                }

                if (item.IsDestroyed())
                {
                    m_GraphicRebuildQueue.RemoveAt(i);
                    item.GraphicUpdateComplete();
                }
            }
        }

        //布局排序委托
        private static readonly Comparison<ICanvasElement> s_SortLayoutFunction = SortLayoutList;

        private static int ParentCount(Transform child)
        {
            if (child == null)
                return 0;

            var parent = child.parent;
            int count = 0;
            while (parent != null)
            {
                count++;
                parent = parent.parent;
            }
            return count;
        }

        private static int SortLayoutList(ICanvasElement x, ICanvasElement y)
        {
            Transform t1 = x.transform;
            Transform t2 = y.transform;

            return ParentCount(t1) - ParentCount(t2);
        }

        #endregion







    }
}
