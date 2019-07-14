using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Anonym.Util
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Tag", menuName = "Anonym/Tag", order = 100)]        
    public class Tag : ScriptableObject
    {
        [SerializeField]
        string _tag;
        public string tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
        public bool HasNoTag { get { return string.IsNullOrEmpty(_tag); } }

        public void Set(string Name, string Tag)
        {
            name = Name;
            tag = Tag;
        }

        public static Tag CreateAsset(string path = null)
        {
            var _instance = ScriptableObject.CreateInstance<Tag>();
            if (!string.IsNullOrEmpty(path))
            {
                string _path = AssetDatabase.GenerateUniqueAssetPath(path);
                AssetDatabase.CreateAsset(_instance, _path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            return _instance;
        }

        static bool compare(Tag a, Tag b)
        {            
            if (a.HasNoTag || b.HasNoTag)
                return false;
            return a.tag.Equals(b.tag);
        }

        public static bool operator !=(Tag a, Tag b)
        {
            Object ObjA = (Object)a;
            Object ObjB = (Object)b;
            if (ObjA == null || ObjB == null)
                return ObjA != ObjB;
            return !compare(a, b);
        }

        public static bool operator ==(Tag a, Tag b)
        {
            Object ObjA = (Object)a;
            Object ObjB = (Object)b;
            if (ObjA == null || ObjB == null)
                return ObjA == ObjB;
            return compare(a, b);
        }

        public override bool Equals(object obj)
        {
            return (Tag) this == (Tag) obj;
        }

        public override int GetHashCode()
        {
            if (this.HasNoTag)
                return 0;
            return this.tag.GetHashCode();
        }
    }
}