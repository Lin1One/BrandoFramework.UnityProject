using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GraphicsStudio.PostProcessing
{
    //using DebugMode = BuiltinDebugViewsModel.Mode;

    [ImageEffectAllowedInSceneView]
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [AddComponentMenu("Effects/PostProcessingEffectComponent", -1)]
    public class PostProcessingBehaviour : MonoBehaviour
    {
        // Inspector fields
        public PostProcessingProfile profile;
        PostProcessingProfile m_PreviousProfile;

        public Func<Vector2, Matrix4x4> jitteredMatrixFunc;

        Camera m_Camera;
        Camera m_PreviousCamera;
        List<PostProcessingComponentBase> m_Components;                     //后处理组件列表
        Dictionary<PostProcessingComponentBase, bool> m_ComponentStates;    //后处理组件状态
        MaterialFactory m_MaterialFactory;
        RenderTextureFactory m_RenderTextureFactory;
        PostProcessingContext m_Context;
        // Internal helpers
        Dictionary<Type, KeyValuePair<CameraEvent, CommandBuffer>> m_CommandBuffers;

        bool m_RenderingInSceneView = false;

        bool m_CompCreated = false;

        // for linear-gamma space
        static RenderTexture m_GammaRT = null;
        static Material m_LinearToGammaMat = null;

        public static RenderTexture GetRenderTexture()
        {
            return m_GammaRT;
        }

        #region Components

        // Effect components
        BuiltinDebugViewsComponent m_DebugViews;
        TaaComponent m_Taa;
        DepthOfFieldComponent m_DepthOfField;
        BloomComponent m_Bloom;
        ColorGradingComponent m_ColorGrading;
        FxaaComponent m_Fxaa;
        ColorAdjustComponent m_ColorAdjust;
        GodRayComponent m_GodRayComponent;
        SkinComponent m_SkinComponent;
        ToneMappingComponent m_ToneMapping;
        NewBloomComponent m_NewBloom;
        SunShaftComponent m_SunShaft;
        GlowComponent m_Glow;
        DummyBlurComponent m_DummyBlurComponent;
        DummyMotionBlurComponent m_DummyMotionBlurComponent;

        #endregion

        #region 生命周期

        private void ResetComponents(PostProcessingContext context, bool bAddComp = false)
        {
            if (bAddComp)
            {
                // Component list
                m_DebugViews = AddComponent(new BuiltinDebugViewsComponent());
                m_Taa = AddComponent(new TaaComponent());
                m_DepthOfField = AddComponent(new DepthOfFieldComponent());
                m_Bloom = AddComponent(new BloomComponent());
                m_ColorGrading = AddComponent(new ColorGradingComponent());
                m_Fxaa = AddComponent(new FxaaComponent());
                m_ColorAdjust = AddComponent(new ColorAdjustComponent());
                m_DummyBlurComponent = AddComponent(new DummyBlurComponent());
                m_DummyMotionBlurComponent = AddComponent(new DummyMotionBlurComponent());
                m_GodRayComponent = AddComponent(new GodRayComponent());
                m_SkinComponent = AddComponent(new SkinComponent());
                m_ToneMapping = AddComponent(new ToneMappingComponent());
                m_SunShaft = AddComponent(new SunShaftComponent());
                m_NewBloom = AddComponent(new NewBloomComponent());
                m_Glow = AddComponent(new GlowComponent());
            }

            // Prepare components
            if (profile != null)
            {
                m_DebugViews.Init(context, profile.debugViews);
                m_Taa.Init(context, profile.antialiasing);
                m_DepthOfField.Init(context, profile.depthOfField);
                m_Bloom.Init(context, profile.bloom);
                m_ColorGrading.Init(context, profile.colorGrading);
                m_Fxaa.Init(context, profile.antialiasing);
                m_ColorAdjust.Init(context, profile.colorAdjust);
                m_DummyBlurComponent.Init(context, profile.DummyBlur);
                m_DummyMotionBlurComponent.Init(context, profile.DummyMotionBlur);
                m_GodRayComponent.Init(context, profile.GodRay);
                m_SkinComponent.Init(context, profile.skin);
                m_ToneMapping.Init(context, profile.tonemapping);
                m_SunShaft.Init(context, profile.sunshaft);
                m_NewBloom.Init(context, profile.newBloom);
                m_Glow.Init(context, profile.glow);
            }
        }

        void SetRenderTarget()
        {
            // Set LINEAR render target...
            if (m_Camera)
            {
                if (!m_LinearToGammaMat)
                    m_LinearToGammaMat = Resources.Load<Material>("LinearToGamma");

                RenderTextureFormat format = m_Camera.allowHDR ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;

                if (m_GammaRT && (m_GammaRT.format != format || m_GammaRT.width != m_Camera.pixelWidth || m_GammaRT.height != m_Camera.pixelHeight))
                {
                    m_Camera.targetTexture = null; // fix error report
                    RenderTexture.ReleaseTemporary(m_GammaRT);
                    m_GammaRT = null;
                }

                if (!m_GammaRT)
                {
                    m_GammaRT = RenderTexture.GetTemporary(m_Camera.pixelWidth, m_Camera.pixelHeight, 24, format, RenderTextureReadWrite.Linear);
                    m_GammaRT.filterMode = FilterMode.Point;
                    m_GammaRT.name = "GAMMA_RENDER_TARGET_TEXTURE";
                }

                m_Camera.targetTexture = m_GammaRT;
            }
        }

        void OnEnable()
        {
            if (m_CompCreated) return;
            m_CommandBuffers = new Dictionary<Type, KeyValuePair<CameraEvent, CommandBuffer>>();

            m_Camera = GetComponent<Camera>();

            // Keep a list of all post-fx for automation purposes
            m_Components = new List<PostProcessingComponentBase>();

            // Prepare context
            m_Context = new PostProcessingContext();
            m_MaterialFactory = new MaterialFactory();
            m_RenderTextureFactory = new RenderTextureFactory();
            m_Context.Reset();
            m_Context.profile = profile;
            m_Context.renderTextureFactory = m_RenderTextureFactory;
            m_Context.materialFactory = m_MaterialFactory;
            m_Context.camera = m_Camera;

            m_PreviousProfile = profile;
            m_PreviousCamera = m_Camera;
            ResetComponents(m_Context, true);

            // Prepare state observers
            m_ComponentStates = new Dictionary<PostProcessingComponentBase, bool>();

            foreach (var component in m_Components)
            {
                m_ComponentStates.Add(component, false);
            }

            useGUILayout = false;
            m_CompCreated = true;

            SetRenderTarget();
        }

        void OnDisable()
        {
            m_CompCreated = false;

            // Clear command buffers
            foreach (var cb in m_CommandBuffers.Values)
            {
                m_Camera.RemoveCommandBuffer(cb.Key, cb.Value);
                cb.Value.Dispose();
            }

            m_CommandBuffers.Clear();

            // Clear components
            if (profile != null)
                DisableComponents();

            m_Components.Clear();
            
            // Factories
            m_MaterialFactory.Dispose();
            m_RenderTextureFactory.Dispose();
            GraphicsUtils.Dispose();

            UICamera._SceneRTGamma = null;
        }

        void OnPreCull()
        {
            // All the per-frame initialization logic has to be done in OnPreCull instead of Update
            // because [ImageEffectAllowedInSceneView] doesn't trigger Update events...

            m_Camera = GetComponent<Camera>();
            if (profile == null || m_Camera == null)
                return;

#if UNITY_EDITOR
            // Track the scene view camera to disable some effects we don't want to see in the
            // scene view
            // Currently disabled effects :
            //  - Temporal Antialiasing
            //  - Depth of Field
            //  - Motion blur
            m_RenderingInSceneView = UnityEditor.SceneView.currentDrawingSceneView != null
                && UnityEditor.SceneView.currentDrawingSceneView.camera == m_Camera;
#endif

            if (m_PreviousProfile != profile || m_PreviousCamera != m_Camera)
            {
                var context = m_Context.Reset();
                context.profile = profile;
                context.renderTextureFactory = m_RenderTextureFactory;
                context.materialFactory = m_MaterialFactory;
                context.camera = m_Camera;
                m_PreviousProfile = profile;
                m_PreviousCamera = m_Camera;
                ResetComponents(context);
            }

            // Handles profile change and 'enable' state observers
            if (m_PreviousProfile != profile)
            {
                DisableComponents();
                m_PreviousProfile = profile;
            }

            CheckObservers();

            // Find out which camera flags are needed before rendering begins
            // Note that motion vectors will only be available one frame after being enabled
            var flags = m_Camera.depthTextureMode;
            foreach (var component in m_Components)
            {
                if (component.active)
                    flags |= component.GetCameraFlags();
            }

            m_Camera.depthTextureMode = flags;

            // Temporal antialiasing jittering, needs to happen before culling
            if (!m_RenderingInSceneView && m_Taa.active && !profile.debugViews.willInterrupt)
                m_Taa.SetProjectionMatrix(jitteredMatrixFunc);

            if (m_DummyBlurComponent.active)
            {
                m_DummyBlurComponent.UpdateProcess();
            }      
            
            if (m_DummyMotionBlurComponent.active)
            {
                m_DummyMotionBlurComponent.UpdateProcess();
            }      
        }

        //渲染前置操作
        void OnPreRender()
        {
            if (profile == null)
                return;

            // Command buffer-based effects should be set-up here
            TryExecuteCommandBuffer(m_DebugViews);
            //TryExecuteCommandBuffer(m_AmbientOcclusion);
            //TryExecuteCommandBuffer(m_ScreenSpaceReflection);
            //TryExecuteCommandBuffer(m_FogComponent);
            TryExecuteCommandBuffer(m_DummyBlurComponent);
            TryExecuteCommandBuffer(m_DummyMotionBlurComponent);

            if (m_GodRayComponent.active)
            {
                m_GodRayComponent.OnPreRender();
            }

            if (m_SkinComponent.active)
                m_SkinComponent.OnPreRender();

            //if (!m_RenderingInSceneView)
            //    TryExecuteCommandBuffer(m_MotionBlur);

            SetRenderTarget();
        }

        void OnPostRender()
        {
            if (profile == null || m_Camera == null)
                return;

            if (!m_RenderingInSceneView && m_Taa.active && !profile.debugViews.willInterrupt)
                m_Context.camera.ResetProjectionMatrix();
        }

        // Classic render target pipeline for RT-based effects
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (profile == null || m_Camera == null)
            {
                // Set to UI
                Graphics.Blit(source, destination, m_LinearToGammaMat, 0);
                UICamera._SceneRTGamma = m_GammaRT;
                Scene3DUICamera._SceneRTGamma = m_GammaRT;
                return;
            }

            // Uber shader setup
            bool uberActive = false;
            bool fxaaActive = m_Fxaa.active;
            bool taaActive = m_Taa.active && !m_RenderingInSceneView;
            bool dofActive = m_DepthOfField.active && !m_RenderingInSceneView;

            var uberMaterial = m_MaterialFactory.Get("Hidden/Post FX/Uber Shader");
            uberMaterial.shaderKeywords = null;

            var src = source;
            var dst = RenderTexture.GetTemporary(src.width, src.height, 0, src.format, RenderTextureReadWrite.Linear);
       
            // SunShaft
            if (m_SunShaft.active)
            {
                var tempRT = m_RenderTextureFactory.Get(src, "PostProcessingBehaviour_sunShaft");
                m_SunShaft.Render(src, tempRT);
                src = tempRT;
            }

            // Bloom
            if (m_NewBloom.active)
            {
                var tempRT = m_RenderTextureFactory.Get(src, "PostProcessingBehaviour_bloom");
                m_NewBloom.Render(src, tempRT);
                src = tempRT;
            }

            // Glow
            if (m_Glow.active)
            {
                var tempRT = m_RenderTextureFactory.Get(src, "PostProcessingBehaviour_glow");
                m_Glow.Render(src, tempRT);
                src = tempRT;
            }

            // Skin
            if (m_SkinComponent.active)
            {
                m_SkinComponent.Render(src, dst);
            }
            
            if (taaActive)
            {
                var tempRT = m_RenderTextureFactory.Get(src, "PostProcessingBehaviour_taa");
                m_Taa.Render(src, tempRT);
                src = tempRT;
            }

            Texture autoExposure = GraphicsUtils.whiteTexture;
            //if (m_EyeAdaptation.active)
            //{
            //    uberActive = true;
            //    autoExposure = m_EyeAdaptation.Prepare(src, uberMaterial);
            //}

            uberMaterial.SetTexture("_AutoExposure", autoExposure);
            
            if (dofActive)
            {
                uberActive = true;
                m_DepthOfField.Prepare(src, uberMaterial, taaActive, m_Taa.jitterVector, m_Taa.model.settings.taaSettings.motionBlending);
            }
            
            if (m_GodRayComponent.active)
            {
                uberActive = true;
                m_GodRayComponent.Prepare(src, uberMaterial, autoExposure);
            }

            if (m_Bloom.active)
            {
                uberActive = true;
                m_Bloom.Prepare(src, uberMaterial, autoExposure);
            }

            if (m_ColorAdjust.active)
            {
                uberActive = true;
                m_ColorAdjust.Prepare(uberMaterial);
            }
            
            //uberActive |= TryPrepareUberImageEffect(m_ChromaticAberration, uberMaterial);
            //uberActive |= TryPrepareUberImageEffect(m_ColorGrading, uberMaterial);
            //uberActive |= TryPrepareUberImageEffect(m_Vignette, uberMaterial);
            //uberActive |= TryPrepareUberImageEffect(m_UserLut, uberMaterial);

            var fxaaMaterial = fxaaActive
                ? m_MaterialFactory.Get("Hidden/Post FX/FXAA")
                : null;

            if (fxaaActive)
            {
                fxaaMaterial.shaderKeywords = null;
                //TryPrepareUberImageEffect(m_Grain, fxaaMaterial);
                //TryPrepareUberImageEffect(m_Dithering, fxaaMaterial);

                if (uberActive)
                {
                    var output = m_RenderTextureFactory.Get(src, "PostProcessingBehaviour_uber");
                    Graphics.Blit(src, output, uberMaterial, 0);
                    src = output;
                }

                if (m_ToneMapping.active)
                {
                    var toneMappingSrc = m_RenderTextureFactory.Get(src, "PostProcessingBehaviour_toneMapping_fxaaActive");
                    m_Fxaa.Render(src, toneMappingSrc);
                    m_ToneMapping.Render(toneMappingSrc, dst);
                }
                else
                    m_Fxaa.Render(src, dst);
            }
            else
            {
                //uberActive |= TryPrepareUberImageEffect(m_Grain, uberMaterial);
                //uberActive |= TryPrepareUberImageEffect(m_Dithering, uberMaterial);

                if (uberActive)
                {
                    if (!GraphicsUtils.isLinearColorSpace)
                        uberMaterial.EnableKeyword("UNITY_COLORSPACE_GAMMA");

                    if (m_ToneMapping.active)
                    {
                        var toneMappingSrc = m_RenderTextureFactory.Get(src, "PostProcessingBehaviour_toneMapping_fxaaUnActive");
                        Graphics.Blit(src, toneMappingSrc, uberMaterial, 0);
                        m_ToneMapping.Render(toneMappingSrc, dst);
                    }
                    else
                        Graphics.Blit(src, dst, uberMaterial, 0);
                }
            }

            if (!uberActive && !fxaaActive)
            {
                if (m_ToneMapping.active)
                    m_ToneMapping.Render(src, dst);
                else
                    Graphics.Blit(src, dst);
            }

            // Set to UI
            Graphics.Blit(dst, destination, m_LinearToGammaMat, 0);
            UICamera._SceneRTGamma = m_GammaRT;
            Scene3DUICamera._SceneRTGamma = m_GammaRT;
            RenderTexture.ReleaseTemporary(dst);

            m_RenderTextureFactory.ReleaseAll();
        }

        void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
                return;

            if (profile == null || m_Camera == null)
                return;

            //if (m_EyeAdaptation.active && profile.debugViews.IsModeActive(DebugMode.EyeAdaptation))
            //    m_EyeAdaptation.OnGUI();
            //else if (m_ColorGrading.active && profile.debugViews.IsModeActive(DebugMode.LogLut))
            //    m_ColorGrading.OnGUI();
            //else if (m_UserLut.active && profile.debugViews.IsModeActive(DebugMode.UserLut))
            //    m_UserLut.OnGUI();
        }

        #endregion

        public void ResetTemporalEffects()
        {
            m_Taa.ResetHistory();
            //m_MotionBlur.ResetHistory();
            //m_EyeAdaptation.ResetHistory();
        }

        #region State management

        List<PostProcessingComponentBase> m_ComponentsToEnable = new List<PostProcessingComponentBase>();
        List<PostProcessingComponentBase> m_ComponentsToDisable = new List<PostProcessingComponentBase>();

        void CheckObservers()
        {
            foreach (var cs in m_ComponentStates)
            {
                var component = cs.Key;
                var state = component.GetModel().enabled;

                if (state != cs.Value)
                {
                    if (state) m_ComponentsToEnable.Add(component);
                    else m_ComponentsToDisable.Add(component);
                }
            }

            for (int i = 0; i < m_ComponentsToDisable.Count; i++)
            {
                var c = m_ComponentsToDisable[i];
                m_ComponentStates[c] = false;
                c.OnDisable();
            }

            for (int i = 0; i < m_ComponentsToEnable.Count; i++)
            {
                var c = m_ComponentsToEnable[i];
                m_ComponentStates[c] = true;
                c.OnEnable();
            }

            m_ComponentsToDisable.Clear();
            m_ComponentsToEnable.Clear();
        }

        void DisableComponents()
        {
            foreach (var component in m_Components)
            {
                var model = component.GetModel();
                if (model != null && model.enabled)
                    component.OnDisable();
            }
        }

        #endregion

        #region Command buffer handling & rendering helpers
        // Placeholders before the upcoming Scriptable Render Loop as command buffers will be
        // executed on the go so we won't need of all that stuff
        CommandBuffer AddCommandBuffer<T>(CameraEvent evt, string name) where T : PostProcessingModel
        {
            var cb = new CommandBuffer { name = name };
            var kvp = new KeyValuePair<CameraEvent, CommandBuffer>(evt, cb);
            m_CommandBuffers.Add(typeof(T), kvp);
            m_Camera.AddCommandBuffer(evt, kvp.Value);
            return kvp.Value;
        }

        void RemoveCommandBuffer<T>() where T : PostProcessingModel
        {
            KeyValuePair<CameraEvent, CommandBuffer> kvp;
            var type = typeof(T);

            if (!m_CommandBuffers.TryGetValue(type, out kvp))
                return;

            m_Camera.RemoveCommandBuffer(kvp.Key, kvp.Value);
            m_CommandBuffers.Remove(type);
            kvp.Value.Dispose();
        }

        CommandBuffer GetCommandBuffer<T>(CameraEvent evt, string name) where T : PostProcessingModel
        {
            CommandBuffer cb;
            KeyValuePair<CameraEvent, CommandBuffer> kvp;

            if (!m_CommandBuffers.TryGetValue(typeof(T), out kvp))
            {
                cb = AddCommandBuffer<T>(evt, name);
            }
            else if (kvp.Key != evt)
            {
                RemoveCommandBuffer<T>();
                cb = AddCommandBuffer<T>(evt, name);
            }
            else cb = kvp.Value;

            return cb;
        }

        void TryExecuteCommandBuffer<T>(PostProcessingComponentCommandBuffer<T> component) where T : PostProcessingModel
        {
            if (component.active)
            {
                var cb = GetCommandBuffer<T>(component.GetCameraEvent(), component.GetName());
                cb.Clear();
                component.PopulateCommandBuffer(cb);
            }
            else RemoveCommandBuffer<T>();
        }

        bool TryPrepareUberImageEffect<T>(PostProcessingComponentRenderTexture<T> component, Material material)
            where T : PostProcessingModel
        {
            if (!component.active)
                return false;

            component.Prepare(material);
            return true;
        }

        T AddComponent<T>(T component) where T : PostProcessingComponentBase
        {
            m_Components.Add(component);
            return component;
        }

        #endregion
    }
}
