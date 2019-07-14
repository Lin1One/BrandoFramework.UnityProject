using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Anonym.Util
{
    [CustomEditor(typeof(SelfieStick))]
    public class SelfieStickEditor : Editor
    {
		SerializedProperty _Transform;
		SerializedProperty iRenderTexture_width;
		SerializedProperty iRenderTexture_Height;
        SerializedProperty localCam;
        SerializedProperty textureWrapMode;
        SerializedProperty filterMode;
        SerializedProperty isometricSortingOrder;
        SerializedProperty spriteRenderer;
        SerializedProperty spRenderObjects;

        SelfieStick _target;

        void OnEnable()
        {
            _target = target as SelfieStick;
            _Transform = serializedObject.FindProperty("_Target");
			iRenderTexture_width = serializedObject.FindProperty("_iRenderTexture_Width");
			iRenderTexture_Height = serializedObject.FindProperty("_iRenderTexture_Height");
            localCam = serializedObject.FindProperty("_localCam");
            textureWrapMode = serializedObject.FindProperty("textureWrapMode");
            filterMode = serializedObject.FindProperty("filterMode");
            isometricSortingOrder = serializedObject.FindProperty("ISO");
            spriteRenderer = serializedObject.FindProperty("sprr");
            spRenderObjects = serializedObject.FindProperty("_renderObjectList");
        }

        public override void OnInspectorGUI()
        {
            bool bSizeUpdated = false;

            serializedObject.Update();

            if (!Application.isPlaying)
            {
                EditorGUILayout.LabelField(new GUIContent("Set Custom Render Texture Propoety", "Camera Orthographic will adjusted automatically.\nWhen you change Height value."));
                EditorGUILayout.PropertyField(localCam);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(iRenderTexture_width);
                EditorGUILayout.PropertyField(iRenderTexture_Height);
                bSizeUpdated = EditorGUI.EndChangeCheck();
                EditorGUILayout.Separator();

                EditorGUILayout.LabelField("Set Texture Mode");
                EditorGUILayout.PropertyField(textureWrapMode);
                EditorGUILayout.PropertyField(filterMode);
            }

            EditorGUILayout.LabelField("Important Objects");
            EditorGUILayout.PropertyField(_Transform);
            EditorGUILayout.PropertyField(isometricSortingOrder);
            EditorGUILayout.PropertyField(spriteRenderer);
            EditorGUILayout.PropertyField(spRenderObjects, true);
            EditorGUILayout.Separator();
            


            serializedObject.ApplyModifiedProperties();

            if (bSizeUpdated)
            {
                Undo.RecordObject(localCam.objectReferenceValue, "Changed : SelfieStick Size");
                _target.AdjustSize();
            }
        }

        public void OnSceneGUI()
        {
            // var t = (target as SelfieStick);

            // EditorGUI.BeginChangeCheck();
            // Vector3 pos = Handles.PositionHandle(t._arm_position, Quaternion.identity);
            // if (EditorGUI.EndChangeCheck())
            // {
            //     Undo.RecordObject(target, "Move point");
            //     t._arm_position = pos;
            //     t.Calc();
            // }
        }
    }
}