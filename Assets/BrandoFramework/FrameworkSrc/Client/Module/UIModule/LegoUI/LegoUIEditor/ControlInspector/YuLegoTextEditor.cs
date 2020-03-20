#region Head

// Author:            Yu
// CreateDate:        2018/8/15 20:42:05
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using Client.LegoUI;

namespace Client.LegoUI.Editor
{
    // TODO REVIEW
    // Have material live under Text
    // move stencil mask into effects *make an efects top level element like there is
    // paragraph and character

    /// <summary>
    /// Editor class used to edit UI Labels.
    /// </summary>
    [CustomEditor(typeof(YuLegoText), true)]
    [CanEditMultipleObjects]
    public class YuLegoTextEditor : GraphicEditor
    {
        SerializedProperty m_Text;
        SerializedProperty m_FontData;
        SerializedProperty m_StyleId;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Text = serializedObject.FindProperty("m_Text");
            m_FontData = serializedObject.FindProperty("m_FontData");
            m_StyleId = serializedObject.FindProperty("m_StyleId");
        }

        public override void OnInspectorGUI()
        {
            var legoText = (YuLegoText) target;

            serializedObject.Update();

            EditorGUILayout.PropertyField(m_Text);
            EditorGUILayout.PropertyField(m_FontData);
            EditorGUILayout.PropertyField(m_StyleId);
            AppearanceControlsGUI();
            RaycastControlsGUI();
            if (GUILayout.Button("AdaptPreferredSize"))
            {
                legoText.AdaptPreferredSize();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}