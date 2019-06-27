#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion

using Common.PrefsData;
using System;

namespace Client.DataTable.Editor
{
    [Serializable]
    [DatiInEditor]
    [YuDatiDesc(YuDatiSaveType.Single, typeof(DataTableEditorDati), "Excel工具")]
    public class DataTableEditorDati : GenericSingleDati<DataTableEditor,DataTableEditorDati>
    {
    }

}