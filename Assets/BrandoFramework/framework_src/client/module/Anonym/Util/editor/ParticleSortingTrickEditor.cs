using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Anonym.Util
{
    [CustomEditor(typeof(ParticleSortingTrick))]
    [CanEditMultipleObjects]
	public class ParticleSortingTrickEditor : Editor {

		bool bFoldout = true;
        bool bPrefab = false;
		ParticleSortingTrick _target;
		// int iMaxRowCount = 6;
		SerializedProperty _fFudgeMax;

		bool undoredo()
        {
            if (Event.current.commandName == "UndoRedoPerformed")
            {
                return true;
            }
            return false;
        }
		void OnEnable()
        {
            _target = (ParticleSortingTrick)target;			
            if (bPrefab = PrefabHelper.IsPrefab(_target.gameObject))
                return;

			_target.Update_Child(true);

			_fFudgeMax = serializedObject.FindProperty("fFudgeMax");
        }

        public override void OnInspectorGUI()
        {
            if (bPrefab)
            {
                base.DrawDefaultInspector();
                return;
            }

            if (undoredo())
                return;

			EditorGUI.indentLevel++;

			serializedObject.Update();
			EditorGUILayout.PropertyField(_fFudgeMax, new GUIContent("MAX Sorting Fudge for UI", 
				"The smaller value is closer to the camera."));
			EditorGUILayout.Separator();
			
			showChild();
			serializedObject.ApplyModifiedProperties();

		}

		private void showChild()
        {
			int iChildCount = _target.Childs.Count;
            using (new EditorGUILayout.HorizontalScope())
            {
                bFoldout = EditorGUILayout.Foldout(bFoldout, 
                    string.Format("ParticleSystems({0})", iChildCount), true);

                EditorGUI.BeginChangeCheck();

                using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightBlue))
                {
                    if (GUILayout.Button("Missing Check", GUILayout.Width(125)))
                    {
                        _target.Update_Child(false);
                    }
                }

                using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightMagenta))
                {
                    if (GUILayout.Button("Clear", GUILayout.Width(75)))
                    {
                        _target.Clear();
                    }                    
                }
            }

            if (bFoldout && iChildCount > 0)
            {
				float foldHeight = 0;
				SerializedProperty _child;

				EditorGUILayout.GetControlRect(GUILayout.Height(foldHeight));

				for(int i = 0 ; i < iChildCount; ++i)
				{
					_child = serializedObject.FindProperty(string.Format(@"_pdArray.Array.data[{0}]", i));
					EditorGUI.PropertyField(ParticleTrickDataDrawer.GetRect(_child), _child);
				}
            }
        }
	}
}