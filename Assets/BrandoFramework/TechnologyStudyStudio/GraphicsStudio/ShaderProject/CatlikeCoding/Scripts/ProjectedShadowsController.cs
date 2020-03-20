using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class ProjectedShadowsController : MonoBehaviour
{
    

    // 当前场景主光
    public Light m_curMainLight;

    public Color shadowColor;

    public float shadowFalloff;

    public float planeHigh;

    int lightDirId;
    int shadowColorId;
    int falloffId;
    public void SetValue()
    {
        Vector4 lightDir = m_curMainLight.transform.position - transform.position;
        lightDir.w = planeHigh;
        Shader.SetGlobalVector(lightDirId, lightDir);
        Shader.SetGlobalColor(shadowColorId, shadowColor);
        Shader.SetGlobalFloat(falloffId, shadowFalloff);
    }

    void InitShaderProperty()
    {
        lightDirId = Shader.PropertyToID("_LightDir");
        shadowColorId = Shader.PropertyToID("_ShadowColor");
        falloffId = Shader.PropertyToID("_ShadowFalloff");
        Debug.Log("===============1" + lightDirId);
        Debug.Log("===============2" + shadowColorId);
        Debug.Log("===============3" + falloffId);
    }

    private void Awake()
    {
        InitShaderProperty();
        SetValue();
    }

    private void Update()
    {
        SetValue();
    }

}
