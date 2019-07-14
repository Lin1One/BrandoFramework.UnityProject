using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Anonym.Isometric
{
    using Util;
	[CustomEditor(typeof(AutoNaming))]
    public class AutoNamingEditor : Editor
    {
        AutoNaming _t;
        SerializedProperty OnOffToggle;        
        SerializedProperty Postfix_Sprite;
        SerializedProperty formatString;
        bool bPrefab = false;

		void OnEnable()
        {
			_t = (AutoNaming) target;
            if (bPrefab = PrefabHelper.IsPrefab(_t.gameObject))
                return;
            OnOffToggle = serializedObject.FindProperty("bAutoName");
            Postfix_Sprite = serializedObject.FindProperty("bPostfix_Sprite");
            formatString = serializedObject.FindProperty("format");
        }

		public override void OnInspectorGUI()
        {	
            if (bPrefab)
            {
                base.DrawDefaultInspector();
                return;
            }
            bool bChanged = false;
            serializedObject.Update();
            EditorGUILayout.PropertyField(OnOffToggle);
            if (OnOffToggle.boolValue)
            {
                EditorGUILayout.LabelField(_t.name, EditorStyles.largeLabel);
                using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightYellow))
                {
                    if (GUILayout.Button("ReName"))
                        _t.AutoName();
                }
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(formatString);    
                EditorGUILayout.PropertyField(Postfix_Sprite);
                if (EditorGUI.EndChangeCheck())
                {
                    bChanged = true;
                }
            }
            serializedObject.ApplyModifiedProperties();
            if (bChanged)
                _t.AutoName();
		}		
    }
}