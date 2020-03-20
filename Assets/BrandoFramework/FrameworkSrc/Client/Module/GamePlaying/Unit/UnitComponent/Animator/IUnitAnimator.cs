#region Head

// Author:            Chengkefu
// CreateDate:        2019/04/25 16:20:00
// Email:             chengkefu0730@live.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using UnityEngine;

namespace Client.GamePlaying.Unit
{
    /// <summary>
    /// 动画控制组件接口
    /// </summary>
    public interface IYuUnitAnimator : IUnitComponent
    {
        void InitAnimator();
        
            /// <summary>
            /// 播放动画
            /// </summary>
            /// <param name="clipName"></param>
            /// <param name="forceSet"></param>
            /// <param name="speed"></param>
            /// <param name="fadeTime"></param>
            void UnitPlayAnima(string clipName, bool forceSet = false, float speed = 1.0f
            , float fadeTime = 0.05f);

        /// <summary>
        /// 播放指定动画片段
        /// </summary>
        /// <param name="clipName">动画片段名</param>
        /// <param name="forceSet">是否强制设为状态</param>
        /// <param name="machineName">状态机名</param>
        /// <param name="animaLenght">原始动画片长度</param>
        /// <param name="fadeTime">淡入淡出时间(0-1)</param>
        /// <param name="speed">播放速度</param>
        /// <param name="isLoop">是否重复播放</param>
        /// <param name="layer">动画层次</param>
        /// <param name="useCrossFade">是否快速混合到最后一帧</param>
        /// <returns></returns>
        bool PlayAnima(string clipName,
            bool forceSet = false,
            string machineName = null,
            float animaLenght = -1.0f,
            float fadeTime = 0.05f,
            float speed = 1.0f,
            bool isLoop = false,
            int layer = 0,
            bool useCrossFade = false);

        /// <summary>
        /// 加载动画资源
        /// </summary>
        /// <param name="controllerId"></param>
        /// <param name="anima"></param>
        void LoadAnimatorController(string controllerId, Animator anima = null);

        string CurAnimaName
        {
            get;
        }
    }
}

