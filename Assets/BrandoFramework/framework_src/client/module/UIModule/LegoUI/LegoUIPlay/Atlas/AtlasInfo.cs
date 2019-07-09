#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/9 20:29:52
// Email:             836045613@qq.com

#endregion


using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.LegoUI
{
    [Serializable]
    public class AtlasInfo
    {
        public string LocAppId;

        /// <summary>
        /// 精灵所属图集映射字典。
        /// </summary>
        public Dictionary<string, string> SpriteIdMap
        = new Dictionary<string, string>();

        public string GetLocAtlasId(string spriteId)
        {
            if (SpriteIdMap.ContainsKey(spriteId))
            {
                var atlasId = SpriteIdMap[spriteId];
                return atlasId;
            }

#if UNITY_EDITOR
            Debug.LogError($"精灵{spriteId}找不到所在图集信息！");
#endif

            return null;
        }

        private void AddSpriteMap(string spriteId, string atlasId)
        {
            if (SpriteIdMap.ContainsKey(spriteId))
            {
                if(SpriteIdMap[spriteId] != atlasId)
                {
#if UNITY_EDITOR
                    Debug.LogError($"在{SpriteIdMap[spriteId]} 和 {atlasId} 出现同名 sprite {spriteId}！");
#endif
                }
                return;
            }

            SpriteIdMap.Add(spriteId, atlasId);
        }

        public void UpdateMap(List<string> spriteNames, string atlasId)
        {
            foreach (var spriteName in spriteNames)
            {
                AddSpriteMap(spriteName.ToLower(), atlasId);
            }
        }
    }
}

