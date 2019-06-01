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

using client_common;
using client_module_event;
using System;
using System.Collections.Generic;

namespace Client.GamePlaying.Unit
{
    /// <summary>
    /// 行为状态机
    /// </summary>
    public class XTwoActStateMachine : UnitComponent , IYuActStateMachine
    {
        private IUnitActState m_curState;     //当前状态
        //private IYuTimerModule m_timerModule;     //计时器模块（记录持续状态时间）
        //private int m_messageEventID = -1;      //用于记录事件id，注销事件
        private Action m_waitTodo;

        private readonly Dictionary<UnitActStateType, IUnitActState> m_dicActState =
            new Dictionary<UnitActStateType, IUnitActState>();      //状态集合

        //private static YuFightStateModule s_fightStateModule;
        //private static YuFightStateModule FightStateModule
        //{
        //    get
        //    {
        //        if (s_fightStateModule == null)
        //        {
        //            s_fightStateModule = YuU3dAppUtility.Injector.Get<YuFightStateModule>();
        //        }
        //        return s_fightStateModule;
        //    }
        //}

        public XTwoActStateMachine()
        {
            //m_timerModule = YuU3dAppUtility.Injector.Get<IYuTimerModule>();

            //Todo 初始化状态集合,之后需要改为根据各角色各自的数据构建
            //移动
            IUnitActState actState = new XTwoActStateMove();
            actState.Init(UnitActStateType.move);
            m_dicActState.Add(UnitActStateType.move, actState);
            ////待机
            //actState = new XTwoActStateIdel();
            //actState.Init(UnitActStateType.idle);
            //m_dicActState.Add(UnitActStateType.idle, actState);
            ////使用技能（包括攻击）
            //actState = new XTwoActStateSkill();
            //actState.Init(UnitActStateType.skill);
            //m_dicActState.Add(UnitActStateType.skill, actState);
            ////等待下一个状态（用于两状态之间的缓冲）
            //actState = new XTwoActStateWait();
            //actState.Init(UnitActStateType.wait);
            //m_dicActState.Add(UnitActStateType.wait, actState);
            ////死亡
            //actState = new XTwoActStateDead();
            //actState.Init(UnitActStateType.die);
            //m_dicActState.Add(UnitActStateType.die, actState);
            ////受击
            //actState = new XTwoActStateHit();
            //actState.Init(UnitActStateType.hit);
            //m_dicActState.Add(UnitActStateType.hit, actState);
            ////互动
            //actState = new XTwoActStateInteraction();
            //actState.Init(UnitActStateType.interaction);
            //m_dicActState.Add(UnitActStateType.interaction, actState);
            ////跳跃
            //actState = new XTwoActStateJump();
            //actState.Init(UnitActStateType.jump);
            //m_dicActState.Add(UnitActStateType.jump, actState);
        }

        ////初始化
        protected override void OnInit()
        {
            //注册事件
            //Injector.Instance.Get<IU3DEventModule>().WatchUnityEvent(
            //    YuUnityEventType.FixedUpdate,  );
            ////Todo 需要定一个状态交互事件
            //m_messageEventID = YuU3dAppUtility.Injector.Get<IYuU3DEventModule>().WatchEvent(
            //    YuEventFactory.GetEventCode("AppEntity", ""), OnMessage);

            ExecuteStateChange(UnitActStateType.idle, -1.0f, null);
        }

        protected override void OnRelease()
        {
            Injector.Instance.Get<IU3DEventModule>().RemoveUnityEvent(YuUnityEventType.FixedUpdate, OnFixedUpdate);
            //if (m_messageEventID != -1)
            //{
            //    //YuU3dAppUtility.Injector.Get<IYuU3DEventModule>().RemoveSpecifiedHandler()
            //    m_messageEventID = -1;
            //}
            m_waitTodo = null;
            m_curState = null;
        }

        /// <summary>
        /// 尝试切换状态
        /// </summary>
        /// <param name="type"></param>
        /// <param name="keepTime"></param>
        /// <param name="enterParam"></param>
        /// <returns></returns>
        public bool TryChangeState(UnitActStateType type, double keepTime = -1.0, object enterParam = null)
        {
            //对状态之间是否能通过输入操作切换做判断
            switch (type)
            {
                case UnitActStateType.idle:
                    //if (!(m_curState is XTwoActStateMove))
                    //    return false;
                    break;
                case UnitActStateType.move:
                    if (!m_curState.MoveBreak())
                        return false;
                    break;
                case UnitActStateType.skill:
                    if (!m_curState.CanUseSkill())
                        return false;
                    break;
                case UnitActStateType.interaction:
                    if (!m_curState.CanUseSkill())
                        return false;
                    break;
                case UnitActStateType.jump:
                    if (!m_curState.CanUseSkill())
                        return false;
                    break;
            }

            return ExecuteStateChange(type, keepTime, enterParam);
        }

        /// <summary>
        /// 复活
        /// </summary>
        /// <returns></returns>
        public void Revive()
        {
            //if (m_curState is XTwoActStateDead)
            //{
            //    m_curState.Exit(Role);
            //    m_curState = m_dicActState[UnitActStateType.idle];
            //    m_curState.Enter(Role, -1 , null);
            //}
        }

        //private IYuTimer m_timer; 

        /// <summary>
        /// 执行状态切换
        /// </summary>
        /// <param name="type"></param>
        /// <param name="keepTime"></param>
        /// <param name="enterParam"></param>
        /// <returns></returns>
        public bool ExecuteStateChange(UnitActStateType type, double keepTime = -1, object enterParam = null)
        {
            IUnitActState temp = m_dicActState[type];

            if (m_curState == temp)  //切换的状态与原状态一样则不执行
            {
                return false;
            }

            //if (m_curState is XTwoActStateDead)
            //{
            //    return false;
            //}

            //先缓冲当前状态，把当前状态设置为等待，以免当前状态触发的离开事件的操作，
            //因状态限制而无法生效
            IUnitActState lastState = m_curState;
            m_curState = m_dicActState[UnitActStateType.wait];
            if (lastState != null)
            {
                lastState.Exit(Role);        //前一个状态的离开事件
            }

            m_curState = temp;

            if (m_curState != null)
            {
                //if (m_timer != null)
                //{
                //    var tempTimer = m_timer;
                //    m_timer = null;
                //    tempTimer.Close();
                //}
                //m_curState.Enter(Role, keepTime, enterParam);       //新事件的进入事件
                //if (keepTime > 0.0f)
                //{
                //    //有持续时间设置的，开始计时
                //    m_timer = m_timerModule.GetOnceTimer(keepTime,
                //        (timer2) =>
                //        {
                //            if(m_timer == timer2)
                //            {
                //                m_timer = null;
                //                if (m_curState != null)
                //                {
                //                    m_curState.TimeOut();
                //                }
                //                ExecuteStateChange(UnitActStateType.wait, -1.0, null);
                //            }
                //        });
                //    m_timer.Start();
                //}
                return true;
            }
            else
            {
                UnityEngine.Debug.LogError(string.Format("未找到相应的行为状态对象：{0}", type));
                m_curState = m_dicActState[UnitActStateType.idle];
            }
            return false;
        }

        public IUnitActState CurState
        {
            get { return m_curState; }
        }

        public void AddNextFixedUpdateAction(Action action)
        {
            if(m_waitTodo == null)
            {
                m_waitTodo = action;
            }
            else
            {
                m_waitTodo += action;
            }
        }

        public void CleanWaitTodo()
        {
            m_waitTodo = null;
        }

        private void OnFixedUpdate()
        {
            //if (FightStateModule.IsScenePaused)
            //{
            //    return;
            //}

            if (m_waitTodo != null)
            {
                m_waitTodo();
                m_waitTodo = null;
            }

            if (m_curState != null)
            {
                m_curState.OnFixedUpdate(Role);
            }
        }

        //状态交互事件触发
        public void Execute(object param)
        {
            if (m_curState != null)
            {
                m_curState.Execute(Role, param);
            }
        }

        public bool CanUseSkill()
        {
            if (m_curState == null)
                return false;

            return m_curState.CanUseSkill();
        }

        public bool CanMove()
        {
            if (m_curState == null)
                return false;

            return m_curState.CanMove();
        }

        public UnitEntityBase GetRole()
        {
            return Role;
        }
    }
}

