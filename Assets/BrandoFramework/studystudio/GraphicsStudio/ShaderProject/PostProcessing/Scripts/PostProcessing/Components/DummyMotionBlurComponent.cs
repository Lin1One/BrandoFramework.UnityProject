/************************************************************
//     文件名      : DummyMotionBlurPostprocess.cs
//     功能描述    : 保留之前版本的 MotionBlurPostprocess.cs
//     负责人      : xufang
//     参考文档    : 无
//     创建日期    : 2019/06/20.
//     Copyright   : Copyright 2014-2017 EZFun Inc.
**************************************************************/

using UnityEngine;
using UnityEngine.Rendering;

namespace GraphicsStudio.PostProcessing
{
    public sealed class DummyMotionBlurComponent : PostProcessingComponentCommandBuffer<DummyMotionBlurModel>
    {
        private bool m_IsRadialBlur = false;
        private bool m_IsIncreasing = true;
        private float m_BlurDist = 0.0f;
        private RenderTexture m_AccumTexture = null;
        private RadialBlurType m_RadialBlurType = RadialBlurType.vary;

        const string k_ShaderString = "EZFun/ImageEffects/RadialBlur";

        const string k_RTName_Accum = "MotionBlur_Accum_RT";

        public override bool active
        {
            get
            {
                return model.enabled && !context.interrupted && m_IsRadialBlur;
            }
        }

        public override CameraEvent GetCameraEvent()
        {
            return CameraEvent.AfterImageEffectsOpaque;
        }

        public override string GetName()
        {
            return "DummyMotionBlur";
        }

        public override void OnDisable()
        {
            if (m_AccumTexture != null)
            {
                UnityEngine.Object.Destroy(m_AccumTexture);
                m_AccumTexture = null;
            }

            model.enabled = false;
        }

        public void SetBlurState(RadialBlurType radialBlurType, float maxBlurDist, float toMaxTime, float toZeroTime, float strength)
        {
            if (radialBlurType == RadialBlurType.vary && this.m_RadialBlurType == RadialBlurType.enhance)
            {
                return;
            }
            this.m_RadialBlurType = radialBlurType;
            if (radialBlurType != RadialBlurType.reduce)
            {
                model.enabled = true;
                m_IsRadialBlur = true;
                m_IsIncreasing = true;
                m_BlurDist = 0;
                model.m_Settings.m_DurationTime = toMaxTime;
                model.m_Settings.m_toZeroTime = toZeroTime;
                model.m_Settings.m_MaxBlurDist = maxBlurDist;
                model.m_Settings.m_Strength = strength;
            }
            else
            {
                model.m_Settings.m_toZeroTime = toZeroTime;
            }
        }

        public void UpdateProcess()
        {
            if (m_IsRadialBlur)
            {
                if (m_IsIncreasing)
                {
                    if (m_BlurDist < model.m_Settings.m_MaxBlurDist)
                    {
                        m_BlurDist += (Time.deltaTime / model.m_Settings.m_DurationTime) * model.m_Settings.m_MaxBlurDist;
                    }
                    else
                    {
                        m_IsIncreasing = false;
                    }
                }
                else if (m_RadialBlurType == RadialBlurType.vary || m_RadialBlurType == RadialBlurType.reduce)
                {
                    m_BlurDist -= (Time.deltaTime / model.m_Settings.m_toZeroTime) * model.m_Settings.m_MaxBlurDist;
                }

                if (m_BlurDist <= 0)
                {
                    m_IsRadialBlur = false;
                    model.enabled = false;
                    m_IsIncreasing = true;
                    m_BlurDist = 0;
                }
            }
        }

        public override void PopulateCommandBuffer(CommandBuffer cb)
        {
            //if (!m_IsRadialBlur)
            //{
            //    Graphics.Blit(source, destination);
            //    return;
            //}

            // Create the accumulation texture
            if (m_AccumTexture == null || m_AccumTexture.width != context.width || m_AccumTexture.height != context.height)
            {
                Object.DestroyImmediate(m_AccumTexture);
                //m_AccumTexture.Release();
                m_AccumTexture = new RenderTexture(context.width / 2, context.height / 2, 0);
                m_AccumTexture.name = k_RTName_Accum;
                m_AccumTexture.hideFlags = HideFlags.HideAndDontSave;
                cb.Blit(BuiltinRenderTextureType.CameraTarget, m_AccumTexture);
            }

            // If Extra Blur is selected, downscale the texture to 4x4 smaller resolution.
            if (model.m_Settings.m_ExtraBlur)
            {
                RenderTexture blurbuffer = RenderTexture.GetTemporary(context.width / 4, context.height / 4, 0);
                m_AccumTexture.MarkRestoreExpected();
                cb.Blit(m_AccumTexture, blurbuffer);
                cb.Blit(blurbuffer, m_AccumTexture);
                RenderTexture.ReleaseTemporary(blurbuffer);
            }

            // Clamp the motion blur variable, so it can never leave permanent trails in the image
            //blurAmount = Mathf.Clamp( blurAmount, 0.0f, 0.92f );

            var motionBlurMaterial = context.materialFactory.Get(k_ShaderString);
            // Setup the texture and floating point values in the shader
            motionBlurMaterial.SetTexture("_MainTex", m_AccumTexture);
            motionBlurMaterial.SetFloat("_fSampleDist", m_BlurDist);
            motionBlurMaterial.SetFloat("_fSampleStrength", model.m_Settings.m_Strength);

            // We are accumulating motion over frames without clear/discard
            // by design, so silence any performance warnings from Unity
            m_AccumTexture.MarkRestoreExpected();

            // Render the image using the motion blur shader
            cb.Blit(BuiltinRenderTextureType.CameraTarget, m_AccumTexture, motionBlurMaterial);
            cb.Blit(m_AccumTexture, BuiltinRenderTextureType.CameraTarget);
        }
    }
}
