#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/13 21:32:02
// Email:             836045613@qq.com

#endregion

using Client.Core;
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
    public class YuLegoTextureRouter : IYuTextureRouter
    {
        [Inject] private readonly IAssetModule m_AssetModule;

        private readonly Dictionary<string, Texture> textureDict
            = new Dictionary<string, Texture>();

        public void WaitTexture(string id, Action<Texture> callback)
        {
#if UNITY_EDITOR
            if (YuUnityUtility.IsEditorMode)
            {
                var tex = GetTexture_AtEditor(id);
                callback?.Invoke(tex);
            }
            else
            {
                WaitTexture_AtPlay(id, (texture) => { callback?.Invoke(texture); });
                ;
            }

#else
            WaitTexture_AtPlay(id, (texture) =>
             {
                 callback?.Invoke(texture);
             });
#endif
        }

#if UNITY_EDITOR

        private Texture GetTexture_AtEditor(string id)
        {
            if (textureDict.ContainsKey(id))
            {
                return textureDict[id];
            }

            //var appHelper = YuU3dAppSettingDati.CurrentActual.Helper;

            //var jpgPath = YuUnityIOUtility.GetAssetsPath(appHelper.AssetDatabaseTexture
            //                                             + id + ".jpg");
            //var pngPath = YuUnityIOUtility.GetAssetsPath(appHelper.AssetDatabaseTexture
            //                                             + id + ".png");
            //var texture = AssetDatabaseUtility.LoadAssetAtPath<Texture>(jpgPath);
            //if (texture == null)
            //{
            //    texture = AssetDatabaseUtility.LoadAssetAtPath<Texture>(pngPath);
            //}

            //if (texture != null)
            //{
            //    textureDict.Add(id, texture);
            //    return texture;
            //}

            //// 从开发资源目录下的RawImage加载
            //jpgPath = YuUnityIOUtility.GetAssetsPath(appHelper.OriginRawImageDir
            //                                         + id + ".jpg");
            //pngPath = YuUnityIOUtility.GetAssetsPath(appHelper.OriginRawImageDir
            //                                         + id + ".png");
            //texture = AssetDatabaseUtility.LoadAssetAtPath<Texture>(jpgPath);
            //if (texture == null)
            //{
            //    texture = AssetDatabaseUtility.LoadAssetAtPath<Texture>(pngPath);
            //}

            //if (texture != null)
            //{
            //    textureDict.Add(id, texture);
            //    return texture;
            //}

            Debug.LogError($"目标RawImage所使用资源{id}在AssetDatabase/texture目录和OriginAsset/RawImage目录都无法找到，" +
                           "请检查是否存在目标资源或者是否在RawImage上使用了一个精灵资源！");
            return null;
        }


#endif

        private void WaitTexture_AtPlay(string id, Action<Texture> callback)
        {
            m_AssetModule.LoadAsync<Texture2D>(id, texture2D => { callback?.Invoke(texture2D); });
        }
    }
}