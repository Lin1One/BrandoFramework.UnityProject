using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

// this from https://forum.unity.com/threads/hiding-the-grid-in-the-scene-view-through-script.442814/

namespace Anonym.Util
{
    public class AnnotationToggleUtility
    {
        private static Type m_annotationUtility;
        private static Type AnnotationUtility
        {
            get
            {
                if (m_annotationUtility == null)
                    m_annotationUtility = Type.GetType("UnityEditor.AnnotationUtility,UnityEditor.dll");

                return m_annotationUtility;
            }
        }

        private static Type m_annotationType;
        private static Type AnnotationType
        {
            get
            {
                if (m_annotationType == null)
                {
                    var enumerator = ((Array)Annotations).GetEnumerator();
                    enumerator.MoveNext();
                    m_annotationType = enumerator.Current.GetType();
                }
                return m_annotationType;
            }
        }

        private static PropertyInfo m_showGridProperty;
        private static PropertyInfo ShowGridProperty
        {
            get
            {
                if (m_showGridProperty == null)
                    m_showGridProperty = AnnotationUtility.GetProperty("showGrid", BindingFlags.Static | BindingFlags.NonPublic);

                return m_showGridProperty;
            }
        }
        public static bool ShowGrid
        {
            get
            {
                return (bool)ShowGridProperty.GetValue(null, null);
            }
            set
            {

                ShowGridProperty.SetValue(null, value, null);
            }
        }

        private static MethodInfo m_getAnnotation;
        private static MethodInfo GetAnnotation
        {
            get
            {
                if (m_getAnnotation == null)
                    m_getAnnotation = AnnotationUtility.GetMethod("GetAnnotations", BindingFlags.Static | BindingFlags.NonPublic);

                return m_getAnnotation;
            }
        }

        private static object m_annotations;
        private static object Annotations
        {
            get
            {
                if (m_annotations == null)
                    m_annotations = GetAnnotation.Invoke(null, null);

                return m_annotations;
            }
        }

        // https://docs.unity3d.com/Manual/ClassIDReference.html
        static void ToggleGizmo(int targetClassID, string className, bool bValue)
        {
            MethodInfo setGizmoEnabled = AnnotationUtility.GetMethod("SetGizmoEnabled", BindingFlags.Static | BindingFlags.NonPublic);

            FieldInfo classIdField = AnnotationType.GetField("classID", BindingFlags.Public | BindingFlags.Instance);
            FieldInfo scriptClassField = AnnotationType.GetField("scriptClass", BindingFlags.Public | BindingFlags.Instance);

            foreach (object annotation in (Array)Annotations)
            {
                int classId = (int)classIdField.GetValue(annotation);
                string scriptClass = (string)scriptClassField.GetValue(annotation);

                if (classId == targetClassID && (string.IsNullOrEmpty(className) || className.Equals(scriptClass)))
                {
                    setGizmoEnabled.Invoke(null, new object[] { classId, scriptClass, bValue ? 1 : 0 });
                    break;
                }
            }
        }

        static bool GetGizmo(int targetID, string className)
        {
            FieldInfo classIdField = AnnotationType.GetField("classID", BindingFlags.Public | BindingFlags.Instance);
            FieldInfo scriptClassField = AnnotationType.GetField("scriptClass", BindingFlags.Public | BindingFlags.Instance);
            FieldInfo gizmoEnabledField = AnnotationType.GetField("gizmoEnabled", BindingFlags.Public | BindingFlags.Instance); ;

            foreach (object annotation in (Array)Annotations)
            {
                int classId = (int)classIdField.GetValue(annotation);
                string scriptClass = (string)scriptClassField.GetValue(annotation);

                if (classId == targetID && (string.IsNullOrEmpty(className) || className.Equals(scriptClass)))
                {
                    return (int)gizmoEnabledField.GetValue(annotation) == 1;
                }
            }
            return false;
        }

        // 65 is BoxCollider
        public static bool ShowBoxColliderGizmo
        {
            get
            {
                return GetGizmo(65, null);
            }
            set
            {
                ToggleGizmo(65, null, value);
            }
        }

        // 223 is Canvas
        public static bool ShowCanvasGizmo
        {
            get
            {
                return GetGizmo(223, null);
            }
            set
            {
                ToggleGizmo(223, null, value);
            }
        }
    }
}