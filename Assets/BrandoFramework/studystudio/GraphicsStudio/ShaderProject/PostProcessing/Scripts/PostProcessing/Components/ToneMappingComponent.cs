using UnityEngine;

namespace GraphicsStudio.PostProcessing
{
    public sealed class ToneMappingComponent : PostProcessingComponent<ToneMappingModel>
    {
        private RenderTexture rt = null;
        private RenderTextureFormat rtFormat;

        public override bool active
        {
            get
            {
                return model.enabled
                        && !context.interrupted;
            }
        }

        public override void OnEnable()
        {
            if (CheckSupport())
            {
                context.camera.allowHDR = true;
            }
        }

        public override void OnDisable()
        {
            model.enabled = false;

            if (rt)
            {
                Object.DestroyImmediate(rt);
                rt = null;
            }
        }

        private bool CheckSupport()
        {
            bool supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
            return supportHDRTextures;
        }

        bool CreateInternalRenderTexture()
        {
		    if (rt)
			    return false;

            rtFormat = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGHalf)? RenderTextureFormat.RGHalf : RenderTextureFormat.ARGBHalf;
            rt = new RenderTexture(1, 1, 0, rtFormat);
            rt.hideFlags = HideFlags.DontSave;		
		    return true;
	    }

        public void Render(RenderTexture source, RenderTexture destination)
        {
            if (!CheckSupport())
            {
                Graphics.Blit(source, destination);
                Debug.LogWarning( "Tonemapping has been disabled as it's not supported on the current platform.");
                return;
            }

            var settings = model.settings;
            var tonemapMaterial = context.materialFactory.Get("sgws/Tonemapper");

            // still here? 
            // =>  adaptive tone mapping:
            // builds an average log luminance, tonemaps according to 
            // middle grey and white values (user controlled)

            // AdaptiveReinhardAutoWhite will calculate white value automagically

            bool freshlyBrewedInternalRt = CreateInternalRenderTexture(); // this retrieves rtFormat, so should happen before rt allocations

            RenderTexture rtSquared  = RenderTexture.GetTemporary(settings.textureSize, settings.textureSize, 0, source.format);
            Graphics.Blit(source, rtSquared);

            int downsample = (int)Mathf.Log(rtSquared.width * 1.0f, 2.0f);

            if (downsample < 2)
            {
                Graphics.Blit(source, destination);
                return;
            }

            int div = 2;
            RenderTexture[] rts = new RenderTexture[downsample];
            for (int i = 0; i < downsample; i++) {
                rts[i] = RenderTexture.GetTemporary(rtSquared.width / div, rtSquared.width / div, 0, rtFormat);
                div *= 2;
            }

            // downsample pyramid

            var lumRt = rts[downsample - 1];
            Graphics.Blit(rtSquared, rts[0], tonemapMaterial, 1);
            if (settings.method == ToneMappingModel.Method.AutoWhite)
            {
                for (int i = 0; i < downsample - 1; i++)
                {
                    Graphics.Blit(rts[i], rts[i + 1], tonemapMaterial, 9);
                    lumRt = rts[i + 1];
                }
            }
            else if (settings.method == ToneMappingModel.Method.ReinHard)
            {
                for (int i = 0; i < downsample - 1; i++)
                {
                    Graphics.Blit(rts[i], rts[i + 1]);
                    lumRt = rts[i + 1];
                }
            }

            // we have the needed values, let's apply adaptive tonemapping

            float adaptionSpeed = settings.adaptionSpeed < 0.001f ? 0.001f : settings.adaptionSpeed;
            tonemapMaterial.SetFloat("_AdaptionSpeed", adaptionSpeed);

            rt.MarkRestoreExpected(); // keeping luminance values between frames, RT restore expected

#if UNITY_EDITOR
            if (Application.isPlaying && !freshlyBrewedInternalRt)
                Graphics.Blit(lumRt, rt, tonemapMaterial, 2);
            else
                Graphics.Blit(lumRt, rt, tonemapMaterial, 3);
#else
			Graphics.Blit (lumRt, rt, tonemapMaterial, freshlyBrewedInternalRt ? 3 : 2); 	
#endif

            float middleGrey = settings.middleGray < 0.001f ? 0.001f : settings.middleGray;
            tonemapMaterial.SetVector("_HdrParams", new Vector4(middleGrey, middleGrey, middleGrey, settings.white * settings.white));
            tonemapMaterial.SetTexture("_SmallTex", rt);
            if (settings.method == ToneMappingModel.Method.ReinHard)
            {
                Graphics.Blit(source, destination, tonemapMaterial, 0);
            }
            else if (settings.method == ToneMappingModel.Method.AutoWhite)
            {
                Graphics.Blit(source, destination, tonemapMaterial, 10);
            }
            else
            {
                Debug.LogError("No valid adaptive tonemapper type found!");
                Graphics.Blit(source, destination); // at least we get the TransformToLDR effect
            }

            // cleanup for adaptive

            for (int i = 0; i < downsample; i++)
            {
                RenderTexture.ReleaseTemporary(rts[i]);
            }
            RenderTexture.ReleaseTemporary(rtSquared);
        }
    }
}
