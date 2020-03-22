using System.IO;
using UnityEngine;

namespace Client.Core
{
    public partial class GameProjectHelper
    {
        private string rootFullPath;

        /// <summary>
        /// 游戏项目根目录
        /// </summary>
        public string RootFullPath
        {
            get
            {
                if (rootFullPath != null)
                {
                    return rootFullPath;
                }

                rootFullPath = Application.dataPath.Replace("Assets", "")
                               + ProjectInfoDati.GetActualInstance().ProjectRootDir;
                if (!rootFullPath.EndsWith("/"))
                {
                    rootFullPath += "/";
                }

                return rootFullPath;
            }
        }



        //public string AtlasInfoSandboxPath => $"{SandboxHotUpdateDir}{ProjectInfoDati.GetActualInstance().CurrentDevelopProjectName}_AtlasInfo.bytes";

        public string AtlasInfoStreamAssetPath => $"{Application.streamingAssetsPath}/" +
            $"{ProjectInfoDati.GetActualInstance().DevelopProjectName}_AtlasInfo.bytes";


        public string AssetbundleInfoStreamingPlayPath => File.Exists($"{Application.persistentDataPath}/{ProjectInfoDati.GetActualInstance().DevelopProjectName}_AssetBundleInfo.bytes") ?
             $"{Application.persistentDataPath}/{ProjectInfoDati.GetActualInstance().DevelopProjectName}_AssetBundleInfo.bytes" :
             $"{Application.streamingAssetsPath}/{ProjectInfoDati.GetActualInstance().DevelopProjectName}_AssetBundleInfo.bytes";

        public string AssetbundleInfoUrl => File.Exists($"{Application.persistentDataPath}/{ProjectInfoDati.GetActualInstance().DevelopProjectName}_AssetBundleInfo.bytes") ?
            $"file://{Application.persistentDataPath}/{ProjectInfoDati.GetActualInstance().DevelopProjectName}_AssetBundleInfo.bytes" :
            $"{Application.streamingAssetsPath}/{ProjectInfoDati.GetActualInstance().DevelopProjectName}_AssetBundleInfo.bytes";

        public string AtlasInfoPlayPath => File.Exists($"{Application.persistentDataPath}/{ProjectInfoDati.GetActualInstance().DevelopProjectName}_AtlasInfo.bytes") ?
            $"{Application.persistentDataPath}/{ProjectInfoDati.GetActualInstance().DevelopProjectName }_AtlasInfo.bytes" :
            $"{Application.streamingAssetsPath}/{ProjectInfoDati.GetActualInstance().DevelopProjectName}_AtlasInfo.bytes";

        public string AtlasInfoUrl => File.Exists($"{Application.persistentDataPath}/{ProjectInfoDati.GetActualInstance().DevelopProjectName}_AtlasInfo.bytes") ?
            $"file://{Application.persistentDataPath}/{ProjectInfoDati.GetActualInstance().DevelopProjectName }_AtlasInfo.bytes" :
            $"{Application.streamingAssetsPath}/{ProjectInfoDati.GetActualInstance().DevelopProjectName}_AtlasInfo.bytes";


        /// <summary>
        /// 应用在沙盒下的AssetBundle根目录。
        /// </summary>
        //public string AssetBundleSandboxDir => $"{SandboxHotUpdateDir}AssetBundle/";

        public string SandboxHotUpdateDir => //YuUnityUtility.IsEditorMode ?
        //    $"{YuSetting.LocalHttpDir}{ProjectInfoDati.GetActualInstance().CurrentDevelopProjectName}/" :
            Application.streamingAssetsPath + $"/HotUpdate/{ProjectInfoDati.GetActualInstance().DevelopProjectName}/";
        
        public string ResourcesAssetsDir { get; }
        public string FinalStreamingAssetsDir { get; }
        public string FinalSandboxDir { get; }
        
       public string AssetInfoSandboxPath => $"{SandboxHotUpdateDir}{ProjectInfoDati.GetActualInstance().DevelopProjectName}_AssetInfo.bytes";
        //public string AssetInfoEditorPath=>$"{AssetDatabaseDir}{ProjectInfoDati.GetActualInstance().CurrentDevelopProjectName}_AssetInfo.bytes";
        
        public string FinalAssetBundleDir { get; }
        
        //public string BundleManifestPath => AssetBundleBuildDir + "AssetBundle.manifest";
    }
}