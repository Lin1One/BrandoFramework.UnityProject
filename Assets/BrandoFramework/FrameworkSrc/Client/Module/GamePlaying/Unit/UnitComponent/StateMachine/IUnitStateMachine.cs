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
    /// 行为状态机接口
    /// </summary>
    public interface IYuActStateMachine
    {
        /// <summary>
        /// 判断是否能通过输入操作切换状态，如果能则执行
        /// </summary>
        bool TryChangeState(UnitActStateType type,double keepTime = -1.0f ,object enterParam = null);

        /// <summary>
        /// 执行状态切换
        /// </summary>
        /// <param name="type"></param>
        /// <param name="keepTime"></param>
        /// <param name="enterParam"></param>
        /// <returns></returns>
        bool ExecuteStateChange(UnitActStateType type, double keepTime = -1, object enterParam = null);

        /// <summary>
        /// 复活
        /// </summary>
        void Revive();

        /// <summary>
        /// 获取当前状态
        /// </summary>
        /// <returns></returns>
        IUnitActState CurState { get; }

        //Role GetRole();(Todo 需要改成框架的角色类)
        /// <summary>
        /// 获取该角色对象
        /// </summary>
        /// <returns></returns>
        UnitEntityBase GetRole();

        /// <summary>
        /// 当前状态是否能使用技能
        /// </summary>
        /// <returns></returns>
        bool CanUseSkill();

        /// <summary>
        /// 当前状态是否能移动
        /// </summary>
        /// <returns></returns>
        bool CanMove();

        void Execute(object param);
    }
}

