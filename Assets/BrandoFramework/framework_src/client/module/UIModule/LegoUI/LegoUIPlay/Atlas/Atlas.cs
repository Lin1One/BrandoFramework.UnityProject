#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/9 20:29:52
// Email:             836045613@qq.com

#endregion

using Common;
using Common.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Client.LegoUI
{
    public interface ILegoAtlas 
    {
        Sprite GetSprite(string spriteId);

        void RestoreSprite(Sprite sp);

        int RefCount { get; }
    }

    /// <summary>
    /// 图集
    /// </summary>
    public class LegoAtlas : ILegoAtlas
    {
        private readonly Dictionary<string, Sprite> spriteDict
        = new Dictionary<string, Sprite>();

        public int RefCount { get; private set; }

        public void Release()
        {

        }

        public Sprite GetSprite(string spriteId)
        {
            if (spriteDict.ContainsKey(spriteId))
            {
                var sp = spriteDict[spriteId];
                RefCount++;
                return sp;
            }
            return null;
        }

        public void RestoreSprite(Sprite sp)
        {
            ////if (!spriteDict.ContainsKey(sp.name))
            ////{
            ////    return;
            ////}

            ////RefCount--;
            ////if (RefCount == 0)
            ////{
            ////    spriteDict.Clear();
            ////    m_BundleRef?.Unuse();
            ////    AtlasPool.Restore(this);
            ////}
        }

        ////private IBundleRef m_BundleRef;

        #region 对象池

        private static IObjectPool<ILegoAtlas> atlasPool;

        private static IObjectPool<ILegoAtlas> AtlasPool
        {
            get
            {
                if (atlasPool != null)
                {
                    return atlasPool;
                }

                atlasPool = new ObjectPool<ILegoAtlas>(
                    () => new LegoAtlas(), 20);
                return atlasPool;
            }
        }

        #endregion

        #region 构造

#if UNITY_EDITOR

        public static ILegoAtlas Create(string path)
        {
            var atlas = (LegoAtlas)AtlasPool.Take();
            var sps = AssetDatabaseUtility.LoadAllAssetsAtPath<Sprite>(path);
            foreach (var sp in sps)
            {
                atlas.spriteDict.Add(sp.name.ToLower(), sp);
            }

            return atlas;
        }
#endif

        ////public static ILegoAtlas Create(IBundleRef bundleRef)
        ////{
        ////    var atlas = (LegoAtlas)AtlasPool.Take();
        ////    atlas.m_BundleRef = bundleRef;
        ////    var sps = atlas.m_BundleRef.LoadAll<Sprite>();
        ////    foreach (var sp in sps)
        ////    {
        ////        atlas.spriteDict.Add(sp.name.ToLower(), sp);
        ////    }

        ////    return atlas;
        ////}


        #endregion
    }
}

