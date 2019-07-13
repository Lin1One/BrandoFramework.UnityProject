#region Head

// Author:            Yu
// CreateDate:        2018/8/23 17:03:03
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Common;
using Common.DataStruct;
using System;
using System.Collections.Generic;


namespace Client.LegoUI
{
    public class YuLegoRxModel : IReset, IDisposable
    {
        #region 自身类型实例对象池

        private static IObjectPool<YuLegoRxModel> rxModelPool;

        private static IObjectPool<YuLegoRxModel> RxModelPool
        {
            get
            {
                if (rxModelPool != null)
                {
                    return rxModelPool;
                }

                rxModelPool = new ObjectPool<YuLegoRxModel>(
                    () => new YuLegoRxModel(), 10
                );

                return rxModelPool;
            }
        }

        #endregion

        #region 存取实例静态API

        public static YuLegoRxModel GetRxModel()
        {
            return RxModelPool.Take();
        }

        public static void RestoreRxMoel(YuLegoRxModel rxModel)
        {
            rxModel.Reset();
            RxModelPool.Restore(rxModel);
        }

        #endregion

        #region 基础响应数据模型字典

        private Dictionary<int, ILegoRxStruct<byte>> rxBytes
            = new Dictionary<int, ILegoRxStruct<byte>>();

        private Dictionary<int, ILegoRxStruct<byte>> RxBytes
        {
            get
            {
                if (rxBytes != null)
                {
                    return rxBytes;
                }

                rxBytes = new Dictionary<int, ILegoRxStruct<byte>>();
                return rxBytes;
            }
        }

        private Dictionary<int, ILegoRxStruct<short>> rxShorts
            = new Dictionary<int, ILegoRxStruct<short>>();

        private Dictionary<int, ILegoRxStruct<short>> RxShorts
        {
            get
            {
                if (rxShorts != null)
                {
                    return rxShorts;
                }

                rxShorts = new Dictionary<int, ILegoRxStruct<short>>();
                return rxShorts;
            }
        }

        private Dictionary<int, ILegoRxStruct<int>> rxInts
            = new Dictionary<int, ILegoRxStruct<int>>();

        private Dictionary<int, ILegoRxStruct<int>> RxInts
        {
            get
            {
                if (rxInts != null)
                {
                    return rxInts;
                }

                rxInts = new Dictionary<int, ILegoRxStruct<int>>();
                return rxInts;
            }
        }

        private Dictionary<int, ILegoRxStruct<long>> rxLongs
            = new Dictionary<int, ILegoRxStruct<long>>();

        private Dictionary<int, ILegoRxStruct<long>> RxLongs
        {
            get
            {
                if (rxLongs != null)
                {
                    return rxLongs;
                }

                rxLongs = new Dictionary<int, ILegoRxStruct<long>>();
                return rxLongs;
            }
        }

        private Dictionary<int, ILegoRxStruct<string>> rxStrs
            = new Dictionary<int, ILegoRxStruct<string>>();

        private Dictionary<int, ILegoRxStruct<string>> RxStrs
        {
            get
            {
                if (rxStrs != null)
                {
                    return rxStrs;
                }

                rxStrs = new Dictionary<int, ILegoRxStruct<string>>();
                return rxStrs;
            }
        }

        private Dictionary<int, ILegoRxStruct<float>> rxFloats
            = new Dictionary<int, ILegoRxStruct<float>>();

        private Dictionary<int, ILegoRxStruct<float>> RxFloats
        {
            get
            {
                if (rxFloats != null)
                {
                    return rxFloats;
                }

                rxFloats = new Dictionary<int, ILegoRxStruct<float>>();
                return rxFloats;
            }
        }

        private Dictionary<int, ILegoRxStruct<double>> rxDoubles =
            new Dictionary<int, ILegoRxStruct<double>>();

        private Dictionary<int, ILegoRxStruct<double>> RxDoubles
        {
            get
            {
                if (rxDoubles != null)
                {
                    return rxDoubles;
                }

                rxDoubles = new Dictionary<int, ILegoRxStruct<double>>();
                return rxDoubles;
            }
        }

        private Dictionary<int, ILegoRxStruct<bool>> rxBools
            = new Dictionary<int, ILegoRxStruct<bool>>();

        private Dictionary<int, ILegoRxStruct<bool>> RxBools
        {
            get
            {
                if (rxBools != null)
                {
                    return rxBools;
                }

                rxBools = new Dictionary<int, ILegoRxStruct<bool>>();
                return rxBools;
            }
        }

        private readonly Dictionary<Type, object> modelDictMap = new Dictionary<Type, object>();

        #region 构造封闭

        private YuLegoRxModel()
        {
            modelDictMap.Add(typeof(byte), RxBytes);
            modelDictMap.Add(typeof(short), RxShorts);
            modelDictMap.Add(typeof(int), RxInts);
            modelDictMap.Add(typeof(long), RxLongs);
            modelDictMap.Add(typeof(string), RxStrs);
            modelDictMap.Add(typeof(float), RxFloats);
            modelDictMap.Add(typeof(double), RxDoubles);
            modelDictMap.Add(typeof(bool), RxBools);
        }

        #endregion

        #endregion

        #region 基础响应数据创建委托字典

        private static readonly Dictionary<Type, Func<object>> modelFuncDict
            = new Dictionary<Type, Func<object>>();

        #endregion

        #region 静态构造

        static YuLegoRxModel()
        {
            modelFuncDict.Add(typeof(byte), YuLegoRxStrcutPool.GetRxByte);
            modelFuncDict.Add(typeof(short), YuLegoRxStrcutPool.GetRxShort);
            modelFuncDict.Add(typeof(int), YuLegoRxStrcutPool.GetRxInt);
            modelFuncDict.Add(typeof(long), YuLegoRxStrcutPool.GetRxLong);
            modelFuncDict.Add(typeof(string), YuLegoRxStrcutPool.GetRxStr);
            modelFuncDict.Add(typeof(float), YuLegoRxStrcutPool.GetRxFloat);
            modelFuncDict.Add(typeof(double), YuLegoRxStrcutPool.GetRxDouble);
            modelFuncDict.Add(typeof(bool), YuLegoRxStrcutPool.GetRxBool);
        }

        #endregion

        #region 数据存在判断

        public bool IsExist<T>(int id)
        {
            var type = typeof(T);
            var modelDict = modelDictMap[type] as Dictionary<int, IRxStruct<T>>;
            // ReSharper disable once PossibleNullReferenceException
            return modelDict.ContainsKey(id);
        }

        #endregion

        #region 具象存取API

        public T Get<T>(int id)
        {
            var type = typeof(T);
            var modelDict = modelDictMap[type] as Dictionary<int, IRxStruct<T>>;
            // ReSharper disable once PossibleNullReferenceException
            if (modelDict.ContainsKey(id))
            {
                return modelDict[id].Value;
            }

#if DEBUG || DEBUG
            //YuDebugUtility.LogError($"类型{type}__Id{id}的数据没有找到！");
#endif

            return default(T);
        }

        public void Set<T>(int id, T value)
        {
            var type = typeof(T);
            var modelDict = modelDictMap[type] as Dictionary<int, IRxStruct<T>>;
            // ReSharper disable once PossibleNullReferenceException
            if (modelDict.ContainsKey(id))
            {
                var rxModel = modelDict[id];
                rxModel.Value = value;
            }
            else
            {
                var newModel = GetNewModel<T>();
                newModel.Value = value;
                modelDict.Add(id, newModel);
            }
        }

        private IRxStruct<T> GetNewModel<T>()
        {
            var type = typeof(T);
            var func = modelFuncDict[type];
            return func() as IRxStruct<T>;
        }

        #endregion

        #region 清理重置

        public void Dispose()
        {
            Reset();
        }

        public void Reset()
        {
            foreach (var rxByte in RxBytes.Values)
            {
                YuLegoRxStrcutPool.RestoreRxByte(rxByte);
            }

            RxBytes.Clear();

            foreach (var rxShort in RxShorts.Values)
            {
                YuLegoRxStrcutPool.RestoreRxShort(rxShort);
            }

            RxShorts.Clear();

            foreach (var rxInt in RxInts.Values)
            {
                YuLegoRxStrcutPool.RestoreRxInt(rxInt);
            }

            RxInts.Clear();

            foreach (var rxLong in RxLongs.Values)
            {
                YuLegoRxStrcutPool.RestoreRxLong(rxLong);
            }

            RxLongs.Clear();

            foreach (var rxStr in RxStrs.Values)
            {
                YuLegoRxStrcutPool.RestoreRxStr(rxStr);
            }

            RxStrs.Clear();

            foreach (var rxFloat in RxFloats.Values)
            {
                YuLegoRxStrcutPool.RestoreRxFloat(rxFloat);
            }

            RxFloats.Clear();

            foreach (var rxDouble in RxDoubles.Values)
            {
                YuLegoRxStrcutPool.RestoreRxDouble(rxDouble);
            }

            RxDoubles.Clear();

            foreach (var rxBool in RxBools.Values)
            {
                YuLegoRxStrcutPool.RestoreRxBool(rxBool);
            }

            RxBools.Clear();
        }

        #endregion
    }
}