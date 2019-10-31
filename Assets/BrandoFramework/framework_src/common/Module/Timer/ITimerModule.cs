#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 10:28:56 AM
// Email:                 836045613@qq.com

#endregion

using System;

namespace Common
{
    /// <summary>
    /// 计时器模块。
    /// </summary>
    public interface ITimerModule:IModule
    {
        /// <summary>
        /// 获得一个计时器。
        /// 默认无需延时启动和执行一次。
        /// </summary>
        /// <param name="frequency">计时器周期频率。</param>
        /// <param name="onTick">计时器周期执行委托。</param>
        /// <param name="executeCount">执行次数默认为1传入-1为无限执行。</param>
        /// <param name="delayTime">计时器延时启动时间默认为-1即无需延时启动。</param>
        /// <param name="onStart">计时器启动时委托，只会被调用一次。</param>
        /// <param name="onPause">计时器暂停时委托，会重复执行。</param>
        /// <param name="onResume">计时器恢复运行时委托，会重复执行。</param>
        /// <param name="onClose">计时器停止时委托，只会执行一次。执行后计时器及回收或销毁。</param>
        /// <param name="friendName">计时器对人友好的命名。</param>
        /// <param name="data">计时器上锁承载的数据，默认为空。</param>
        /// <param name="customContinueCheck">自定义的计时器继续执行检查器，可以通过该委托来自定义计时器继续运行的判断规则。</param>
        /// <returns></returns>
        ITimer GetTimer
        (
            double frequency,
            Action<ITimer> onTick,
            int executeCount = 1,
            double delayTime = -1,
            Action<ITimer> onStart = null,
            Action<ITimer> onPause = null,
            Action<ITimer> onResume = null,
            Action<ITimer> onClose = null,
            string friendName = null,
            object data = null,
            Func<int, bool> customContinueCheck = null
        );

        /// <summary>
        /// 获得一个只运行一次的计时器。
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="onClose">计时器关闭时回调。</param>
        /// <returns></returns>
        ITimer StartOnceTimer(double frequency, Action<ITimer> onClose = null);

        /// <summary>
        /// 获得一个一直循环运行的计时器。
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="onTick">计时器周期回调函数。</param>
        /// <returns></returns>
        ITimer StartLoopTimer(double frequency, Action<ITimer> onTick = null);

        /// <summary>
        /// 获得一个指定运行次数的计时器。
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="runNum"></param>
        /// <param name="onTick">计时器周期回调函数。</param>
        /// <returns></returns>
        ITimer StartLimitedTimer(double frequency, int runNum, Action<ITimer> onTick = null);

        /// <summary>
        /// 获得一个延时指定时间启动的计时器。
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="delay"></param>
        /// <param name="onStart"></param>
        /// <param name="onTick"></param>
        /// <returns></returns>
        ITimer StartDelayTimer(double frequency, int delay, Action<ITimer> onStart = null,
            Action<ITimer> onTick = null);


        /// <summary>
        /// 回收一个计时器。
        /// </summary>
        /// <param name="timer"></param>
        void RestoreTimer(ITimer timer);
    }
}