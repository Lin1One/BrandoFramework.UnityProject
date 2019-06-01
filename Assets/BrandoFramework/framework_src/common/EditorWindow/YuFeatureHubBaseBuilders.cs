#region Head

// Author:            liuruoyu1981
// CreateDate:        2019/1/21 17:34:28
// Email:             35490136@qq.com || liuruoyu1981@gmail.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

namespace YuU3dOdinEditor
{
    #region 基础菜单项构建器实现

    //[YuMenuItemBuilderDesc(typeof(YuFeatureHub), "应用配置菜单项构建器")]
    //public class YuU3dAppSettingMenuItemBuilder : IYuMenuEditorWindowItemBuilder
    //{
    //    public void BuildMenuItem(OdinMenuTree tree)
    //    {
    //        var datiDescAttr = typeof(YuU3dAppSettingDati).GetAttribute<YuDatiDescAttribute>();
    //        tree.AddAllDatisAtPath(YuU3dDatiUtility.GetSaveDir(typeof(YuU3dAppSettingDati)),
    //            datiDescAttr.Title,
    //            s =>
    //            {
    //                var dati = YuU3dAppSettingDati.GetMultiAtId(s);
    //                YuU3dDatiUtility.ReflectSetAppId(dati, s);
    //                return dati;
    //            }, o => o is YuU3dAppSettingDati);
    //    }
    //}

    //[YuMenuItemBuilderDesc(typeof(YuFeatureHub), "应用资源ID自动化菜单项构建器")]
    //public class YuU3dAppAssetIdAutoFlowMenuItemBuilder : IYuMenuEditorWindowItemBuilder
    //{
    //    public void BuildMenuItem(OdinMenuTree tree)
    //    {
    //        var datiDescAttr = typeof(YuU3dAppAssetIdDati).GetAttribute<YuDatiDescAttribute>();
    //        tree.AddAllDatisAtPath(YuU3dDatiUtility.GetSaveDir(typeof(YuU3dAppAssetIdDati)),
    //            datiDescAttr.Title,
    //            s =>
    //            {
    //                var dati = YuU3dAppAssetIdDati.GetMultiAtId(s);
    //                YuU3dDatiUtility.ReflectSetAppId(dati, s);
    //                return dati;
    //            }, o => o is YuU3dAppAssetIdDati);
    //    }
    //}

    //[YuMenuItemBuilderDesc(typeof(YuFeatureHub), "应用资源数据自动化菜单项构建器")]
    //public class YuU3dAppAssetInfoAotuFlowMenuItemBuilder : IYuMenuEditorWindowItemBuilder
    //{
    //    public void BuildMenuItem(OdinMenuTree tree)
    //    {
    //        var datiDescAttr = typeof(YuAppAssetInfoDati).GetAttribute<YuDatiDescAttribute>();
    //        tree.AddAllDatisAtPath(YuU3dDatiUtility.GetSaveDir(typeof(YuAppAssetInfoDati)),
    //            datiDescAttr.Title,
    //            s =>
    //            {
    //                var dati = YuAppAssetInfoDati.GetMultiAtId(s);
    //                YuU3dDatiUtility.ReflectSetAppId(dati, s);
    //                return dati;
    //            }, o => o is YuAppAssetInfoDati);
    //    }
    //}

    //[YuMenuItemBuilderDesc(typeof(YuFeatureHub), "应用图片资源数据自动化菜单项构建器")]
    //public class YuAppTextureInfoAotuFlowMenuItemBuilder : IYuMenuEditorWindowItemBuilder
    //{
    //    public void BuildMenuItem(OdinMenuTree tree)
    //    {
    //        var datiDescAttr = typeof(YuAppAssetInfoDati).GetAttribute<YuDatiDescAttribute>();
    //        tree.AddAllDatisAtPath(YuU3dDatiUtility.GetSaveDir(typeof(YuAppTextureInfoDati)),
    //            datiDescAttr.Title,
    //            s =>
    //            {
    //                var dati = YuAppTextureInfoDati.GetMultiAtId(s);
    //                YuU3dDatiUtility.ReflectSetAppId(dati, s);
    //                return dati;
    //            }, o => o is YuAppTextureInfoDati);
    //    }
    //}

    //[YuMenuItemBuilderDesc(typeof(YuFeatureHub), "应用GM配置菜单项构建器")]
    //public class YuU3dGMSpannerMenuItemBuilder : IYuMenuEditorWindowItemBuilder
    //{
    //    public void BuildMenuItem(OdinMenuTree tree)
    //    {
    //        var datiDescAttr = typeof(YuU3dAppGMSpannerDati).GetAttribute<YuDatiDescAttribute>();
    //        tree.AddAllDatisAtPath(YuU3dDatiUtility.GetSaveDir(typeof(YuU3dAppGMSpannerDati)),
    //            datiDescAttr.Title,
    //            s =>
    //            {
    //                var dati = YuU3dAppGMSpannerDati.GetMultiAtId(s);
    //                YuU3dDatiUtility.ReflectSetAppId(dati, s);
    //                return dati;
    //            }, o => o is YuU3dAppGMSpannerDati);
    //    }
    //}

    //[YuMenuItemBuilderDesc(typeof(YuFeatureHub), "AssetBundle应用打包配置菜单项构建器")]
    //public class YuU3dAppAssetBundleAppBuildMenuItemBuilder : IYuMenuEditorWindowItemBuilder
    //{
    //    public void BuildMenuItem(OdinMenuTree tree)
    //    {
    //        var datiDescAttr = typeof(YuU3dAppAssetBundleSettingDati).GetAttribute<YuDatiDescAttribute>();
    //        tree.AddAllDatisAtPath(YuU3dDatiUtility.GetSaveDir(
    //                typeof(YuU3dAppAssetBundleSettingDati)),
    //            datiDescAttr.Title,
    //            s =>
    //            {
    //                var dati = YuU3dAppAssetBundleSettingDati.GetMultiAtId(s);
    //                YuU3dDatiUtility.ReflectSetAppId(dati, s);
    //                dati.OnEnable();
    //                return dati;
    //            }, o => o is YuU3dAppAssetBundleSettingDati);
    //    }
    //}


    //[YuMenuItemBuilderDesc(typeof(YuFeatureHub), "应用资源标记器")]
    //public class YuAssetMarkSettingDatiMenuItemBuilder : IYuMenuEditorWindowItemBuilder
    //{
    //    public void BuildMenuItem(OdinMenuTree tree)
    //    {
    //        var datiDescAttr = typeof(YuAssetMarkSettingDati).GetAttribute<YuDatiDescAttribute>();
    //        tree.AddAllDatisAtPath(YuU3dDatiUtility.GetSaveDir(
    //                typeof(YuAssetMarkSettingDati)),
    //            datiDescAttr.Title,
    //            s =>
    //            {
    //                var dati = YuAssetMarkSettingDati.GetMultiAtId(s);
    //                YuU3dDatiUtility.ReflectSetAppId(dati, s);
    //                dati.OnEnable();
    //                return dati;
    //            }, o => o is YuAssetMarkSettingDati);
    //    }
    //}

    ////[YuMenuItemBuilderDesc(typeof(YuFeatureHub), "AssetBundle数据查看器菜单项构建器")]
    ////public class YuU3dAppAssetBundleInfoViewerMenuItemBuilder : IYuMenuEditorWindowItemBuilder
    ////{
    ////    public void BuildMenuItem(OdinMenuTree tree)
    ////    {
    ////        var datiDescAttr = typeof(YuU3dAppAssetBundleInfoDati).GetAttribute<YuDatiDescAttribute>();
    ////        tree.AddAllDatisAtPath(YuU3dDatiUtility.GetSaveDir(typeof(YuU3dAppAssetBundleInfoDati)),
    ////            datiDescAttr.Title,
    ////            s =>
    ////            {
    ////                var dati = YuU3dAppAssetBundleInfoDati.GetMultiAtId(s);
    ////                YuU3dDatiUtility.ReflectSetAppId(dati, s);
    ////                return dati;
    ////            }, o => o is YuU3dAppAssetBundleInfoDati);
    ////    }
    ////}

    //[YuMenuItemBuilderDesc(typeof(YuFeatureHub), "AssetBundle多进程打包器菜单项构建器")]
    //public class YuU3dAssetBundleMultiProcessBuilderMenuItemBuilder : IYuMenuEditorWindowItemBuilder
    //{
    //    public void BuildMenuItem(OdinMenuTree tree)
    //    {
    //        var datiDescAttr = typeof(YuU3dAssetBundleMultiProcessDati).GetAttribute<YuDatiDescAttribute>();
    //        tree.AddAllDatisAtPath(YuU3dDatiUtility.GetSaveDir(typeof(YuU3dAssetBundleMultiProcessDati)),
    //            datiDescAttr.Title,
    //            s =>
    //            {
    //                var dati = YuU3dAssetBundleMultiProcessDati.GetMultiAtId(s);
    //                YuU3dDatiUtility.ReflectSetAppId(dati, s);
    //                return dati;
    //            }, o => o is YuU3dAssetBundleMultiProcessDati);
    //    }
    //}

    //[YuMenuItemBuilderDesc(typeof(YuFeatureHub), "应用多平台打包器配置菜单项构建器")]
    //public class YuU3dCrossPlatformPackerMenuItemBuilder : IYuMenuEditorWindowItemBuilder
    //{
    //    public void BuildMenuItem(OdinMenuTree tree)
    //    {
    //        var datiDescAttr = typeof(YuCrossPlatformPackerDati).GetAttribute<YuDatiDescAttribute>();
    //        tree.AddAllDatisAtPath(YuU3dDatiUtility.GetSaveDir(typeof(YuCrossPlatformPackerDati)),
    //            datiDescAttr.Title,
    //            s =>
    //            {
    //                var dati = YuCrossPlatformPackerDati.GetMultiAtId(s);
    //                YuU3dDatiUtility.ReflectSetAppId(dati, s);
    //                return dati;
    //            }, o => o is YuCrossPlatformPackerDati);
    //    }
    //}

    #endregion
}