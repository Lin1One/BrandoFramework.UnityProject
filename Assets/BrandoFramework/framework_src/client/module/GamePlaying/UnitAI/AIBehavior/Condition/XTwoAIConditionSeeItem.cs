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
//
//using YuU3dPlay;

//namespace XTwoPlay
//{
//    /// <summary>
//    /// AI条件，是否看到掉落物品
//    /// Init(param):float(视野距离)
//    /// </summary>
//    public class XTwoAIConditionSeeItem : YuAIConditionBase
//    {
//        private readonly XTwoUnitModule m_unitModule =
//            YuU3dAppUtility.Injector.Get<XTwoUnitModule>();
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

//        private bool isMove = false;

//        protected override AIBehaviorState OnUpdate()
//        {
//            Vector2 selfPos = m_role.U3DData.Position2D;
//            var itemList = m_unitModule.GetUnitsByType(XTwoUnitType.dropItem);
//            foreach (var item in itemList)
//            {
//                var dropItem = (item as XTwoDropItemData);
//                if (dropItem == null ||
//                    (dropItem.ownerId != 0 && dropItem.ownerId != m_role.ID)) //主角不可捡
//                {
//                    continue;
//                }

//                var targetPos = dropItem.Unit?.U3DData?.Position2D;
//                if (targetPos.HasValue)
//                {
//                    float dis = Vector2.Distance(selfPos, targetPos.Value);
//                    if (dis < m_seeDistance)
//                    {
//                        m_role.AIControl.BehaviorParam = item.GuId;
//                        return AIBehaviorState.Success;
//                    }
//                }
//            }

//            return AIBehaviorState.Failure;
//        }
//    }
//}

