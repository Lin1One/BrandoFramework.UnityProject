#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 10:28:56 AM
// Email:                 836045613@qq.com

#endregion

using System;
using System.Timers;

namespace Common
{
    /// <summary>
    /// 计时器调度器。
    /// </summary>
    public static class TimerDispatcher
    {
        public const int INTERVAL = 16;
        private static Timer nativeTimer;
        private static ITimer[] timers = new ITimer[10];

        public static void RegisterTimer(ITimer timer)
        {
            if (timer.UniqueId >= timers.Length)
            {
                // 扩容
                var newLength = timers.Length * 2;
                var newTimers = new ITimer[newLength];
                MoveExistTimerToNewTimers(newTimers);
            }

            if (timers[timer.UniqueId] != null)
            {
                throw new Exception(
                    $"尝试重复添加同一个计时器，计时器ID为：{timer.UniqueId}！"
                );
            }

            // 将计时器加入计时器数组
            timers[timer.UniqueId] = timer;

            if (nativeTimer != null)
            {
                return;
            }

            InitNativeTimer();
        }

        public static void RemoveTimer(ITimer timer)
        {
            if (!timers[timer.UniqueId].Equals(timer))
            {
                throw new Exception(
                    $"Id为{timer.UniqueId}的目标计时器不存在，移除失败！"
                );
            }

            timers[timer.UniqueId] = null;
        }

        /// <summary>
        /// 暂停所有计时器运行。
        /// </summary>
        public static void StopDispatch()
        {
            if (nativeTimer == null)
            {
                return;
            }

            nativeTimer.Enabled = false;
        }

        /// <summary>
        /// 恢复所有计时器运行。
        /// </summary>
        public static void ResumeDispatch()
        {
            if (nativeTimer == null)
            {
                return;
            }

            nativeTimer.Enabled = true;
        }

        /// <summary>
        /// 关闭计时器调度器。
        /// 这会关闭当前所有的计时器实例并内部的清空计时器数组。
        /// </summary>
        public static void Close()
        {
            var timerCount = timers.Length;

            for (var i = 0; i < timerCount; i++)
            {
                timers[i]?.Close();
            }

            timers = null;
            nativeTimer.Close();
        }

        private static void MoveExistTimerToNewTimers(ITimer[] newTimers)
        {
            for (var i = 0; i < timers.Length; i++)
            {
                var timer = timers[i];
                if (timer == null)
                {
                    continue;
                }

                newTimers[i] = timer;
            }

            timers = newTimers;
        }

        private static void InitNativeTimer()
        {
            nativeTimer = new Timer(INTERVAL);
            nativeTimer.Elapsed += NativeTimerOnElapsed;
            nativeTimer.Start();
        }

        private static void NativeTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            var timerCount = timers.Length;

            for (var i = 0; i < timerCount; i++)
            {
                timers[i]?.Update();
            }
        }
    }
}