/************************************************************
//     文件名      : GodRayComponent.cs
//     功能描述    : Godray后处理
//     负责人      : xufang
//     参考文档    : 无
//     创建日期    : 2019/10/22
//     Copyright   : Copyright 2014-2017 EZFun Inc.
**************************************************************/

using UnityEngine;
using UnityEngine.Rendering;

namespace GraphicsStudio.PostProcessing
{
    public sealed class GodRayComponent : PostProcessingComponentRenderTexture<GodRayModel>
    {
        static class Uniforms
        {
            internal static readonly int _ThresholdID = Shader.PropertyToID("_threshold");
            internal static readonly int _ViewPortLightPosID = Shader.PropertyToID("_viewPortLightPos");
            internal static readonly int _CameraDepthTexID = Shader.PropertyToID("_CustomDepthTex");
            internal static readonly int _OffsetsID = Shader.PropertyToID("_offsets");
            internal static readonly int _GodRayBlurTexID = Shader.PropertyToID("_godRayBlurTex");
            internal static readonly int _GodRayLightColorID = Shader.PropertyToID("_godRayLightColor");
            internal static readonly int _GodRayIntensityID = Shader.PropertyToID("_godRayIntensity");
            internal static readonly int _GodRayLightParamID = Shader.PropertyToID("_lightParam");
            internal static readonly int _GodRayEffectParamID = Shader.PropertyToID("_effectParam");
        }

        const string k_ReplaceShaderName = "Unlit/CustomDepthOnly";
        const string k_GodRayShaderName = "Hidden/Post FX/GodRay";
        const string k_DepthRTName = "_GodRayDepthCameraRT";

        private CustomDepthCamera m_depthCamera = null;

        // 场景深度图
        private RenderTexture m_depthTex = null;
        
        // 场景主光
        private Light m_mainLight = null;

        public override bool active
        {
            get
            {
                return model.enabled && !context.interrupted;
            }
        }

        public override void OnDisable()
        {
            if (m_depthCamera != null)
            {
                m_depthCamera.SetRenderTexture(null);
                m_depthCamera = null;
            }

            if (m_depthTex != null)
            {
                UnityEngine.Object.Destroy(m_depthTex);
                m_depthTex = null;
            }

            m_mainLight = null;
            model.enabled = false;
        }

        public void OnPreRender()
        {
            if (m_depthCamera != null)
            {
                m_depthCamera.OnPreRender();
            }
        }

        private bool CheckResource(Camera mainCam)
        {
            SceneShaderController shaderCtrl = SceneShaderController.Instance;
            if (mainCam == null || shaderCtrl == null)
            {
                return false;
            }

            if (m_depthTex == null)
            {
                m_depthTex = new RenderTexture(mainCam.pixelWidth, mainCam.pixelHeight, 16, RenderTextureFormat.Depth);
                m_depthTex.name = k_DepthRTName;
                m_depthTex.hideFlags = HideFlags.HideAndDontSave;
            }

            if (m_depthCamera == null)
            {
                m_depthCamera = new CustomDepthCamera();
                // 计算大物件的深度图
                int cullingLayer = LayerMask.GetMask("Ground", "Building", "Tree", "Ignore Raycast", "3DName");
                m_depthCamera.Init(mainCam, m_depthTex, k_ReplaceShaderName, "DepthReplaceTag", cullingLayer, false);
            }

            if (m_mainLight == null)
            {
                //m_mainLight = shaderCtrl.m_curMainLight;
            }

            return true;
        }

        public void Prepare(RenderTexture source, Material uberMaterial, Texture autoExposure)
        {
            Camera mainCam = Camera.main;
            if (mainCam == null || !CheckResource(mainCam))
            {
                return;
            }

            var godrayMaterial = context.materialFactory.Get(k_GodRayShaderName);
            if (godrayMaterial == null)
            {
                Debug.LogError("GodRayComponent: error material!");
                return;
            }

            int rtWidth = context.width / 2;
            int rtHeight = context.height / 2;
            if (m_mainLight !=null)
            {
                // 计算当前光源所在的世界位置
                Vector3 viewPortLightPos = mainCam.WorldToViewportPoint(model.settings.targetViewPos - m_mainLight.transform.forward * 1000.0f);
                godrayMaterial.SetVector(Uniforms._ViewPortLightPosID, new Vector4(viewPortLightPos.x, viewPortLightPos.y, viewPortLightPos.z, 0));
            }
            else
            {
                godrayMaterial.SetVector(Uniforms._ViewPortLightPosID, new Vector3(0.5f, 0.5f, 0));
            }
            
            godrayMaterial.SetVector(Uniforms._ThresholdID, model.settings.threshold);
            godrayMaterial.SetVector(Uniforms._GodRayLightParamID, new Vector4(model.settings.lightPow, model.settings.lightRadius, 0, 0));
            godrayMaterial.SetVector(Uniforms._GodRayEffectParamID, new Vector4(model.settings.decay, model.settings.exposure, model.settings.depthThreshold, 0));
            godrayMaterial.SetTexture(Uniforms._CameraDepthTexID, m_depthTex);
            RenderTexture temp1 = context.renderTextureFactory.Get(rtWidth, rtHeight, 0, RenderTextureFormat.Default, "GodRayComponent_Temp1");
            Graphics.Blit(source, temp1, godrayMaterial, 0);
            float samplerOffset = model.settings.blurOffset / context.width;
            RenderTexture temp2 = context.renderTextureFactory.Get(rtWidth, rtHeight, 0, RenderTextureFormat.Default, "GodRayComponent_Temp2");
            for (int i = 0; i < model.settings.blurIterations; i++)
            {
                float offset = samplerOffset * (i * 2 + 1);
                godrayMaterial.SetVector(Uniforms._OffsetsID, new Vector4(offset, offset, 0, 0));
                Graphics.Blit(temp1, temp2, godrayMaterial, 1);

                offset = samplerOffset * (i * 2 + 2);
                godrayMaterial.SetVector(Uniforms._OffsetsID, new Vector4(offset, offset, 0, 0));
                Graphics.Blit(temp2, temp1, godrayMaterial, 1);
            }

            context.renderTextureFactory.Release(temp2);

            uberMaterial.SetTexture(Uniforms._GodRayBlurTexID, temp1);
            uberMaterial.SetVector(Uniforms._GodRayLightColorID, (m_mainLight == null) ? model.settings.lightColor : m_mainLight.color);
            uberMaterial.SetFloat(Uniforms._GodRayIntensityID, model.settings.intensity);
            uberMaterial.EnableKeyword("GODRAY");
        }
    }
}
