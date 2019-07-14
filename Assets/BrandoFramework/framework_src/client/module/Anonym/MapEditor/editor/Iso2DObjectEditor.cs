using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Sprites;

namespace Anonym.Isometric
{
    using Util;

	[CustomEditor(typeof(Iso2DObject))]
     public class Iso2DObjectEditor : Editor
    {
		bool bPrefab = false;
        Sprite tileSprite;
        
        public static float Max_Slider = 2f;

		void OnEnable()
        {
			if (bPrefab = PrefabHelper.IsPrefab((target as Iso2DObject).gameObject))
                return;
        }

        bool undoredo()
        {
            if (Event.current.commandName == "UndoRedoPerformed")
            {
                Repaint();
                return true;
            }
            return false;
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
            
			EditorGUILayout.Separator();
			CustomEditorGUI.Iso2DObjectField(serializedObject);
            EditorGUILayout.Separator();			
		}

		public void OnSceneGUI()
        {
			if (bPrefab)
                return;
		}
        
    }
}