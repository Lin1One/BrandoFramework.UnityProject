#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 10:28:56 AM
// Email:                 836045613@qq.com

#endregion

using System;
using System.Threading;

namespace Common
{
    /// <summary>
    /// 基础计时器实现。
    /// </summary>
    public class CommonTimer: TimerBase
    {
        #region 字段

        private static int GlobalId = -1;

        #endregion

        #region 构造

        public CommonTimer()
        {
            Interlocked.Increment(ref GlobalId);
            UniqueId = GlobalId;
            Status = TimerStatus.Unstart;
        }

        #endregion

        #region 计时器属性设置

        public override ITimer SetDelayTime(double delay)
        {
            DelayStartTime = delay * 1000;
            return this;
        }

        public override ITimer SetFrequency(double frequency)
        {
            Frequency = frequency * 1000;
            return this;
        }

        #endregion

        #region 计时器操作

        public override void Start()
        {
            if (Status != TimerStatus.Unstart)
            {
                //YuDebugUtility.LogError($"计时器{UniqueId}已经启动了，无法再次启动！");
                return;
            }

            Status = TimerStatus.Running;
            StarDateTime = DateTime.Now;
            // 将自身注册到计时器调度器并尝试启动调度器中的原生计时器
            TimerDispatcher.RegisterTimer(this);
        }

        public override void ReStart()
        {
            TickRunTime = 0f;
        }

        public override void Close()
        {
            Reset();
            Status = TimerStatus.Close;
            TimerDispatcher.RemoveTimer(this);
        }

        public override void Update()
        {
            if (Status != TimerStatus.Running)
            {
                return;
            }

            if (RequireInvokeDelayStart)
            {
                TryDelayStart();
            }
            else
            {
                Tick();
            }
        }

        private void TryDelayStart()
        {
            // 无需延时启动
            if (DelayStartTime < 0)
            {
                RequireInvokeDelayStart = false;
                //Debug.Log($"计时器{UniqueId}无需延时启动，将立即启动！");
                OnStart?.Invoke(this);
                Tick();
                return;
            }

            // 更新当前延时启动已运行时间
            DelayStartRunTime += TimerDispatcher.INTERVAL;
            //            Debug.Log($"计时器{UniqueId}延时启动时间为{DelayStartTime}，已运行{DelayStartRunTime}！");

            // 延时启动时间未到
            if (DelayStartRunTime <= DelayStartTime)
            {
                return;
            }

            OnStart?.Invoke(this);
            RequireInvokeDelayStart = false;
        }

        private void Tick()
        {
            TickRunTime += TimerDispatcher.INTERVAL;
            //Debug.Log($"计时器{UniqueId}周期运行时间为{TickRunTime}！");
            if (TickRunTime <= Frequency)
            {
                return;
            }

            TickRunTime = 0;
            ResidualCount--;
            RunNum++;
            OnTick?.Invoke(this);
            TryClose();
        }

        #endregion
    }
}