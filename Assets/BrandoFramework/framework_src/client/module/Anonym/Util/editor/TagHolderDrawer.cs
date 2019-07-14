using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Anonym.Util
{
    [CustomPropertyDrawer(typeof(TagHolder))]
    public class TagHolderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            TagHolderField(position, property);
        }

        public static void TagHolderField(Rect position, SerializedProperty property)
        {
            SerializedProperty spName = property.FindPropertyRelative("_name");
            SerializedProperty spTagList = property.FindPropertyRelative("_tagList");
            SerializedProperty spTag = property.FindPropertyRelative("_tag");

            if (spTagList.objectReferenceValue == null)
            {
                EditorGUI.ObjectField(position, spTagList, new GUIContent(spName.stringValue));
            }
            else
            {
                float fGap = 5f;
                Rect rtTags = new Rect(position.position, new Vector2(position.width - EditorGUIUtility.singleLineHeight * 1.2f - fGap, position.size.y));
                Rect rtBtn = new Rect(rtTags.xMax + fGap, rtTags.yMin, position.width - rtTags.width - fGap, rtTags.height);
                if (GUI.Button(rtBtn, "x"))
                {
                    spTagList.objectReferenceValue = spTag.objectReferenceValue = null;
                }
                else
                {
                    TagList tagList = spTagList.objectReferenceValue as TagList;
                    var strings = tagList.getTagStringArray();
                    int index = -1;
                    if (spTag.objectReferenceValue != null)
                    {
                        var selectedTagString = (spTag.objectReferenceValue as Tag).tag;
                        index = ArrayUtility.IndexOf<string>(strings, selectedTagString);
                    }
                    index = EditorGUI.Popup(rtTags, spName.stringValue, index, strings);
                    if (index >= 0)
                    {
                        var tag = tagList.GetTag(strings[index]);
                        if (tag != null)
                            spTag.objectReferenceValue = tag;
                    }
                }
            }
        }

        public static bool TagHolderField(Rect position, TagHolder tagHolder)
        {
            bool bChanged = false;
            if (tagHolder == null)
            {
                EditorGUI.LabelField(position, "Null: new TagHolder() first!");
                return false;
            }

            if (tagHolder._tagList == null)
            {
                tagHolder._tagList = EditorGUI.ObjectField(position, new GUIContent(tagHolder._name), (Object)tagHolder._tagList, typeof(TagList), allowSceneObjects:false) as TagList;
            }
            else
            {
                float fGap = 5f;
                Rect rtTags = new Rect(position.position, new Vector2(position.width - EditorGUIUtility.singleLineHeight * 1.2f - fGap, position.size.y));
                Rect rtBtn = new Rect(rtTags.xMax + fGap, rtTags.yMin, position.width - rtTags.width - fGap, rtTags.height);
                if (GUI.Button(rtBtn, "x"))
                {
                    tagHolder._tagList = null;
                    tagHolder._tag = null;
                    bChanged = true;
                }
                else
                {
                    var strings = tagHolder._tagList.getTagStringArray();
                    int index = -1;
                    if (tagHolder._tag != null)
                    {
                        var selectedTagString = tagHolder._tag.tag;
                        index = ArrayUtility.IndexOf<string>(strings, selectedTagString);
                    }

                    EditorGUI.BeginChangeCheck();
                    index = EditorGUI.Popup(rtTags, tagHolder._name, index, strings);
                    bChanged = EditorGUI.EndChangeCheck();

                    if (index >= 0)
                    {
                        var tag = tagHolder._tagList.GetTag(strings[index]);
                        if (tag != null)
                            tagHolder._tag = tag;
                    }
                }
            }
            return bChanged;
        }
    }
}
