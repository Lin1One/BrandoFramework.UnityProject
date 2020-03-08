#region Head

// Author:           Yuzhou
// CreateDate:    2018/4/18 23:16:56

#endregion

using Client;
using Client.Assets;
using Client.Core;
using Client.DataTable;
using Client.GamePlaying.Unit;
using Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TestProject
{
    public class TestProjectBootstrap : GameBootstrap
    {
        /// <summary>
        /// 基础映射。
        /// </summary>
        protected override void Mapping()
        {
            var injector = Injector.Instance;
            injector.Map<IU3DEventModule, U3DEventModule>();
            injector.Map<IAssetModule, AssetModule>();
            injector.Map<IAssetInfoHelper, AssetInfoHelper>();
            injector.Map<IUnitModule, UnitModuleBase>();
            injector.Map<IDataTableModule, DataTableModule>();
            injector.Map<ISerializer, ProtobufSerializer>();
        }

        /// <summary>
        /// 启动时模块初始化
        /// </summary>
        protected override void InitModuleAtStart()
        {

        }

        /// <summary>
        /// 加载配置数据
        /// </summary>
        protected override void ApplyAppAndModuleConfig()
        {
            evenModuleConfig.ApplyConfig();
        }

        /// <summary>
        /// 启动应用。
        /// 调用指定应用模块的动态入口。
        /// </summary>
        protected override void StartGame()
        {
            var unitModule = Injector.Instance.Get<IUnitModule>();
            var eventModule = Injector.Instance.Get<IU3DEventModule>();
            eventModule.InitModule();
            eventModule.WatchEvent(ProjectCoreEventCode.AppInited, () => Debug.Log("第一个事件"));
            eventModule.WatchEvent(ProjectCoreEventCode.AppSwitchCurrentApp, (object obj) => Debug.Log(obj));
            eventModule.TriggerEvent(ProjectCoreEventCode.AppInited, null,() => Debug.Log("第一个事件完成"));
            eventModule.TriggerEvent(ProjectCoreEventCode.AppSwitchCurrentApp, "第二个事件",() => Debug.Log("第二个事件完成"));
            eventModule.WatchEvent<int>(ProjectCoreEventCode.AppApkDownProgressUpdate, (int i) => Debug.Log("第三个事件"));
            eventModule.TriggerEventSync<int>(ProjectCoreEventCode.AppApkDownProgressUpdate, 1, () => Debug.Log("第三个事件完成"));
            //unitModule.RegistUintType<UnitEntityTest>();
            //Injector.Instance.Get<IDataTableModule>().AsyncLoadEntitys<TestProject_ExcelEntity_Test>(list => Debug.Log(list.Count));
            //unitModule.CreateUnit<UnitEntityTest>(10000, UnitType.MainUnit, "abao_model", (a) => Debug.Log("创建完成"), null, true);
            //var excelDataList  = Injector.Instance.Get<IDataTableModule>().GetRecords<TestProject_ExcelEntity_Test>();
            //Debug.Log(excelDataList.Count  + "!!!");

        }

        [FoldoutGroup("事件模块配置")]
        public EventModuleConfig evenModuleConfig;
    }
}