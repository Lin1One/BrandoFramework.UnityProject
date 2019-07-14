using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Anonym.Util
{
    public static class UndoUtil
    {
        public static bool IsPrefabConnected(this Object _object)
        {
#if UNITY_EDITOR && UNITY_2018_3_OR_NEWER
            return PrefabUtility.IsPartOfPrefabAsset(_object) || PrefabUtility.IsPartOfPrefabInstance(_object);
#else
            if (_object is MonoBehaviour)
                return PrefabHelper.IsPrefab((_object as MonoBehaviour).gameObject);

            return PrefabHelper.IsPrefab(_object as GameObject);
#endif
        }

        public static void Record(Object[] _objects, string _name)
        {
#if UNITY_EDITOR
            var prefabObjects = _objects.Where(o => o.IsPrefabConnected());
            foreach (var one in prefabObjects)
                Record(one, _name);

            var notPrefabObjects = _objects.Where(o => !o.IsPrefabConnected());
            Undo.RecordObjects(notPrefabObjects.ToArray(), _name);
#endif
        }

        public static void Record(Object _object, string _name)
        {
#if UNITY_EDITOR
            if (_object.IsPrefabConnected())
                PrefabUtility.RecordPrefabInstancePropertyModifications(_object);
            else
                Undo.RecordObject(_object, _name);
#endif
        }

        public static void Create(Object _object, string _name)
        {
#if UNITY_EDITOR
            if (_object.IsPrefabConnected())
                PrefabUtility.RecordPrefabInstancePropertyModifications(_object);
            else
                Undo.RegisterCreatedObjectUndo(_object, _name);
#endif
        }

        public static bool Delete(Object _object)
        {
#if UNITY_EDITOR
            if (_object.IsPrefabConnected())
            {
                PrintNotSupportedMSG();
                return false;
            }
            else
                Undo.DestroyObjectImmediate(_object);
#else
            GameObject.Destroy(_object);
#endif
            return true;
        }

        public static void PrintNotSupportedMSG()
        {
            Debug.LogWarning("That's not supported with Prefab Connected Object. Please, unpack prefab completely before do this.\nOr Or Open Prefab and edit it.");
        }
    }
}
