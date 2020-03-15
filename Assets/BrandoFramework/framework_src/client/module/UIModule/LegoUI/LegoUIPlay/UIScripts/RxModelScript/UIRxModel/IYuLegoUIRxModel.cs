#region Head

// Author:            Yu
// CreateDate:        2018/8/28 21:19:24
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System.Collections.Generic;
using YuU3dPlay;

namespace Client.LegoUI
{
    public interface IYuLegoUIRxModel : IRelease
    {
        T GetControlRxModel<T>(string id) where T : class, IRelease;

        Dictionary<string, object> BaseModels { get; }

        Dictionary<string, IYuLegoUIRxModel> SonComponentModels { get; }

        void InitRxModel();

        /// <summary>
        /// 模型所绑定的 LegoUI 
        /// </summary>
        ILegoUI MapUI { get; set; }

        /// <summary>
        /// 将自身持有的所有数据模型实例的值都拷贝为目标数据模型实例的值。
        /// </summary>
        /// <param name="target"></param>
        void Copy(IYuLegoUIRxModel target);

        void CopyFromJson();

    }
}