#region Head

// Author:            Yu
// CreateDate:        2018/7/25 20:54:52
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System.Threading;
namespace client_module_network
{


    /// <summary>
    /// socket通信心跳处理器。
    /// </summary>
    public class NetHeartHandler
    {
        //private IYuTimerModule timeModule;

        //private IYuTimerModule TimeModule
        //{
        //    get
        //    {
        //        if (timeModule == null)
        //        {
        //            timeModule = YuU3dAppUtility.Injector.Get<IYuTimerModule>();
        //        }
        //        return timeModule;
        //    }
        //}

        //private IYuTimer timer;

        //private int _frequency = 30;

        //private const int TickLimit = 3;

        //private int UnTickCount = 0;

        //protected YuHeartHandler()
        //{
        //    YuU3dAppUtility.Injector.Get<IYuU3DEventModule>().WatchEvent(YuUnityEventCode.Net_HeartTick, OnHeartTickResponce);
        //}

        //public void SetFrequency(int frequency)
        //{
        //    _frequency = frequency;
        //}

        //public virtual void BeginSendHeartTick()
        //{
        //    SendHeartTick();
        //    timer = TimeModule.GetLoopTimer(_frequency, HeartTickOnTime);
        //    timer.Start();
        //}

        //protected virtual void SendHeartTick()
        //{

        //}

        //protected virtual void OnHeartTickResponce()
        //{
        //    UnTickCount = 0;
        //}

        //protected virtual void OnHeartTickOutOfTime()
        //{
        //    timer.Close();
        //}

        //private void HeartTickOnTime(IYuTimer timer)
        //{
        //    UnTickCount++;
        //    if (UnTickCount > TickLimit)
        //    {
        //        OnHeartTickOutOfTime();
        //    }
        //    else
        //    {
        //        SendHeartTick();
        //    }
        //}
    }

}