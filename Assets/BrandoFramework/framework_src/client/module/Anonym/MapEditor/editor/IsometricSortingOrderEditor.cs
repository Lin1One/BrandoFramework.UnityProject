using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Anonym.Isometric
{
    using Util;

    [CanEditMultipleObjects]
	[CustomEditor(typeof(IsometricSortingOrder), true)]
    public class IsometricSortingOrderEditor : Editor
    {
        bool bPrefab;
		SerializedProperty _iParticleSortingAdd;
        SerializedProperty _iExternSortingAdd;
        SerializedProperty _iLastSortingOrder;
        // SerializedProperty _bStaticISO;
        
		IsometricSortingOrder _target;

		void OnEnable()
        {
			if ((_target = (IsometricSortingOrder)target) == null)
				return;

            if (bPrefab = PrefabHelper.IsPrefab(_target.gameObject))
                return;

			_iParticleSortingAdd = serializedObject.FindProperty("iParticleSortingAdd");
            _iLastSortingOrder = serializedObject.FindProperty("iLastSortingOrder");
            _iExternSortingAdd = serializedObject.FindProperty("_iExternAdd");
            // _bStaticISO = serializedObject.FindProperty("bStaticISO");
        }

		public override void OnInspectorGUI()
        {
            if (bPrefab){
                base.DrawDefaultInspector();
                return;
            }

			serializedObject.Update();

            EditorGUILayout.Separator();

            if(_iExternSortingAdd.intValue != 0)
                EditorGUILayout.LabelField("Extern Sorting Order : ", _iExternSortingAdd.intValue.ToString());

            bool bCorruptedSortingOrder = _target.IsCorrupted_LastSortingOrder();
            using (new EditorGUI.DisabledGroupScope(bCorruptedSortingOrder))
            {
                if (!bCorruptedSortingOrder)
                {
                    var basis = _target.GetISOBasis();
                    Util.CustomEditorGUI.NewParagraph(string.Format("[SortingOrder {0}]", _target.CalcSortingOrder(true)));
                    if (basis != null)
                    {
                        EditorGUILayout.ObjectField("Modified by", basis, typeof(ISOBasis), allowSceneObjects: true);
                    }
                    EditorGUI.indentLevel = 0;
                }
                else
                    EditorGUILayout.LabelField("Sorting Order was Corrupted or Disabled");
            }
            
            EditorGUI.BeginChangeCheck();
            
            using (new EditorGUI.DisabledGroupScope(IsoMap.isNormalSOMode))
            {
                EditorGUILayout.PropertyField(_iParticleSortingAdd, new GUIContent("iAdd for ParticleSorting : "));
            }

            bool bUpdated = EditorGUI.EndChangeCheck();
            
			serializedObject.ApplyModifiedProperties();
            
            if (bUpdated)
            {
                _target.Update_SortingOrder(true);
            }
		}
    }
}