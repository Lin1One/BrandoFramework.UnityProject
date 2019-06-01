
using Common.PrefsData;
using Common.Utility;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace Common.Setting
{
    /// <summary>
    /// Yu整体解决方案自身配置。
    /// </summary>
    public class YuSetting : YuAbsSingleSetting<YuSetting>
    {
        [LabelText("解决方案跟目录路径")]
        [SerializeField]
        public string yuRootDir = "_Yu";

        public string YuRootDir => yuRootDir;
        public string YuRootFullDir => Application.dataPath + "/" + YuRootDir + "/";

        //[LabelText("解决方案程序集的拷贝目标目录")]
        //[SerializeField]
        //[FolderPath]
        //public string YuDLLUseDIr;

        //[LabelText("是否启用Protobuf序列化")]
        //[SerializeField]
        //public bool IsUseProtobuf;

        //[LabelText("是否自动生成资源Id脚本和数据")]
        //[SerializeField]
        //public bool IsAutoAssetId;

        //[LabelText("界面语言")] [SerializeField] public YuLanguageType LanguageType;

        //[BoxGroup("程序集同步")]
        //[LabelText("自动拷贝框架程序集目标目录")]
        //[Tooltip("该字段为空则默认不自动拷贝")]
        ////[FolderPath] 会自动被填充为相对路径因此禁用该特性。
        //public string CopyTargetDir;

        //[BoxGroup("程序集同步")]
        //[LabelText("需拷贝程序集的类库Id列表")]
        //[Tooltip("该字段为空则默认拷贝所有有更新的 Dll")]
        //public List<string> CopySrcIds = new List<string>()
        //{
        //    "YuCommon",
        //    "YuPlay",
        //    "ILRuntimeCore",
        //    "OdinPlay",
        //    "YuExcelPlay",
        //    "YuGUIPlay",
        //    "YuILRuntimePlay",
        //    "YuLegoUIPlay",
        //    "YuLogPlay",
        //    "YuSocketPlay",
        //    "YuProtobufPlay",
        //    "YuTweenPlay",

        //    // editor
        //    "OdinEditor",
        //    "YuExcelEditor",
        //    "YuLegoUIEditor",
        //    "YuLogEditor",
        //    "YuEditor",
        //    "YuTweenEditor",
        //    "YuEditorHub"
        //};

        //public static string LocalHttpDir => Application.dataPath + "/HotUpdate/";

        private List<string> assemblyIds;

        public List<string> AllAssemblyId
        {
            get
            {
                if (assemblyIds != null && assemblyIds.Count > 0)
                {
                    return assemblyIds;
                }

                if (!YuUnityUtility.HasYuSrc)
                {
                    var asmDefinePaths = YuIOUtility.GetPathsContainSonDir(Instance.YuRootFullDir + "YuDLL/")
                        .Where(p => p.EndsWith(".dll"));
                    assemblyIds = asmDefinePaths.Select(Path.GetFileNameWithoutExtension).ToList();
                    return assemblyIds;
                }
                else
                {
                    var asmDefinePaths = YuIOUtility.GetPathsContainSonDir(Instance.YuRootFullDir + "YuSrc/")
                        .Where(p => p.EndsWith(".asmdef"));
                    assemblyIds = asmDefinePaths.Select(Path.GetFileNameWithoutExtension).ToList();
                    return assemblyIds;
                }
            }
        }
    }
}