
using System;

namespace client_module_event
{
    #region 非泛型

    /// <summary>
    /// 无需目标数据的可计数的事件处理器。
    /// </summary>
    public class YuTallyEventHandler : ITallyEventHandler
    {
        #region 字段

        /// <summary>
        /// 事件执行委托。
        /// </summary>
        private Action EventAction { get;  set; }

        /// <summary>
        /// 事件的剩余可执行次数。
        /// </summary>
        /// <value>The residue count.</value>
        public int ResidueCount { get; private set; }

        /// <summary>
        /// 事件处理器的身份Id。
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; private set; }

        #endregion

        public YuTallyEventHandler(int residueNum, Action action)
        {
            ResidueCount = residueNum;
            EventAction = action;
            Id = TallyEventHandlerCounter.GetEventHandlerId();
        }

        /// <summary>
        /// 处理事件。
        /// 1. 如果剩余次数为0或者事件处理委托为空则退出。
        /// 2. 剩余次数大于0则执行并减一剩余次数。
        /// 3. 剩余次数小于0则直接执行（无限执行）。
        /// </summary>
        public void HandleEvent()
        {
            if (EventAction == null || ResidueCount == 0) return;

            if (ResidueCount < 0)
            {
                EventAction();
                return;
            }
            else
            {
                ResidueCount--;
                EventAction();
            }
        }

        public bool CheckMatch(Action action)
        {
            return action == EventAction;
        }
    }

    #endregion

    #region 泛型

    /// <summary>
    /// 需要目标数据的可计数的事件处理器。
    /// </summary>
    public sealed class TallyEventHandler<TData> : ITallyEventHandler<TData>
    {
        /// <summary>
        /// 事件执行委托。
        /// </summary>
        /// <value>The event action.</value>
        private Action<TData> EventAction { get; set; }

        /// <summary>
        /// 事件的剩余可执行次数。
        /// </summary>
        /// <value>The residue count.</value>
        public int ResidueCount { get; private set; }

        /// <summary>
        /// 事件处理器的身份Id。
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; private set; }

        public TallyEventHandler(int residueCount, Action<TData> action)
        {
            ResidueCount = residueCount;
            EventAction = action;
            Id = TallyEventHandlerCounter.GetEventHandlerId();
        }

        /// <summary>
        /// 处理事件。
        /// 1. 如果剩余次数为0或者事件处理委托为空则退出。
        /// 2. 剩余次数大于0则执行并减一剩余次数。
        /// 3. 剩余次数小于0则直接执行（无限执行）。
        /// </summary>
        public void HandleEvent(TData eventData)
        {
            if (EventAction == null || ResidueCount == 0) return;

            //YuPrefor.StartRecord(EventAction);
            if (ResidueCount < 0)
            {
                EventAction(eventData);
                //YuPrefor.EndRecord(EventAction);
                return;
            }
            else
            {
                ResidueCount--;
                EventAction(eventData);
            }
            //YuPrefor.EndRecord(EventAction);
        }

        public bool CheckMatch(Action<TData> action)
        {
            return action == EventAction;
        }
    }

    #endregion

}

