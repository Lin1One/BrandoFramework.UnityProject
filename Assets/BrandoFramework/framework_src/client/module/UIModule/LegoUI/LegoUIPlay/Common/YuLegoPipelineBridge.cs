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
        List<Action<ILegoUI>> GetPipelineAction(string id, UIPipelineType pipelineType);

        void Register(string id, UIPipelineType pipelineType, Action<ILegoUI> callback);

        void Remove(string id, UIPipelineType pipelineType, Action<ILegoUI> callback);
    }

    [Singleton]
    public class YuLegoPipelineBridge : IYuLegoPipelineBridge
    {
        private Dictionary<string, Dictionary<UIPipelineType, List<Action<ILegoUI>>>> pipelineActions;

        private Dictionary<string, Dictionary<UIPipelineType, List<Action<ILegoUI>>>> PipelineActions
        {
            get
            {
                if (pipelineActions != null)
                {
                    return pipelineActions;
                }

                pipelineActions = new Dictionary<string, Dictionary<UIPipelineType, List<Action<ILegoUI>>>>();
                return pipelineActions;
            }
        }

        public List<Action<ILegoUI>> GetPipelineAction(string id, UIPipelineType pipelineType)
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

        public void Register(string id, UIPipelineType pipelineType, Action<ILegoUI> callback)
        {
            if (!PipelineActions.ContainsKey(id))
            {
                PipelineActions.Add(id, new Dictionary<UIPipelineType, List<Action<ILegoUI>>>());
            }

            var uiActions = PipelineActions[id];
            if (!uiActions.ContainsKey(pipelineType))
            {
                uiActions.Add(pipelineType, new List<Action<ILegoUI>>());
            }

            var actions = uiActions[pipelineType];
            if (!actions.Contains(callback))
            {
                actions.Add(callback);
            }
        }

        public void Remove(string id, UIPipelineType pipelineType, Action<ILegoUI> callback)
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