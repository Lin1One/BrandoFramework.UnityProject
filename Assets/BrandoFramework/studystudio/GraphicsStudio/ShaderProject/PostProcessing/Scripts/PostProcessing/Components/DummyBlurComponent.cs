/************************************************************
//     文件名      : DummyBlurPostprocess.cs
//     功能描述    : 保留之前版本的 BlurPostprocess.cs
//     负责人      : xufang
//     参考文档    : 无
//     创建日期    : 2019/06/20.
//     Copyright   : Copyright 2014-2017 EZFun Inc.
**************************************************************/

using UnityEngine;
using UnityEngine.Rendering;

namespace GraphicsStudio.PostProcessing
{
    public sealed class DummyBlurComponent : PostProcessingComponentCommandBuffer<DummyBlurModel>
    {
        public RenderTexture m_RenderTexture = null;

        private int m_OldHeight = 0;
        private int m_OldWidht = 0;
        private const float m_Speed = 5;
        public static float m_DownsampleMax = 2;
        private const float m_BlurSizeMax = 1;

        const string k_ShaderString = "Hidden/MobileBlur";

        public override bool active
        {
            get
            {
                return model.enabled
                       //&& model.settings.bloom.intensity > 0f
                       && !context.interrupted;
            }
        }

        public void UpdateProcess()
        {
            if ((model.m_Settings.m_Process >= 2))
            {
                return;
            }

            model.m_Settings.m_Process += Time.deltaTime * m_Speed;
            if (model.m_Settings.m_Process >= 1)
            {
                if (!model.m_Settings.m_IsForward)
                {
                    model.enabled = false;
                }
                else
                {
                    model.m_Settings.m_Process = 2;
                    model.m_Settings.m_Downsample = 2;
                    model.m_Settings.m_BlurSize = m_BlurSizeMax;
                }
            }
            if (model.m_Settings.m_IsForward)
            {
                //m_Settings.m_Downsample = (int)Mathf.Lerp(0, m_DownsampleMax, m_Process);
                model.m_Settings.m_BlurSize = Mathf.Lerp(0, m_BlurSizeMax, model.m_Settings.m_Process);
            }
            else
            {
                //m_Settings.m_Downsample = (int)Mathf.Lerp(m_DownsampleMax, 0, m_Process);
                model.m_Settings.m_BlurSize = Mathf.Lerp(m_BlurSizeMax, 0, model.m_Settings.m_Process);
            }
        }

        public override CameraEvent GetCameraEvent()
        {
            return CameraEvent.AfterImageEffectsOpaque;
        }

        public override string GetName()
        {
            return "DummyBlur";
        }

        public override void OnDisable()
        {
            if (m_RenderTexture != null)
            {
                UnityEngine.Object.Destroy(m_RenderTexture);
                m_RenderTexture = null;
            }

            model.enabled = false;
        }

        public override void PopulateCommandBuffer(CommandBuffer cb)
        {
            var blurMaterial = context.materialFactory.Get(k_ShaderString);

            float widthMod = 1.0f / (1.0f * (1 << model.m_Settings.m_Downsample));
            blurMaterial.SetVector("_Parameter", new Vector4(model.m_Settings.m_BlurSize * widthMod, -model.m_Settings.m_BlurSize * widthMod, 0.0f, 0.0f));
            //source.filterMode = FilterMode.Bilinear;

            int rtW = context.width >> model.m_Settings.m_Downsample;
            int rtH = context.height >> model.m_Settings.m_Downsample;

            //RenderTextureFormat format = m_PostprocessChain.m_DownsampleFormat;
            RenderTextureFormat format = RenderTextureFormat.Default;

            if (m_RenderTexture == null)
            {
                // downsample
                m_RenderTexture = new RenderTexture(rtW, rtH, 0, format);
                m_RenderTexture.filterMode = FilterMode.Bilinear;
                m_RenderTexture.Create();
                m_OldHeight = rtH;
                m_OldWidht = rtW;
            }
            if (m_OldWidht != rtW || m_OldHeight != rtH)
            {
                m_RenderTexture.Release();
                m_RenderTexture.height = rtH;
                m_RenderTexture.width = rtW;
                m_OldHeight = rtH;
                m_OldWidht = rtW;
                m_RenderTexture.Create();
            }

            m_RenderTexture.MarkRestoreExpected();
            cb.Blit(BuiltinRenderTextureType.CameraTarget, m_RenderTexture, blurMaterial, 0);
            //Graphics.Blit(source, m_RenderTexture, m_BlurMaterial, 0);

            var passOffs = model.m_Settings.m_BlurType == BlurType.StandardGauss ? 0 : 2;

            for (int i = 0; i < model.m_Settings.m_BlurIterations; i++)
            {
                float iterationOffs = (i * 1.0f);
                blurMaterial.SetVector("_Parameter", new Vector4(model.m_Settings.m_BlurSize * widthMod + iterationOffs, -model.m_Settings.m_BlurSize * widthMod - iterationOffs, 0.0f, 0.0f));

                // vertical blur
                RenderTexture rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, format);
                rt2.MarkRestoreExpected();
                rt2.filterMode = FilterMode.Bilinear;
                cb.Blit(m_RenderTexture, rt2, blurMaterial, 1 + passOffs);
                //Graphics.Blit(m_RenderTexture, rt2, m_BlurMaterial, 1 + passOffs);

                m_RenderTexture.MarkRestoreExpected();
                // horizontal blur
                cb.Blit(rt2, m_RenderTexture, blurMaterial, 2 + passOffs);
                //Graphics.Blit(rt2, m_RenderTexture, m_BlurMaterial, 2 + passOffs);
                RenderTexture.ReleaseTemporary(rt2);
            }

            cb.Blit(m_RenderTexture, BuiltinRenderTextureType.CameraTarget);
            //Graphics.Blit(m_RenderTexture, destination);
        }
    }
}
