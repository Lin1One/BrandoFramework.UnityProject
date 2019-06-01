#region Head

// Author:            Chengkefu
// CreateDate:        2018/10/10 20:01:23
// Email:             chengkefu0730@live.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Client.Assets;
using client_common;
using System;
using UnityEngine;

namespace Client.GamePlaying.Unit
{
    /// <summary>
    /// 动画控制类，除了作为角色的组件，也可以通用管理动画
    /// </summary>
    public class XTwoUnitAnimator : UnitComponent, IYuUnitAnimator
    {
        private Animator m_animator;
        RuntimeAnimatorController m_controlAssetRef;       //管理动画控制器资源



        private string m_curPlayClip;
        /// <summary>
        /// 当前正在播放的动画片段
        /// </summary>
        public string CurAnimaName
        {
            get
            {
                return m_curPlayClip;
            }
        }
        
        private string m_lastClip;      //计时动画中上一个动画的名称
        private float m_speed;
        private int m_eventId = -1;

        //private IYuTimer m_timer;       //用于判断计时动画中，最后一个有效计时

        private float m_runCycle;

        public XTwoUnitAnimator()
        {

        }

        protected override void OnInit()
        {
            m_animator = Role.U3DData.Trans?.GetComponent<Animator>();

            if (m_animator == null)
            {
                return;
            }
            //            SetAnima(controllerId, anima, (yuAnima) =>
            //            {

            //                bool isFind = false;
            //                foreach (var clip in anima.runtimeAnimatorController.animationClips)
            //                {
            //                    if (clip.name == "run")
            //                    {
            //                        m_runCycle = clip.length;
            //                        isFind = true;
            //                        break;
            //                    }
            //                }
            //#if UNITY_EDITOR
            //                if (!isFind)
            //                {
            //                    Debug.LogError("角色的动画控制器未找到run动画片段");
            //                }
            //#endif
            //            });

        }

        public void InitAnimator()
        {
            m_animator = Role.U3DData.Trans?.GetComponent<Animator>();

            if (m_animator == null)
            {
                return;
            }
        }

        //private static IYuU3DEventModule s_eventModule;
        //private static IYuU3DEventModule EventModule
        //{
        //    get
        //    {
        //        if (s_eventModule == null)
        //        {
        //            s_eventModule = YuU3dAppUtility.Injector.Get<IYuU3DEventModule>();
        //        }
        //        return s_eventModule;
        //    }
        //}

        //private static IYuTimerModule s_timerModule;
        //private static IYuTimerModule TimerModule
        //{
        //    get
        //    {
        //        if (s_timerModule == null)
        //        {
        //            s_timerModule = YuU3dAppUtility.Injector.Get<IYuTimerModule>();
        //        }
        //        return s_timerModule;
        //    }
        //}



        public void SetAnima(string controllerId, Animator animator, Action<XTwoUnitAnimator> callback = null)
        {
            m_animator = animator;
            m_animator.applyRootMotion = false;
            m_speed = 1.0f;

            if (!string.IsNullOrEmpty(controllerId))
            {
               Injector.Instance.Get<IAssetModule>().LoadAsync<RuntimeAnimatorController>(controllerId,
                    assetRef =>
                    {
                        if (assetRef != null)
                        {
                            m_controlAssetRef = assetRef;
                            m_animator.runtimeAnimatorController = assetRef;
                            Speed = m_speed;

                            if (!string.IsNullOrEmpty(m_curPlayClip))
                            {
                                PlayAnima(m_curPlayClip, true);
                            }
                            if (callback != null)
                            {
                                callback(this);
                            }
                        }
                    });
            }
        }

        public XTwoUnitAnimator(string controllerId, Animator animator, Action<XTwoUnitAnimator> callback = null)
        {
            m_animator = animator;
            m_animator.applyRootMotion = false;
            m_speed = 1.0f;

            if (!string.IsNullOrEmpty(controllerId))
            {
                Injector.Instance.Get<IAssetModule>().LoadAsync<RuntimeAnimatorController>(controllerId,
                    assetRef =>
                    {
                        if (assetRef != null && m_animator != null)
                        {
                            m_controlAssetRef = assetRef;
                            m_animator.runtimeAnimatorController = assetRef;
                            Speed = m_speed;

                            if (!string.IsNullOrEmpty(m_curPlayClip))
                            {
                                PlayAnima(m_curPlayClip, true);
                            }
                            if (callback != null)
                            {
                                callback(this);
                            }
                        }
                    });
            }

        }

        private void RegistEvent()
        {
            //if (m_eventId > 0)
            //{
            //    EventModule.RemoveSpecifiedHandler(YuUnityEventCode.Scene_SetActive, m_eventId);
            //    m_eventId = -1;
            //}

            //m_eventId = EventModule.WatchEvent(YuUnityEventCode.Scene_SetActive, (param) =>
            //{
            //    bool isActive = (bool)param;
            //    if (isActive)
            //    {
            //        RefreshAnima();
            //    }
            //});
        }

        private void RemoveEvent()
        {
            //if (m_eventId > 0)
            //{
            //    EventModule.RemoveSpecifiedHandler(YuUnityEventCode.Scene_SetActive, m_eventId);
            //    m_eventId = -1;
            //}
        }

        protected override void OnRelease()
        {
            RemoveEvent();

            m_curPlayClip = null;
            m_controlAssetRef = null;
            m_animator = null;
            m_lastClip = null;
            //m_timer = null;
        }

        public float Speed
        {
            get
            {
                return m_speed;
            }
            private set
            {
                m_speed = value;
                if (m_animator != null)
                {
                    m_animator.speed = m_speed;
                }
            }
        }

        /// <summary>
        /// 持有动画组件的模型GameObject被激活显示时，
        /// 会播放默认动画，
        /// 所以要调用此接口，播放动画组件记录动画
        /// </summary>
        public void RefreshAnima()
        {
            UnitPlayAnima(m_curPlayClip, true);
        }

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
        public bool PlayAnima(string clipName,
            bool forceSet = false,
            string machineName = null,
            float animaLenght = -1.0f,
            float fadeTime = 0.05f,
            float speed = 1.0f,
            bool isLoop = false,
            int layer = 0,
            bool useCrossFade = false)
        {
            if (m_animator == null)
            {
                m_curPlayClip = clipName;
                m_lastClip = clipName;
                return false;
            }

            if (m_curPlayClip == clipName && !forceSet)
            {
                return false;
            }

            if (animaLenght > 0)  //计时动画，
            {
                //m_lastClip = m_curPlayClip;
                //m_timer = TimerModule.GetOnceTimer(animaLenght, (timer) =>
                // {
                //     if (m_timer != timer || m_lastClip == null)
                //     {
                //         return;
                //     }
                //     PlayAnima(m_lastClip);
                // });
                //m_timer.Start();
            }
            else
            {
                m_lastClip = null;
            }

            m_curPlayClip = clipName;

            if (m_animator.gameObject.activeInHierarchy)
            {
                if (useCrossFade)
                {
                    m_animator.CrossFade(clipName, 0, layer, 1.0f);
                }
                else
                {
                    m_animator.CrossFadeInFixedTime(clipName, fadeTime, layer, 0.0f);
                }
            }

            Speed = speed;

            return true;
        }

        /// <summary>
        /// 角色播放动画,只有身为角色组件的对象了，才能调用此接口
        /// </summary>
        /// <param name="clipName"></param>
        /// <param name="forceSet"></param>
        /// <param name="speed"></param>
        public void UnitPlayAnima(string clipName, bool forceSet = false, float speed = 1.0f
            , float fadeTime = 0.05f)
        {
            if (Role == null)
            {
                return;
            }

            //var mount = Role.GetComponent<XTwoUnitMount>();
            //var pendant = Role.GetComponent<XTwoUnitPendantManager>();

            //if (mount != null && mount.IsOnMount
            //    && mount.Trans != null)       //坐骑状态
            //{
            //    if (clipName == "run" || clipName == "idle")
            //    {
            //        mount.PlayAnima(clipName, forceSet, speed);
            //        PlayAnima(string.Format("qicheng_{0}", clipName),
            //            forceSet, null, -1, fadeTime, speed);
            //        return;
            //    }
            //}
            //else if (pendant != null && Role.PendantManager.IsShowWings)  //戴翅膀状态
            //{
            //    if (clipName == "run" || clipName == "idle")
            //    {
            //        PlayAnima(string.Format("fukong_{0}", clipName),
            //            forceSet, null, -1, fadeTime, speed);
            //        return;
            //    }
            //}
            //普通

            PlayAnima(clipName, forceSet, null, -1, fadeTime, speed);
        }

        /// <summary>
        /// 加载动画资源,只有身为角色组件的对象了，才能调用此接口
        /// </summary>
        /// <param name="controllerId"></param>
        /// <param name="anima"></param>
        public void LoadAnimatorController(string controllerId, Animator anima = null)
        {
            if (anima == null)
            {
                anima = Role.U3DData.Trans != null ?
                    Role.U3DData.Trans.GetComponent<Animator>() : null;

                if (anima == null)
                {
                    return;
                }
            }

            SetAnima(controllerId, anima, (yuAnima) =>
            {

                bool isFind = false;
                foreach (var clip in anima.runtimeAnimatorController.animationClips)
                {
                    if (clip.name == "run")
                    {
                        m_runCycle = clip.length;
                        isFind = true;
                        break;
                    }
                }
#if UNITY_EDITOR
                    if (!isFind)
                {
                    Debug.LogError("角色的动画控制器未找到run动画片段");
                }
#endif
                });

        }
    }
}

