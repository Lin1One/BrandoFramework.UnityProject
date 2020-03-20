#region Head

// Author:        LinYuzhou
// CreateDate:    2019/10/29 18:15:56 PM
// Email:         836045613@qq.com

#endregion


using Client.Assets;
using Client.Core;
using Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Client.DataTable
{
    /// <summary>
    /// 用于unity环境的Excel数据表模块。
    /// </summary>
    [Singleton]
    public class DataTableModule :IDataTableModule
    {
        private static ISerializer serializer;

        private static ISerializer Serializer =>
            serializer ?? (serializer = Injector.Instance.Get<ISerializer>());

        public void AsyncLoadEntitys<T>(Action<List<T>> onLoaded) where T : IExcelEntity<T>, new()
        {
            TryGetDataAsync(onLoaded);
        }

        public List<T> GetRecords<T>(Predicate<T> func = null) where T : IExcelEntity<T>, new()
        {
            //同步，异步冲突时处理
            if(ExcelDataWrapper<T>.isAsyncLoading)
            {

            }
            List<T> datas;
            if (func == null)
            {
                TryGetData(out datas);
                return datas;
            }
            else
            {
                TryGetData(out datas);
                datas = datas.FindAll(func);
                return datas;
            }
        }

        public T GetRecordAtIndex<T>(int index) where T : IExcelEntity<T>, new()
        {
            List<T> datas;
            TryGetData(out datas);
            if (index >= datas.Count)
            {
#if UNITY_EDITOR
                Debug.LogError($"目标数据索引超出数据表{typeof(T).Name}的最大数量！");
#endif
                return default(T);
            }
            return datas[index];
        }

//        public T GetRecordAtIndex<T>(int index) where T : IExcelEntity<T>, new()
//        {
//            List<T> datas;
//            TryGetData(out datas);
//            if (index >= datas.Count)
//            {
//#if UNITY_EDITOR
//                Debug.LogError($"目标数据索引超出数据表{typeof(T).Name}的最大数量！");
//#endif
//                return default(T);
//            }
//            return datas[index];
//        }

        public T GetRecordFirst<T>(Predicate<T> func) where T : IExcelEntity<T>, new()
        {
            if (func == null)
            {
                return default(T);
            }
            else
            {
                List<T> datas;
                TryGetData(out datas);
                return datas.Find(func);
            }
        }

        private bool TryGetData<T>(out List<T> data) where T : IExcelEntity<T>
        {
            bool gotData = true;
            if(ExcelDataWrapper<T>.Entitys == null)
            {
                try
                {
                    var assetId = typeof(T).Name.ToLower();
                    var assetModule = Injector.Instance.Get<IAssetModule>();
                    var textAsset = assetModule.Load<TextAsset>(assetId);
                    var bytes = textAsset.bytes;
                    //Serializer.DeSerialize<List<T>>(bytes);
                    ExcelDataWrapper<T>.Entitys = Serializer.DeSerialize<List<T>>(bytes);
                    assetModule.ReleaseAsset(assetId);
                    gotData = true;
                }
                catch (Exception e)
                {
                    ExcelDataWrapper<T>.Entitys = new List<T>();
                    gotData = false;
                    throw new Exception($"在反序列化配置表类型{typeof(T).Name}时发生异常，" +
                                        $"异常信息为{e.Message}，异常信息为{e.StackTrace}！！");
                }
            }
            data = ExcelDataWrapper<T>.Entitys;
            return gotData;
        }

        

        private void TryGetDataAsync<T>(Action<List<T>> onLoaded) where T : IExcelEntity<T>
        {
            var assetId = typeof(T).Name.ToLower();
            var assetModule = Injector.Instance.Get<IAssetModule>();
            var textAsset = assetModule.Load<TextAsset>(assetId);
            if (textAsset == null || textAsset == null)
            {
                Debug.LogError($"在反序列化配置表类型{assetId}时出错,找不到该表格)");
                return;
            }
            var bytes = textAsset.bytes;

            ExcelDataWrapper<T>.onLoaded = onLoaded;
            ExcelDataWrapper<T>.isAllLoaded = false;
            Injector.Instance.Get<IU3DEventModule>().WatchUnityEvent(
                UnityEventType.Update, CheckLoadEnd<T>);

            ExcelDataWrapper<T>.isAsyncLoading = true;
            Task.Run(() =>
            {
                ExcelDataWrapper<T>.Entitys = Serializer.DeSerialize<List<T>>(bytes);
                ExcelDataWrapper<T>.isAllLoaded = true;
                ExcelDataWrapper<T>.isAsyncLoading = false;
            });
        }

        private static void CheckLoadEnd<T>() where T : IExcelEntity<T>
        {
            if (ExcelDataWrapper<T>.isAllLoaded)
            {
                Injector.Instance.Get<IU3DEventModule>().RemoveUnityEvent(
                    UnityEventType.Update, CheckLoadEnd<T>);
                var assetId = typeof(T).Name.ToLower();
                Injector.Instance.Get<IAssetModule>().ReleaseAsset(assetId);
                ExcelDataWrapper<T>.onLoaded?.Invoke(ExcelDataWrapper<T>.Entitys);
            }
        }
    }
}