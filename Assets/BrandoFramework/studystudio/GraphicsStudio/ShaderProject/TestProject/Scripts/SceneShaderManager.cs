using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneShaderManager : MonoBehaviour
{
    public static SceneShaderManager Instance = null;

    public bool isRealTimeLight = false;

    public Texture2D lightMapTexture;
    private int lightMapTextureID = -1;

    public Texture2D lightColorTexture;
    private int lightColorTextureID = -1;

    public Vector4 lightMapWeight = Vector4.one;
    private int lightMapWeightID = -1;

    public Color shadowColor = new Vector4(0.1f, 0.1f, 0.1f, 1);
    private int shadowColorID = -1;

    public Light globalLight = null;
    private Vector3 defaultGlobalLightDir = new Vector3(0.3f, -0.8f, 0.5f);
    private int globalLightColorID = -1;
    private int globalLightDirID = -1;

    private bool m_IsOverrideLightStrength = true;
    public float lightStrength = 1.0f;
    private int lightStrengthID = -1;

    [System.NonSerialized]
    public Color ambientColor = Color.white;
    public Color defaultAmbientColor = Color.white;
    private int ambientColorID = -1;

    public Color pointLightColor = Color.white;
    private int pointLightColorID = -1;

    public float pointLightIntensity = 1.0f;
    private int pointLightIntensityID = -1;

    // 当前场景主光
    [HideInInspector]
    public Light m_curMainLight = null;


    private void Awake()
    {
        Instance = this;
        InitShaderProperty();
        SetPropertyValue();
        SetRealTimeLightState(isRealTimeLight);
    }

    void Update()
    {
        SetPropertyValue();
    }

    void InitShaderProperty()
    {
        if (lightMapTextureID == -1)
        {
            lightMapTextureID = Shader.PropertyToID("_ShadowMaskMap");
            lightColorTextureID = Shader.PropertyToID("_LightColorMap");
            shadowColorID = Shader.PropertyToID("_ShadowColor");
            lightMapWeightID = Shader.PropertyToID("_LightMapWeight");
            globalLightColorID = Shader.PropertyToID("_GlobalLightColor");
            globalLightDirID = Shader.PropertyToID("_GlobalLightDir");
            pointLightColorID = Shader.PropertyToID("_PointLightColor");
            pointLightIntensityID = Shader.PropertyToID("_PointLightIntensity");

        }
        if (ambientColorID == -1)
        {
            ambientColorID = Shader.PropertyToID("_AmbientColor");
        }
        if (lightStrengthID == -1)
        {
            lightStrengthID = Shader.PropertyToID("_LightStrength");
        }
    }

    public void SetPropertyValue()
    {
        if (lightMapTextureID != -1)
        {
            //Shader.SetGlobalColor(shadowColorID, shadowColor);
            //Shader.SetGlobalVector(lightMapWeightID, m_LightMapWeight);
            //Shader.SetGlobalTexture(lightMapTextureID, m_lightMapTexture);
            //Shader.SetGlobalTexture(lightColorTextureID, m_spotLightColorTexture);
            if (globalLight != null)
            {
                Shader.SetGlobalColor(globalLightColorID, globalLight.color);
                Shader.SetGlobalVector(globalLightDirID, -globalLight.transform.forward);
            }
            else
            {
                Shader.SetGlobalColor(globalLightColorID, Color.white);
                Shader.SetGlobalVector(globalLightDirID, -defaultGlobalLightDir);
            }

            //Shader.SetGlobalColor(m_PointLightColorID, m_PointLightColor);
            //Shader.SetGlobalFloat(m_PointLightIntensityID, m_PointLightIntensity);
        }

        if (ambientColorID != -1)
        {
            Shader.SetGlobalColor(ambientColorID, ambientColor);
        }

        if (lightStrengthID != -1)
        {
            if (m_IsOverrideLightStrength)
            {
                Shader.SetGlobalFloat(lightStrengthID, lightStrength);
            }
            else
            {
                Shader.SetGlobalFloat(lightStrengthID, 1.0f);
            }
        }
    }

    public void SetRealTimeLightState(bool open)
    {
        if (open)
        {
            Shader.EnableKeyword("_REALTIME_ON");
        }
        else
        {
            Shader.DisableKeyword("_REALTIME_ON");
        }
    }
}
