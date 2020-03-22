#region Head

// Author:            Yu
// CreateDate:        2019/1/10 15:08:21
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Client.Utility;
using Common;
using Common.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

using YuU3dPlay;

#pragma warning disable 649

namespace Client.LegoUI
{
    [Singleton]
    public class YuLegoSpriteRouter : ISpriteRouter
    {
#if UNITY_EDITOR 

        private readonly YuLegoSpriteCacher_AtEditor spriteCacher
            = new YuLegoSpriteCacher_AtEditor();

#endif

        [Inject] private readonly LegoAtlasLoader atlasLoader;

        public Sprite GetSprite(string spriteId)
        {
#if UNITY_EDITOR 
            return GetSprite_AtEditor(spriteId);
#else
            return GetSprite_AtPlay(spriteId);
#endif
        }

#if UNITY_EDITOR

        private bool? m_isLoadBundle = null;
        private bool? IsLoadBundle
        {
            get
            {
                if (m_isLoadBundle == null)
                {
                    //var appEntity = U3dGlobal.Get<IYuU3dAppEntity>();
                    //m_isLoadBundle = appEntity.RunSetting.IsLoadFromAssetBundle;
                }
                return m_isLoadBundle;
            }
        }

        private Sprite GetSprite_AtEditor(string spriteId)
        {
            if (IsLoadBundle == true)
            {
                try
                {
                    var targetSp = atlasLoader.GetSprite(spriteId);
                    if (targetSp != null)
                    {
                        return targetSp;
                    }
                    Debug.LogError("Ab加载精灵图失败：" + spriteId);
                }
                catch (Exception e)
                {
                    Debug.LogError("Ab加载精灵图失败：" + spriteId);
                    Debug.LogError(e.Message + e.StackTrace);
                }
            }

            var targetSp2 = spriteCacher.GetSprite(spriteId);
            return targetSp2;
        }

#endif

        private Sprite GetSprite_AtPlay(string spriteId)
        {
            var targetSp = atlasLoader.GetSprite(spriteId);
            return targetSp;
        }

#if UNITY_EDITOR 

        private sealed class YuLegoSpriteCacher_AtEditor
        {
            #region Image

            //private readonly Dictionary<string, Sprite> buildinSpriteDict
            //    = new Dictionary<string, Sprite>();

            private readonly Dictionary<string, Sprite> appSpriteDict
                = new Dictionary<string, Sprite>();

            //private readonly YuU3dAppSetting currentU3DApp;

            public YuLegoSpriteCacher_AtEditor()
            {
//                InitBuildinSprites();
                //if (Application.isPlaying)
                //{
                //    var appEntity = YuU3dAppUtility.Injector.Get<IYuU3dAppEntity>();
                //    currentU3DApp = appEntity.CurrentRuningU3DApp;
                //}
                //else
                //{
                //    currentU3DApp = YuU3dAppSettingDati.CurrentActual;
                //}
            }

//            private void InitBuildinSprites()
//            {
//                var buildSpIds = YuLegoConstant.YuSpriteIds;
//                foreach (var spId in buildSpIds)
//                {
//                    var sp = Resources.Load<Sprite>("YuSprite/" + spId);
//                    buildinSpriteDict.Add(sp.name, sp);
//                }
//            }

            public Sprite GetSprite(string spName)
            {
                if (string.IsNullOrEmpty(spName))
                {
                    throw new Exception("精灵资源Id不能为空！");
                }

                //if (buildinSpriteDict.ContainsKey(spName))
                //{
                //    return buildinSpriteDict[spName];
                //}

                var lowerAssetId = spName.ToLower();

                if (appSpriteDict.ContainsKey(lowerAssetId))
                {
                    return appSpriteDict[lowerAssetId];
                }

                ////var path = currentU3DApp.Helper.OriginAtlasSpriteDir
                ////           + spName + ".png";
                var sp = AssetDatabaseUtility.LoadAssetAtPath<Sprite>("");
                if (sp == null)
                {
                    Debug.Log($"目标精灵{spName}在内建和应用精灵中都不存在！");
                }

                appSpriteDict.Add(lowerAssetId, sp);
                return sp;
            }

            #endregion
        }


#endif
    }
}