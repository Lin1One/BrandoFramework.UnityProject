namespace GameWorld
{
    /// <summary>
    /// 进度报告委托
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="progress"></param>
    public delegate void ProgressUpdateDelegate(string msg, float progress);
    public delegate bool ProgressUpdateCancelableDelegate(string msg, float progress);

    public enum ObjsToCombineTypes
    {
        prefabOnly,
        sceneObjOnly,
        dontCare
    }

    public enum CombineOutputOptions
    {
        bakeIntoPrefab,
        bakeMeshsInPlace,
        bakeTextureAtlasesOnly,
        bakeIntoSceneObject
    }

    public enum RendererType
    {
        meshRenderer,
        skinnedMeshRenderer
    }

    public enum OutputOptions
    {
        bakeIntoSceneObject,    //渲染为场景内游戏物体
        bakeMeshAssetsInPlace,  //
        bakeIntoPrefab
    }

    public enum LightmapOptions
    {
        // Use this if all source objects are lightmapped and you want to preserve it.
        // All source objects must use the same lightmap. DOES NOT WORK IN UNITY 5."
        //使用原有的光照贴图，需源对象都已生成光照贴图，所有源对象必须使用相同的光照贴图
        preserve_current_lightmapping,
        // A UV2 channel will not be generated for the combined mesh
        ignore_UV2,
        // Use this if UV2 is being used for something other than lightmaping
        copy_UV2_unchanged,
        // Use this if you want to bake a lightmap after the combined mesh has been generated
        //生成新的 UV2 LightMap
        generate_new_UV2_layout,
        // Use this if your meshes include a custom lightmap that you want to use with the combined mesh
        copy_UV2_unchanged_to_separate_rects,
    }


    public enum PackingAlgorithmEnum
    {
        UnitysPackTextures,
        MeshBakerTexturePacker,
        MeshBakerTexturePacker_Fast,
        MeshBakerTexturePacker_Horizontal, //special packing packs all horizontal. makes it possible to use an atlas with tiling textures
        MeshBakerTexturePacker_Vertical, //special packing packs all Vertical. makes it possible to use an atlas with tiling textures
    }

    /// <summary>
    /// 纹理平铺方式
    /// </summary>
    public enum TextureTilingTreatment
    {
        none,
        considerUVs,
        edgeToEdgeX,
        edgeToEdgeY,
        edgeToEdgeXY, // One image in atlas.
        unknown,
    }

    /// <summary>
    /// 验证级别
    /// </summary>
    public enum ValidationLevel
    {
        none,
        quick,  //快速
        robust  //完整
    }
}

