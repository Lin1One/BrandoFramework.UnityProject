#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using UnityEditor;
using UnityEngine;

namespace Common.Editor
{
    /// <summary>
    /// Project ������չ��
    /// </summary>
    public  class ProjectWindowExtend 
    {
        #region �Ҽ��˵���չ
        /// <summary>
        /// ����ѡ����Դ�� Assets ·��
        /// </summary>
        [MenuItem("Assets/��Դ����/·��/����Assets·��", false)]
        public static void CopyAssetPath()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            TextEditor text2Editor = new TextEditor();
            text2Editor.text = path;
            text2Editor.OnFocus();
            text2Editor.Copy();
        }

        /// <summary>
        /// ����ѡ����Դ�� Assets ·��
        /// </summary>
        [MenuItem("Assets/��Դ����/·��/���Ʊ���·��", false)]
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

