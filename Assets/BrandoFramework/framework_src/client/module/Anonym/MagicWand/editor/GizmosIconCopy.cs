using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class GizmosIconCopy
{
    public const string resourcePath = "/Anonym/MagicWand/Resource";
    const string subPath = "/Gizmos";

    public static string FullResourcePath { get { return Application.dataPath + resourcePath; } }
    public static string FromPath { get { return FullResourcePath + subPath; } }

    static GizmosIconCopy()
    {
#if UNITY_2018_3_OR_NEWER
        return;
#else
        string toPath = Application.dataPath + "/Gizmos";
        string fileName;

        DirectoryInfo dirInfo = new DirectoryInfo(FromPath);
        var files = dirInfo.GetFiles();
        for (int i = 0; i < files.Length; ++i)
        {
            fileName = files[i].Name;
            string fileNameWithToPath = toPath + "/" + fileName;

            if (!Directory.Exists(toPath))
                Directory.CreateDirectory(toPath);

            if (File.Exists(fileNameWithToPath))
                continue;

            FileUtil.CopyFileOrDirectory(FromPath + "/" + fileName, fileNameWithToPath);
        }
#endif
    }
}
