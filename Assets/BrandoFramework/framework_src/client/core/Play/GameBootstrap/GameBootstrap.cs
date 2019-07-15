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

        #region Unity 生命周期
        protected virtual void Awake()
        {
            Bootstrap();
        }

        protected void Bootstrap()
        {
            Mapping();
            LoadAppSettingDateBeforeBootstrap();
            StartGame();
        }


        /// <summary>
        /// 模块映射。
        /// </summary>
        protected virtual void Mapping()
        {
        }

        /// <summary>
        /// 加载配置数据
        /// </summary>
        protected virtual void LoadAppSettingDateBeforeBootstrap()
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




        #region 清理（仅编辑器下）

#if UNITY_EDITOR
        private void OnApplicationQuit()
        {
        }
#endif

        #endregion


        
    }
}