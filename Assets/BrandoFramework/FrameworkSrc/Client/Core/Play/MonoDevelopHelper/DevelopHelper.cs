#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using UnityEngine;

namespace Client
{
    /// <summary>
    /// 开发助手。
    /// 凡是挂载了该类型实例的预制件都需要进行运行时精简化处理。
    /// 精简化处理是指将运行时无需使用的脚本从预制件上剥离以便提高运行时性能。
    /// </summary>
    [ExecuteInEditMode]
    public class DevelopHelper : MonoBehaviour
    {
    }
}