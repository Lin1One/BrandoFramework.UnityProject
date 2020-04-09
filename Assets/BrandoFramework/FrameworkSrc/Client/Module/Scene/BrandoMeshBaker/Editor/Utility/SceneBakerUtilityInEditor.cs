/**
 *	DLLs cannot interpret preprocessor directives, so this class acts as a "bridge"
 */
using UnityEngine;
using UnityEditor;

namespace GameWorld.Editor
{
    public class SceneBakerUtilityInEditor
    {
        #region Platform

        //Used to map the activeBuildTarget to a string argument needed by TextureImporter.GetPlatformTextureSettings
        //The allowed values for GetPlatformTextureSettings are "Web", "Standalone", "iPhone", "Android" and "FlashPlayer".
        public static string GetPlatformString()
        {
#if (UNITY_4_6 || UNITY_4_7 || UNITY_4_5 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4_0_1 || UNITY_4_0 || UNITY_3_5)
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone){
                return "iPhone";	
            }
#else
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {
                return "iPhone";
            }
#endif
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WSAPlayer)
            {
                return "Windows Store Apps";
            }
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.PSP2)
            {
                return "PSP2";
            }
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.PS4)
            {
                return "PS4";
            }
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.XboxOne)
            {
                return "XboxOne";
            }
#if (UNITY_2017_3_OR_NEWER)
#else
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.SamsungTV)
            {
                return "Samsung TV";
            }
#endif
#if (UNITY_5_5_OR_NEWER)
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.N3DS)
            {
                return "Nintendo 3DS";
            }
#endif
#if (UNITY_5_3 || UNITY_5_2 || UNITY_5_3_OR_NEWER)
#if (UNITY_2018_1_OR_NEWER)
            // wiiu support was removed in 2018.1
#else
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WiiU)
            {
                return "WiiU";
            }
#endif
#endif
#if (UNITY_5_3 || UNITY_5_3_OR_NEWER)
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.tvOS)
            {
                return "tvOS";
            }
#endif
#if (UNITY_2018_2_OR_NEWER)

#else
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Tizen)
            {
                return "Tizen";
            }
#endif
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                return "Android";
            }
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneLinux ||
                EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneLinux64 ||
                EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneLinuxUniversal ||
                EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows ||
                EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64 ||
#if UNITY_2017_3_OR_NEWER
                EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSX
#else
                EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSXIntel ||
                EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSXIntel64 ||
                EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSXUniversal
#endif
                )
            {
                return "Standalone";
            }
#if !UNITY_5_4_OR_NEWER
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebPlayer ||
                EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebPlayerStreamed
                )
            {
                return "Web";
            }
#endif
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
            {
                return "WebGL";
            }
            return null;
        }

        #endregion

        #region UnityEditor

        public static void RegisterUndo(UnityEngine.Object o, string s)
        {
            Undo.RecordObject(o, s);
        }

        public static void SetInspectorLabelWidth(float width)
        {
            EditorGUIUtility.labelWidth = width;
        }

        #endregion

        #region Asset

        public static void UpdateIfDirtyOrScript(SerializedObject so)
        {
#if UNITY_5_6_OR_NEWER
            so.UpdateIfRequiredOrScript();
#else
            so.UpdateIfDirtyOrScript();
#endif
        }

        public static UnityEngine.Object PrefabUtility_GetCorrespondingObjectFromSource(GameObject go)
        {
#if UNITY_2018_2_OR_NEWER
            return PrefabUtility.GetCorrespondingObjectFromSource(go);
#else
            return PrefabUtility.GetPrefabParent(go);
#endif
        }

        public static bool IsAutoPVRTC(TextureImporterFormat platformFormat, TextureImporterFormat platformDefaultFormat)
        {
            if ((
#if UNITY_2017_1_OR_NEWER
                    platformFormat == TextureImporterFormat.Automatic
#elif UNITY_5_5_OR_NEWER
                    platformFormat == TextureImporterFormat.Automatic ||
                    platformFormat == TextureImporterFormat.Automatic16bit ||
                    platformFormat == TextureImporterFormat.AutomaticCompressed ||
                    platformFormat == TextureImporterFormat.AutomaticCompressedHDR ||
                    platformFormat == TextureImporterFormat.AutomaticCrunched ||
                    platformFormat == TextureImporterFormat.AutomaticHDR
#else
                    platformFormat == TextureImporterFormat.Automatic16bit ||
                    platformFormat == TextureImporterFormat.AutomaticCompressed ||
                    platformFormat == TextureImporterFormat.AutomaticCrunched
#endif
                ) && (
                    platformDefaultFormat == TextureImporterFormat.PVRTC_RGB2 ||
                    platformDefaultFormat == TextureImporterFormat.PVRTC_RGB4 ||
                    platformDefaultFormat == TextureImporterFormat.PVRTC_RGBA2 ||
                    platformDefaultFormat == TextureImporterFormat.PVRTC_RGBA4
                ))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// ��ȡ��Ϸ����Ԥ�������ͣ��������壬Ԥ���壬ģ��Ԥ���壩
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static PrefabType GetPrefabType(UnityEngine.Object obj)
        {
#if UNITY_2018_3_OR_NEWER
            if (PrefabUtility.IsPartOfNonAssetPrefabInstance(obj))
            {
                return PrefabType.sceneInstance;
            }
            PrefabAssetType assetType = PrefabUtility.GetPrefabAssetType(obj);
            if (assetType == PrefabAssetType.NotAPrefab)
            {
                return PrefabType.sceneInstance;
            }
            else if (assetType == PrefabAssetType.Model)
            {
                return PrefabType.modelPrefab;
            }
            else
            {
                return PrefabType.prefab;
            }
#else
            PrefabType prefabType = PrefabUtility.GetPrefabType(obj);
            if (prefabType == PrefabType.ModelPrefab)
            {
                return MB_PrefabType.modelPrefab;
            } else if (prefabType == PrefabType.Prefab)
            {
                return MB_PrefabType.prefab;
            } else
            {
                return MB_PrefabType.sceneInstance;
            }
#endif
        }

        public static void UnpackPrefabInstance(UnityEngine.GameObject go, ref SerializedObject so)
        {
#if UNITY_2018_3_OR_NEWER
            UnityEngine.Object targetObj = null;
            if (so != null)
                targetObj = so.targetObject;
            PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);

            // This is a workaround for a nasty Unity bug. The call to UnpackPrefabInstance
            // corrupts the serialized object, Recreate a clean reference here.
            if (so != null)
                so = new SerializedObject(targetObj);
#else
            // Do nothing.
#endif
        }

        public static void ReplacePrefab(GameObject gameObject, string assetPath, ReplacePrefabOption replacePrefabOptions)
        {
#if UNITY_2018_3_OR_NEWER
            PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, assetPath, InteractionMode.AutomatedAction);
#else
            GameObject obj = (GameObject) AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
            PrefabUtility.ReplacePrefab(gameObject, obj, (ReplacePrefabOptions) replacePrefabOptions);
#endif
        }
        #endregion
    }

    public enum ReplacePrefabOption
    {
        mbDefault = 0,
        connectToPrefab = 1,
        nameBased = 2,
    }

    public enum PrefabType
    {
        modelPrefab,
        prefab,
        sceneInstance,
    }
}