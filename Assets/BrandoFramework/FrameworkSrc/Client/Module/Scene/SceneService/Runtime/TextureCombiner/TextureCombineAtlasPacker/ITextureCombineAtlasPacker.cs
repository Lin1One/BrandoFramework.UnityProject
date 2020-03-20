using GameWorld;
using System.Collections;
using UnityEngine;

namespace Client.Scene
{
    internal interface ITextureCombineAtlasPacker
    {
        /// <summary>
        /// 将纹理转换为可读格式
        /// </summary>
        /// <returns></returns>
        void ConvertTexturesToReadableFormats(
            TextureCombinePipelineData data,
            EditorMethodsInterface textureEditorMethods);

        /// <summary>
        /// 计算Atlas Rects
        /// </summary>
        /// <param name="data"></param>
        /// <param name="doMultiAtlas"></param>
        /// <returns></returns>
        AtlasPackingResult[] CalculateAtlasRectangles(TextureCombinePipelineData data, bool doMultiAtlas);

        /// <summary>
        /// 创建 Atlas
        /// </summary>
        /// <returns></returns>
        void CreateAtlases(TextureCombinePipelineData data,
            TextureCombineHandler combiner,
            AtlasPackingResult packedAtlasRects,
            Texture2D[] atlases, 
            EditorMethodsInterface textureEditorMethods);
    }
}
