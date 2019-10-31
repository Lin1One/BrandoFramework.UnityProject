#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 19:58:56
// Email:                 836045613@qq.com

#endregion


namespace Client.Core
{
    public enum LoadState : byte
    {
        NotLoad,
        Loaded,
        Loading,
        AddTask, //递归添加任务中
    }
}
