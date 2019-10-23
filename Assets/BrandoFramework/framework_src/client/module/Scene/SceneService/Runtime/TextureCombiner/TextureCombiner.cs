using System.IO;
using UnityEditor;

namespace Client.Scene
{
    public class TextureCombiner
    {
        /// <summary>
        /// 创建合并材质数据资源
        /// </summary>
        public static void CreateCombinedMaterialInfoAsset(string pth)
        {
            //TextureCombineEntrance mom = (TextureCombineEntrance)target;
            string baseName = Path.GetFileNameWithoutExtension(pth);
            if (baseName == null || baseName.Length == 0)
                return;
            string folderPath = pth.Substring(0, pth.Length - baseName.Length - 6);

            //List<string> matNames = new List<string>();
            ////多材质
            //if (mom.doMultiMaterial)
            //{
            //    for (int i = 0; i < mom.resultMaterials.Length; i++)
            //    {
            //        matNames.Add(folderPath + baseName + "-mat" + i + ".mat");
            //        AssetDatabase.CreateAsset(new Material(Shader.Find("Diffuse")), matNames[i]);
            //        mom.resultMaterials[i].combinedMaterial = (Material)AssetDatabase.LoadAssetAtPath(matNames[i], typeof(Material));
            //    }
            //}
            //else
            //{
            //    matNames.Add(folderPath + baseName + "-mat.mat");
            //    Material newMat = null;
            //    if (mom.GetObjectsToCombine().Count > 0 && mom.GetObjectsToCombine()[0] != null)
            //    {
            //        Renderer r = mom.GetObjectsToCombine()[0].GetComponent<Renderer>();
            //        if (r == null)
            //        {
            //            Debug.LogWarning("Object " + mom.GetObjectsToCombine()[0] + " does not have a Renderer on it.");
            //        }
            //        else
            //        {
            //            if (r.sharedMaterial != null)
            //            {
            //                newMat = new Material(r.sharedMaterial);
            //                TextureCombineEntrance.ConfigureNewMaterialToMatchOld(newMat, r.sharedMaterial);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        Debug.Log("If you add objects to be combined before creating the Combined Material Assets. " +
            //            "Then Mesh Baker will create a result material that is a duplicate of the material on the first object to be combined. " +
            //            "This saves time configuring the shader.");
            //    }

            //    if (newMat == null)
            //    {
            //        newMat = new Material(Shader.Find("Diffuse"));
            //    }
            //    AssetDatabase.CreateAsset(newMat, matNames[0]);
            //    mom.resultMaterial = (Material)AssetDatabase.LoadAssetAtPath(matNames[0], typeof(Material));
            //}
            ////create the TextureBakeResults
            //AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TextureBakeResults>(), pth);
            //mom.textureBakeResults = (TextureBakeResults)AssetDatabase.LoadAssetAtPath(pth, typeof(TextureBakeResults));
            AssetDatabase.Refresh();
        }

    }
        

}