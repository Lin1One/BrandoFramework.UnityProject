
using System;

namespace Client.Core
{
    public class EventHandler : IEventHandler
    {
        private Delegate EventAction { get;  set; }
        public int Id { get; private set; }


        public EventHandler(Delegate action)
        {
            EventAction = action;
            Id = TallyEventHandlerCounter.GetEventHandlerId();
        }

        public void HandleEvent(object data)
        {
            if (EventAction == null) return;
            if(data == null)
            {
                var action = EventAction as Action;
                action();
            }
            else
            {
                var action = EventAction as Action<object>;
                action(data);
            }
        }

        public bool CheckMatch(Delegate action)
        {
            return action == EventAction;
        }
    }

    //public sealed class EventHandler<TData> : IEventHandler<TData>
    //{

    //    private Action<TData> EventAction { get; set; }

    //    public int Id { get; private set; }

    //    public EventHandler(Action<TData> action)
    //    {
    //        EventAction = action;
    //        Id = TallyEventHandlerCounter.GetEventHandlerId();
    //    }

    //    public void HandleEvent(TData eventData)
    //    {
    //        if (EventAction == null) return;
    //        EventAction(eventData);
    //    }

    //    public bool CheckMatch(Action<TData> action)
    //    {
    //        return action == EventAction;
    //    }
    //}

    //public sealed class EventHandler<T1,T2> : IEventHandler<T1,T2>
    //{

    //    private Action<T1, T2> EventAction { get; set; }

    //    public int Id { get; private set; }

    //    public EventHandler(Action<T1, T2> action)
    //    {
    //        EventAction = action;
    //        Id = TallyEventHandlerCounter.GetEventHandlerId();
    //    }

    //    public void HandleEvent(T1 eventData1, T2 eventData2)
    //    {
    //        if (EventAction == null) return;
    //        EventAction(eventData1, eventData2);
    //    }

    //    public bool CheckMatch(Action<T1, T2> action)
    //    {
    //        return action == EventAction;
    //    }
    //}

}

