#region Head

// Author:            ChengKeFu
// CreateDate:        2018/10/25 20:49:22
// Email:             chengkefu0730@live.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Client.GamePlaying.Unit;
using System.Collections.Generic;
using UnityEngine;

namespace Client.GamePlaying.AI
{
    /// <summary>
    /// 行为树构建器
    /// </summary>
    public class YuAIBehaviorTreeBuilder 
    {
        private UnitEntityBase unitEntity;      //AI主体
        private YuAIBehaviorBase rootBehavior;    //根行为节点
        Stack<YuAIBehaviorBase> behaviorNodes = 
            new Stack<YuAIBehaviorBase>();  //行为栈

        //添加一个行为到栈
        private void AddBehavior(YuAIBehaviorBase behavior)
        {
            if(unitEntity == null)
            {
#if UNITY_EDITOR
                Debug.LogError("错误：未设置AI主体");
#endif
            }

            if(rootBehavior ==null)
            {
                rootBehavior = behavior;
            }
            else
            {
                //栈顶的行为，尝试添加新行为为子子行为
                behaviorNodes.Peek().AddChild(behavior); 
            }

            behaviorNodes.Push(behavior);
        }


        #region 链式 各种行为的创建、退出接口

        /// <summary>
        /// 设置AI的主体,一直持续到End()
        /// </summary>
        /// <param name="role"></param>
        public void SetAISubject(UnitEntityBase role)
        {
            unitEntity = role;
        }

        /// <summary>
        /// 回退一级行为
        /// </summary>
        /// <returns></returns>
        public YuAIBehaviorTreeBuilder Back()
        {
            behaviorNodes.Pop();
            return this;
        }

        /// <summary>
        /// 结束行为创建
        /// </summary>
        /// <returns></returns>
        public YuAIBehaviorTree End()
        {
            while (behaviorNodes.Count > 0)
            {
                behaviorNodes.Pop();
            }
            YuAIBehaviorTree tree = new YuAIBehaviorTree(rootBehavior);
            //初始化行为树创建器
            unitEntity = null;
            rootBehavior = null;

            return tree;
        }

        /// <summary>
        /// 创建一个顺序器
        /// 依次执行子行为，直到一个失败、或是全执行完则结束
        /// </summary>
        /// <returns></returns>
        public YuAIBehaviorTreeBuilder Sequence()
        {
            YuAIBehaviorBase behavior = new YuAISequence();
            AddBehavior(behavior);
            return this;
        }

        /// <summary>
        /// 创建一个选择器
        /// </summary>
        /// <returns></returns>
        public YuAIBehaviorTreeBuilder Selector()
        {
            YuAIBehaviorBase behavior = new YuAISelector();
            AddBehavior(behavior);
            return this;
        }

        /// <summary>
        /// 创建一个装饰器：重复执行
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public YuAIBehaviorTreeBuilder Repeat(uint times = 0)
        {
            YuAIBehaviorBase behavior = new YuAIRepeat((int)times);
            AddBehavior(behavior);
            return this;
        }

        /// <summary>
        /// 创建一个主动选择器
        /// </summary>
        /// <returns></returns>
        public YuAIBehaviorTreeBuilder ActiveSelector()
        {
            YuAIBehaviorBase behavior = new YuAIActiveSelector();
            AddBehavior(behavior);
            return this;
        }

        /// <summary>
        /// 创建一个并行器
        /// </summary>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        /// <returns></returns>
        public YuAIBehaviorTreeBuilder Parallel(
            YuAIParallel.EPolicy success, YuAIParallel.EPolicy failure)
        {
            YuAIBehaviorBase behavior = new YuAIParallel(success, failure);
            AddBehavior(behavior);
            return this;
        }

        /// <summary>
        /// 创建一个监视器
        /// </summary>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        /// <returns></returns>
        public YuAIBehaviorTreeBuilder Monitor(
            YuAIParallel.EPolicy success, YuAIParallel.EPolicy failure)
        {
            YuAIBehaviorBase behavior = new YuAIMonitor(success, failure);
            AddBehavior(behavior);
            return this;
        }

        #region 泛型创建条件、动作

        /// <summary>
        /// 根据类型创建一个动作
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="param">动作子类对应的Init参数</param>
        /// <returns>行为构建器</returns>
        public YuAIBehaviorTreeBuilder Action<T>(object param)  
            where T : YuAIActionBase, new()
        {
            YuAIActionBase behavior = new T();
            behavior.SetRole(unitEntity);
            behavior.Init(param);
            AddBehavior(behavior);
            return this;
        }

        /// <summary>
        /// 根据类型创建一个条件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="param">条件子类对应的Init参数</param>
        /// <param name="isNegation">是否反转条件</param>
        /// <returns>行为构建器</returns>
        public YuAIBehaviorTreeBuilder Condition<T>(object param,bool isNegation) 
            where T : YuAIConditionBase, new()
        {
            YuAIConditionBase behavior = new T();
            behavior.SetRoleAndIsNegation(unitEntity, isNegation);
            behavior.Init(param);
            AddBehavior(behavior);
            return this;
        }


        #endregion

        #endregion
    }
}

