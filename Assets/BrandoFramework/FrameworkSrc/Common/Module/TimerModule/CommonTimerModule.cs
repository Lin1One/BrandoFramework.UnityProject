#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 10:28:56 AM
// Email:                 836045613@qq.com

#endregion

using System;

namespace Common
{
    /// <summary>
    /// 通用计时器模块。
    /// </summary>
    [Singleton]
    public class CommonTimerModule : ITimerModule
    {
        private IGenericObjectPool<ITimer> timerPool;

        private IGenericObjectPool<ITimer> TimerPool
        {
            get
            {
                if (timerPool != null)
                {
                    return timerPool;
                }

                timerPool = new GenericObjectPool<ITimer>(
                    () => new CommonTimer(), 20);
                return timerPool;
            }
        }

        public void InitModule()
        {
           
        }

        public void Dispose()
        {
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
            var timer = timerPool.Take();
            timer.SetFrequency(frequency)
                .SetOnTick(onTick)
                .SetExecuteCount(executeCount)
                .SetDelayTime(delayTime)
                .SetOnStart(onStart)
                .SetOnPause(onPause)
                .SetOnResume(onResume)
                .SetOnClose(onClose)
                .SetFriendName(friendName)
                .SetData(data)
                .SetContinueChecker(customContinueCheck);
            return timer;
        }

        public ITimer StartOnceTimer(double frequency, Action<ITimer> onClose = null)
        {
            throw new NotImplementedException();
        }

        public ITimer StartLoopTimer(double frequency, Action<ITimer> onTick = null)
        {
            throw new NotImplementedException();
        }

        public ITimer StartLimitedTimer(double frequency, int runNum, Action<ITimer> onTick = null)
        {
            throw new NotImplementedException();
        }

        public ITimer StartDelayTimer(double frequency, int delay, Action<ITimer> onStart = null,
            Action<ITimer> onTick = null)
        {
            throw new NotImplementedException();
        }


        public void RestoreTimer(ITimer timer)
        {
            TimerPool.Restore(timer);
        }

    }
}