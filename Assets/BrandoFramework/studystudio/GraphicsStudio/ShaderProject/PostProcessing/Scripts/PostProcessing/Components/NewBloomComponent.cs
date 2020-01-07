using UnityEngine;

namespace GraphicsStudio.PostProcessing
{
    public sealed class NewBloomComponent : PostProcessingComponent<NewBloomModel>
    {
        bool doHdr = false;
        Material brightPassFilterMaterial;
        Material screenBlend;
        Material separableBlurMaterial;

        public override bool active
        {
            get
            {
                return model.enabled
                        && !context.interrupted;
            }
        }

        public override void OnDisable()
        {
            model.enabled = false;
        }

        void BrightFilter(float thresh, float useAlphaAsMask, RenderTexture from , RenderTexture to)
        {
            if (doHdr)
                brightPassFilterMaterial.SetVector("threshhold", new Vector4(thresh, 1.0f, 0.0f, 0.0f));
            else
                brightPassFilterMaterial.SetVector("threshhold", new Vector4(thresh, 1.0f / (1.0f - thresh), 0.0f, 0.0f));
            brightPassFilterMaterial.SetFloat("useSrcAlphaAsMask", useAlphaAsMask);
            Graphics.Blit(from, to, brightPassFilterMaterial);
        }

        public void Render(RenderTexture source, RenderTexture destination)
        {
            var settings = model.settings;

            brightPassFilterMaterial = context.materialFactory.Get("sgws/BrightPassFilterForBloom");
            screenBlend = context.materialFactory.Get("sgws/BlendForBloom");
            separableBlurMaterial = context.materialFactory.Get("sgws/SeparableBlurPlus");

            doHdr = false;
            if (settings.hdr == NewBloomModel.HDRBloomMode.Auto)
                doHdr = source.format == RenderTextureFormat.ARGBHalf && context.camera.allowHDR;
            else
            {
                doHdr = settings.hdr == NewBloomModel.HDRBloomMode.On;
            }

            bool supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
            doHdr = doHdr && supportHDRTextures;

            NewBloomModel.BloomScreenBlendMode realBlendMode = settings.screenBlendMode;
            if (doHdr)
                realBlendMode = NewBloomModel.BloomScreenBlendMode.Add;


            var rtFormat = (doHdr) ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.Default;
            RenderTexture halfRezColor = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, rtFormat);
            RenderTexture quarterRezColor = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, rtFormat);
            RenderTexture secondQuarterRezColor = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, rtFormat);
            RenderTexture thirdQuarterRezColor = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, rtFormat);

            float widthOverHeight = (1.0f * source.width) / (1.0f * source.height);
            float oneOverBaseSize = 1.0f / 512.0f;

            // downsample

            Graphics.Blit(source, halfRezColor, screenBlend, 2); // <- 2 is stable downsample
            Graphics.Blit(halfRezColor, quarterRezColor, screenBlend, 2); // <- 2 is stable downsample

            RenderTexture.ReleaseTemporary(halfRezColor);

            // cut colors (threshholding)

            BrightFilter(settings.bloomThreshhold, settings.useSrcAlphaAsMask, quarterRezColor, secondQuarterRezColor);
            quarterRezColor.DiscardContents();

            // blurring

            if (settings.bloomBlurIterations < 1) settings.bloomBlurIterations = 1;

            for (int iter = 0; iter < settings.bloomBlurIterations; iter++ ) {
                float spreadForPass = (1.0f + (iter * 0.5f)) * settings.sepBlurSpread;
                separableBlurMaterial.SetVector("offsets", new Vector4(0.0f, spreadForPass * oneOverBaseSize, 0.0f, 0.0f));

                RenderTexture src = iter == 0 ? secondQuarterRezColor : quarterRezColor;
                Graphics.Blit(src, thirdQuarterRezColor, separableBlurMaterial);
                src.DiscardContents();

                separableBlurMaterial.SetVector("offsets", new Vector4((spreadForPass / widthOverHeight) * oneOverBaseSize, 0.0f, 0.0f, 0.0f));
                Graphics.Blit(thirdQuarterRezColor, quarterRezColor, separableBlurMaterial);
                thirdQuarterRezColor.DiscardContents();
            }

            // screen blend bloom results to color buffer
            screenBlend.SetFloat("_Intensity", settings.bloomIntensity);
            screenBlend.SetTexture("_ColorBuffer", source);
            Graphics.Blit(quarterRezColor, destination, screenBlend, (int)realBlendMode);

            RenderTexture.ReleaseTemporary(quarterRezColor);
            RenderTexture.ReleaseTemporary(secondQuarterRezColor);
            RenderTexture.ReleaseTemporary(thirdQuarterRezColor);
        }

    }
}

