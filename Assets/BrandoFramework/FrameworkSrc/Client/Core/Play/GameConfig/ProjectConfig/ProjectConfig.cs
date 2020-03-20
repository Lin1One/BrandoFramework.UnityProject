#region Head

// Author:            LinYuzhou
// Email:             

#endregion

using Common.PrefsData;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Client.Core
{
    [Serializable]
    public class ProjectInfoDati : GenericSingleDati<ProjectInfo, ProjectInfoDati>
    {
    }

    [Serializable]
    public class ProjectInfo
    {
        [LabelWidth(130)]
        [GUIColor( 0.1f,0.8f,0.1f)]
        [Title("��Ŀ��Ϣ����", titleAlignment: TitleAlignments.Centered)]
        [LabelText("��ǰ������Ŀ����")]
        public string DevelopProjectName;

        [BoxGroup("Dir")]
        [LabelWidth(130)]
        [FolderPath]
        [LabelText("��Ŀ��·��")]
        public string ProjectRootDir;

        [BoxGroup("Dir")]
        [LabelWidth(130)]
        [FolderPath]
        [LabelText("AssetdatabaseĿ¼")]
        public string CurrentProjectAssetDatabaseDirPath;

        [BoxGroup("Dir")]
        [LabelWidth(130)]
        [FolderPath]
        [LabelText("AssetBundle���Ŀ¼")]
        public string AssetBundleBuildDir;

        [BoxGroup("Script")]
        [LabelWidth(130)]
        [LabelText("��Ŀ����ʱ����·��")]
        [FilePath]
        public string ProjectRuntimeAssemblyPath;

        [BoxGroup("Script")]
        [LabelWidth(130)]
        [LabelText("��Ŀ����ʱ�ű������ռ�")]
        public string ProjectRuntimeScriptDefines;

        [BoxGroup("Script")]
        [LabelWidth(130)]
        [LabelText("��Ŀ�༭���������ռ�")]
        public string ProjectEditorScriptingDefines;

        [BoxGroup("Script")]
        [LabelWidth(130)]
        [LabelText("��ĿԤ����")]
        public string ProjectScriptingDefines;


    }
}

