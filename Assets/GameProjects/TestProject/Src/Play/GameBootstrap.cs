#region Head

// Author:           Yuzhou
// CreateDate:    2018/4/18 23:16:56

#endregion

using Client.Assets;
using Client.GamePlaying.AI;
using Client.GamePlaying.Unit;
using client_common;
using client_module_event;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class GameBootstrap : MonoBehaviour
    {
        protected virtual void Awake()
        {
            
            Bootstrap();
        }

        #region OdinGUI可视化配置

        [LabelText("启动的应用Id列表最后一个默认为当前运行应用")]
        [SerializeField]
        [FoldoutGroup("运行应用列表")]
        public List<string> StartAppIds = new List<string>();

        [HideLabel]
        [SerializeField]
        [FoldoutGroup("运行时配置")]
        public YuAppsRunSetting RunSetting;

        #endregion

        #region 启动

        protected void Bootstrap()
        {
            Mapping();
            LoadAppSettingDateBeforeBootstrap();
            StartGame();
        }


        /// <summary>
        /// 基础映射。
        /// </summary>
        protected virtual void Mapping()
        {
            var injector = Injector.Instance;
            injector.Mapping<IU3DEventModule, U3DEventModule>();
            injector.Mapping<IAssetModule, AssetModule>();

            injector.Mapping<IAssetInfoHelper, AssetInfoHelper>();
        }

        /// <summary>
        /// 启动应用。
        /// 调用指定应用模块的动态入口。
        /// </summary>
        protected virtual void StartGame()
        {
            var unitModule = Injector.Instance.Get<UnitModule>();
            unitModule.RegistUintType<UnitEntityTest>();
            var unit = unitModule.CreateUnit<UnitEntityTest>(
                null, 1, UnitType.Monster, "abao_model", 
                (newUnit) =>
                {
                    newUnit.AnimaControl.InitAnimator();
                    YuAIBehaviorTreeBuilder builder = new YuAIBehaviorTreeBuilder();
                    builder.SetAISubject(newUnit);


                    var unitBehaviourTree = BehaviorTreeVisualization.Instance.LoadCurrentHaviourTree("");
                //    builder.
                //    Repeat().
                //        Selector().                           //主动选择
                //        Sequence().                             //顺序器
                //            Condition<UnitDistanceCondition>(new Vector2(0, 2), false).
                //                Back().
                //            Action<UnitPatrolMoveAction>(1).
                //                Back().
                //            Back().
                //        Action<UnitUseSkillAction>(2).
                //            Back().
                //   Back().
                //End();
                    newUnit.AIControl.ResetBehaviorTree(unitBehaviourTree);

                    //BehaviorTreeVisualization.Instance.BindBehaviourTree(unitBehaviourTree.m_rootBehavior, 
                    //    BehaviorTreeVisualization.Instance.m_rootBehavior);
                    GameObject.Find("Main Camera").transform.SetParent(newUnit.U3DData.Trans);
                },
                true);

        }

        private void DebugForAI()
        {
            Debug.Log("1111");
        }

        #endregion



        /// <summary>
        /// 加载配置数据
        /// </summary>
        protected virtual void LoadAppSettingDateBeforeBootstrap()
        {
            
        }


        #region 清理（仅编辑器下）

#if UNITY_EDITOR
        private void OnApplicationQuit()
        {
        }
#endif

        #endregion


        
    }
}