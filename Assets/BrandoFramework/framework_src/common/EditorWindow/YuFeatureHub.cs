using Common.DataStruct;
using Common.ScriptCreate;
using DraftBorad;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;

namespace Common.EditorWindow
{
    [YuEditorWindowTitle("编辑器功能窗口")]
    [YuEditorWindowSize(500, 400)]
    public class EditorFunctionMenuWindow : YuAbsMenuEditorWindow<EditorFunctionMenuWindow>
    {
        [MenuItem("Brando/编辑器功能窗口")]
        private static void Open() => OpenWindow();

        protected override void BuildBeforeMenuItems(OdinMenuTree tree)
        {
            tree.Add("脚本创建器", YuScriptCreater.Instance,
                EditorIcons.Info);

            tree.Add("测试面板", new MethedInvokeBorad(),
                EditorIcons.SingleUser);
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