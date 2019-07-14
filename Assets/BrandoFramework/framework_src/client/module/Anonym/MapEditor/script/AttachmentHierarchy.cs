using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Isometric
{
    public interface IGameObject
    {
        GameObject gameObject { get; }
    }
    public interface IAttachment
    {
        int IndentLevel { get; }
        bool Init(GameObject _obj, int _indentLevel);
        void Clear();
    }

    [System.Serializable]
    public class Attachment<T> : IAttachment, IGameObject where T : class, new()
    {
        [SerializeField]
        public T AttachedObj;        
        [SerializeField]
        public int indentLevel;

        public int IndentLevel { get { return indentLevel; } }

        public GameObject gameObject
        {
            get {
                if (AttachedObj != null)
                {
                    if (typeof(T).IsCOMObject)
                        return (AttachedObj as Component).gameObject;
                    else if (AttachedObj is IGameObject)
                        return (AttachedObj as IGameObject).gameObject;
                }
                return null;
            }
        }

        virtual public bool Init(GameObject _obj, int _indentLevel)
        {
            Clear();
            indentLevel = _indentLevel;
            if (typeof(T).IsSubclassOf(typeof(Component)))
                AttachedObj = _obj.GetComponent<T>(); // InChildren
            else
                AttachedObj = new T();
            return AttachedObj != null;
        }

        public void Clear()
        {
            indentLevel = 0;
            AttachedObj = null;
        }
    }

    [System.Serializable]
    public class AttachmentHierarchy<T> where T : IAttachment, IGameObject, new()
    {
        [SerializeField]
        public bool bFoldout = true;
        [SerializeField]
        public List<T> childList = new List<T>();

        public AttachmentHierarchy()
        {
            Clear();
        }

        public bool Init(GameObject _obj, int _indentLevel = 0, int iMaxDepth = -1)
        {
            Clear();

            var _childs = GetFromChild(_obj, _indentLevel, iMaxDepth);
            if (_childs != null && _childs.Count > 0)
                childList.AddRange(_childs);
            return childList.Count > 0;
        }

        static List<T> GetFromChild(GameObject _obj, int _indentLevel, int iMaxDepth)
        {
            List<T> list = new List<T>();

            if (_obj != null)
            {
                T _child = new T();
                if (_child.Init(_obj, _indentLevel++))
                    list.Add(_child);

                if (iMaxDepth < 0 || _indentLevel < iMaxDepth)
                {
                    for (int i = 0; i < _obj.transform.childCount; ++i)
                    {
                        list.AddRange(GetFromChild(_obj.transform.GetChild(i).gameObject, _indentLevel, iMaxDepth));
                    }
                }
            }
            return list;
        }

        public void Clear()
        {
            bFoldout = true; 
            childList.Clear();
        }
    }
}