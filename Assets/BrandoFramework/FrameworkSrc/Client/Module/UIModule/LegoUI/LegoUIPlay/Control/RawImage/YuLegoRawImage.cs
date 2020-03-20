#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using Common;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Client.LegoUI
{
    /// <summary>
    /// If you don't have or don't wish to create an atlas, you can simply use this script to draw a texture.
    /// Keep in mind though that this will create an extra draw call with each RawImage present, so it's
    /// best to use it only for backgrounds or temporary visible graphics.
    /// </summary>
    [AddComponentMenu("Yu/LegoUI/YuLego Raw Image", 3)]
    public class YuLegoRawImage :
        YuAbsLegoMaskableGraphic,
        IYuLegoRawImage
    {
        #region UGuiSrc

        [FormerlySerializedAs("m_Tex")]
        [SerializeField]
        Texture m_Texture;

        [SerializeField] Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

        protected YuLegoRawImage()
        {
            useLegacyMeshGeneration = false;
        }

        /// <summary>
        /// Returns the texture used to draw this Graphic.
        /// </summary>
        public override Texture mainTexture
        {
            get
            {
                if (m_Texture == null)
                {
                    if (material != null && material.mainTexture != null)
                    {
                        return material.mainTexture;
                    }

                    return s_WhiteTexture;
                }

                return m_Texture;
            }
        }

        /// <summary>
        /// Texture to be used.
        /// </summary>
        public Texture texture
        {
            get { return m_Texture; }
            set
            {
                if (m_Texture == value)
                    return;

                m_Texture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// UV rectangle used by the texture.
        /// </summary>
        public Rect uvRect
        {
            get { return m_UVRect; }
            set
            {
                if (m_UVRect == value)
                    return;
                m_UVRect = value;
                SetVerticesDirty();
            }
        }

        /// <summary>
        /// Adjust the scale of the Graphic to make it pixel-perfect.
        /// </summary>
        public override void SetNativeSize()
        {
            Texture tex = mainTexture;
            if (tex != null)
            {
                int w = Mathf.RoundToInt(tex.width * uvRect.width);
                int h = Mathf.RoundToInt(tex.height * uvRect.height);
                rectTransform.anchorMax = rectTransform.anchorMin;
                rectTransform.sizeDelta = new Vector2(w, h);
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            Texture tex = mainTexture;
            vh.Clear();
            if (tex != null)
            {
                var r = GetPixelAdjustedRect();
                var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);
                var scaleX = tex.width * tex.texelSize.x;
                var scaleY = tex.height * tex.texelSize.y;
                {
                    var color32 = color;
                    vh.AddVert(new Vector3(v.x, v.y), color32,
                        new Vector2(m_UVRect.xMin * scaleX, m_UVRect.yMin * scaleY));
                    vh.AddVert(new Vector3(v.x, v.w), color32,
                        new Vector2(m_UVRect.xMin * scaleX, m_UVRect.yMax * scaleY));
                    vh.AddVert(new Vector3(v.z, v.w), color32,
                        new Vector2(m_UVRect.xMax * scaleX, m_UVRect.yMax * scaleY));
                    vh.AddVert(new Vector3(v.z, v.y), color32,
                        new Vector2(m_UVRect.xMax * scaleX, m_UVRect.yMin * scaleY));

                    vh.AddTriangle(0, 1, 2);
                    vh.AddTriangle(2, 3, 0);
                }
            }
        }

        #endregion

        #region lego

        private Texture2D tex2D;

        public string TextureId
        {
            get => texture.name;
            set
            {

                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                RwRouter.WaitTexture(value, (tex) =>
                {
                    texture = tex;
                });
            }
        }

        private static YuLegoTextureRouter rwRouter;

        private static YuLegoTextureRouter RwRouter
        {
            get
            {
                if (rwRouter != null)
                {
                    return rwRouter;
                }

                rwRouter = Injector.Instance.Get<YuLegoTextureRouter>();
                return rwRouter;
            }
        }

        #endregion

        #region 元数据变形

        public override void Metamorphose(LegoUIMeta uiMeta)
        {
            MetamorphoseStage = LegoMetamorphoseStage.Metamorphosing;

            var rawImageMeta = uiMeta.NextRawImage;
            RectMeta = uiMeta.CurrentRect;
            YuLegoUtility.MetamorphoseRect(RectTransform, RectMeta);
            Metamorphose(rawImageMeta);

            MetamorphoseStage = LegoMetamorphoseStage.Completed;
        }

        public void Metamorphose(LegoRawImageMeta rawImageMeta)
        {
            TryLoadTexture(rawImageMeta);
            color = rawImageMeta.LegoColor.ToColor();
            raycastTarget = rawImageMeta.RaycastTarget;
        }

        private void TryLoadTexture(LegoRawImageMeta rawImageMeta)
        {
            var textureId = rawImageMeta.TextureId;
            if (string.IsNullOrEmpty(textureId))
            {
                return;
            }

            TextureId = textureId;
        }

        #endregion

        #region 数据响应

        public void ReceiveTextureChange(string newValue)
        {
            if (string.IsNullOrEmpty(newValue))
            {
                transform.gameObject.SetActive(false);
                return;
            }
            TextureId = newValue;
        }

        #endregion
    }
}