using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.Reflection;

namespace Anonym.Isometric
{
    using Util;
    [CustomPropertyDrawer(typeof(IsoTile))]
    public class IsoTileDrawer : PropertyDrawer
    {
        TmpTexture2D tmpTexture2D = new TmpTexture2D();
        static int cellSize = 40;
        static int border = 2;

        public static int RectHeight { get { return cellSize + border * 2; } }
        public static Rect GetRect()
        {
            Rect rt = EditorGUILayout.GetControlRect(
                        new GUILayoutOption[] { GUILayout.Height(RectHeight), GUILayout.ExpandWidth(true) });
            return EditorGUI.IndentedRect(rt);
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return RectHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //if (Event.current.type == EventType.ScrollWheel)
            //{
            //    EditorGUIUtility.ExitGUI();
            //    return;
            //}
                
            SerializedProperty sp = property.serializedObject.FindProperty(property.propertyPath);
            if (sp != property)
                sp = property;
            if (sp.objectReferenceValue == null)
                return;
            GameObject _target = ((Component)sp.objectReferenceValue).gameObject;
            Color borderColor = Util.CustomEditorGUI.Color_Tile;

            Rect rect = position;
            Rect rect_inside = new Rect(rect.xMin + border, rect.yMin + border, rect.width - border * 2, rect.height - border * 2);

            Rect rect_preview = new Rect(rect_inside.xMin, rect_inside.yMin, cellSize, rect_inside.height);
            Rect rect_info_name =
                new Rect(rect_inside.xMin + cellSize, rect_inside.yMin,
                    rect_inside.width - cellSize * 3, rect_inside.height / 2);
            Rect rect_info_Sub =
                new Rect(rect_info_name.xMin, rect_info_name.yMin + cellSize / 2,
                    rect_info_name.width, rect_info_name.height);
            Rect rect_delete = new Rect(rect_inside.xMax - cellSize * 2, rect_inside.yMin, cellSize, rect_inside.height);
            Rect rect_select = new Rect(rect_inside.xMax - cellSize, rect_inside.yMin, cellSize, rect_inside.height);

            float fExWidth = rect_delete.width + rect_select.width;
            rect_info_name.width += fExWidth;
            rect_info_Sub.width += fExWidth;

            CustomEditorGUI.DrawBordereddRect(rect, borderColor, rect_inside, Color.clear);
            
            tmpTexture2D.MakeRenderImage(_target, null, Color.clear);
            tmpTexture2D.DrawRect(rect_preview);

            int iLv = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUI.LabelField(rect_info_name, _target.name, EditorStyles.boldLabel);

            EditorGUI.indentLevel = iLv;

            if (GUI.enabled)
            {
                using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightYellow))
                {
                    if (GUI.Button(rect_delete.ReSize(8f, 8f), "Del!"))
                    {
                        Undo.DestroyObjectImmediate(_target.gameObject);
                    }
                }
                using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightMagenta))
                {
                    if (GUI.Button(rect_select, "Go!"))
                    {
                        Selection.activeGameObject = _target.gameObject;
                    }
                }
            }
        }
    }
}