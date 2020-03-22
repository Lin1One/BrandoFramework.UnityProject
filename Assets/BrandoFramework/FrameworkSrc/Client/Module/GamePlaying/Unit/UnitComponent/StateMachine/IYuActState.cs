#region Head

// Author:            Yu
// CreateDate:        2018/10/9 19:37:00
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

namespace Client.GamePlaying.Unit
{
    /// <summary>
    /// 行为状态接口
    /// </summary>
    public interface IUnitActState
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="type"></param>
        void Init(UnitActStateType type);

        /// <summary>
        /// 进入状态触发
        /// </summary>
        void Enter(UnitEntityBase role, double duration, object param);

        /// <summary>
        /// Unity FixedUpdate触发
        /// </summary>
        void OnFixedUpdate(UnitEntityBase role);

        /// <summary>
        /// 状态结束触发
        /// </summary>
        void Exit(UnitEntityBase role);

        /// <summary>
        /// 状态交互事件触发
        /// </summary>
        /// <param name="param"></param>
        void Execute(UnitEntityBase role, object param);

        /// <summary>
        /// 是否能移动
        /// </summary>
        bool CanMove();

        /// <summary>
        /// 是否能被移动打断状态
        /// </summary>
        /// <returns></returns>
        bool MoveBreak();


        /// <summary>
        /// 是否能释放
        /// </summary>
        /// <returns></returns>
        bool CanUseSkill();


        void TimeOut();
    }
}

