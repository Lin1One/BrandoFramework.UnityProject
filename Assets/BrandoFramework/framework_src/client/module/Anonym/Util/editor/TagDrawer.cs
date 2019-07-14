using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Anonym.Util
{
    using Util;
    [CustomPropertyDrawer(typeof(Tag))]
    public class TagDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Tag _tag = property.objectReferenceValue as Tag;
            if (_tag == null)
            {
                EditorGUI.ObjectField(position, property);
            }
            else
            {
                EditorGUI.ObjectField(position, property, new GUIContent(_tag.tag));
            }
        }
    }
}
