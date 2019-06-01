using Common.Config;
using Common.Utility;
using System.IO;
using UnityEngine;

namespace Client
{
    public partial class YuAppHelper
    {
        private string rootFullPath;

        public string RootFullPath
        {
            get
            {
                if (rootFullPath != null)
                {
                    return rootFullPath;
                }

                rootFullPath = Application.dataPath.Replace("Assets", "")
                               + U3dDevelopConfig.Config.AppRootDir;
                if (!rootFullPath.EndsWith("/"))
                {
                    rootFullPath += "/";
                }

                return rootFullPath;
            }
        }



        //public string AtlasInfoSandboxPath => $"{SandboxHotUpdateDir}{U3dDevelopConfig.Config.CurrentDevelopProjectName}_AtlasInfo.bytes";

        public string AtlasInfoStreamAssetPath => $"{Application.streamingAssetsPath}/{U3dDevelopConfig.Config.CurrentDevelopProjectName}_AtlasInfo.bytes";


        public string AssetbundleInfoStreamingPlayPath => File.Exists($"{Application.persistentDataPath}/{U3dDevelopConfig.Config.CurrentDevelopProjectName}_AssetBundleInfo.bytes") ?
             $"{Application.persistentDataPath}/{U3dDevelopConfig.Config.CurrentDevelopProjectName}_AssetBundleInfo.bytes" :
             $"{Application.streamingAssetsPath}/{U3dDevelopConfig.Config.CurrentDevelopProjectName}_AssetBundleInfo.bytes";

        public string AssetbundleInfoUrl => File.Exists($"{Application.persistentDataPath}/{U3dDevelopConfig.Config.CurrentDevelopProjectName}_AssetBundleInfo.bytes") ?
            $"file://{Application.persistentDataPath}/{U3dDevelopConfig.Config.CurrentDevelopProjectName}_AssetBundleInfo.bytes" :
            $"{Application.streamingAssetsPath}/{U3dDevelopConfig.Config.CurrentDevelopProjectName}_AssetBundleInfo.bytes";

        public string AtlasInfoPlayPath => File.Exists($"{Application.persistentDataPath}/{U3dDevelopConfig.Config.CurrentDevelopProjectName}_AtlasInfo.bytes") ?
            $"{Application.persistentDataPath}/{U3dDevelopConfig.Config.CurrentDevelopProjectName }_AtlasInfo.bytes" :
            $"{Application.streamingAssetsPath}/{U3dDevelopConfig.Config.CurrentDevelopProjectName}_AtlasInfo.bytes";

        public string AtlasInfoUrl => File.Exists($"{Application.persistentDataPath}/{U3dDevelopConfig.Config.CurrentDevelopProjectName}_AtlasInfo.bytes") ?
            $"file://{Application.persistentDataPath}/{U3dDevelopConfig.Config.CurrentDevelopProjectName }_AtlasInfo.bytes" :
            $"{Application.streamingAssetsPath}/{U3dDevelopConfig.Config.CurrentDevelopProjectName}_AtlasInfo.bytes";


        /// <summary>
        /// 应用在沙盒下的AssetBundle根目录。
        /// </summary>
        //public string AssetBundleSandboxDir => $"{SandboxHotUpdateDir}AssetBundle/";

        public string SandboxHotUpdateDir => //YuUnityUtility.IsEditorMode ?
        //    $"{YuSetting.LocalHttpDir}{U3dDevelopConfig.Config.CurrentDevelopProjectName}/" :
            Application.streamingAssetsPath + $"/HotUpdate/{U3dDevelopConfig.Config.CurrentDevelopProjectName}/";
        
        public string ResourcesAssetsDir { get; }
        public string FinalStreamingAssetsDir { get; }
        public string FinalSandboxDir { get; }
        
       public string AssetInfoSandboxPath => $"{SandboxHotUpdateDir}{U3dDevelopConfig.Config.CurrentDevelopProjectName}_AssetInfo.bytes";
        //public string AssetInfoEditorPath=>$"{AssetDatabaseDir}{U3dDevelopConfig.Config.CurrentDevelopProjectName}_AssetInfo.bytes";
        
        public string FinalAssetBundleDir { get; }
        
        //public string BundleManifestPath => AssetBundleBuildDir + "AssetBundle.manifest";
    }
}