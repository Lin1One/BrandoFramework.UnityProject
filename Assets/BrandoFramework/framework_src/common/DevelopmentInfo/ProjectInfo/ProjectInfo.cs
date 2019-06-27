#region Head

// Author:            LinYuzhou
// Email:             

#endregion

using Common.PrefsData;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Common.Config
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
        [Title("项目信息配置", titleAlignment: TitleAlignments.Centered)]
        [LabelText("当前开发项目名称")]
        public string CurrentDevelopProjectName;

        [BoxGroup("Dir")]
        [LabelWidth(130)]
        [FolderPath]
        [LabelText("项目根路径")]
        public string ProjectRootDir;

        [BoxGroup("Dir")]
        [LabelWidth(130)]
        [FolderPath]
        [LabelText("项目Assetdatabase目录")]
        public string CurrentProjectAssetDatabaseDirPath;

        [BoxGroup("Script")]
        [LabelWidth(130)]
        [LabelText("项目运行时脚本命名空间")]
        public string ProjectRuntimeScriptDefines;

        [BoxGroup("Script")]
        [LabelWidth(130)]
        [LabelText("项目编辑环境命名空间")]
        public string ProjectEditorScriptingDefines;

        [BoxGroup("Script")]
        [LabelWidth(130)]
        [LabelText("项目预编译")]
        public string ProjectScriptingDefines;


    }
}

