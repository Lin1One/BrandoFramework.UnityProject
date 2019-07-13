#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using UnityEditor;
using UnityEditor.UI;

namespace Client.LegoUI.Editor
{
    [CustomEditor(typeof(YuLegoButton), true)]
    [CanEditMultipleObjects]
    public class YuLegoButtonEditor : SelectableEditor
    {
        SerializedProperty m_OnClickProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_OnClickProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}