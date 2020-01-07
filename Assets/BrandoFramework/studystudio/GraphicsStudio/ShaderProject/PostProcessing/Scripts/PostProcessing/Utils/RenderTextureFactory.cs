using System;
using System.Collections.Generic;
using UnityEngine;

namespace GraphicsStudio.PostProcessing
{
    public sealed class RenderTextureFactory : IDisposable
    {
        HashSet<RenderTexture> m_TemporaryRTs;

        public RenderTextureFactory()
        {
            m_TemporaryRTs = new HashSet<RenderTexture>();
        }

        public RenderTexture Get(RenderTexture baseRenderTexture, string name)
        {
            return Get(
                baseRenderTexture.width,
                baseRenderTexture.height,
                baseRenderTexture.depth,
                baseRenderTexture.format,
                baseRenderTexture.sRGB ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Linear,
                baseRenderTexture.filterMode,
                baseRenderTexture.wrapMode, name
                );
        }

        public RenderTexture Get(int width, int height, int depthBuffer, RenderTextureFormat format, string name)
        {
            return Get(width, height, depthBuffer, format, RenderTextureReadWrite.Default, FilterMode.Bilinear, TextureWrapMode.Clamp, name);
        }

        public RenderTexture Get(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite rw, string name)
        {
            return Get(width, height, depthBuffer, format, rw, FilterMode.Bilinear, TextureWrapMode.Clamp, name);
        }

        public RenderTexture Get(int width, int height, int depthBuffer = 0, RenderTextureFormat format = RenderTextureFormat.Default, RenderTextureReadWrite rw = RenderTextureReadWrite.Default, FilterMode filterMode = FilterMode.Bilinear, TextureWrapMode wrapMode = TextureWrapMode.Clamp, string name = "FactoryTempTexture")
        {
            var rt = RenderTexture.GetTemporary(width, height, depthBuffer, format, rw); // add forgotten param rw
            rt.filterMode = filterMode;
            rt.wrapMode = wrapMode;
            rt.name = name;
            m_TemporaryRTs.Add(rt);
            return rt;
        }

        public void Release(RenderTexture rt)
        {
            if (rt == null)
                return;

            if (!m_TemporaryRTs.Contains(rt))
                throw new ArgumentException(string.Format("Attempting to remove a RenderTexture that was not allocated: {0}", rt));

            m_TemporaryRTs.Remove(rt);
            RenderTexture.ReleaseTemporary(rt);
        }

        public void ReleaseAll()
        {
            var enumerator = m_TemporaryRTs.GetEnumerator();
            while (enumerator.MoveNext())
                RenderTexture.ReleaseTemporary(enumerator.Current);

            m_TemporaryRTs.Clear();
        }

        public void Dispose()
        {
            ReleaseAll();
        }
    }
}
