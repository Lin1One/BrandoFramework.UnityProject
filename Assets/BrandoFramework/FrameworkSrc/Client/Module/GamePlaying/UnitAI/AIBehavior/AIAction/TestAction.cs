#region Head

// Author:            Yu
// CreateDate:        2018/10/24 21:32:21
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Client.GamePlaying.Unit;
using UnityEngine;

namespace Client.GamePlaying.AI
{
    /// <summary>
    /// 游戏实体AI行为
    /// </summary>
    public class TestAction : YuAIActionBase
    {
        public override void Init(object param)
        {
            Debug.Log("当前行为初始化");
        }

        public override void AddChild(YuAIBehaviorBase child)
        {
#if UNITY_EDITOR
            Debug.LogWarning("这是一个AI动作，不具备子节点：YuAIActionPatrol");
#endif
        }

        private int index = 0;
        private Vector3[] testPosition =
        {
            new Vector3(0,0,0),
            new Vector3(1,0,0),
            new Vector3(1,1,0),
            new Vector3(1,0,0)
        };


        public override void Release()
        {
            Debug.Log("当前行为释放");
        }

        protected override void Enter()
        {
            Debug.Log("当前行为Enter");
        }

        protected override void Exit()
        {
            Debug.Log("当前行为退出");
        }

        protected override AIBehaviorState Update()
        {
            //unit.U3DData.Trans.localPosition = (testPosition[index]);
            //unit.AnimaControl.PlayAnima("run");
            //index = (index + 1) % 4;
            Debug.Log("当前行为完成");
            return AIBehaviorState.Success;
        }
    }
}

