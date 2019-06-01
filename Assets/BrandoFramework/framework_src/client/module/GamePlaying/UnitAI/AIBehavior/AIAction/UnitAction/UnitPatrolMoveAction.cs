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


using System.Collections.Generic;
using UnityEngine;

namespace Client.GamePlaying.AI
{
    /// <summary>
    /// 行为状态基类
    /// </summary>
    public class UnitPatrolMoveAction : YuAIActionBase
    {
        public override void Init(object param)
        {
            Debug.Log((int)param);
        }

        public override void AddChild(YuAIBehaviorBase child)
        {
        }

        public override List<YuAIBehaviorBase> GetChildren()
        {
            return null;
        }

        public override void Release()
        {
        }

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
        }

        protected override AIBehaviorState Update()
        {
            //unit.U3DData.Trans.localPosition = (testPosition[index]);
            unit.AnimaControl.PlayAnima("run");
            unit.U3DData.Position2D +=  unit.U3DData.TargetDir * 5 * Time.deltaTime;
            unit.U3DData.Trans.position = new Vector3(unit.U3DData.Position2D.x, unit.U3DData.Trans.position.y, unit.U3DData.Position2D.y) ;
               //缓存目标地点
            //float lastDis = (unit.U3DData.TargetPos - unit.U3DData.Position2D).magnitude;
            //EffectiveMove(newCoord);
            

            //更新人物朝向

            //unit.U3DData.SetDirect( Vector3.Slerp(unit.U3DData.Forward,
            //   m_forward, 10 * Time.fixedDeltaTime))

            


            return AIBehaviorState.Success;

        }

        //生效U3D对象位移,参数填入目的地世界坐标
        protected virtual void EffectiveMove(Vector2 pos)
        {
            unit.U3DData.Trans.Translate(new Vector3(pos.x, unit.U3DData.Trans.position.y, pos.y));
            //if (EffectMoveTrans != null)
            //{
            //    EffectMoveTrans.position = m_curPos3D;
            //}
        }


    }
}

