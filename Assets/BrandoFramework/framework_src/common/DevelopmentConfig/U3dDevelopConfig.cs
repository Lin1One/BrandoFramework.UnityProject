#region Head

// Author:            LinYuzhou
// Email:             

#endregion

using UnityEngine;

namespace Common.Config
{
    public class U3dDevelopConfig 
    {
        private static U3dDevelopConfig config;
        public static U3dDevelopConfig Config
        {
            get
            {
                if(config == null)
                {
                    config = new U3dDevelopConfig();
                }
                return config;
            }
        }


        public string CurrentDevelopProjectName = "TestProject";
        public string AppRootDir = "TestProjects";

        public string CurrentProjectAssetDatabaseDirPath =>
            $"{Application.dataPath}/GameProjects/{CurrentDevelopProjectName}/AssetDatabase/";
    }
}

