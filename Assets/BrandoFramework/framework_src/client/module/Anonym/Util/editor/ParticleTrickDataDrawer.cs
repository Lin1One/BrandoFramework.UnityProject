using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.Reflection;

namespace Anonym.Util
{
    [CustomPropertyDrawer(typeof(ParticleTrickData))]
    public class ParticleTrickDataDrawer : PropertyDrawer
    {
        static int iRows = 7;

        public static float GetCellHeight(SerializedProperty property)
        {
            bool bFoldout = property.FindPropertyRelative ("bUseTrick").boolValue;
            float lineCount = bFoldout ? iRows : 1;
            return EditorGUIUtility.singleLineHeight * lineCount;
        }

        public static Rect GetRect(SerializedProperty property)
        {            
            Rect rt = EditorGUILayout.GetControlRect(
                        new GUILayoutOption[] { GUILayout.Height(GetCellHeight(property)), 
                        GUILayout.ExpandWidth(true) });
            return EditorGUI.IndentedRect(rt);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)   
        {
            if (property.serializedObject.isEditingMultipleObjects)
                return EditorGUIUtility.singleLineHeight;

            return GetCellHeight(property);
        }
    
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.isEditingMultipleObjects)
            {
                EditorGUI.LabelField(position, "Multiple objects can not be modified at the same time.");
                return;
            }

            //if (Event.current.type == EventType.ScrollWheel)
            //{
            //    EditorGUIUtility.ExitGUI();
            //    return;
            //}

            ParticleSortingTrick _base = (ParticleSortingTrick)property.serializedObject.targetObject;
            EditorGUI.BeginProperty (position, label, property);

            SerializedProperty _spUse = property.FindPropertyRelative ("bUseTrick");
            SerializedProperty _spOverrideColor = property.FindPropertyRelative("bOverrideColor");

            bool bUse = _spUse.boolValue;

            Rect[] _rts_lines = position.Division(1, _spUse.boolValue ? iRows : 1);
            Rect[] _rts_title = _rts_lines[0].Division(new float[2]{0.5f, 0.5f}, null);

            using (var result = new EditorGUI.ChangeCheckScope())
            {
                _spUse.boolValue = EditorGUI.ToggleLeft (_rts_title[0], new GUIContent("Use Trick(" + label.text + ")"), bUse);
                if (result.changed)
                {
                    if (_spUse.boolValue && _spOverrideColor.boolValue)
                    {
                        _base.Sync(false);
                    }
                    else
                    {
                        _base.Clear();
                    }
                }
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(_rts_title[1], property.FindPropertyRelative ("_ps"), GUIContent.none);
            EditorGUI.EndDisabledGroup();
            
            if (bUse)
            {                
                Rect[] _rts_lr;
                string [] _elements = SortingLayer.layers.Select( r => r.name).ToArray();

                EditorGUI.BeginChangeCheck();

                SerializedProperty _spSortingLayer = property.FindPropertyRelative ("sortingLayer");
                _spSortingLayer.intValue = EditorGUI.Popup(EditorGUI.IndentedRect(_rts_lines[1]), "SortingLayer",
                    SortingLayer.layers.Length > _spSortingLayer.intValue ? _spSortingLayer.intValue : 0, _elements);

                SerializedProperty _spFudge = property.FindPropertyRelative ("fParticleSortingFudge");
                EditorGUI.Slider(EditorGUI.IndentedRect(_rts_lines[2]), _spFudge, -_base.fFudgeMax, _base.fFudgeMax, "Sorting Fudge");

                _rts_lr = _rts_lines[3].Division(2, 1);
                SerializedProperty _spCollision = property.FindPropertyRelative ("bDynamicParticle_SelfColliderOn");
                _spCollision.boolValue = EditorGUI.ToggleLeft(EditorGUI.IndentedRect(_rts_lr[0]), 
                    new GUIContent("Trick: Death on Collision", "Removes particles when there is a collision object between the particle origin and the particle position to improve the pass-through problem. Similar to PS's Collision but slightly different result"), 
                    _spCollision.boolValue);

                if (_spCollision.boolValue)
                {
                    SerializedProperty _spLayerMask = property.FindPropertyRelative ("layerMask");
                    _spLayerMask.intValue = EditorGUI.MaskField(_rts_lr[1], _spLayerMask.intValue, _elements);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    ParticleTrickData.Apply_SortingData((ParticleSystemRenderer) property.FindPropertyRelative ("_prr").objectReferenceValue, 
                        _spFudge.floatValue, _spSortingLayer.intValue);
                }

                _rts_lr = _rts_lines[4].Division(2, 1);                                    

                EditorGUI.BeginChangeCheck();
                _spOverrideColor.boolValue = EditorGUI.ToggleLeft(EditorGUI.IndentedRect(_rts_lr[0]), 
                    new GUIContent("Trick: Start Color", "Fixed the problem of color splashing when particles were created by modifying ParticleSystem's startColor."), 
                    _spOverrideColor.boolValue);

                if (EditorGUI.EndChangeCheck())
                {
                    if (_spOverrideColor.boolValue)
                    {
                        _base.Sync(false);
                    }
                    else
                    {
                        _base.Clear();
                    }

                    // ParticleTrickData.Sync_ColorData(
                    //     (ParticleSystem) property.FindPropertyRelative ("_ps").objectReferenceValue, 
                    //     _spOverrideColor.boolValue);
                }

                if (_spOverrideColor.boolValue)
                {
                    SerializedProperty _spColor = property.FindPropertyRelative ("hiddenColor");
                    using (new EditorGUI.DisabledGroupScope(true))
                    {
                        using (var result = new EditorGUI.ChangeCheckScope())
                        {
                            EditorGUI.PropertyField(_rts_lr[1], _spColor, new GUIContent("Color for Obscured state"));
                            if (result.changed)
                            {
                                SerializedProperty _spTmp = property.FindPropertyRelative ("_ps");
                                if (_spTmp != null)
                                {
                                    ParticleSystem _ps = (ParticleSystem) _spTmp.objectReferenceValue;
                                    if (_ps != null)
                                    {
                                        var _main = _ps.main;
                                        _main.startColor = _spColor.colorValue;
                                    }
                                }
                            }
                        }
                    }
                }

                if (_spOverrideColor.boolValue)
                {
                    SerializedProperty _spStartColorData = property.FindPropertyRelative ("startColorData");
                    SerializedProperty _spSDMode = _spStartColorData.FindPropertyRelative ("_mode");
                    
                    EditorGUI.indentLevel++;
                    EditorGUI.PropertyField(EditorGUI.IndentedRect(_rts_lines[5]), _spSDMode);

                    _rts_lr = (_rts_lines[6] = EditorGUI.IndentedRect(_rts_lines[6])).Division(2, 1);

                    switch(_spSDMode.enumValueIndex)
                    {
                        case (int)ParticleSystemGradientMode.Color:
                            EditorGUI.PropertyField(_rts_lines[6], _spStartColorData.FindPropertyRelative ("_color1"));
                            break;
                        case (int)ParticleSystemGradientMode.Gradient:
                            EditorGUI.PropertyField(_rts_lines[6], _spStartColorData.FindPropertyRelative ("_gradient1"));
                            break;
                        case (int)ParticleSystemGradientMode.RandomColor:
                            EditorGUI.PropertyField(_rts_lines[6], _spStartColorData.FindPropertyRelative ("_gradient1"));
                            break;
                        case (int)ParticleSystemGradientMode.TwoColors:
                            EditorGUI.PropertyField(_rts_lr[0], _spStartColorData.FindPropertyRelative ("_color1"));
                            EditorGUI.PropertyField(_rts_lr[1], _spStartColorData.FindPropertyRelative ("_color2"));
                            break;
                        case (int)ParticleSystemGradientMode.TwoGradients:
                            EditorGUI.PropertyField(_rts_lr[0], _spStartColorData.FindPropertyRelative ("_gradient1"));
                            EditorGUI.PropertyField(_rts_lr[1], _spStartColorData.FindPropertyRelative ("_gradient2"));
                            break;
                    }

                    EditorGUI.indentLevel--;
                }

            }
            EditorGUI.EndProperty();
        }
    }
}

