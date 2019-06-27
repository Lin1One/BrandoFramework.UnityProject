#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion

using Common.EditorWindow;
using Sirenix.OdinInspector.Editor;
using YuU3dOdinEditor;

namespace Client.DataTable.Editor
{
    //[YuMenuItemBuilderDesc(typeof(YuFeatureHub), "Excel工具菜单项构建器")]
    public class YuU3dExcelSpannerMenuItemBuilder : IYuMenuEditorWindowItemBuilder
    {
        public void BuildMenuItem(OdinMenuTree tree)
        {
            tree.Add("Excel工具", YuU3dExcelSpannerDati.GetSingleDati());
        }
    }
}