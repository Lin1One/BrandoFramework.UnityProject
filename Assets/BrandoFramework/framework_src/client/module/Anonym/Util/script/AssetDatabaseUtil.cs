using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Anonym.Util
{
    public static class AssetDatabaseUtil
    {
        public static void DeleteAsset_All_Unregistered_Child<T>(this Object parent, IEnumerable<T> register) where T: class
        {
#if UNITY_EDITOR
            var childs = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(parent));
            var expiredList = childs.Where(c => c != parent && !register.Contains(c as T)).ToArray();
            for (int i = 0; i < expiredList.Length; ++i)
            {
                DeleteAsset(expiredList[i], true);
            }
            AssetDatabase.SaveAssets();
#endif
        }

        public static void DeleteAsset(this Object target, bool bWithotSave = false)
        {
#if UNITY_EDITOR
            if (AssetDatabase.IsSubAsset(target))
            {
                Object.DestroyImmediate(target, true);
            }
            else
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(target));

            if (!bWithotSave)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
#endif
        }
    }
}
