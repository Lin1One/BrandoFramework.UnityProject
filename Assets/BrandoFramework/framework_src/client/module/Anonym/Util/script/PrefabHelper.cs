using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Anonym.Util
{
    public static class PrefabHelper
    {
        public static bool IsPrefab(GameObject go)
        {
            if (go == null || go.scene == null)
                return false;

            return go.scene.rootCount == 0;
        }

        public static bool IsPrefab(GameObject[] gos)
        {
            return gos.Any(go => IsPrefab(go));
        }

//        public static bool IsPrefab(Object target)
//        {
//#if UNITY_EDITOR && UNITY_2018_3_OR_NEWER
//            PrefabAssetType type = PrefabUtility.GetPrefabAssetType(target);
//            if (type != PrefabAssetType.NotAPrefab)
//                return true;
//#else
//            if (PrefabUtility.GetPrefabType(target).Equals(PrefabType.Prefab))
//                return true;
//#endif
//            return false;
//        }
//        public static bool IsPrefab(Object[] targets)
//        {
//            return targets.Any(o => IsPrefab(o));
//        }

#if UNITY_EDITOR

        public static GameObject CreatePrefab(string path, GameObject go)
        {
#if UNITY_2018_3_OR_NEWER
            return PrefabUtility.SaveAsPrefabAsset(go, path);
#else
            return PrefabUtility.CreatePrefab(path, go);
#endif
        }

#endif
    }
    }