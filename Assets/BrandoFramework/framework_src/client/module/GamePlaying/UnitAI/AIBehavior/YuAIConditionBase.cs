#region Head

// Author:            Yu
// CreateDate:        2018/10/24 21:38:25
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Client.GamePlaying.Unit;

namespace Client.GamePlaying.AI
{
    /// <summary>
    /// AI行为条件基类
    /// </summary>
    public abstract class YuAIConditionBase :YuAIBehaviorBase
    {
        protected UnitEntityBase unitEntity;    //行为主体
        protected bool m_isNegation = false;    //是否取反

        public void SetRoleAndIsNegation(UnitEntityBase unit, bool isNegation)
        {
            unitEntity = unit;
            m_isNegation = isNegation;
        }

        public override void AddChild(YuAIBehaviorBase child)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning("这是一个AI条件，不具备子节点：YuAIActionPatrol");
#endif
        }

        public abstract void Init(object param);

        //注意派生类不能再继承Update()，而要继承OnUpdate();
        protected override AIBehaviorState Update()
        {
            AIBehaviorState state = OnUpdate();

            if(m_isNegation)
            {
                if(state == AIBehaviorState.Failure)
                {
                    return AIBehaviorState.Success;
                }
                if (state == AIBehaviorState.Success)
                {
                    return AIBehaviorState.Failure;
                }
            }
            return state;
        }

        protected abstract AIBehaviorState OnUpdate();
    }
}
