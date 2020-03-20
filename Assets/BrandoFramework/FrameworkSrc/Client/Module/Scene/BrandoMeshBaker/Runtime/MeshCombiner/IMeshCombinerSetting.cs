namespace GameWorld
{
    public interface IMeshCombinerSettingHolder
    {
        IMeshCombinerSetting GetMeshBakerSettings();
#if UNITY_EDITOR
        UnityEditor.SerializedProperty GetMeshBakerSettingsAsSerializedProperty();
#endif
    }

    public interface IMeshCombinerSetting
    {
        bool doBlendShapes { get; set; }
        bool doCol { get; set; }
        bool doNorm { get; set; }
        bool doTan { get; set; }
        bool doUV { get; set; }
        bool doUV3 { get; set; }
        bool doUV4 { get; set; }
        LightmapOptions lightmapOption { get; set; }
        float uv2UnwrappingParamsHardAngle { get; set; }
        float uv2UnwrappingParamsPackMargin { get; set; }
        bool optimizeAfterBake { get; set; }
        bool recenterVertsToBoundsCenter { get; set; }
        bool clearBuffersAfterBake { get; set; }
        RendererType renderType { get; set; }
    }
}