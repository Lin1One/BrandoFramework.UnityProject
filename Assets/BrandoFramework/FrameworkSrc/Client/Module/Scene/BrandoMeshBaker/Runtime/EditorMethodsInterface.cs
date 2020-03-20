using UnityEngine;
using System.Collections.Generic;

namespace GameWorld
{
    /// <summary>
    /// M b2_ texture combiner editor methods.
    /// Contains functionality such as changeing texture formats
    /// Which is only available in the editor. These methods have all been put in a
    /// class so that the UnityEditor namespace does not need to be included in any of the
    /// the runtime classes.
    /// texture ºÏ²¢±à¼­Æ÷º¯Êý
    /// </summary>
    public interface EditorMethodsInterface
    {
        void Clear();
        void RestoreReadFlagsAndFormats(ProgressUpdateDelegate progressInfo);
        void SetReadWriteFlag(Texture2D tx, bool isReadable, bool addToList);
        void AddTextureFormat(Texture2D tx, bool isNormalMap);
        void SaveAtlasToAssetDatabase(Texture2D atlas, ShaderTextureProperty texPropertyName, int atlasNum, Material resMat);
        //void SetMaterialTextureProperty(Material target, ShaderTextureProperty texPropName, string texturePath);
        //void SetNormalMap(Texture2D tx);
        bool IsNormalMap(Texture2D tx);
        string GetPlatformString();
        void SetTextureSize(Texture2D tx, int size);
        bool IsCompressed(Texture2D tx);
        void CheckBuildSettings(long estimatedAtlasSize);
        bool CheckPrefabTypes(ObjsToCombineTypes prefabType, List<GameObject> gos);
        bool ValidateSkinnedMeshes(List<GameObject> mom);
        void CommitChangesToAssets();
        void OnPreTextureBake();
        void OnPostTextureBake();
        //Needed because io.writeAllBytes does not exist in webplayer.
        void Destroy(UnityEngine.Object o);
    }
}
