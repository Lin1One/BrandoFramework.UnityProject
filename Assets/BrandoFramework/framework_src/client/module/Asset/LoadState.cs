#region Head

// Author:                liuruoyu1981
// CreateDate:            5/17/2019 12:40:39 PM
// Email:                 liuruoyu1981@gmail.com

#endregion


namespace Client.Assets
{
    public enum LoadState : byte
    {
        NotLoad,
        Loaded,
        Loading,
        AddTask, //递归添加任务中
    }
}
