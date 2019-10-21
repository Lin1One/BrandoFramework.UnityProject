using Client.Assets.Editor;
using Client.DataTable.Editor;
using Client.GamePlaying.AI;
using Client.Scene.Editor;
using Common.Config;
using Common.DataStruct;
using Common.EditorWindow;
using Common.ScriptCreate;
using DraftBorad;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;

namespace Client.Core.Editor
{
    [YuEditorWindowTitle("编辑器功能窗口")]
    [YuEditorWindowSize(500, 400)]
    public class EditorFunctionMenuWindow : YuAbsMenuEditorWindow<EditorFunctionMenuWindow>
    {
        [MenuItem("Brando/编辑器功能窗口")]
        private static void Open() => OpenWindow();

        protected override void BuildBeforeMenuItems(OdinMenuTree tree)
        {
            tree.Add("开发者信息", DeveloperInfoDati.GetSingleDati(), EditorIcons.SingleUser);
            tree.Add("项目信息", ProjectInfoDati.GetSingleDati(), EditorIcons.UnityLogo);
            tree.Add("脚本创建器", ScriptCreaterDati.GetSingleDati(),EditorIcons.Info);
            tree.Add("数据表", DataTableEditorDati.GetSingleDati(),EditorIcons.Table);
            tree.Add("AssetBundle", AssetBundleEditorDati.GetSingleDati(), EditorIcons.File);
            tree.Add("场景编辑", SceneEditorDati.GetSingleDati(), EditorIcons.LightBulb);
            tree.Add("测试面板", new MethedInvokeBorad(),EditorIcons.SingleUser);
            tree.Add("行为树", BehaviorTreeVisualization.Instance,EditorIcons.Tree);
            //tree.Add("核心配置", YuU3dCoreSettingDati.GetSingleDati(),
            //    EditorIcons.SettingsCog, YuMenuItemSetting.CommonMenuItemSetting);

            //tree.Add("自定义程序集", YuU3dAssemblyDefineDati.GetSingleDati(),
            //    EditorIcons.CloudsThunder, YuMenuItemSetting.CommonMenuItemSetting);
            //tree.Add("Prefs工具", new YuU3dPrefsSpanner(),
            //    EditorIcons.DayCalendar, YuMenuItemSetting.CommonMenuItemSetting);
        }

        public static void OpenItem(string builderId)
        {
            var itemIndex = Instance.Builders.FindIndex(b => b.TypeId() == builderId);
            OpenItem(2 + itemIndex);
        }

        public static void OpenItem(int index)
        {
            var instance = OpenWindow();
            instance.SwitchToTargetItem(index);
        }
    }
}