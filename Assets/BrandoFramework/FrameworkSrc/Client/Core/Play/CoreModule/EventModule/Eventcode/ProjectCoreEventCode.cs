namespace Client.Core
{
    /// <summary>
    /// 该类以静态实例的形式提供常用的Unity核心事件码。
    /// </summary>
    public static class ProjectCoreEventCode
    {
        #region 测试

        /// <summary>
        /// 用于测试只执行一次的事件。
        /// </summary>
        public static readonly EventCode Test_ExexuteOne
            = EventFactory.GetEventCode("Test", "ExecuteOne");

        /// <summary>
        /// 用于测试无限执行的事件。
        /// </summary>
        public static readonly EventCode Test_ExecuteTen
            = EventFactory.GetEventCode("Test", "ExecuteTen");

        #endregion

        #region AppEntity

        /// <summary>
        /// Unity应用初始化完成。
        /// </summary>
        public static readonly EventCode AppInited
            = EventFactory.GetEventCode("AppEntity", "Inited");

        /// <summary>
        /// 切换当前应用。
        /// </summary>
        public static readonly EventCode AppSwitchCurrentApp
            = EventFactory.GetEventCode("AppEntity", "SwitchCurrentApp");

        /// <summary>
        /// 当在安卓环境下使用APK整包更新时，用于接受APK的下载进度。
        /// </summary>
        public static readonly EventCode AppApkDownProgressUpdate
            = EventFactory.GetEventCode("AppEntity", "ApkDownProgressUpdate");

        /// <summary>
        /// Unity应用当前的多语言设置发生改变。
        /// </summary>
        public static readonly EventCode AppIl8nChanged
            = EventFactory.GetEventCode("AppEntity", "Il8nChanged");

        /// <summary>
        /// 返回初始化状态。
        /// </summary>
        public static readonly EventCode AppReturnInitState
            = EventFactory.GetEventCode("AppEntity", "ReturnInitState");

        /// <summary>
        /// 应用处于新手引导状态时发生了交互行为。
        /// 点击、拖拽、滑动等。
        /// </summary>
        public static readonly EventCode AppHappeninteractAtGuideState
            = EventFactory.GetEventCode("AppEntity", "HappeninteractAtGuideState");

        /// <summary>
        /// 热更新已完成。
        /// 各模块实例可以观察该事件，用于模块在热更完成时更新自身状态。
        /// </summary>
        public static readonly EventCode AppHotUpdateCompeted
            = EventFactory.GetEventCode("AppEntity", "HotUpdateCompeted");

        /// <summary>
        /// 应用退出。
        /// </summary>
        public static readonly EventCode AppQuit
            = EventFactory.GetEventCode("AppEntity", "Quit");

        /// <summary>
        /// 应用的背景音乐音量改变。
        /// </summary>
        public static readonly EventCode App_MusicVolumeChange
            = EventFactory.GetEventCode("AppEntity", "MusicVolumeChange");

        /// <summary>
        /// 应用的背景音乐开启状态改变。
        /// </summary>
        public static readonly EventCode App_MusicStateChange
            = EventFactory.GetEventCode("AppEntity", "MusicStateChange");

        /// <summary>
        /// 应用的音效音量改变。
        /// </summary>
        public static readonly EventCode App_SoundVolumeChange
            = EventFactory.GetEventCode("AppEntity", "SoundVolumeChange");

        /// <summary>
        /// 应用的音效开启状态改变。
        /// </summary>
        public static readonly EventCode App_SoundStateChange
            = EventFactory.GetEventCode("AppEntity", "SoundStateChange");

        /// <summary>
        /// 应用的语音音量大小改变。
        /// </summary>
        public static readonly EventCode App_VoiceVolumeChange
            = EventFactory.GetEventCode("AppEntity", "SoundVolumeChange");

        /// <summary>
        /// 应用的语音开启状态改变。
        /// </summary>
        public static readonly EventCode App_VoiceStateChange
            = EventFactory.GetEventCode("AppEntity", "SoundStateChange");

        #endregion

        #region 视图

        /// <summary>
        /// 加载界面执行进度更新。
        /// </summary>
        public static readonly EventCode ViewLoadingUpdate
            = EventFactory.GetEventCode("View", "LoadingUpdate");

        /// <summary>
        /// 打开加载界面。
        /// </summary>
        public static readonly EventCode View_OpenLoading
            = EventFactory.GetEventCode("View", "OpenLoading");

        /// <summary>
        /// 完成加载
        /// </summary>
        public static readonly EventCode View_CompleteLoading
            = EventFactory.GetEventCode("View", "CompleteLoading");

        /// <summary>
        /// 进入主界面。
        /// </summary>
        public static readonly EventCode View_EnterMain
            = EventFactory.GetEventCode("View", "EnterMain");

        /// <summary>
        /// 界面创建完成
        /// </summary>
        public static readonly EventCode View_Created
            = EventFactory.GetEventCode("View", "Created");

        
        #endregion

        #region 通信

        /// <summary>
        /// 应用首次建立网络连接。
        /// </summary>
        public static readonly EventCode NetOnFirstConnected
            = EventFactory.GetEventCode("Net", "OnFirstConnected");

        /// <summary>
        /// 应用已重新建立网络连接。
        /// </summary>
        public static readonly EventCode NetReConnected
            = EventFactory.GetEventCode("Net", "ReConnected");

        /// <summary>
        /// 网络连接成功。
        /// </summary>
        public static readonly EventCode NetConnectSucceed
            = EventFactory.GetEventCode("Net", "ConnectSucceed");

        /// <summary>
        /// 尝试重连。
        /// </summary>
        public static readonly EventCode NetTryReConnect
            = EventFactory.GetEventCode("Net", "TryReConnect");

        /// <summary>
        /// 当重连尝试次数到达最大次数时放弃重连。
        /// </summary>
        public static readonly EventCode NetGiveupReConnect
            = EventFactory.GetEventCode("Net", "GiveupReConnect");

        /// <summary>
        /// 有网络消息到达。
        /// </summary>
        public static readonly EventCode NetReceiveMessage
            = EventFactory.GetEventCode("Net", "ReceiveMessage");

        /// <summary>
        /// 有发送消息（客户端请求）加入队列。
        /// </summary>
        public static readonly EventCode NetSendMessageEnqueue
            = EventFactory.GetEventCode("Net", "SendMessageEnqueue");

        /// <summary>
        /// 网络连接超时。
        /// </summary>
        public static readonly EventCode NetConnectTimeOut
            = EventFactory.GetEventCode("Net", "ConnectTimeOut");

        /// <summary>
        /// 网络连接断开。
        /// </summary>
        public static readonly EventCode NetDisconnect
            = EventFactory.GetEventCode("Net", "Disconnect");

        /// <summary>
        /// 发生通信错误。
        /// </summary>
        public static readonly EventCode NetHappenError
            = EventFactory.GetEventCode("Net", "HappenError");

        /// <summary>
        /// 开启网络消息处理。
        /// </summary>
        public static readonly EventCode Net_EnableMessageHandle
            = EventFactory.GetEventCode("Net", "EnableMessageHandle");

        /// <summary>
        /// 暂停网络消息处理，该事件用于在某些资源加载比较耗时，
        /// 后续的网络相关的业务逻辑处理依赖于正在加载的资源的情况。
        /// 当有这种情况发生时，可暂停网络消息处理以等待资源加载完成。
        /// </summary>
        public static readonly EventCode Net_PauseMessageHandle
            = EventFactory.GetEventCode("Net", "PauseMessageHandle");

        /// <summary>
        /// 心跳回复
        /// </summary>
        public static readonly EventCode Net_HeartTick
            = EventFactory.GetEventCode("Net", "HeartTick");

        #endregion

        #region 按键

        /// <summary>
        /// 回退键被按下。
        /// </summary>
        public static readonly EventCode Key_EscapeDown
            = EventFactory.GetEventCode("Key", "EscapeDown");

        #endregion

        #region 触摸

        #endregion

        #region 场景

        /// <summary>
        /// 算出寻路路径点
        /// 参数：List<Vector2>
        /// </summary>
        public static readonly EventCode Scene_FindPathPoints
            = EventFactory.GetEventCode("Scene", "FindPathPoints");

        /// <summary>
        /// 主角移动事件
        /// 参数：Vector3
        /// </summary>
        public static readonly EventCode Scene_SelfOnMove
            = EventFactory.GetEventCode("Scene", "SelfOnMove");

        /// <summary>
        /// 主角移动跨格事件  
        /// 参数：Point2
        /// </summary>
        public static readonly EventCode Scene_SelfOverCell
            = EventFactory.GetEventCode("Scene", "SelfOverCell");

        /// <summary>
        /// 操作模块的转向事件
        /// 参数：Vector3
        /// </summary>
        public static readonly EventCode Operat_Rotate
            = EventFactory.GetEventCode("Operat", "Rotate");

        /// <summary>
        /// 地图打开完成事件
        /// 参数：int
        /// </summary>
        public static readonly EventCode Scene_OpenMap
            = EventFactory.GetEventCode("Scene", "OpenMap");

        /// <summary>
        /// 点击游戏实体事件
        /// 参数：Collider
        /// </summary>
        public static readonly EventCode Scene_ClickAtUnit
            = EventFactory.GetEventCode("Scene", "ClickAtUnit");

        /// <summary>
        /// 生成采集物品事件
        /// 参数：int
        /// </summary>
        public static readonly EventCode Scene_CreatCollectionItem
            = EventFactory.GetEventCode("Scene", "CreatCollectionItem");

        /// <summary>
        /// 完成场景互动事件
        /// 参数：string(互动名)
        /// </summary>
        public static readonly EventCode Scene_InteractionDone
            = EventFactory.GetEventCode("Scene", "InteractionDone");

        /// <summary>
        /// 场景互动事件执行失败
        /// 参数：string(互动名)
        /// </summary>
        public static readonly EventCode Scene_InteractionFail
            = EventFactory.GetEventCode("Scene", "InteractionFail");

        /// <summary>
        /// 寻路行为结束
        /// 参数：bool(是否达到目的地)  bool(是否是主角)
        /// </summary>
        public static readonly EventCode Scene_PathEnd
            = EventFactory.GetEventCode("Scene", "PathEnd");

        /// <summary>
        /// 场景切换相位状态
        /// 参数：bool(true：切换到相位  false：切出相位)
        /// </summary>
        public static readonly EventCode Scene_ChangePhase
            = EventFactory.GetEventCode("Scene", "ChangePhase");

        /// <summary>
        /// 请求跳跃点信息
        /// 参数：int(地图id)
        /// </summary>
        public static readonly EventCode Scene_RequestJumpPoint
            = EventFactory.GetEventCode("Scene", "RequestJumpPoint");

        /// <summary>
        /// 发送跳跃点信息
        /// 参数：int(地图编号),Point2[](出发点),Point2[](结束点)
        /// </summary>
        public static readonly EventCode Scene_SendJumpPoint
            = EventFactory.GetEventCode("Scene", "SendJumpPoint");

        /// <summary>
        /// 主角死亡
        /// 参数：null
        /// </summary>
        public static readonly EventCode Scene_SelfDie
            = EventFactory.GetEventCode("Scene", "SelfDie");

        /// <summary>
        /// 主角复活
        /// 参数：null
        /// </summary>
        public static readonly EventCode Scene_SelfRevive
            = EventFactory.GetEventCode("Scene", "SelfRevive");

        /// <summary>
        /// 整个场景被隐藏或显示
        /// 参数：bool
        /// </summary>
        public static readonly EventCode Scene_SetActive
            = EventFactory.GetEventCode("Scene", "SetActive");

        ///// <summary>
        ///// 摄像机位置变化
        ///// 参数：Vector3(坐标),Vector3(焦点)
        ///// </summary>
        //public static readonly EventCode Scene_CameraTranslate
        //    = YuEventFactory.GetEventCode("Scene", "CameraTranslate");

        #endregion

        #region 技能

        /// <summary>
        /// 主角开始执行一段技能
        /// 参数：YuSkillClipInfoBase 技能片段信息对象
        /// </summary>
        public static readonly EventCode Skill_StartClip =
            EventFactory.GetEventCode("Skill", "StartClip");

        /// <summary>
        /// 主角的技能结束
        /// 参数：null
        /// </summary>
        public static readonly EventCode Skill_SelfSkillEnd =
            EventFactory.GetEventCode("Skill", "SelfSkillEnd");

        /// <summary>
        /// 主角的宠物开始执行一段技能
        /// 参数：YuSkillClipInfoBase 技能片段信息对象
        /// </summary>
        public static readonly EventCode Skill_PetStartClip =
            EventFactory.GetEventCode("Skill", "PetStartClip");

        /// <summary>
        /// 离线竞技场中，玩家或对手执行一段技能
        /// 参数：bool 是否是主角,YuSkillClipInfoBase 技能片段信息对象
        /// </summary>
        public static readonly EventCode Skill_OfflineStartClip =
            EventFactory.GetEventCode("Skill", "OfflineStartClip");

        /// <summary>
        /// 技能片段结束事件
        /// 参数：YuSkillClipBase ; long(guid)
        /// </summary>
        public static readonly EventCode Skill_ClipEnd =
           EventFactory.GetEventCode("Skill", "ClipEnd");



        #endregion

        #region 调试

        /// <summary>
        /// 发送GM命令。
        /// </summary>
        public static readonly EventCode Debug_GmCommand
            = EventFactory.GetEventCode("Debug", "GmCommand");

        #endregion

        #region 输入控制

        /// <summary>
        /// 摇杆被拖动。
        /// </summary>
        public static readonly EventCode Input_DragRocker
            = EventFactory.GetEventCode("Input", "DragRocker");

        /// <summary>
        /// 摇杆停止拖动。
        /// </summary>
        public static readonly EventCode Input_EndDragRocker
            = EventFactory.GetEventCode("Input", "EndDragRocker");

        /// <summary>
        /// 输入设备，触发移动操作
        /// 参数：Vector2
        /// </summary>
        public static readonly EventCode Input_RockerMove =
            EventFactory.GetEventCode("Input", "RockerMove");

        /// <summary>
        /// 输入设备，触发移动操作
        /// 参数：Vector2
        /// </summary>
        public static readonly EventCode Input_Move =
            EventFactory.GetEventCode("Input", "Move");

        /// <summary>
        /// 输入设备，触发技能操作
        /// 参数：string
        /// </summary>
        public static readonly EventCode Input_Skill =
            EventFactory.GetEventCode("Input", "Skill");

        /// <summary>
        /// 输入设备，滑屏操作
        /// 参数：Vector2
        /// </summary>
        public static readonly EventCode Input_DragScreen =
            EventFactory.GetEventCode("Input", "DragScreen");

        /// <summary>
        /// 输入设备，鼠标滚轮（或手机平台的双指缩放操作）
        /// 参数：float
        /// </summary>
        public static readonly EventCode Input_MouseWheel =
            EventFactory.GetEventCode("Input", "MouseWheel");

        #endregion

        #region 输入控制通过审核

        /// <summary>
        /// 允许移动操作
        /// 参数：Vector2
        /// </summary>
        public static readonly EventCode AllowInput_Move =
            EventFactory.GetEventCode("AllowInput", "Move");

        #endregion

#if UNITY_EDITOR
        #region 开发阶段作弊

        /// <summary>
        /// 修改主角移动速度
        /// 参数：float
        /// </summary>
        public static readonly EventCode Cheat_ChangeMoveSpeed
            = EventFactory.GetEventCode("Cheat", "ChangeMoveSpeed");

        #endregion
#endif
    }
}