using UnityEngine;

namespace GraphicsStudio.PostProcessing
{
    public sealed class SunShaftComponent : PostProcessingComponent<SunShaftModel>
    {
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

        void DrawBorder(RenderTexture dest, Material material)
        {
            float x1;
            float x2;
            float y1;
            float y2;

            RenderTexture.active = dest;
            bool invertY = true; // source.texelSize.y < 0.0f;
                                          // Set up the simple Matrix
            GL.PushMatrix();
            GL.LoadOrtho();

            for (int i = 0; i < material.passCount; i++)
            {
                material.SetPass(i);

                float y1_ , y2_ ;
                if (invertY)
                {
                    y1_ = 1.0f; y2_ = 0.0f;
                }
                else
                {
                    y1_ = 0.0f; y2_ = 1.0f;
                }

                // left	        
                x1 = 0.0f;
                x2 = 0.0f + 1.0f / (dest.width * 1.0f);
                y1 = 0.0f;
                y2 = 1.0f;
                GL.Begin(GL.QUADS);

                GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
                GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
                GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
                GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

                // right
                x1 = 1.0f - 1.0f / (dest.width * 1.0f);
                x2 = 1.0f;
                y1 = 0.0f;
                y2 = 1.0f;

                GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
                GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
                GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
                GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

                // top
                x1 = 0.0f;
                x2 = 1.0f;
                y1 = 0.0f;
                y2 = 0.0f + 1.0f / (dest.height * 1.0f);

                GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
                GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
                GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
                GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

                // bottom
                x1 = 0.0f;
                x2 = 1.0f;
                y1 = 1.0f - 1.0f / (dest.height * 1.0f);
                y2 = 1.0f;

                GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
                GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
                GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
                GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

                GL.End();
            }

            GL.PopMatrix();
        }

        public void Render(RenderTexture source, RenderTexture destination)
        {
            var settings = model.settings;
            var sunShaftsMaterial = context.materialFactory.Get("sgws/SunShaftsComposite");
            var simpleClearMaterial = context.materialFactory.Get("sgws/SimpleClear");

            // we actually need to check this every frame
            if (settings.useDepthTexture)
                context.camera.depthTextureMode |= DepthTextureMode.Depth;


            int divider = 4;
            if (settings.resolution == SunShaftModel.Resolution.Normal)
                divider = 2;
            else if (settings.resolution == SunShaftModel.Resolution.High)
                divider = 1;

            Vector3 v = settings.sunPos;
            v.z = 0.0f;

            int rtH = source.height / divider;
            int rtW = source.width / divider;

            RenderTexture lrColorB;
            RenderTexture lrDepthBuffer = RenderTexture.GetTemporary(rtW, rtH, 0);

            // mask out everything except the skybox
            // we have 2 methods, one of which requires depth buffer support, the other one is just comparing images

            sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(1.0f, 1.0f, 0.0f, 0.0f) * settings.sunShaftBlurRadius);
            sunShaftsMaterial.SetVector("_SunPosition", new Vector4(v.x, v.y, v.z, settings.maxRadius));
            sunShaftsMaterial.SetFloat("_NoSkyBoxMask", 1.0f - settings.useSkyBoxAlpha);

            if (!settings.useDepthTexture)
            {
                RenderTexture tmpBuffer  = RenderTexture.GetTemporary(source.width, source.height, 0);
                RenderTexture.active = tmpBuffer;
                GL.ClearWithSkybox(false, context.camera);

                sunShaftsMaterial.SetTexture("_Skybox", tmpBuffer);
                Graphics.Blit(source, lrDepthBuffer, sunShaftsMaterial, 3);
                RenderTexture.ReleaseTemporary(tmpBuffer);
            }
            else
            {
                Graphics.Blit(source, lrDepthBuffer, sunShaftsMaterial, 2);
            }

            // paint a small black small border to get rid of clamping problems
            DrawBorder(lrDepthBuffer, simpleClearMaterial);

            // radial blur:

            int radialBlurIterations = Mathf.Clamp(settings.radialBlurIterations, 1, 4);

            float ofs = settings.sunShaftBlurRadius * (1.0f / 768.0f);

            sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));
            sunShaftsMaterial.SetVector("_SunPosition", new Vector4(v.x, v.y, v.z, settings.maxRadius));

            for (int it2 = 0; it2 < radialBlurIterations; it2++ ) {
                // each iteration takes 2 * 6 samples
                // we update _BlurRadius each time to cheaply get a very smooth look

                lrColorB = RenderTexture.GetTemporary(rtW, rtH, 0);
                Graphics.Blit(lrDepthBuffer, lrColorB, sunShaftsMaterial, 1);
                RenderTexture.ReleaseTemporary(lrDepthBuffer);
                ofs = settings.sunShaftBlurRadius * (((it2 * 2.0f + 1.0f) * 6.0f)) / 768.0f;
                sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));

                lrDepthBuffer = RenderTexture.GetTemporary(rtW, rtH, 0);
                Graphics.Blit(lrColorB, lrDepthBuffer, sunShaftsMaterial, 1);
                RenderTexture.ReleaseTemporary(lrColorB);
                ofs = settings.sunShaftBlurRadius * (((it2 * 2.0f + 2.0f) * 6.0f)) / 768.0f;
                sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));
            }

            // put together:

            if (v.z >= 0.0)
                sunShaftsMaterial.SetVector("_SunColor", new Vector4(settings.sunColor.r, settings.sunColor.g, settings.sunColor.b, settings.sunColor.a) * settings.sunShaftIntensity);
            else
                sunShaftsMaterial.SetVector("_SunColor", Vector4.zero); // no backprojection !
            sunShaftsMaterial.SetTexture("_ColorBuffer", lrDepthBuffer);
            Graphics.Blit(source, destination, sunShaftsMaterial, (settings.blendMode == SunShaftModel.BlendMode.Screen) ? 0 : 4);

            RenderTexture.ReleaseTemporary(lrDepthBuffer);
        }

    }
}

