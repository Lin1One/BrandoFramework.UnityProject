#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 10:28:56 AM
// Email:                 836045613@qq.com

#endregion

using System;

namespace Common
{
    /// <summary>
    /// 计时器抽象基类。
    /// 提供基础数据结构及API。
    /// </summary>
    public abstract class TimerBase : ITimer
    {
        #region 字段及属性

        /// <summary>
        /// 计时器已执行次数。
        /// </summary>
        public int RunNum { get; protected set; }

        /// <summary>
        /// 计时器从启动到现在运行的总时间（秒数）。
        /// </summary>
        public double RunTotalTime => (DateTime.Now - StarDateTime).TotalSeconds;

        /// <summary>
        /// 计时器启动时间戳。
        /// </summary>
        protected DateTime StarDateTime;

        /// <summary>
        /// 是否需要执行延时启动逻辑。
        /// </summary>
        protected bool RequireInvokeDelayStart = true;

        /// <summary>
        /// 唯一Id（多线程安全）。 
        /// 自动生成，无法被外部修改。
        /// </summary>
        public int UniqueId { get; protected set; }

        /// <summary>
        /// 计时器状态。
        /// </summary>
        public TimerStatus Status { get; protected set; }

        /// <summary>
        /// 计时器周期委托。
        /// </summary>
        protected Action<ITimer> OnTick;

        /// <summary>
        /// 计时器启动委托。
        /// </summary>
        protected Action<ITimer> OnStart;

        /// <summary>
        /// 计时器暂停委托。
        /// </summary>
        protected Action<ITimer> OnPause;

        /// <summary>
        /// 计时器恢复运行时委托。
        /// </summary>
        protected Action<ITimer> OnResume;

        /// <summary>
        /// 计时器关闭委托。
        /// </summary>
        protected Action<ITimer> OnClose;

        /// <summary>
        /// 计时器执行频率。
        /// </summary>
        protected double Frequency;

        /// <summary>
        /// 外部设置的对人友好名（用于调试）。
        /// </summary>
        protected string FriendName;

        /// <summary>
        /// 剩余可运行次数。
        /// 默认规则为大于0或小于0则继续运行，等于0则停止运行。
        /// </summary>
        protected int ResidualCount = -1;

        /// <summary>
        /// 计时器继续运行检查器
        /// </summary>
        protected Func<int, bool> m_ContinueCheck;

        /// <summary>
        /// 延迟启动值，默认值为-1表示无需延时启动。
        /// </summary>
        protected double DelayStartTime = -1;

        /// <summary>
        /// 计时器附加数据。
        /// </summary>
        public object Data { get; protected set; }

        /// <summary>
        /// 计时器当前计时周期内已运行时间
        /// </summary>
        protected double TickRunTime;

        /// <summary>
        /// 延迟启动已运行时间。
        /// </summary>
        protected double DelayStartRunTime;

        /// <summary>
        /// 手动关闭计时器，是否会触发结束回调
        /// </summary>
        protected bool InvokeCloseTriggerCallback;

        #endregion

        #region 回调设置

        public virtual ITimer SetOnStart(Action<ITimer> onStart)
        {
            if (Status != TimerStatus.Unstart)
            {
                PrintDisableInfo();
                return this;
            }

            OnStart = onStart;
            return this;
        }

        public virtual ITimer SetOnClose(Action<ITimer> onClose)
        {
            if (Status != TimerStatus.Unstart)
            {
                PrintDisableInfo();
                return this;
            }

            OnClose = onClose;
            return this;
        }

        public virtual ITimer SetOnPause(Action<ITimer> onPause)
        {
            if (Status != TimerStatus.Unstart)
            {
                PrintDisableInfo();
                return this;
            }

            OnPause = onPause;
            return this;
        }

        public virtual ITimer SetOnTick(Action<ITimer> onTick)
        {
            if (Status != TimerStatus.Unstart)
            {
                PrintDisableInfo();
                return this;
            }

            OnTick = onTick;
            return this;
        }

        public virtual ITimer SetOnResume(Action<ITimer> onResume)
        {
            if (Status != TimerStatus.Unstart)
            {
                PrintDisableInfo();
                return this;
            }

            OnResume = onResume;
            return this;
        }

        #endregion

        #region 属性设置

        public virtual ITimer SetExecuteCount(int runCount)
        {
            if (Status != TimerStatus.Unstart)
            {
                PrintDisableInfo();
                return this;
            }

            ResidualCount = runCount;
            return this;
        }

        public virtual ITimer SetFriendName(string friendName)
        {
            if (Status != TimerStatus.Unstart)
            {
                PrintDisableInfo();
                return this;
            }

            FriendName = friendName;
            return this;
        }

        public virtual ITimer SetFrequency(double frequency)
        {
            if (Status != TimerStatus.Unstart)
            {
                PrintDisableInfo();
                return this;
            }

            Frequency = frequency;
            return this;
        }

        public virtual ITimer SetContinueChecker(Func<int, bool> checker)
        {
            if (Status != TimerStatus.Unstart)
            {
                PrintDisableInfo();
                return this;
            }

            m_ContinueCheck = checker;
            return this;
        }

        public virtual ITimer SetData(object data)
        {
            if (Status != TimerStatus.Unstart)
            {
                PrintDisableInfo();
                return this;
            }

            Data = data;
            return this;
        }

        public virtual ITimer SetDelayTime(double delay)
        {
            if (Status != TimerStatus.Unstart)
            {
                PrintDisableInfo();
                return this;
            }

            DelayStartTime = delay;
            return this;
        }

        public ITimer IsInvokCloseTriggerCallback(bool value)
        {
            InvokeCloseTriggerCallback = value;

            return this;
        }

        #endregion

        #region 计时器操作

        public abstract void Start();

        public abstract void Close();

        public abstract void Update();

        public virtual ITimer Resume()
        {
            Status = TimerStatus.Running;
            return this;
        }

        public abstract void ReStart();

        public ITimer Pause()
        {
            Status = TimerStatus.Pause;
            OnPause?.Invoke(this);
            return this;
        }

        protected virtual void TryClose()
        {
            var isContinue = ContinueCheck(ResidualCount);
            if (isContinue)
            {
                return;
            }
            Close();
        }

        /// <summary>
        /// 重置计时器状态以便重复利用
        /// </summary>
        public virtual void Reset()
        {
            OnStart = null;
            OnClose = null;
            OnPause = null;
            OnTick = null;
            Data = null;
            m_ContinueCheck = null;
            Status = TimerStatus.Unstart;
            TickRunTime = 0.0;
            DelayStartTime = 0f;
            ResidualCount = -1;
            Frequency = 0;
        }

        #endregion

        private void PrintDisableInfo()
        {
#if UNITY_EDITOR
            //YuDebugUtility.Log($"计时器{UniqueId}已启动，不允许修改其属性！");
#endif
        }

        /// <summary>
        /// 默认的计时器可否继续运行判断委托。
        /// 剩余次数大于或者小于零则可继续运行。
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private bool DefaultContinueCheck(int num)
        {
            return num > 0 || num < 0;
        }

        protected Func<int, bool> ContinueCheck => m_ContinueCheck ?? DefaultContinueCheck;
    }
}