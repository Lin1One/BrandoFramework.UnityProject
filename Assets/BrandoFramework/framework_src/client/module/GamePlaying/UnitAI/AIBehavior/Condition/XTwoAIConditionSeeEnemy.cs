//#region Head

//// Author:            Chengkefu
//// CreateDate:        2018/12/4 10:58:32
//// Email:             chengkefu0730@live.com

///*
// * 修改日期  ：
// * 修改人    ：
// * 修改内容  ：
//*/

//#endregion


//using System.Collections.Generic;
//using UnityEngine;

//namespace Client.GamePlaying.AI
//{
//    /// <summary>
//    /// AI条件，看到敌人(只适用于主角)
//    /// Init(param):float(视野距离)
//    /// </summary>
//    public class XTwoAIConditionSeeEnemy : YuAIConditionBase
//    {
//        //private readonly XTwoUnitModule m_unitModule = 
//        //    YuU3dAppUtility.Injector.Get<XTwoUnitModule>();
//        //private readonly XTwoSelectTargetModule selectModule =
//        //    YuU3dAppUtility.Injector.Get<XTwoSelectTargetModule>();
//        private float m_seeDistance; //视野距离

//        public override void Init(object param)
//        {
//            try
//            {
//                m_seeDistance = (float)param;
//            }
//            catch
//            {
//                m_seeDistance = 10.0f;
//            }

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
//            Vector2 selfPos = unitEntity.U3DData.Position2D;
//            if (unitEntity.AIControl.TargetUnit == null ||
//                unitEntity.AIControl.TargetUnit.ActStateMachine == null ||
//                unitEntity.AIControl.TargetUnit.ActStateMachine.CurState is XTwoActStateDead)  //如果没有目标或是目标死亡
//            {
//                //寻找新的目标
//                unitEntity.AIControl.TargetUnit = null;
//                List<XTwoLivingDataBase> enemyList = m_unitModule.GetEnemyDataList();

//                Vector2 enemyPos;
//                XTwoLivingDataBase targetEnemy = null;
//                float minDis = float.MaxValue;
//                YuUnitBase enemyNear = null;
//                int curLevel = 1; //当前敌人的优先级，1小怪，2敌人，3玩家，
//                foreach (var enemy in enemyList)    //找最近的
//                {
//                    if (enemy.Unit == null)
//                    {
//                        continue;
//                    }
//                    int unitLevel;
//                    if (enemy.Camp == "0") //小怪
//                    {
//                        unitLevel = 1;
//                    }
//                    else if(enemy.Camp == "2") // boss
//                    {
//                        unitLevel = 2;
//                    }
//                    else   //在这里一定会是玩家
//                    {
//                        unitLevel = 3;
//                    }
                    
//                    if(unitLevel < curLevel)        //优先级低则略过
//                    {
//                        continue;
//                    }

//                    enemyPos = enemy.Unit.U3DData.Position2D;
//                    float dis = Vector2.Distance(selfPos, enemyPos);

//                    if (dis < m_seeDistance &&     //必须在指定范围内，且
//                        (dis< minDis || unitLevel > curLevel)) //最近或优先级高
//                    {
//                        curLevel = unitLevel;
//                        minDis = dis;
//                        enemyNear = enemy.Unit;
//                        targetEnemy = enemy;
//                    }
//                }
//                if(enemyNear != null)
//                {
//                    unitEntity.AIControl.TargetUnit = enemyNear;
//                    selectModule.SelectTarget(targetEnemy);

//                    return AIBehaviorState.Success;
//                }
//                return AIBehaviorState.Failure; //没找到则返回失败
//            }
//            //判断是否太远
//            if (Vector2.Distance(selfPos, unitEntity.AIControl.TargetUnit.U3DData.Position2D) > m_seeDistance * 2)
//            {
//                unitEntity.AIControl.TargetUnit = null;
//                return AIBehaviorState.Failure;
//            }


//            return AIBehaviorState.Success;
//        }
//    }
//}

