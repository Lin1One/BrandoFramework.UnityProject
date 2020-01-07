using UnityEngine;

namespace GraphicsStudio.PostProcessing
{
    public sealed class GlowComponent : PostProcessingComponent<GlowModel>
    {
        Material compositeMaterial = null;
        Material blurMaterial = null;
        Material downsampleMaterial = null;

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

        // Performs one blur iteration.
        public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
        {
            var settings = model.settings;
            float off = 0.5f + iteration * settings.blurSpread;
            Graphics.BlitMultiTap(source, dest, blurMaterial,
                new Vector2(off, off),
                new Vector2(-off, off),
                new Vector2(off, -off),
                new Vector2(-off, -off)
            );
        }

        // Downsamples the texture to a quarter resolution.
        private void DownSample4x(RenderTexture source, RenderTexture dest)
        {
            var settings = model.settings;
            downsampleMaterial.color = new Color(settings.glowTint.r, settings.glowTint.g, settings.glowTint.b, settings.glowTint.a / 4.0f);
            Graphics.Blit(source, dest, downsampleMaterial);
        }

        public void BlitGlow(RenderTexture source, RenderTexture dest)
        {
            var settings = model.settings;
            compositeMaterial.color = new Color(1F, 1F, 1F, Mathf.Clamp01(settings.glowIntensity));
            Graphics.Blit(source, dest, compositeMaterial);
        }

        public void Render(RenderTexture source, RenderTexture destination)
        {
            var settings = model.settings;

            compositeMaterial = context.materialFactory.Get("sgws/GlowCompose");
            downsampleMaterial = context.materialFactory.Get("sgws/Glow Downsample");
            blurMaterial = context.materialFactory.Get("sgws/GlowConeTap");

            // Clamp parameters to sane values
            float glowIntensity = Mathf.Clamp(settings.glowIntensity, 0.0f, 10.0f);
            float blurIterations = Mathf.Clamp(settings.blurIterations, 0, 30);
            float blurSpread = Mathf.Clamp(settings.blurSpread, 0.5f, 1.0f);

            int rtW = source.width / 4;
            int rtH = source.height / 4;
            RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);

            // Copy source to the 4x4 smaller texture.
            DownSample4x(source, buffer);

            // Blur the small texture
            float extraBlurBoost = Mathf.Clamp01((glowIntensity - 1.0f) / 4.0f);
            blurMaterial.color = new Color(1F, 1F, 1F, 0.25f + extraBlurBoost);

            for (int i = 0; i < blurIterations; i++)
            {
                RenderTexture buffer2 = RenderTexture.GetTemporary(rtW, rtH, 0);
                FourTapCone(buffer, buffer2, i);
                RenderTexture.ReleaseTemporary(buffer);
                buffer = buffer2;
            }
            Graphics.Blit(source, destination);

            BlitGlow(buffer, destination);

            RenderTexture.ReleaseTemporary(buffer);
        }

    }
}

