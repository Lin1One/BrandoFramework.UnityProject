using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Anonym.Util
{
    [CustomEditor(typeof(TileWand))]
    [CanEditMultipleObjects]
    public class TileWandEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // base.DrawDefaultInspector();
            serializedObject.Update();
            var tileWand = target as TileWand;
            SerializedProperty spTilePrefab = serializedObject.FindProperty("Prefab");

            EditorGUILayout.ObjectField(spTilePrefab);
            CustomEditorGUI.Button(true, CustomEditorGUI.Color_LightYellow, "Update Icon", () =>
            {
                var selection = Selection.objects.Where(s => s is TileWand).GetEnumerator();
                while(selection.MoveNext())
                {
                    var current = selection.Current as TileWand;
                    if (current != null)
                        current.UpdateIcon();
                }
            });

            serializedObject.ApplyModifiedProperties();
        }
    }
}
