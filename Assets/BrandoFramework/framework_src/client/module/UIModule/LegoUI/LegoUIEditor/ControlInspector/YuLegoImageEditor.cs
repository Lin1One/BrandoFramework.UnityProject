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

using System.Linq;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.UI;
using UnityEngine;
using Client.LegoUI;

namespace Client.LegoUI.Editor
{
    /// <summary>
    /// Editor class used to edit UI Sprites.
    /// </summary>
    [CustomEditor(typeof(YuLegoImage), true)]
    [CanEditMultipleObjects]
    public class YuLegoImageEditor : GraphicEditor
    {
        SerializedProperty m_FillMethod;
        SerializedProperty m_FillOrigin;
        SerializedProperty m_FillAmount;
        SerializedProperty m_FillClockwise;
        SerializedProperty m_Type;
        SerializedProperty m_FillCenter;
        SerializedProperty m_Sprite;
        SerializedProperty m_PreserveAspect;
        GUIContent m_SpriteContent;
        GUIContent m_SpriteTypeContent;
        GUIContent m_ClockwiseContent;
        AnimBool m_ShowSlicedOrTiled;
        AnimBool m_ShowSliced;
        AnimBool m_ShowTiled;
        AnimBool m_ShowFilled;
        AnimBool m_ShowType;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_SpriteContent = new GUIContent("Source Image");
            m_SpriteTypeContent = new GUIContent("Image ScriptType");
            m_ClockwiseContent = new GUIContent("Clockwise");

            m_Sprite = serializedObject.FindProperty("m_Sprite");
            m_Type = serializedObject.FindProperty("m_Type");
            m_FillCenter = serializedObject.FindProperty("m_FillCenter");
            m_FillMethod = serializedObject.FindProperty("m_FillMethod");
            m_FillOrigin = serializedObject.FindProperty("m_FillOrigin");
            m_FillClockwise = serializedObject.FindProperty("m_FillClockwise");
            m_FillAmount = serializedObject.FindProperty("m_FillAmount");
            m_PreserveAspect = serializedObject.FindProperty("m_PreserveAspect");

            m_ShowType = new AnimBool(m_Sprite.objectReferenceValue != null);
            m_ShowType.valueChanged.AddListener(Repaint);

            var typeEnum = (YuLegoImage.Type) m_Type.enumValueIndex;

            m_ShowSlicedOrTiled =
                new AnimBool(!m_Type.hasMultipleDifferentValues && typeEnum == YuLegoImage.Type.Sliced);
            m_ShowSliced = new AnimBool(!m_Type.hasMultipleDifferentValues && typeEnum == YuLegoImage.Type.Sliced);
            m_ShowTiled = new AnimBool(!m_Type.hasMultipleDifferentValues && typeEnum == YuLegoImage.Type.Tiled);
            m_ShowFilled = new AnimBool(!m_Type.hasMultipleDifferentValues && typeEnum == YuLegoImage.Type.Filled);
            m_ShowSlicedOrTiled.valueChanged.AddListener(Repaint);
            m_ShowSliced.valueChanged.AddListener(Repaint);
            m_ShowTiled.valueChanged.AddListener(Repaint);
            m_ShowFilled.valueChanged.AddListener(Repaint);

            SetShowNativeSize(true);
        }

        protected override void OnDisable()
        {
            m_ShowType.valueChanged.RemoveListener(Repaint);
            m_ShowSlicedOrTiled.valueChanged.RemoveListener(Repaint);
            m_ShowSliced.valueChanged.RemoveListener(Repaint);
            m_ShowTiled.valueChanged.RemoveListener(Repaint);
            m_ShowFilled.valueChanged.RemoveListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SpriteGUI();
            AppearanceControlsGUI();
            RaycastControlsGUI();

            m_ShowType.target = m_Sprite.objectReferenceValue != null;
            if (EditorGUILayout.BeginFadeGroup(m_ShowType.faded))
                TypeGUI();
            EditorGUILayout.EndFadeGroup();

            SetShowNativeSize(false);
            if (EditorGUILayout.BeginFadeGroup(m_ShowNativeSize.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_PreserveAspect);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFadeGroup();
            NativeSizeButtonGUI();

            serializedObject.ApplyModifiedProperties();
        }

        void SetShowNativeSize(bool instant)
        {
            var type = (YuLegoImage.Type) m_Type.enumValueIndex;
            bool showNativeSize = (type == YuLegoImage.Type.Simple || type == YuLegoImage.Type.Filled) &&
                                  m_Sprite.objectReferenceValue != null;
            base.SetShowNativeSize(showNativeSize, instant);
        }

        /// <summary>
        /// Draw the atlas and Image selection fields.
        /// </summary>
        protected void SpriteGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_Sprite, m_SpriteContent);
            if (EditorGUI.EndChangeCheck())
            {
                var newSprite = m_Sprite.objectReferenceValue as Sprite;
                if (newSprite)
                {
                    var oldType = (YuLegoImage.Type) m_Type.enumValueIndex;
                    if (newSprite.border.SqrMagnitude() > 0)
                    {
                        m_Type.enumValueIndex = (int) YuLegoImage.Type.Sliced;
                    }
                    else if (oldType == YuLegoImage.Type.Sliced)
                    {
                        m_Type.enumValueIndex = (int) YuLegoImage.Type.Simple;
                    }
                }
            }
        }

        /// <summary>
        /// Sprites's custom properties based on the type.
        /// </summary>
        protected void TypeGUI()
        {
            EditorGUILayout.PropertyField(m_Type, m_SpriteTypeContent);

            ++EditorGUI.indentLevel;
            {
                var typeEnum = (YuLegoImage.Type) m_Type.enumValueIndex;

                bool showSlicedOrTiled = (!m_Type.hasMultipleDifferentValues &&
                                          (typeEnum == YuLegoImage.Type.Sliced || typeEnum == YuLegoImage.Type.Tiled));
                if (showSlicedOrTiled && targets.Length > 1)
                    showSlicedOrTiled = targets.Select(obj => obj as YuLegoImage).All(img => img.hasBorder);

                m_ShowSlicedOrTiled.target = showSlicedOrTiled;
                m_ShowSliced.target = (showSlicedOrTiled && !m_Type.hasMultipleDifferentValues &&
                                       typeEnum == YuLegoImage.Type.Sliced);
                m_ShowTiled.target = (showSlicedOrTiled && !m_Type.hasMultipleDifferentValues &&
                                      typeEnum == YuLegoImage.Type.Tiled);
                m_ShowFilled.target = (!m_Type.hasMultipleDifferentValues && typeEnum == YuLegoImage.Type.Filled);

                YuLegoImage image = target as YuLegoImage;
                if (EditorGUILayout.BeginFadeGroup(m_ShowSlicedOrTiled.faded))
                {
                    if (image.hasBorder)
                        EditorGUILayout.PropertyField(m_FillCenter);
                }

                EditorGUILayout.EndFadeGroup();

                if (EditorGUILayout.BeginFadeGroup(m_ShowSliced.faded))
                {
                    if (image.sprite != null && !image.hasBorder)
                        EditorGUILayout.HelpBox("This Image doesn't have a border.", MessageType.Warning);
                }

                EditorGUILayout.EndFadeGroup();

                if (EditorGUILayout.BeginFadeGroup(m_ShowTiled.faded))
                {
                    if (image.sprite != null && !image.hasBorder &&
                        (image.sprite.texture.wrapMode != TextureWrapMode.Repeat || image.sprite.packed))
                        EditorGUILayout.HelpBox(
                            "It looks like you want to tile a sprite with no border. It would be more efficient to modify the Sprite properties, clear the Packing tag and set the Wrap mode to Repeat.",
                            MessageType.Warning);
                }

                EditorGUILayout.EndFadeGroup();

                if (EditorGUILayout.BeginFadeGroup(m_ShowFilled.faded))
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(m_FillMethod);
                    if (EditorGUI.EndChangeCheck())
                    {
                        m_FillOrigin.intValue = 0;
                    }

                    switch ((YuLegoImage.FillMethod) m_FillMethod.enumValueIndex)
                    {
                        case YuLegoImage.FillMethod.Horizontal:
                            m_FillOrigin.intValue =
                                (int) (YuLegoImage.OriginHorizontal) EditorGUILayout.EnumPopup("Fill Origin",
                                    (YuLegoImage.OriginHorizontal) m_FillOrigin.intValue);
                            break;
                        case YuLegoImage.FillMethod.Vertical:
                            m_FillOrigin.intValue =
                                (int) (YuLegoImage.OriginVertical) EditorGUILayout.EnumPopup("Fill Origin",
                                    (YuLegoImage.OriginVertical) m_FillOrigin.intValue);
                            break;
                        case YuLegoImage.FillMethod.Radial90:
                            m_FillOrigin.intValue =
                                (int) (YuLegoImage.Origin90) EditorGUILayout.EnumPopup("Fill Origin",
                                    (YuLegoImage.Origin90) m_FillOrigin.intValue);
                            break;
                        case YuLegoImage.FillMethod.Radial180:
                            m_FillOrigin.intValue =
                                (int) (YuLegoImage.Origin180) EditorGUILayout.EnumPopup("Fill Origin",
                                    (YuLegoImage.Origin180) m_FillOrigin.intValue);
                            break;
                        case YuLegoImage.FillMethod.Radial360:
                            m_FillOrigin.intValue =
                                (int) (YuLegoImage.Origin360) EditorGUILayout.EnumPopup("Fill Origin",
                                    (YuLegoImage.Origin360) m_FillOrigin.intValue);
                            break;
                    }

                    EditorGUILayout.PropertyField(m_FillAmount);
                    if ((YuLegoImage.FillMethod) m_FillMethod.enumValueIndex > YuLegoImage.FillMethod.Vertical)
                    {
                        EditorGUILayout.PropertyField(m_FillClockwise, m_ClockwiseContent);
                    }
                }

                EditorGUILayout.EndFadeGroup();
            }
            --EditorGUI.indentLevel;
        }

        /// <summary>
        /// All graphics have a preview.
        /// </summary>
        public override bool HasPreviewGUI()
        {
            return true;
        }

        /// <summary>
        /// Draw the Image preview.
        /// </summary>
        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            YuLegoImage image = target as YuLegoImage;
            if (image == null) return;

            Sprite sf = image.sprite;
            if (sf == null) return;

            YuLegoSpriteDrawUtility.DrawSprite(sf, rect, image.canvasRenderer.GetColor());
        }

        /// <summary>
        /// Info String drawn at the bottom of the Preview
        /// </summary>
        public override string GetInfoString()
        {
            YuLegoImage image = target as YuLegoImage;
            Sprite sprite = image.sprite;

            int x = (sprite != null) ? Mathf.RoundToInt(sprite.rect.width) : 0;
            int y = (sprite != null) ? Mathf.RoundToInt(sprite.rect.height) : 0;

            return string.Format("Image Size: {0}x{1}", x, y);
        }
    }
}