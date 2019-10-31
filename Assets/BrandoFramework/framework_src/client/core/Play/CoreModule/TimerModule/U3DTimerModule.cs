#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 10:28:56 AM
// Email:                 836045613@qq.com

#endregion

using System;
using Common;

namespace Client.Core
{
    /// <summary>
    /// U3d运行时计时器模块基础实现。
    /// </summary>
    [Singleton]
    public class U3DTimerModule : ITimerModule
    { 
        public void Init()
        {

        }

        private IGenericObjectPool<U3dTimer> timerPool;

        private IGenericObjectPool<U3dTimer> TimerPool
        {
            get
            {
                if (timerPool != null)
                {
                    return timerPool;
                }

                timerPool = new GenericObjectPool<U3dTimer>(
                    () => new U3dTimer(), 20);
                return timerPool;
            }
        }

        public ITimer GetTimer(double frequency, 
            Action<ITimer> onTick, 
            int executeCount = 1, 
            double delayTime = -1,
            Action<ITimer> onStart = null,
            Action<ITimer> onPause = null, 
            Action<ITimer> onResume = null, 
            Action<ITimer> onClose = null,
            string friendName = null, 
            object data = null,
            Func<int, bool> customContinueCheck = null)
        {
            var timer = TimerPool.Take();
            timer.Init(frequency, onTick, executeCount, delayTime, onStart, onPause, onResume, onClose,
                friendName, data, customContinueCheck);
            return timer;
        }

        public ITimer StartOnceTimer(double frequency, Action<ITimer> onClose = null)
        {
            var timer = TimerPool.Take();
            timer.Init(frequency)
                .SetFrequency(frequency)
                .SetExecuteCount(1)
                .SetOnClose(onClose);
            timer.Start();
            return timer;
        }

        public ITimer StartLoopTimer(double frequency, Action<ITimer> onTick = null)
        {
            var timer = TimerPool.Take();
            timer.Init(frequency)
                .SetFrequency(frequency)
                .SetOnTick(onTick);
            timer.Start();
            return timer;
        }

        public ITimer StartLimitedTimer(double frequency, int runNum, Action<ITimer> onTick = null)
        {
            var timer = TimerPool.Take();
            timer.Init(frequency)
                .SetFrequency(frequency)
                .SetExecuteCount(runNum)
                .SetOnTick(onTick);
            timer.Start();
            return timer;
        }

        public ITimer StartDelayTimer(double frequency, int delay, Action<ITimer> onStart = null,
            Action<ITimer> onTick = null)
        {
            var timer = TimerPool.Take();
            timer.Init(frequency)
                .SetFrequency(frequency)
                .SetDelayTime(delay)
                .SetOnStart(onStart);
            timer.Start();
            return timer;
        }

        public void RestoreTimer(ITimer timer)
        {
            TimerPool.Restore((U3dTimer) timer);
        }
    }
}