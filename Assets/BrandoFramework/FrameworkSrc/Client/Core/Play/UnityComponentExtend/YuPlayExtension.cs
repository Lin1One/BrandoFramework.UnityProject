#region Head

// Author:           Yu
// CreateDate:    2018/4/20 9:40:58
// Email:             Yu@gmail.com || 35490136@qq.com

#endregion

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Client.Extend
{
    /// <summary>
    /// unity下通用扩展。
    /// </summary>
    public static class YuPlayExtension
    {
        public static Transform NextBrother(this Transform transform)
        {
            var nextIndex = transform.GetSiblingIndex() + 1;
            if (transform.parent.childCount > nextIndex)
            {
                return transform.parent.GetChild(nextIndex);
            }

            return null;
        }

        public static RectTransform RectTransform(this Transform transform)
        {
            RectTransform trm = null;

            try
            {
                trm = transform.GetComponent<RectTransform>();
            }
            catch (Exception e)
            {
                Debug.Log(transform.name);
            }

            return trm;
        }

        public static RectTransform FindParent<T>(this RectTransform rect, out string path)
        {
            path = null;

            while (true)
            {
                path = rect.parent.name + "/" + path;
                var t = rect.parent.GetComponent<T>();
                if (t != null) return rect;
                rect = rect.parent.RectTransform();
            }
        }

        public static T AddComponent<T>(this Transform tram) where T : Component
        {
            var component = tram.gameObject.AddComponent<T>();
            return component;
        }

        public static T AddComponent<T>(this RectTransform rect) where T : Component
        {
            var component = rect.gameObject.AddComponent<T>();
            return component;
        }

        #region Gameobject

        public static List<GameObject> GetAllChild(this GameObject parent)
        {
            var trm = parent.transform;
            var childCount = trm.childCount;
            var childGos = new List<GameObject>();

            for (var i = 0; i < childCount; i++)
            {
                var go = trm.GetChild(i).gameObject;
                childGos.Add(go);
            }

            return childGos;
        }

        public static List<GameObject> GetAllChild(this Transform transform)
            => GetAllChild(transform.gameObject);

        private static void DeleteAllChild(this GameObject parent)
        {
            var childGos = parent.GetAllChild();
            foreach (var childGo in childGos)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(childGo);
                }
                else
                {
                    Object.DestroyImmediate(childGo);
                }
            }
        }

        public static void DeleteAllChild(this Transform transform)
            => DeleteAllChild(transform.gameObject);

        public static GameObject FindChild(this GameObject parent, Func<GameObject, bool> where)
        {
            var childs = parent.GetAllChild();
            foreach (var child in childs)
            {
                if (where(child))
                {
                    return child;
                }
            }

            return null;
        }

        /// <summary>
        /// 在给定的游戏对象下查找所有符合提交的子对象。
        /// 该方法只会查找第一级子物体不会进行递归查找。
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static List<GameObject> FindAllChild(this GameObject parent, Func<GameObject, bool> where)
        {
            var result = new List<GameObject>();
            var childs = parent.GetAllChild();

            foreach (var child in childs)
            {
                if (where(child))
                {
                    result.Add(child);
                }
            }

            return result;
        }

        /// <summary>
        /// 查找所有层级中找到的第一个子物体
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Transform FindProgeny(this Transform parent,string name)
        {
            if (parent == null)
            {
                return null;
            }

            Transform[] children = parent.GetComponentsInChildren<Transform>();
            foreach (var child in children)
            {
                if (child.name == name)
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>
        /// 查找所有层级的子物体
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<Transform> FindAllProgeny(this Transform parent, string name)
        {
            List<Transform> list = new List<Transform>();
            if (parent == null)
            {
                return list;
            }

            Transform[] children = parent.GetComponentsInChildren<Transform>();
            foreach (var child in children)
            {
                if (child.name == name)
                {
                    list.Add(child);
                }
            }
            return list;
        }

        #endregion

        #region 锚点

        /// <summary>
        /// 设置为左上锚点。
        /// 滚动视图的所有子项都需要设置为该锚点类型。
        /// </summary>
        /// <param name="rect"></param>
        public static void AsLeftTop(this RectTransform rect)
        {
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
        }

        #endregion

        #region Play

        public static bool HasComponent<T>(this GameObject go)
           where T : Component
        {
            return go.GetComponent<T>() != null;
        }

        public static bool HasComponent<T>(this Transform trm)
            where T : Component
        {
            return trm.gameObject.GetComponent<T>() != null;
        }

        public static T GetByPath<T>(this GameObject go, string path)
        {
            return go.transform.Find(path).GetComponent<T>();
        }

        public static T GetByPath<T>(this Transform trm, string path)
        {
            return trm.Find(path).GetComponent<T>();
        }

        #region Transform
		
		/// <summary>
        /// 设置游戏对象以及其下所有层的子物体layer
        /// </summary>
        /// <param name="layer"></param>
        public static void SetAllLayer(this GameObject gameObject,int layer)
        {
            foreach(var item in gameObject.GetComponentsInChildren<Transform>())
            {
                item.gameObject.layer = layer;
            }
        }

        public static Transform Grandfather(this Transform trm)
        {
            return trm.parent.parent;
        }

        public static Transform Grandfather(this GameObject go)
        {
            return go.transform.parent.parent;
        }

        public static GameObject AddChild(this GameObject parent, GameObject prefab)
        {
            var go = Object.Instantiate(prefab);
            if (go != null && parent != null)
            {
                var trm = go.transform;
                trm.SetParent(parent.transform, false);
                go.layer = parent.gameObject.layer;
            }

            return go;
        }

        public static Transform AddChild(this Transform root, string path)
        {
            var array = path.Split('/');
            var parents = new List<Transform>();

            for (var index = 0; index < array.Length; index++)
            {
                var p = GetPathByIndex(array, index + 1);
                var target = root.Find(p);
                if (target != null)
                {
                    parents.Add(target);
                }
                else
                {
                    var go = new GameObject(array[index]);
                    go.transform.SetParent(index == 0 ? root : parents[index - 1]);
                    parents.Add(go.transform);
                }
            }

            var result = root.Find(path);
            return result;
        }

        private static string GetPathByIndex(string[] array, int index)
        {
            var path = string.Empty;

            for (var i = 0; i < index; i++)
            {
                if (i == 0)
                {
                    path = array[i];
                }
                else
                {
                    path = path + "/" + array[i];
                }
            }

            return path;
        }

        #endregion


        #endregion
    }
}