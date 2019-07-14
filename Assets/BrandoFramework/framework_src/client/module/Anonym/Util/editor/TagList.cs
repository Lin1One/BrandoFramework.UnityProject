using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Anonym.Util
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "TagList", menuName = "Anonym/TagList", order = 101)]
    public class TagList : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        string desc;

        [SerializeField]
        List<Tag> tags = new List<Tag>();
        public string[] getTagStringArray()
        {
            return tags.Where(t => t != null && !string.IsNullOrEmpty(t.tag)).Select(t => t.tag).ToArray();
        }

        void discint()
        {
            var newList = tags.Distinct().ToList();
            if (tags.Count != newList.Count)
            {
                Debug.Log("Taglist can not contain duplicate content.");
                tags = newList;
            }
        }

        public void ClearGarbageSubAsset()
        {
            var childs = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
            var expiredList = childs.Where(c => c != this && !tags.Contains(c as Tag)).ToArray();
            for (int i = 0; i < expiredList.Length; ++i)
            {
                DestroyImmediate(expiredList[i], true);
            }
            AssetDatabase.SaveAssets();
        }

        public void AddEmptyElement()
        {
            tags.Add(null);
        }

        public Tag GetOtherTag(string tagString)
        {
            return tags.Find(t => t != null && !t.tag.Equals(tagString));
        }

        public Tag GetTag(string tagString)
        {
            return tags.Find(t => t != null && t.tag.Equals(tagString));
        }

        public bool AddNewTag(string tagString)
        {
            if (GetTag(tagString) != null)
                return false;

            AddEmptyElement();
            var _instance = Tag.CreateAsset();
            tags[tags.Count - 1] = _instance;
            _instance.Set(string.Format("T[{0}]", tagString), tagString);

            AssetDatabase.AddObjectToAsset(_instance, this);

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return true;
        }

        public void ClearAllEmptyElement()
        {
            tags.RemoveAll(t => t == null);
        }

        public bool bHasEmptyElement()
        {
            return tags.Contains(null);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {

        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            discint();
        }
    }
}