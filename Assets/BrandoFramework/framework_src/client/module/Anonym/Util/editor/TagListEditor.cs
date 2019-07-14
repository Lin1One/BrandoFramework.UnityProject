using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Anonym.Isometric
{
    using Util;
    [CustomEditor(typeof(TagList))]
    [DisallowMultipleComponent]
    public class TagListEditor : Editor
    {
        string sNewTag;
        TagList _taglist;

        bool bHelp_FaildToAddNewTag;

        private void OnEnable()
        {
            _taglist = target as TagList;
        }

        public override void OnInspectorGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                bool bHasEmpty = _taglist.bHasEmptyElement();
                CustomEditorGUI.Button(!bHasEmpty, CustomEditorGUI.Color_LightBlue, "Add a empty element", () => _taglist.AddEmptyElement());
                CustomEditorGUI.Button(bHasEmpty, CustomEditorGUI.Color_LightRed, "Clear all empty elements", () => _taglist.ClearAllEmptyElement());
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                sNewTag = EditorGUILayout.TextField(sNewTag);
                CustomEditorGUI.Button(!string.IsNullOrEmpty(sNewTag), CustomEditorGUI.Color_LightBlue, "Add a new Tag", () => bHelp_FaildToAddNewTag = !_taglist.AddNewTag(sNewTag));
                CustomEditorGUI.Button(true, CustomEditorGUI.Color_LightRed, "Clear all Garbage sub assets", () => _taglist.ClearGarbageSubAsset());
            }

            HelpBox();
            DrawDefaultInspector();
        }

        void HelpBox()
        {
            if (bHelp_FaildToAddNewTag)
            {
                EditorGUILayout.HelpBox("There is already a Tag has same string!", MessageType.Info);
            }
        }
    }
}
