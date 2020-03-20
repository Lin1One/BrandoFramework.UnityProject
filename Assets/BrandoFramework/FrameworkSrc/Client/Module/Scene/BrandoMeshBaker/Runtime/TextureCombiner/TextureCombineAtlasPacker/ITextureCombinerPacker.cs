using System.Collections;
using UnityEngine;

namespace GameWorld
{
    internal interface ITextureCombinerPacker
    {
        /// <summary>
        /// 将纹理转换为可读格式
        /// </summary>
        /// <returns></returns>
        IEnumerator ConvertTexturesToReadableFormats(ProgressUpdateDelegate progressInfo,
            CombineTexturesIntoAtlasesCoroutineResult result,
            TexturePipelineData data,
            TextureCombineHandler combiner,
            EditorMethodsInterface textureEditorMethods);

        /// <summary>
        /// 计算Atlas Rects
        /// </summary>
        /// <param name="data"></param>
        /// <param name="doMultiAtlas"></param>
        /// <returns></returns>
        AtlasPackingResult[] CalculateAtlasRectangles(TexturePipelineData data, bool doMultiAtlas);

        /// <summary>
        /// 创建 Atlas
        /// </summary>
        /// <returns></returns>
        IEnumerator CreateAtlases(ProgressUpdateDelegate progressInfo,
            TexturePipelineData data,
            TextureCombineHandler combiner,
            AtlasPackingResult packedAtlasRects,
            Texture2D[] atlases, 
            EditorMethodsInterface textureEditorMethods);
    }
}
