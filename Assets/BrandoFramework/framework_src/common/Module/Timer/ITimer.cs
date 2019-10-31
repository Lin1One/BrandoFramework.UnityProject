#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 10:28:56 AM
// Email:                 836045613@qq.com

#endregion

using System;

namespace Common
{
    /// <summary>
    /// 计时器接口。
    /// </summary>
    public interface ITimer : IReset
    {
        #region 基础数据结构

        /// <summary>
        /// 计时器已执行次数。
        /// </summary>
        int RunNum { get; }

        /// <summary>
        /// 计时器当前的运行总时长。
        /// </summary>
        double RunTotalTime { get; }

        /// <summary>
        /// 唯一Id（多线程安全）。 
        /// </summary>
        int UniqueId { get; }

        /// <summary>
        /// 计时器状态。
        /// </summary>
        TimerStatus Status { get; }

        #endregion

        #region 属性设置

        /// <summary>
        /// 设置频率。
        /// </summary>
        /// <param name="frequency"></param>
        /// <returns></returns>
        ITimer SetFrequency(double frequency);

        /// <summary>
        /// 设置计时器周期处理委托。
        /// </summary>
        /// <param name="onTick"></param>
        /// <returns></returns>
        ITimer SetOnTick(Action<ITimer> onTick);

        /// <summary>
        /// 设置计时器持续运行条件检查委托。
        /// </summary>
        /// <param name="checker"></param>
        /// <returns></returns>
        ITimer SetContinueChecker(Func<int, bool> checker);

        /// <summary>
        /// 设置计时器上所需附加的数据。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ITimer SetData(object data);

        /// <summary>
        /// 设置延时启动值。
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        ITimer SetDelayTime(double delay);

        /// <summary>
        /// 设置计时器启动时处理委托。
        /// </summary>
        /// <param name="onStart"></param>
        /// <returns></returns>
        ITimer SetOnStart(Action<ITimer> onStart);

        /// <summary>
        /// 设置计时器关闭处理委托。
        /// </summary>
        /// <param name="onClose"></param>
        /// <returns></returns>
        ITimer SetOnClose(Action<ITimer> onClose);

        /// <summary>
        /// 设置计时器暂停委托。
        /// </summary>
        /// <param name="onPause"></param>
        /// <returns></returns>
        ITimer SetOnPause(Action<ITimer> onPause);

        /// <summary>
        /// 设置计时器恢复运行时委托。
        /// </summary>
        /// <param name="onResume"></param>
        /// <returns></returns>
        ITimer SetOnResume(Action<ITimer> onResume);

        /// <summary>
        /// 设置计时器运行次数。
        /// </summary>
        /// <param name="runCount"></param>
        /// <returns></returns>
        ITimer SetExecuteCount(int runCount);

        /// <summary>
        /// 给计时器设定一个友好的名字
        /// </summary>
        /// <param name="friendName"></param>
        /// <returns></returns>
        ITimer SetFriendName(string friendName);

        #endregion

        #region 状态改变函数

        /// <summary>
        /// 更新计时器。
        /// </summary>
        void Update();

        /// <summary>
        /// 关闭计时器。
        /// </summary>
        void Close();

        /// <summary>
        /// 启动计时器。
        /// </summary>
        /// <returns></returns>
        void Start();

        void ReStart();

        /// <summary>
        /// 暂停计时器。
        /// </summary>
        /// <returns></returns>
        ITimer Pause();

        /// <summary>
        /// 恢复计时器。
        /// </summary>
        /// <returns></returns>
        ITimer Resume();

        /// <summary>
        /// 手动调用关闭接口，是否会触发计时结束回调,(计时器默认会触发)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        ITimer IsInvokCloseTriggerCallback(bool value);

        #endregion
    }
}