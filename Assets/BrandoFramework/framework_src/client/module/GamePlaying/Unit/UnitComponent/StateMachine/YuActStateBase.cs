#region Head

// Author:            Yu
// CreateDate:        2018/10/9 19:37:00
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

namespace Client.GamePlaying.Unit
{
    /// <summary>
    /// 行为状态基类
    /// </summary>
    public abstract class UnitActStateBase : IUnitActState
    {
        public UnitActStateType StateType { get; private set; }
        
        public abstract void Enter(UnitEntityBase role,double duration, object param);

        public abstract void Execute(UnitEntityBase role,object param);

        public abstract void Exit(UnitEntityBase role);

        public abstract void OnFixedUpdate(UnitEntityBase role);

        public void Init(UnitActStateType type)
        {
            StateType = type;
        }

        public abstract bool MoveBreak();

        public abstract bool CanMove();

        public abstract bool CanUseSkill();

        public virtual void TimeOut()
        {

        }
    }


}

