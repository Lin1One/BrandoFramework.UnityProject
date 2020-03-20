#region Head

// Author:           Yuzhou
// CreateDate:    2018/4/18 23:16:56

#endregion

using Common;
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

#if UNITY_EDITOR
        private void OnApplicationQuit()
        {
        }
#endif

        #region 启动器功能

        protected void Bootstrap()
        {
            Mapping();
            InitModuleAtStart();
            ApplyAppAndModuleConfig();
            StartGame();
        }


        /// <summary>
        /// 模块接口映射。
        /// </summary>
        protected virtual void Mapping()
        {
        }

        /// <summary>
        /// 启动时模块初始化
        /// </summary>
        protected virtual void InitModuleAtStart()
        {

        }

        /// <summary>
        /// 加载配置数据
        /// </summary>
        protected virtual void ApplyAppAndModuleConfig()
        {

        }

        /// <summary>
        /// 启动应用。
        /// 调用指定应用模块的动态入口。
        /// </summary>
        protected virtual void StartGame()
        {
        }

        #endregion

        #region 可视化配置

        //[LabelText("启动的应用Id列表最后一个默认为当前运行应用")]
        //[SerializeField]
        //[FoldoutGroup("运行应用列表")]
        //public List<string> StartAppIds = new List<string>();

        [HideLabel]
        [SerializeField]
        //[FoldoutGroup("游戏启动配置")]
        public BootstrapConfig gameBootstrapConfig;

        #endregion

    }
}