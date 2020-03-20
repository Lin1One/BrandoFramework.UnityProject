#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using UnityEditor;
using UnityEngine;

namespace Common.Editor
{
    /// <summary>
    /// Project 窗口扩展类
    /// </summary>
    public  class ProjectWindowExtend 
    {
        #region 右键菜单扩展
        /// <summary>
        /// 复制选中资源的 Assets 路径
        /// </summary>
        [MenuItem("Assets/资源操作/路径/复制Assets路径", false)]
        public static void CopyAssetPath()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            TextEditor text2Editor = new TextEditor();
            text2Editor.text = path;
            text2Editor.OnFocus();
            text2Editor.Copy();
        }

        /// <summary>
        /// 复制选中资源的 Assets 路径
        /// </summary>
        [MenuItem("Assets/资源操作/路径/复制本地路径", false)]
        public static void CopyLocalPath()
        {
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            string projectFolder = Application.dataPath.Replace("Assets", "");
            TextEditor text2Editor = new TextEditor();
            text2Editor.text = projectFolder + assetPath;
            text2Editor.OnFocus();
            text2Editor.Copy();
        }

        #endregion
    }
}

