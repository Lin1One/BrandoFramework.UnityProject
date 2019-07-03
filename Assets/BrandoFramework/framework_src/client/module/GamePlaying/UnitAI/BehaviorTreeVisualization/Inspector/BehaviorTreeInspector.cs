#region Head

// Author:            Yuzhou
// CreateDate:        2018/10/24 20:55:42
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Common;
using Common.Config;
using Common.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.GamePlaying.AI
{
    [Serializable]
    public class BehaviorTreeVisualization
    {
        private static BehaviorTreeVisualization instance;
        public static BehaviorTreeVisualization Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BehaviorTreeVisualization();
                }
                return instance;
            }
        }


        #region Editor

        [BoxGroup("当前设置", CenterLabel = true)]
        [LabelText("当前选中节点")]
        public BehaviorTreeBaseNode currentBehaviourNode;

        [BoxGroup("当前设置", CenterLabel = true)]
        [Button("添加新节点")]
        public void AddChildBehaviourNode(NodeType nodeType, object parme)
        {
            BehaviorTreeBaseNode behaviorNode = new BehaviorTreeBaseNode();
            
            switch (nodeType)
            {
                case NodeType.YuAIRepeat:
                    int times = (int)parme;
                    YuAIBehaviorBase behavior = new YuAIRepeat(times);
                    behaviorNode.BindBehaviour(behavior);
                    currentBehaviourNode.BindingBehaviour?.AddChild(behavior);
                    break;
                case NodeType.Simple:
                    var action = new UnitPatrolMoveAction();
                    action.Init(parme);
                    currentBehaviourNode?.BindingBehaviour?.AddChild(action);
                    behaviorNode.BindBehaviour(action);
                    break;

            }
            
            currentBehaviourNode.childNodes.Add(behaviorNode);
        }

        #endregion

        public BehaviorTreeBaseNode m_rootBehavior = new BehaviorTreeBaseNode ();

        public void BindBehaviourTree(YuAIBehaviorBase rootTreeBehaviour, BehaviorTreeBaseNode BehaviourNode)
        {
            var currentBehaviour = rootTreeBehaviour;
            var childrenBehaviour = currentBehaviour.GetChildren();
            if (childrenBehaviour != null)
            {
                foreach (var child in childrenBehaviour)
                {
                    var childNode = new BehaviorTreeBaseNode();
                    BindBehaviourTree(child, childNode);
                    childNode.BindBehaviour(child);
                    BehaviourNode.childNodes.Add(childNode);
                }
            }
        }


        public YuAIBehaviorTree CreateBehaviourTree()
        {
            YuAIBehaviorTree tree = new YuAIBehaviorTree(instance.m_rootBehavior.childNodes[0].BindingBehaviour);
            return tree;
        }

        [Button("保存行为树",buttonSize: ButtonSizes.Medium)]
        public void SaveCurrentHaviourTree()
        {
            var writePath = $"{Application.streamingAssetsPath}/{ProjectInfoDati.GetActualInstance().DevelopProjectName}/Config/BehaviourTreeData.byte";
            SerializeUtility.SerializeAndWriteTo(instance, writePath);
        }

        [Button("加载行为树", buttonSize: ButtonSizes.Medium)]
        public YuAIBehaviorTree LoadCurrentHaviourTree(string behaviourId)
        {
            var writePath = $"{Application.streamingAssetsPath}/{ProjectInfoDati.GetActualInstance().DevelopProjectName}/Config/BehaviourTreeData.byte";
            var newInstance = SerializeUtility.DeSerialize<BehaviorTreeVisualization>(writePath);
            instance = newInstance;
            return CreateBehaviourTree();
        }

    }

    public enum NodeType
    {
        Composite,  //复合行为
        Condition,  //条件行为
        Decorator,  //修饰行为
        
        Simple,      //简单行为
        YuAIRepeat  //重复器
    }
}

