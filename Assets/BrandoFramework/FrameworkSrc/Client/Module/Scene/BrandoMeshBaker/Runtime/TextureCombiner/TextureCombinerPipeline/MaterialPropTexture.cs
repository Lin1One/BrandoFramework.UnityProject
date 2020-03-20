using System;
using UnityEngine;

namespace GameWorld
{
    /*
     Like a material but also stores its tiling info since the same texture
     with different tiling may need to be baked to a separate spot in the atlas
     note that it is sometimes possible for textures with different tiling to share an atlas rectangle
     To accomplish this need to store:
             uvTiling per TexSet (can be set to 0,0,1,1 by pushing tiling down into material tiling)
             matTiling per MeshBakerMaterialTexture (this is the total tiling baked into the atlas)
             matSubrectInFullSamplingRect per material (a MeshBakerMaterialTexture can be used by multiple materials. This is the subrect in the atlas)
     Normally UVTilings is applied first then material tiling after. This is difficult for us to use when baking meshes. It is better to apply material
     tiling first then UV Tiling. There is a transform for modifying the material tiling to handle this.
     once the material tiling is applied first then the uvTiling can be pushed down into the material tiling.

         Also note that this can wrap a procedural texture. The procedural texture is converted to a Texture2D in Step2 NOT BEFORE. This is important so that can
         build packing layout quickly. 

             Should always check if texture is null using 'isNull' function since Texture2D could be null but ProceduralTexture not
             Should not call GetTexture2D before procedural textures are created

         there will be one of these per material texture property (maintex, bump etc...)
     */
    //不同 tiling 的相同的 Texture 将被划分到 Atlas 不同位置
    //要实现此需要存储：
    //    每个TexSet的uvTiling（可以通过将平铺向下推到材料平铺中来将其设置为0、0、1、1）
    //    每个MeshBakerMaterialTexture的matTiling（这是烘焙到Atlas 中的总tiling）
    //    每个材质的matSubrectInFullSamplingRect（MeshBakerMaterialTexture可以由多种材质使用。这是图集中的子矩形）
    //先处理材质 tiling ，然后进行UV Tiling 

    //另请注意，这可以包装过程纹理。之前在步骤2中将过程纹理转换为Texture2D。这很重要，这样可以
    //         快速建立包装布局。
    //应该始终使用'isNull'函数检查texture是否为null，因为Texture2D可以为null，但ProceduralTexture不可以
    //在创建过程纹理之前不应调用GetTexture2D
    public class MaterialPropTexture
    {
        private Texture2D _t;

        public Texture2D t
        {
            set { _t = value; }
        }

        internal static bool readyToBuildAtlases = false;

        //像素密度
        public float texelDensity;

        /// <summary>
        /// 采样矩形，包括材料tiling 和uv tiling 。 大多数时候，这是与maintex，bumpmap等相同，
        /// 但是不必如此。 可以有一个主texture与另一个拼贴和凹凸贴图。 如果所有属性都相同，则可以合并这些属性
        /// </summary>
        private DRect encapsulatingSamplingRect;
        public DRect GetEncapsulatingSamplingRect()
        {
            return encapsulatingSamplingRect;
        }

        public void SetEncapsulatingSamplingRect(MaterialPropTexturesSet ts, DRect r)
        {
            encapsulatingSamplingRect = r;
        }
        /// <summary>

        ///重要提示：有两个material Tiling Rect。 这些都是存储合并后材质纹理属性的。
        ///用于bake Atlas，而不用于将材质映射到Atlas和变换UV。
        ///material tiling 用于纹理，对于不同的属性，这些属性可能不同：maintex，bumpmap等。
        ///如果所有属性都相同，则可以合并MB_TexSets。
        /// </summary>
        public DRect matTilingRect { get; private set; }

        public MaterialPropTexture() { }
        public MaterialPropTexture(Texture tx)
        {
            if (tx is Texture2D)
            {
                _t = (Texture2D)tx;
            }
            else
            {
                Debug.LogError("Texture must be Texture2D " + tx);
            }
        }

        public MaterialPropTexture(Texture tx, Vector2 matTilingOffset, Vector2 matTilingScale, float texelDens)
        {
            if (tx is Texture2D)
            {
                _t = (Texture2D)tx;
            }
            else
            {
                Debug.LogError("Texture must be Texture2D " + tx);
            }
            matTilingRect = new DRect(matTilingOffset, matTilingScale);
            texelDensity = texelDens;
        }

        //This should never be called until we are readyToBuildAtlases
        public Texture2D GetTexture2D()
        {
            if (!readyToBuildAtlases)
            {
                Debug.LogError("This function should not be called before Step3. For steps 1 and 2 should always call methods like isNull, width, height");
                throw new Exception("GetTexture2D called before ready to build atlases");
            }
            return _t;
        }

        public bool isNull
        {
            get { return _t == null; }
        }

        public int width
        {
            get
            {
                if (_t != null)
                    return _t.width;
                throw new Exception("Texture was null. can't get width");
            }
        }

        public int height
        {
            get
            {
                if (_t != null)
                    return _t.height;
                throw new Exception("Texture was null. can't get height");
            }
        }
        public string GetTexName()
        {
            if (_t != null)
                return _t.name;
            return "null";
        }

        public bool AreTexturesEqual(MaterialPropTexture b)
        {
            if (_t == b._t ) return true;
            return false;
        }
    }
    
}
