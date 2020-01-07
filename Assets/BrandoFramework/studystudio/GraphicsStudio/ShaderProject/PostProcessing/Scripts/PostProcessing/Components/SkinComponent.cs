using UnityEngine;

namespace GraphicsStudio.PostProcessing
{
    public sealed class SkinComponent : PostProcessingComponent<SkinModel>
    {
        static class Uniforms
        {
            internal static readonly int _SSSWidth = Shader.PropertyToID("_SSSWidth");
            internal static readonly int _Fov = Shader.PropertyToID("_Fov");
            internal static readonly int _CameraDepthTexID = Shader.PropertyToID("_CustomDepthTex");
        }

        const string k_ReplaceShaderName = "Unlit/CustomDepthOnly";
        const string k_DepthRTName = "_SkinDepthCameraRT";

        private CustomDepthCamera m_depthCamera = null;

        // 角色深度图
        private RenderTexture m_depthTex = null;

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

            //model.enabled = false;
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
                // 角色深度图
                int cullingLayer = LayerMask.GetMask("Player", "SafeArea");
                m_depthCamera.Init(mainCam, m_depthTex, k_ReplaceShaderName, "DepthReplaceTag", cullingLayer, false, -800);
            }

            return true;
        }

        public void Render(RenderTexture source, RenderTexture destination)
        {
            if (Camera.main == null || !CheckResource(Camera.main))
            {
                return;
            }

            var settings = model.settings;
            var blurMat = context.materialFactory.Get("sgws/SkinBlurShader");

            blurMat.EnableKeyword("SCENE_CHARACTER");

            blurMat.SetFloat(Uniforms._SSSWidth, settings.sssWidth);
            blurMat.SetFloat(Uniforms._Fov, context.camera.fieldOfView);
            blurMat.SetTexture(Uniforms._CameraDepthTexID, m_depthTex);

            RenderTexture tmpRT0 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            tmpRT0.filterMode = FilterMode.Point;
            tmpRT0.wrapMode = TextureWrapMode.Clamp;
            tmpRT0.name = "SkinPostProcessTmpRT0";

            Graphics.Blit(source, tmpRT0);
            Graphics.Blit(tmpRT0, source, blurMat, 0);
            RenderTexture.ReleaseTemporary(tmpRT0);

            RenderTexture tmpRT1 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            tmpRT1.filterMode = FilterMode.Point;
            tmpRT1.wrapMode = TextureWrapMode.Clamp;
            tmpRT1.name = "SkinPostProcessTmpRT1";

            Graphics.Blit(source, tmpRT1);
            Graphics.Blit(tmpRT1, source, blurMat, 1);
            RenderTexture.ReleaseTemporary(tmpRT1);

            blurMat.DisableKeyword("SCENE_CHARACTER");
        }
    }
}
