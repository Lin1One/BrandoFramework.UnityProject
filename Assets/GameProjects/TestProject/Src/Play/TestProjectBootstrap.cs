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
using UnityEngine;

namespace TestProject
{
    public class TestProjectBootstrap : GameBootstrap
    {
        //public TableDataBoard<TestProject_ExcelEntity_Test> 
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
            injector.Mapping<IDataTableModule, DataTableModule>();
            injector.Mapping<ISerializer, ProtobufSerializer>();
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
            //unitModule.RegistUintType<UnitEntityTest>();
            Injector.Instance.Get<IDataTableModule>().AsyncLoadEntitys<TestProject_ExcelEntity_Test>(list => Debug.Log(list.Count));
            //unitModule.CreateUnit<UnitEntityTest>(10000, UnitType.MainUnit, "abao_model", (a) => Debug.Log("创建完成"), null, true);
            var excelDataList  = Injector.Instance.Get<IDataTableModule>().GetRecords<TestProject_ExcelEntity_Test>();
            Debug.Log(excelDataList.Count  + "!!!");

        }
        
    }
}