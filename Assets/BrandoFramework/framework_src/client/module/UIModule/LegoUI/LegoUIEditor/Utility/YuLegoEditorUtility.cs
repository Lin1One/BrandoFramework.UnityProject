using Common.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Client.LegoUI.Editor
{
    public static class YuLegoEditorUtility
    {
        public const string LEGO_VIEW = "LegoView";
        public const string LEGO_COMPONENT = "LegoComponent";

        public static List<string> GetLatestMetaIds()
        {
            ////var app = YuU3dAppSettingDati.CurrentActual;
            ////if (app == null)
            ////{
            ////    return null;
            ////}

            ////var metaDir = app.Helper.AssetDatabaseLegoMetaDir;
            ////var paths = IOUtility.GetPathsContainSonDir(metaDir, s => s.EndsWith(".txt"));
            ////var fileNames = paths.ToList().Select(Path.GetFileNameWithoutExtension).ToList();
            ////return fileNames;
            return null;
        }

        public static void AddUIMenus(GenericMenu menu, List<string> metaIds, int startIndex, string typeStr
            , GenericMenu.MenuFunction2 action)
        {
            int charCode = 65;

            for (int i = 0; i < 26; i++)
            {
                foreach (var metaId in metaIds)
                {
                    var c = metaId[startIndex];
                    if (c == charCode)
                    {
                        var menuStr = $"{typeStr}/{(char) charCode}/";
                        menu.AddItem(new GUIContent(menuStr + metaId), false, action, metaId);
                    }
                }

                charCode++;
            }
        }
    }
}