using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class SceneShaderController : MonoBehaviour
{
    // light
    public Texture2D m_lightMapTexture;
    public Texture2D m_lightColorTexture;
    public Texture2D m_spotLightColorTexture;
    public Color m_ShadowColor = new Vector4(0.1f, 0.1f, 0.1f, 1);
    public Vector4 m_LightMapWeight = Vector4.one;

    public Color m_PointLightColor = Color.white;
    public float m_PointLightIntensity = 1.0f;

    public Light m_GlobalLight = null;
    public Color m_DefaultAmbientColor = Color.white;
  
    public float m_LightStrength = 1.0f;

    // 当前场景主光
    [HideInInspector]
    public Light m_curMainLight = null;

    // fog
    public bool _m_Fog = false;
    public bool m_Fog
    {
        get{
            return _m_Fog;
        }
        set{
            _m_Fog = value;
            RenderSettings.fog = value;
        }
    }
    public Color m_FogColor = Color.white;
    public float m_FogDensity = 0;
    public float m_FogStartDistance = 0;
    public float m_FogEndDistance = 0;
    public FogMode m_FogMode = FogMode.Linear;

    // Ambient
    [System.NonSerialized]
    public Color m_AmbientColor = Color.white;
    public UnityEngine.Rendering.AmbientMode EnviromentLightingSource = UnityEngine.Rendering.AmbientMode.Trilight;
    public Color SkyColor = new Color(0.427f, 0.427f, 0.427f);
    public Color EquatorColor = new Color(0.537f, 0.537f, 0.537f);
    public Color GroundColor = new Color(0.631f, 0.631f, 0.631f);

    // Shadow
    public bool m_affectActorShadow = false;
    public Vector4 m_actorShadowParams = Vector4.zero;

    ///// 实现
    public static SceneShaderController Instance = null;
    public static bool m_runGame = false;
    private bool m_IsOverrideLightStrength = false;

    private Vector3 m_DefaultGlobalLightDir = new Vector3(0.3f, -0.8f, 0.5f);

    private UnityEngine.Rendering.AmbientMode _EnviromentLightingSource = UnityEngine.Rendering.AmbientMode.Trilight;
    private Color _SkyColor = new Color(0.427f, 0.427f, 0.427f);
    private Color _EquatorColor = new Color(0.537f, 0.537f, 0.537f);
    private Color _GroundColor = new Color(0.631f, 0.631f, 0.631f);

    private int m_lightMapTextureID = -1;
    private int m_LightColorTextureID = -1;
    private int m_ShadowColorID = -1;
    private int m_LightMapWeightID = -1;
    private int m_GlobalLightColorID = -1;
    private int m_GlobalLightDirID = -1;
    private int m_LightStrengthID = -1;
    private int m_AmbientColorID = -1;
    private int m_PointLightColorID = -1;
    private int m_PointLightIntensityID = -1;

    public void SetValue()
    {
        if (m_lightMapTextureID != -1)
        {
            Shader.SetGlobalColor(m_ShadowColorID, m_ShadowColor);
            Shader.SetGlobalVector(m_LightMapWeightID, m_LightMapWeight);
            Shader.SetGlobalTexture(m_lightMapTextureID, m_lightMapTexture);
            Shader.SetGlobalTexture(m_LightColorTextureID, m_spotLightColorTexture);
            if (m_GlobalLight != null){
                Shader.SetGlobalColor(m_GlobalLightColorID, m_GlobalLight.color);
                Shader.SetGlobalVector(m_GlobalLightDirID, -m_GlobalLight.transform.forward);
            }
            else{
                Shader.SetGlobalColor(m_GlobalLightColorID, Color.white);
                Shader.SetGlobalVector(m_GlobalLightDirID, -m_DefaultGlobalLightDir);
            }

            Shader.SetGlobalColor(m_PointLightColorID, m_PointLightColor);
            Shader.SetGlobalFloat(m_PointLightIntensityID, m_PointLightIntensity);
        }

        if (m_AmbientColorID != -1)
        {
            Shader.SetGlobalColor(m_AmbientColorID, m_AmbientColor);
        }

        if (m_LightStrengthID != -1)
        {
            if (m_IsOverrideLightStrength)
            {
                Shader.SetGlobalFloat(m_LightStrengthID, m_LightStrength);
            }
            else
            {
                Shader.SetGlobalFloat(m_LightStrengthID, 1.0f);
            }
        }
    }

    public void SetOverrideLightStrength(bool val)
    {
        m_IsOverrideLightStrength = val;
    }

    void InitShaderProperty()
    {
        if (m_lightMapTextureID == -1)
        {
            m_lightMapTextureID = Shader.PropertyToID("_ShadowMaskMap");
            m_LightColorTextureID = Shader.PropertyToID("_LightColorMap");
            m_ShadowColorID = Shader.PropertyToID("_ShadowColor");
            m_LightMapWeightID = Shader.PropertyToID("_LightMapWeight");
            m_GlobalLightColorID = Shader.PropertyToID("_GlobalLightColor");
            m_GlobalLightDirID = Shader.PropertyToID("_GlobalLightDir");
            m_PointLightColorID = Shader.PropertyToID("_PointLightColor");
            m_PointLightIntensityID = Shader.PropertyToID("_PointLightIntensity");

        }
        if (m_AmbientColorID == -1)
        {
            m_AmbientColorID = Shader.PropertyToID("_AmbientColor");
        }
        if (m_LightStrengthID == -1)
        {
            m_LightStrengthID = Shader.PropertyToID("_LightStrength");
        }
    }

    private void Awake()
    {
        InitShaderProperty();
        SetValue();
        Instance = this;
        m_AmbientColor = m_DefaultAmbientColor;
    }

    private void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        if (!m_runGame)
        {
            ModifyLightMap();
        }
 
        InitShaderProperty();
        SetValue();
        if (Application.isPlaying)
        {
            ApplyModifyFog();
        }
    }

    [ExecuteInEditMode]
    private void OnValidate()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            InitShaderProperty();
            SetValue();
            ApplyModifyFog();
        }
    }

    public void ApplyModifyFog()
    {
        RenderSettings.fog = _m_Fog;
        RenderSettings.fogColor = m_FogColor;
        RenderSettings.fogDensity = m_FogDensity;
        RenderSettings.fogMode = m_FogMode;
        RenderSettings.fogStartDistance = m_FogStartDistance;
        RenderSettings.fogEndDistance = m_FogEndDistance;
    }

    private void Update()
    {
        SetValue();
        SetEnviromentLighting();
    }

    bool m_hasSeting = false;
    private void SetEnviromentLighting()
    {
        if (!m_hasSeting)
        {
            if (!m_hasSeting)
            {
                _EnviromentLightingSource = RenderSettings.ambientMode;
                _SkyColor = RenderSettings.ambientSkyColor;
                _EquatorColor = RenderSettings.ambientEquatorColor;
                _GroundColor = RenderSettings.ambientGroundColor;
            }

            if (Application.isPlaying)
            {
                RenderSettings.ambientMode = EnviromentLightingSource;
                RenderSettings.ambientSkyColor = SkyColor;
                RenderSettings.ambientEquatorColor = EquatorColor;
                RenderSettings.ambientGroundColor = GroundColor;
                m_hasSeting = true;
            }

            if(m_affectActorShadow)
            {
                GameObject ShadowRenderCameraSetupGB = GameObject.Find("ShadowRenderCameraSetup");
                if(ShadowRenderCameraSetupGB != null)
                {
                    //ShadowRender sr = ShadowRenderCameraSetupGB.GetComponent<ShadowRender>();
                    //if(sr != null)
                    //{
                    //    sr.xRotation = m_actorShadowParams.x;
                    //    sr.yRotation = m_actorShadowParams.y;
                    //}
                    //FollowPlayerOffset fpo = ShadowRenderCameraSetupGB.GetComponent<FollowPlayerOffset>();
                    //fpo.xOffset = m_actorShadowParams.z;
                    //fpo.zOffset = m_actorShadowParams.w;
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        RenderSettings.ambientMode = _EnviromentLightingSource;
        RenderSettings.ambientSkyColor = _SkyColor;
        RenderSettings.ambientEquatorColor = _EquatorColor;
        RenderSettings.ambientGroundColor = _GroundColor;
    }

    public void ModifyLightMap()
    {
        LightmapData[] lightmapData = LightmapSettings.lightmaps;
        int lightmapCount = lightmapData.Length;
        if (lightmapCount > 0)
        {
            lightmapData[lightmapCount - 1].lightmapDir = m_lightMapTexture;
            if (m_lightColorTexture != null)
            {
                lightmapData[lightmapCount - 1].lightmapColor = m_lightColorTexture;
            }
        }

        LightmapSettings.lightmaps = lightmapData;
    }

    public void ClearLightMap()
    {
        LightmapSettings.lightmaps = null;
    }
}
