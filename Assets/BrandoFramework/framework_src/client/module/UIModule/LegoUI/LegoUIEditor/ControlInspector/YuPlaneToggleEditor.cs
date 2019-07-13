using UnityEditor;
using UnityEditor.UI;
using Client.LegoUI;

namespace Client.LegoUI.Editor
{
    [CustomEditor(typeof(YuLegoPlaneToggle), true)]
    [CanEditMultipleObjects]
    public class YuPlaneToggleEditor : SelectableEditor
    {
        SerializedProperty m_OnClickProperty;
        SerializedProperty m_IsOnProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
            m_IsOnProperty = serializedObject.FindProperty("m_IsOn");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_IsOnProperty);
            EditorGUILayout.PropertyField(m_OnClickProperty);
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
    }
}