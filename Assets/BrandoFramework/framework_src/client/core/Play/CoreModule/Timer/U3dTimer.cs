using System;
using System.Threading;
using UnityEngine;
using Common;

namespace Client.Core
{
    /// <summary>
    /// U3d运行时计时器。
    /// </summary>
    public class U3dTimer : TimerBase
    {
        /// <summary>
        /// 全局编号。
        /// </summary>
        private static int GlobalId;

        private readonly IU3DEventModule _eventModule;

        private ITimerModule s_timerModule;
        private ITimerModule TimerModule
        {
            get
            {
                if (s_timerModule == null)
                {
                    s_timerModule = Injector.Instance.Get<ITimerModule>();
                }
                return s_timerModule;
            }
        }

        public U3dTimer()
        {
            Interlocked.Increment(ref GlobalId);
            UniqueId = GlobalId;
            _eventModule = Injector.Instance.Get<IU3DEventModule>();
        }

        public ITimer Init(double frequency, 
            Action<ITimer> onTick = null, 
            int runNum = 1, 
            double delayTime = -1,
            Action<ITimer> onStart = null,
            Action<ITimer> onPause = null, 
            Action<ITimer> onResume = null, 
            Action<ITimer> onClose = null,
            string friendName = null, 
            object data = null,
            Func<int, bool> customContinueCheck = null)
        {
            Frequency = frequency;
            OnTick = onTick;
            RunNum = runNum;
            DelayStartTime = delayTime;
            OnStart = onStart;
            OnPause = onPause;
            OnResume = onResume;
            OnClose = onClose;
            FriendName = friendName;
            Data = data;
            m_ContinueCheck = customContinueCheck;
            InvokeCloseTriggerCallback = true;

            return this;
        }

        #region 计时器操作

        public override void Close()
        {
            if (Status == TimerStatus.Close)
            {
#if UNITY_EDITOR
                //YuDebugUtility.Log(
                //    $"Id{UniqueId}_名{FriendName}的计时器已经停止了，无法再次停止！");
#endif
                return;
            }

            Status = TimerStatus.Close;
            _eventModule.RemoveUnityEvent(UnityEventType.Update, Update);
            if (InvokeCloseTriggerCallback || !ContinueCheck(ResidualCount))
            {
                OnClose?.Invoke(this);
            }
            CheckRestore();
            Reset();
        }

        public override void Start()
        {
            Status = TimerStatus.Running;
            StarDateTime = DateTime.Now;
            OnStart?.Invoke(this);
            _eventModule.WatchUnityEvent(UnityEventType.Update, Update);
        }

        public override void ReStart()
        {
            TickRunTime = 0f;
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

        private void CheckRestore()
        {
            if(Status == TimerStatus.Close)
            {
                TimerModule.RestoreTimer(this);
            }
        }

        private void TryDelayStart()
        {
            // 无需延时启动
            if (DelayStartTime < 0)
            {
                RequireInvokeDelayStart = false;
                Tick();
                return;
            }

            // 更新当前延时启动已运行时间
            DelayStartRunTime += Time.deltaTime;

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
            TickRunTime += Time.deltaTime;
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