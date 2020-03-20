using GameWorld;
using System;
using UnityEngine;

namespace Client.Scene
{
    [Serializable]
    public class GameObjectCombineSetting
    {
        [SerializeField] internal bool DoMultiMaterial = false;
        [SerializeField] internal int atlasPadding = 1;
        [SerializeField] internal int maxAtlasWidth = 0;
        [SerializeField] internal int maxAtlasHeight = 0;
        [SerializeField] internal bool useMaxAtlasHeightOverride = false;
        [SerializeField] internal bool useMaxAtlasWidthOverride = false;
        [SerializeField] internal bool resizePowerOfTwoTextures = false;    //POT texture 重新设置尺寸（算上 padding）
        [SerializeField] internal bool fixOutOfBoundsUVs = false;
        [SerializeField] internal int maxTilingBakeSize = 1024;
        [SerializeField] internal bool saveAtlasesAsAssets = false;
        [SerializeField] internal PackingAlgorithmEnum _packingAlgorithm = PackingAlgorithmEnum.UnitysPackTextures;
        [SerializeField] internal bool meshBakerTexturePackerForcePowerOfTwo = true;              //贴图 Atlas 为 POT
        [SerializeField] internal bool normalizeTexelDensity = false;
        [SerializeField] internal bool considerNonTextureProperties = false;
    }
}

