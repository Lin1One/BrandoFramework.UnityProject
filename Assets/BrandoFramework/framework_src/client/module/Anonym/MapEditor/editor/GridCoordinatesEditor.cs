using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Anonym.Isometric
{
    using Util;
    [CanEditMultipleObjects]
	[CustomEditor(typeof(GridCoordinates))]
    public class GridCoordinatesEditor : Editor
    {
		SerializedProperty _snapFree;
		GridCoordinates _target;

        bool bNoGrid = false;

		void OnEnable()
        {
			_snapFree = serializedObject.FindProperty("bSnapFree");
            _target = (GridCoordinates) target;
            bNoGrid = _target.grid == null;
        }

		public override void OnInspectorGUI()
        {
			serializedObject.Update();
            EditorGUILayout.Separator();
			GridCoordinatesField();  
			serializedObject.ApplyModifiedProperties();
		}

		public void OnSceneGUI()
        {
			if (bNoGrid || PrefabHelper.IsPrefab(_target.gameObject))
                return;

			if (!_snapFree.boolValue)
            {
                int iOddCount = 9;
                for (int i = 0 ; i < iOddCount; ++i)
                {
                    Handles.color = Color.red;
                    Handles.DotHandleCap(i * 3, _target.transform.position 
                        + new Vector3((i - (iOddCount - 1)/2) * _target.GridInterval.x, 0, 0), 
                        Quaternion.identity, 0.025f, EventType.Repaint);
                    Handles.color = Color.green;
                    Handles.DotHandleCap(i * 3 + 1, _target.transform.position 
                        + new Vector3(0, (i - (iOddCount - 1)/2) * _target.GridInterval.y, 0), 
                        Quaternion.identity, 0.025f, EventType.Repaint);
                    Handles.color = Color.blue;
                    Handles.DotHandleCap(i * 3 + 2, _target.transform.position 
                        + new Vector3(0, 0, (i - (iOddCount - 1)/2) * _target.GridInterval.z), 
                        Quaternion.identity, 0.025f, EventType.Repaint);
                }
            }
		}

        void GCSnapToggle(GridCoordinates gc, bool bNewSnap)
        {
            if (gc == null)
                return;
            Undo.RecordObject(gc, "GridCoordinates: Snap Free");
            gc.bSnapFree = bNewSnap;
            gc.Apply_SnapToGrid();
            EditorUtility.SetDirty(gc);
        }

        void GridCoordinatesField()
        {
            GridCoordinates _gc = (GridCoordinates)(target);
            Vector3 _gridXYZ = _gc._xyz;

            float _fWidth = EditorGUIUtility.currentViewWidth * 0.475f;
            float _mfWidth = _fWidth * 0.95f;

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    CustomEditorGUI.NewParagraph("[Grid Coordinates]", GUILayout.MaxWidth(_mfWidth * 0.75f));

                    EditorGUI.showMixedValue = _snapFree.hasMultipleDifferentValues;
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.ToggleLeft( new GUIContent("Snap To Grid", "Snap to Grid Coordinates"),
                                !_gc.bSnapFree, GUILayout.MaxWidth(_mfWidth * 0.75f));
                    if (EditorGUI.EndChangeCheck())
                    {
                        foreach(var one in targets)
                        {
                            GCSnapToggle((GridCoordinates) one, !_gc.bSnapFree);
                        }
                    }
                    EditorGUI.showMixedValue = false;
                    
                    if (!_gc.bSnapFree)
                    {
                        EditorGUILayout.LabelField(string.Format("F{1:0}_({0:0}, {2:0})",
                            _gridXYZ.x, _gridXYZ.y, _gridXYZ.z), GUILayout.MaxWidth(_mfWidth * 0.75f));
                        EditorGUILayout.Separator();
                    }
                }

                using (new EditorGUILayout.VerticalScope(GUILayout.Width(_fWidth * 0.4f)))
                {
                    Util.CustomEditorGUI.NewParagraph("[Transform Position]", GUILayout.MaxWidth(_mfWidth * 0.9f));
                    EditorGUILayout.LabelField("Global" + _gc.transform.position, GUILayout.MaxWidth(_mfWidth * 0.9f));
                    EditorGUILayout.LabelField("Local" + _gc.transform.localPosition, GUILayout.MaxWidth(_mfWidth * 0.9f));
                }
            }
        }
    }
}