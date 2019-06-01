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

        public BehaviorTreeBaseNode m_rootBehavior = new BehaviorTreeBaseNode ();

        public void BindBehaviourTree(YuAIBehaviorBase rootTreeBehaviour, BehaviorTreeBaseNode BehaviourNode)
        {
            var currentBehaviour = rootTreeBehaviour;
            var childrenBehaviour = currentBehaviour.GetChildren();
            if (childrenBehaviour != null)
            {
                foreach(var child in childrenBehaviour)
                {
                    var childNode = new BehaviorTreeBaseNode();
                    BindBehaviourTree(child, childNode);
                    childNode.BindBehaviour(child);
                    BehaviourNode.childNodes.Add(childNode);
                }
            }
        }

         
    }
}

