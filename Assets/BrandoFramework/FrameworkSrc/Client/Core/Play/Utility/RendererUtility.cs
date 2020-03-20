using UnityEngine;

namespace Client
{
    public class RendererUtility
    {
        #region Material

        //public static void ConfigureNewMaterialToMatchOld(Material newMat, Material original)
        //{
        //    if (original == null)
        //    {
        //        Debug.LogWarning("Original material is null, could not copy properties to " + newMat + ". Setting shader to " + newMat.shader);
        //        return;
        //    }
        //    newMat.shader = original.shader;
        //    newMat.CopyPropertiesFromMaterial(original);
        //    ShaderTextureProperty[] texPropertyNames = TextureCombinerPipeline.shaderTexPropertyNames;
        //    for (int j = 0; j < texPropertyNames.Length; j++)
        //    {
        //        Vector2 scale = Vector2.one;
        //        Vector2 offset = Vector2.zero;
        //        if (newMat.HasProperty(texPropertyNames[j].name))
        //        {
        //            newMat.SetTextureOffset(texPropertyNames[j].name, offset);
        //            newMat.SetTextureScale(texPropertyNames[j].name, scale);
        //        }
        //    }
        //}

        #endregion
    }
}