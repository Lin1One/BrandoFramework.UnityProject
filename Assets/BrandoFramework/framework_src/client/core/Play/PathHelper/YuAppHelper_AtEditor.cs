//using System.Reflection;
//using UnityEngine;

//namespace YuU3dPlay
//{
//    public partial class YuAppHelper
//    {
//        #region 路径

//        #region Cs

//        public string CsPlayDir
//        {
//            get
//            {
//                var rootPath = LocU3DApp.AppRootDir;
//                if (!rootPath.EndsWith("/"))
//                {
//                    rootPath += "/";
//                }

//                var path = rootPath + "Src/Play/";
//                return path;
//            }
//        }

//        public string CsPlayProtoDIr => CsPlayDir + "Proto/";

//        public string CsNetRequestDir
//        {
//            get
//            {
//                var path = CsPlayDir + "NetRequest/";
//                return path;
//            }
//        }

//        public string CsNetResponseDir
//        {
//            get
//            {
//                var path = CsPlayDir + "NetResponse/";
//                return path;
//            }
//        }

//        public string CsDllDir
//        {
//            get
//            {
//                var path = CsPlayDir + "Dll/";
//                return path;
//            }
//        }

//        public string CsEditorDir
//        {
//            get
//            {
//                var path = CsPlayDir + "Editor/";
//                return path;
//            }
//        }

//        /// <summary>
//        /// 应用初始化器脚本存放目录。
//        /// </summary>
//        public string CsAppInitializerDir
//        {
//            get
//            {
//                var path = CsPlayDir + "AppInitializer/";
//                return path;
//            }
//        }

//        /// <summary>
//        /// Excel数据表脚本目录。
//        /// </summary>
//        public string CsExcelDir
//        {
//            get
//            {
//                var path = CsPlayDir + "Excel/";
//                return path;
//            }
//        }

//        public string CsAssetIdDir => CsPlayDir + "AssetId/";

//        #region 视图脚本

//        public string CsLegoUIDir
//        {
//            get
//            {
//                var path = CsPlayDir + "LegoUI/";
//                return path;
//            }
//        }

//        #endregion

//        #endregion

//        #region AssetDatabase

//        private string assetDatabaseDir_Assets;
//        public string AssetDatabaseDir_Assets
//        {
//            get
//            {
//                if (assetDatabaseDir_Assets != null)
//                {
//                    return assetDatabaseDir_Assets;
//                }

//                assetDatabaseDir_Assets = "Assets" + AssetDatabaseDir
//                    .Replace(Application.dataPath, "");
//                return assetDatabaseDir_Assets;
//            }
//        }

//        /// <summary>
//        /// 资源根目录
//        /// </summary>
//        public string AssetDatabaseDir
//        {
//            get
//            {
//                var path = RootFullPath + "AssetDatabase/";
//                return path;
//            }
//        }

//        /// <summary>
//        /// 首包位置
//        /// </summary>
//        public string AssetDatabaseFirstPackageDir
//        {
//            get
//            {
//                var path = RootFullPath + "AssetDatabase/Package_01/";
//                return path;
//            }
//        }

//        public string AssetDatabaseLegoUIRxModelDir => AssetDatabaseDir + "LegoUIRxModel/";

//        public string AssetDatabaseMusicDir
//        {
//            get
//            {
//                var path = AssetDatabaseDir + "Music/";
//                return path;
//            }
//        }

//        public string AssetDatabaseUiTweenMetaDir => AssetDatabaseDir + "UiTweenMeta/";


//        public string AssetDatabaseAtlasDir
//        {
//            get
//            {
//                var path = AssetDatabaseDir + "Package_01/Atlas/";
//                return path;
//            }
//        }

//        public string AssetDatabaseShareAtlas => AssetDatabaseAtlasDir + "ShareAtlas/";
//        public string AssetDatabaseUIAtlas => AssetDatabaseAtlasDir + "UIAtlas/";
//        public string AssetDatabaseDynamicAtlas => AssetDatabaseAtlasDir + "DynamicAtlas/";

//        public string AssetDatabaseBinaryDir
//        {
//            get
//            {
//                var path = AssetDatabaseDir + "Binary/";
//                return path;
//            }
//        }

//        public string AssetDatabaseFontDir
//        {
//            get
//            {
//                var path = AssetDatabaseFirstPackageDir + "Font/";
//                return path;
//            }
//        }

//        public string AssetDatabaseExcel => AssetDatabaseFirstPackageDir + "Excel/";

//        public string AssetDatabaseMaterial
//        {
//            get
//            {
//                var path = AssetDatabaseDir + "Material/";
//                return path;
//            }
//        }

//        public string AssetDatabaseShader
//        {
//            get
//            {
//                var path = AssetDatabaseDir + "Shader/";
//                return path;
//            }
//        }

//        public string AssetDatabaseLegoMetaDir
//        {
//            get
//            {
//                var path = AssetDatabaseFirstPackageDir + "LegoMeta/";
//                return path;
//            }
//        }

//        public string AssetDatabaseViewDir
//        {
//            get
//            {
//                var path = AssetDatabaseDir + "View/";
//                return path;
//            }
//        }

//        public string AssetDatabaseViewCustomControlDir
//        {
//            get
//            {
//                var path = AssetDatabaseDir + "CustomControl/";
//                return path;
//            }
//        }

//        /// <summary>
//        /// 用于发布及运行时的视图模板预制件目录。
//        /// </summary>
//        public string AssetDatabaseViewTemplateDir
//        {
//            get
//            {
//                var path = AssetDatabaseDir + "ViewTemplate/";
//                return path;
//            }
//        }

//        public string AssetDatabaseSoundEffect
//        {
//            get
//            {
//                var path = AssetDatabaseDir + "Sound/";
//                return path;
//            }
//        }

//        public string AssetDatabaseTexture
//        {
//            get
//            {
//                var path = AssetDatabaseDir + "RawImage/";
//                return path;
//            }
//        }

//        public string AssetDataBaseModelDir => AssetDatabaseDir + "Model/";

//        #endregion

//        #region DevelopAsset

//        public string DevelopAssetDir
//        {
//            get
//            {
//                var path = RootFullPath + "DevelopAsset/";
//                return path;
//            }
//        }

//        public string DevelopLegoRxModelDir
//        {
//            get
//            {
//                var path = DevelopAssetDir + "LegoRxModel/";
//                return path;
//            }
//        }

//        public string DevelopScrollViewRxModelDir => DevelopLegoRxModelDir + "ScrollView/";

//        public string DevelopViewDir
//        {
//            get
//            {
//                var path = DevelopAssetDir + "View/";
//                return path;
//            }
//        }

//        public string DevelopViewTemplateDir
//        {
//            get
//            {
//                var path = DevelopAssetDir + "ViewTemplate/";
//                return path;
//            }
//        }

//        public string DevelopCustomControlDir
//        {
//            get
//            {
//                var path = DevelopAssetDir + "CustomControl/";
//                return path;
//            }
//        }

//        /// <summary>
//        /// 用于存放开发阶段离线自定义控件数据模型预制件的目录。
//        /// </summary>
//        public string DevelopOfflineCustomControlModelDir
//        {
//            get
//            {
//                var path = DevelopAssetDir + "OfflineCustomControlModel/";
//                return path;
//            }
//        }

//        /// <summary>
//        /// 用于存放开发阶段使用的Excel数据实体列表资产文件的目录。
//        /// </summary>
//        public string DevelopExcelEntitysDir
//        {
//            get
//            {
//                var path = DevelopAssetDir + "ExcelEntitys/";
//                return path;
//            }
//        }

//        #endregion

//        #region OriginalAsset

//        public string OriginalRootDir
//        {
//            get
//            {
//                var path = RootFullPath + "OriginalAsset/";
//                return path;
//            }
//        }

//        private string OriginAtlasDir => OriginalRootDir + "Atlas/";
//        public string OriginUIAtlasDir => OriginAtlasDir + "UIAtlas/";
//        public string OriginShareAtlas => OriginAtlasDir + "ShareAtlas/";
//        public string OriginDynamicAtlas => OriginAtlasDir + "DynamicAtlas/";

//        public string OriginAtlasSpriteDir
//        {
//            get
//            {
//                var path = OriginalRootDir + "AtlasSprite/";
//                return path;
//            }
//        }

//        public string OriginRawImageDir
//        {
//            get
//            {
//                var path = OriginalRootDir + "RawImage/";
//                return path;
//            }
//        }

//        public string OriginAtlasSplitDir
//        {
//            get
//            {
//                var path = OriginalRootDir + "AtlasSplit/";
//                return path;
//            }
//        }

//        public string OriginExcelFileDir
//        {
//            get
//            {
//                var path = OriginalRootDir + "Excel/";
//                return path;
//            }
//        }

//        public string Protobuf
//        {
//            get
//            {
//                var path = OriginalRootDir + "Protobuf/";
//                return path;
//            }
//        }

//        public string TextureDir
//        {
//            get
//            {
//                var path = OriginalRootDir + "Texture/";
//                return path;
//            }
//        }

//        #endregion

//        #region 应用场景路径（开发及打包）

//        public string GetDeveloperScenePath(string develperId)
//        {
//            var path = LocU3DApp.AppRootDir + LocU3DApp.LocAppId + $"_Develop_Bootstrap_{develperId}.unity";
//            return path;
//        }

//        public string ReleaseScenePath
//        {
//            get
//            {
//                var path = LocU3DApp.AppRootDir + LocU3DApp.LocAppId + "_Release_Bootstrap.unity";
//                return path;
//            }
//        }

//        #endregion

//        #region 路径

//        #region AssetBundle

//        /// <summary>
//        /// 用于开发的本地Http模块器地址。
//        /// </summary>
//        /// <value>The local http root dir.</value>
//        public string LocalHttpRootDir
//        {
//            get
//            {
//                var path = $"{YuSetting.LocalHttpDir}{LocU3DApp.LocAppId}/";
//                return path;
//            }
//        }

//        /// <summary>
//        /// 应用AssetBundle文件输出目录。
//        /// </summary>
//        public string AssetBundleBuildDir
//        {
//            get
//            {
//                var path = $"{LocalHttpRootDir}AssetBundle/";
//                return path;
//            }
//        }

//        #endregion

//        #endregion

//        #region Ts

//        public string TsRootDir
//        {
//            get
//            {
//                var path = LocU3DApp.AppRootDir + "TypeScript/";
//                return path;
//            }
//        }

//        public string TsProjectDir
//        {
//            get
//            {
//                var path = TsRootDir + "Project/";
//                return path;
//            }
//        }

//        public string TsAssetDatabaseDir
//        {
//            get
//            {
//                var path = AssetDatabaseDir + "TypeScript/";
//                return path;
//            }
//        }

//        #endregion

//        #region 资源Id及资源数据

//        public string CsAssetIdPath
//        {
//            get
//            {
//                var path = CsPlayDir + $"{LocU3DApp.LocAppId}_AssetId.cs";
//                return path;
//            }
//        }

//        public string AssetInfoFileName
//        {
//            get
//            {
//                var name = $"{LocU3DApp.LocAppId.ToLower()}_assetinfos";
//                return name;
//            }
//        }

//        public string ViewMetaInfoFileName
//        {
//            get
//            {
//                var name = $"{LocU3DApp.LocAppId.ToLower()}_viewmetainfo";
//                return name;
//            }
//        }

//        public string AssetInfoFullPath
//        {
//            get
//            {
//                var path = AssetDatabaseBinaryDir +
//                           $"{AssetInfoFileName}.bytes";
//                return path;
//            }
//        }

//        public string AssetInfoAssetDatabasePath
//        {
//            get
//            {
//                var path = "Assets" + AssetInfoFullPath.Replace(Application.dataPath, "");
//                return path;
//            }
//        }

//        public string TsAssetIdPath
//        {
//            get
//            {
//                var path = TsProjectDir + LocU3DApp.LocAppId.ToLower()
//                                        + "_assetid.ts";
//                return path;
//            }
//        }

//        #endregion

//        #region 配置

//        /// <summary>
//        /// 视图配置文件的资源名。
//        /// </summary>
//        public string ViewSettingName
//        {
//            get
//            {
//                var name = LocU3DApp.LocAppId + "_ViewSetting";
//                return name.ToLower();
//            }
//        }

//        /// <summary>
//        /// 视图配置Json文件存放路径。
//        /// </summary>
//        public string ViewSettingPath
//        {
//            get
//            {
//                var path = AssetDatabaseDir + ViewSettingName + ".json";
//                return path;
//            }
//        }

//        #endregion

//        #region 辅助函数

//        public string GetViewPrefabPath(GameObject view, bool isDevelop)
//        {
//            string path;

//            if (isDevelop)
//            {
//                path = DevelopViewDir + view.name + ".prefab";
//            }
//            else
//            {
//                path = AssetDatabaseViewDir + view.name + ".prefab";
//            }

//            return path;
//        }

//        public string GetViewTemplatePrefabPath(GameObject template, bool isDevelop = true)
//        {
//            string path;

//            if (isDevelop)
//            {
//                path = DevelopViewTemplateDir + template.name + ".prefab";
//            }
//            else
//            {
//                path = AssetDatabaseViewTemplateDir + template.name + ".prefab";
//            }

//            return path;
//        }

//        public string GetCustomControlPrefabPath(GameObject control, bool isDevelop = true)
//        {
//            string path;

//            if (isDevelop)
//            {
//                path = DevelopCustomControlDir + control.name + ".prefab";
//            }
//            else
//            {
//                path = AssetDatabaseViewCustomControlDir + control.name + ".prefab";
//            }

//            return path;
//        }

//        #endregion

//        #region 动态入口

//        /// <summary>
//        /// 应用初始化器主入口脚本路径。
//        /// </summary>
//        public string Initializer_MainFilePath
//        {
//            get
//            {
//                var path = CsAppInitializerDir
//                           + LocU3DApp.LocAppId + "_Initializer_Main.cs";
//                return path;
//            }
//        }

//        /// <summary>
//        /// 应用初始化器负责建立视图行为处理路由器映射关系的分部类脚本路径。
//        /// </summary>
//        public string Initializer_MapViewActionRouterPath
//        {
//            get
//            {
//                var path = CsAppInitializerDir + LocU3DApp.LocAppId
//                                               + "_Initializer_MapViewActionRouter.cs";
//                return path;
//            }
//        }

//        /// <summary>
//        /// 应用初始化器负责初始化视图数据模型映射关系的分部类脚本路径。
//        /// </summary>
//        public string Initializer_InitViewModelPath
//        {
//            get
//            {
//                var path = CsAppInitializerDir + LocU3DApp.LocAppId
//                                               + "_Initializer_InitViewModel.cs";
//                return path;
//            }
//        }

//        #endregion

//        #region 实例获取路径

//        #region 视图

//        #region 路由

//        public string GetViewRouterPath(string type)
//        {
//            var path = $"{LocU3DApp.LocAppId}/Router/View/{type}Router";
//            return path;
//        }

//        public string ViewModelRouterGetPath
//        {
//            get
//            {
//                var path = $"{LocU3DApp.LocAppId}/Router/View/ModelRouter";
//                return path;
//            }
//        }

//        #endregion

//        #region 视图数据模型存储器

//        public string ViewModelStorageGetPath
//        {
//            get
//            {
//                var path = $"{LocU3DApp.LocAppId}/Router/View/ModelStorage";
//                return path;
//            }
//        }

//        #endregion

//        #region 视图创建器

//        /// <summary>
//        /// 视图构造器实例获取路径。
//        /// </summary>
//        public string ViewCreatorGetPath
//        {
//            get
//            {
//                var path = $"{LocU3DApp.LocAppId}/Creator/View/ViewCreator";
//                return path;
//            }
//        }

//        #endregion

//        #endregion

//        #endregion

//        #region 程序集

//        public string LibraryDllPath
//        {
//            get
//            {
//                var path = Application.dataPath.Replace("Assets", "")
//                           + "Library/ScriptAssemblies/" + LocU3DApp.PlayAsmId + ".dll";
//                return path;
//            }
//        }

//        public string LibraryPdbPath
//        {
//            get
//            {
//                var path = Application.dataPath.Replace("Assets", "")
//                           + "Library/ScriptAssemblies/" + LocU3DApp.PlayAsmId + ".pdb";
//                return path;
//            }
//        }

//        public Assembly LibraryAssembly
//        {
//            get
//            {
//                var path = Application.dataPath.Replace("Assets", "")
//                           + "Library/ScriptAssemblies/" + LocU3DApp.LocAppId + "Play.dll";
//                var asm = Assembly.LoadFile(path);
//                return asm;
//            }
//        }

//        #endregion

//        #region APk

//        public string ApkVersionInfoEditorPath
//        {
//            get { return null; }
//        }

//        public string ApkVersionInfoSandboxPath
//        {
//            get { return null; }
//        }

//        public string ApkVersionTempSandboxPath
//        {
//            get { return null; }
//        }

//        #endregion

//        #region 资源数据

//        private string streamingAssetsDir_Assets;
//        public string StreamingAssetsDir_Assets
//        {
//            get
//            {
//                if (streamingAssetsDir_Assets != null)
//                {
//                    return streamingAssetsDir_Assets;
//                }

//                streamingAssetsDir_Assets = "Assets" + StreamingAssetsDir
//                                              .Replace(Application.dataPath, "");
//                return streamingAssetsDir_Assets;
//            }
//        }

//        //public string StreamingAssetsDir => RootFullPath + "StreamingAssets/";
//        //public string AssetInfoStreamingEditorPath => $"{StreamingAssetsDir}{LocU3DApp.LocAppId}_AssetInfo.bytes";
//        //public string AssetInfoLocalHttpPath => $"{LocalHttpRootDir}{LocU3DApp.LocAppId}_AssetInfo.bytes";

//        //public string ResourcesDir => RootFullPath + "Resources/";

//        ////分包资源路径
//        //public string SubPackageAssetDir => RootFullPath + "AssetPackage/";

//        //分包资源 UnityAsset 路径
//        private string subPackageAssetDir_Assets;
//        public string SubPackageAssetDir_Assets
//        {
//            get
//            {
//                if (subPackageAssetDir_Assets == null)
//                {
//                    subPackageAssetDir_Assets = "Assets" + SubPackageAssetDir
//                        .Replace(Application.dataPath, "");
//                }
//                return subPackageAssetDir_Assets;
//            }
//        }
//        #endregion

//        #region 平台打包

//        public string Windows64PackPath => Application.dataPath.Replace("Assets", "")
//                                          + $"PackOut/{LocU3DApp.LocAppId}/Windows64/{LocU3DApp.LocAppId}.exe";
//        public string AndroidPackPath => Application.dataPath.Replace("Assets", "")
//                                  + $"PackOut/{LocU3DApp.LocAppId}/Android/{LocU3DApp.LocAppId}.apk";



//        #endregion

//        #endregion
//    }
//}


