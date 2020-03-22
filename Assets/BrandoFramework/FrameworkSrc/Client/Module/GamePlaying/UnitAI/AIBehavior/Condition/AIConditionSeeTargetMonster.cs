//#region Head

//// Author:            LinYuzhou
//// CreateDate:        2018/12/4 10:58:32
//// Email:             836045613@qq.com

///*
// * 修改日期  ：
// * 修改人    ：
// * 修改内容  ：
//*/

//#endregion

//using System.Collections.Generic;
//using UnityEngine;
//
//using YuU3dPlay;

//namespace XTwoPlay
//{
//    /// <summary>
//    /// AI条件，看到指定敌人(只适用于主角)
//    /// Init(param):string(怪物id),float(视野距离)
//    /// </summary>
//    public class XTwoAIConditionSeeTargetMonster : YuAIConditionBase
//    {
//        private readonly XTwoUnitModule m_unitModule = 
//            YuU3dAppUtility.Injector.Get<XTwoUnitModule>();
//        private readonly XTwoSelectTargetModule selectModule =
//            YuU3dAppUtility.Injector.Get<XTwoSelectTargetModule>();
//        private float m_seeDistance; //视野距离
//        private string m_mosnterId;

//        public override void Init(object param)
//        {
//            try
//            {
//                object[] paramArr = (object[])param;
//                m_mosnterId = (string)paramArr[0];
//                m_seeDistance = (float)paramArr[1];
//            }
//            catch
//            {
//#if DEBUG
//                Debug.LogError("XTwoAIConditionSeeTargetMonster 参数错误");
//#endif 
//                m_seeDistance = 10.0f;
//            }
//#if DEBUG
//            if (string.IsNullOrEmpty(m_mosnterId ) || m_mosnterId =="0")
//            {
//                Debug.LogError("XTwoAIConditionSeeTargetMonster 参数错误,目标为空");
//            }
//#endif
//        }
        
//        public override void Release()
//        {
           
//        }

//        protected override void Enter()
//        {
            
//        }

//        protected override void Exit()
//        {
            
//        }

//        protected override AIBehaviorState OnUpdate()
//        {
//            Vector2 selfPos = m_role.U3DData.Position2D;
//            if(m_role.AIControl.TargetUnit != null &&
//                m_role.AIControl.TargetUnit.ActStateMachine != null &&
//                !(m_role.AIControl.TargetUnit.ActStateMachine.CurState is XTwoActStateDead)) //如果有目标且目标没死亡
//            {
//                //判断是否太远
//                if (Vector2.Distance(selfPos, m_role.AIControl.TargetUnit.U3DData.Position2D) > m_seeDistance * 2)
//                {
//                    m_role.AIControl.TargetUnit = null;
//                }
//                else
//                {
//                    var monster = m_unitModule.TryGetUnitData(m_role.AIControl.TargetUnit.ID, XTwoUnitType.monster)
//                    as XTwoMonsterData;
//                    if (monster?.templateId == m_mosnterId)
//                    {
//                        return AIBehaviorState.Success;
//                    }
//                }
//            }

            

//            {
//                //寻找新的目标
//                m_role.AIControl.TargetUnit = null;
//                var enemyList = m_unitModule.GetGivenTypeUnitDic(XTwoUnitType.monster);

//                Vector2 enemyPos;
//                XTwoLivingDataBase targetEnemy = null;
//                float minDis = float.MaxValue;
//                YuUnitBase enemyNear = null;
//                foreach (XTwoMonsterData enemy in enemyList.Values)    //找最近的指定怪物
//                {
//                    if (enemy.templateId != m_mosnterId ||
//                        enemy.IsDead ||
//                        enemy.Unit == null)
//                    {
//                        continue;
//                    }

//                    enemyPos = enemy.Unit.U3DData.Position2D;
//                    float dis = Vector2.Distance(selfPos, enemyPos);

//                    if (dis < m_seeDistance &&     //必须在指定范围内，且
//                        (dis< minDis)) //最近
//                    {
//                        minDis = dis;
//                        enemyNear = enemy.Unit;
//                        targetEnemy = enemy;
//                    }
//                }
//                if(enemyNear != null)
//                {
//                    m_role.AIControl.TargetUnit = enemyNear;
//                    selectModule.SelectTarget(targetEnemy);

//                    return AIBehaviorState.Success;
//                }
//                return AIBehaviorState.Failure; //没找到则返回失败
//            }
           
//        }
//    }
//}

