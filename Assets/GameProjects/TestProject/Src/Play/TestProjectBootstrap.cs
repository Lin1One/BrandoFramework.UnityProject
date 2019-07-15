#region Head

// Author:           Yuzhou
// CreateDate:    2018/4/18 23:16:56

#endregion

using Client.Assets;
using Client.GamePlaying.Unit;
using client_module_event;
using Common;
using UnityEngine;

namespace Client
{
    public class TestProjectBootstrap : GameBootstrap
    {
        /// <summary>
        /// 基础映射。
        /// </summary>
        protected override void Mapping()
        {
            var injector = Injector.Instance;
            injector.Mapping<IU3DEventModule, U3DEventModule>();
            injector.Mapping<IAssetModule, AssetModule>();
            injector.Mapping<IAssetInfoHelper, AssetInfoHelper>();
            injector.Mapping<IUnitModule, UnitModuleBase>();
        }

        /// <summary>
        /// 加载配置数据
        /// </summary>
        protected override void LoadAppSettingDateBeforeBootstrap()
        {

        }

        /// <summary>
        /// 启动应用。
        /// 调用指定应用模块的动态入口。
        /// </summary>
        protected override void StartGame()
        {
            var unitModule = Injector.Instance.Get<IUnitModule>();
            unitModule.RegistUintType<UnitEntityTest>();

            unitModule.CreateUnit<UnitEntityTest>(10000, UnitType.MainUnit, "abao_model", (a) => Debug.Log("创建完成"), null, true);
        }
        
    }
}