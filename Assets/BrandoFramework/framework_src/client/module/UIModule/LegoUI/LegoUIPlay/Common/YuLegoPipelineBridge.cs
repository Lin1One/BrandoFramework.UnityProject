using Common;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Client.LegoUI
{
    /// <summary>
    /// 乐高UI生命周期桥接器。
    /// 提供目标UI外部生命周期处理委托的获取和存储。
    /// </summary>
    public interface IYuLegoPipelineBridge
    {
        List<Action<IYuLegoUI>> GetPipelineAction(string id, YuUIPipelineType pipelineType);

        void Register(string id, YuUIPipelineType pipelineType, Action<IYuLegoUI> callback);

        void Remove(string id, YuUIPipelineType pipelineType, Action<IYuLegoUI> callback);
    }

    [Singleton]
    public class YuLegoPipelineBridge : IYuLegoPipelineBridge
    {
        private Dictionary<string, Dictionary<YuUIPipelineType, List<Action<IYuLegoUI>>>> pipelineActions;

        private Dictionary<string, Dictionary<YuUIPipelineType, List<Action<IYuLegoUI>>>> PipelineActions
        {
            get
            {
                if (pipelineActions != null)
                {
                    return pipelineActions;
                }

                pipelineActions = new Dictionary<string, Dictionary<YuUIPipelineType, List<Action<IYuLegoUI>>>>();
                return pipelineActions;
            }
        }

        public List<Action<IYuLegoUI>> GetPipelineAction(string id, YuUIPipelineType pipelineType)
        {
            if (!PipelineActions.ContainsKey(id))
            {
#if DEBUG
                Debug.LogError($"目标UI{id}没有生命周期委托存在！");
#endif
                return null;
            }

            var uiActions = PipelineActions[id];
            if (uiActions.ContainsKey(pipelineType))
            {
#if DEBUG
                Debug.LogError($"目标UI{id}的生命周期{pipelineType}没有对应的委托存在！");
#endif
                return null;
            }

            var actions = uiActions[pipelineType];
            return actions;
        }

        public void Register(string id, YuUIPipelineType pipelineType, Action<IYuLegoUI> callback)
        {
            if (!PipelineActions.ContainsKey(id))
            {
                PipelineActions.Add(id, new Dictionary<YuUIPipelineType, List<Action<IYuLegoUI>>>());
            }

            var uiActions = PipelineActions[id];
            if (!uiActions.ContainsKey(pipelineType))
            {
                uiActions.Add(pipelineType, new List<Action<IYuLegoUI>>());
            }

            var actions = uiActions[pipelineType];
            if (!actions.Contains(callback))
            {
                actions.Add(callback);
            }
        }

        public void Remove(string id, YuUIPipelineType pipelineType, Action<IYuLegoUI> callback)
        {
            if (!PipelineActions.ContainsKey(id))
            {
                return;
            }

            var uiActions = PipelineActions[id];
            if (!uiActions.ContainsKey(pipelineType))
            {
                return;
            }

            var actions = uiActions[pipelineType];
            if (!actions.Contains(callback))
            {
                return;
            }

            actions.Remove(callback);
        }
    }
}