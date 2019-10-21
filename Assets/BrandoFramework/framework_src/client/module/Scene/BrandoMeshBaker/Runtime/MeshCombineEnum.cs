namespace GameWorld
{
    /// <summary>
    /// ���ȱ���ί��
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
        bakeIntoSceneObject,    //��ȾΪ��������Ϸ����
        bakeMeshAssetsInPlace,  //
        bakeIntoPrefab
    }

    public enum LightmapOptions
    {
        // Use this if all source objects are lightmapped and you want to preserve it.
        // All source objects must use the same lightmap. DOES NOT WORK IN UNITY 5."
        //ʹ��ԭ�еĹ�����ͼ����Դ���������ɹ�����ͼ������Դ�������ʹ����ͬ�Ĺ�����ͼ
        preserve_current_lightmapping,
        // A UV2 channel will not be generated for the combined mesh
        ignore_UV2,
        // Use this if UV2 is being used for something other than lightmaping
        copy_UV2_unchanged,
        // Use this if you want to bake a lightmap after the combined mesh has been generated
        //�����µ� UV2 LightMap
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
    /// ����ƽ�̷�ʽ
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
    /// ��֤����
    /// </summary>
    public enum ValidationLevel
    {
        none,
        quick,  //����
        robust  //����
    }
}

