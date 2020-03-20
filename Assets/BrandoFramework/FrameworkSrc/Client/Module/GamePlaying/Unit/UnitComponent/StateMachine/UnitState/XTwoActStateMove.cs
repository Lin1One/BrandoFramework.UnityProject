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
    public class XTwoActStateMove : UnitActStateBase
    {
        public override bool CanMove()
        {
            return true;
        }

        public override bool CanUseSkill()
        {
            return true;
        }

        public override bool MoveBreak()
        {
            return false;
        }

        public override void Enter(UnitEntityBase role, double duration, object param)
        {
            
            role.AnimaControl.UnitPlayAnima("run", false /*, speed*/);
        }

        public override void Execute(UnitEntityBase role,object param)
        {

        }

        public override void Exit(UnitEntityBase role)
        {
        }

        public override void OnFixedUpdate(UnitEntityBase role)
        {
            
        }
    }
}

