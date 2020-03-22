#region Head

// Author:            Yu
// CreateDate:        2018/8/28 10:54:14
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Common;


namespace Client.LegoUI
{
    public static class YuLegoRxStrcutPool
    {
        #region 延迟构建的基础响应数据对象池

        private static IGenericObjectPool<ILegoRxStruct<byte>> bytePool;

        private static IGenericObjectPool<ILegoRxStruct<byte>> BytePool
        {
            get
            {
                if (bytePool != null)
                {
                    return bytePool;
                }

                bytePool = new GenericObjectPool<ILegoRxStruct<byte>>(
                    () => new YuLegoRxByte(), 10
                );

                return bytePool;
            }
        }

        private static IGenericObjectPool<ILegoRxStruct<short>> shortPool;

        private static IGenericObjectPool<ILegoRxStruct<short>> ShortPool
        {
            get
            {
                if (shortPool != null)
                {
                    return shortPool;
                }

                shortPool = new GenericObjectPool<ILegoRxStruct<short>>(
                    () => new YuLegoRxShort(), 10
                );

                return shortPool;
            }
        }

        private static IGenericObjectPool<ILegoRxStruct<int>> intPool;

        private static IGenericObjectPool<ILegoRxStruct<int>> IntPool
        {
            get
            {
                if (intPool != null)
                {
                    return intPool;
                }

                intPool = new GenericObjectPool<ILegoRxStruct<int>>(
                    () => new YuLegoRxInt(), 100
                );

                return intPool;
            }
        }

        private static IGenericObjectPool<ILegoRxStruct<long>> longPool;

        private static IGenericObjectPool<ILegoRxStruct<long>> LongPool
        {
            get
            {
                if (longPool != null)
                {
                    return longPool;
                }

                longPool = new GenericObjectPool<ILegoRxStruct<long>>(
                    () => new YuLegoRxLong(), 10
                );

                return longPool;
            }
        }

        private static IGenericObjectPool<ILegoRxStruct<string>> strPool;

        private static IGenericObjectPool<ILegoRxStruct<string>> StrPool
        {
            get
            {
                if (strPool != null)
                {
                    return strPool;
                }

                strPool = new GenericObjectPool<ILegoRxStruct<string>>(
                    () => new YuLegoRxString(), 100
                );

                return strPool;
            }
        }

        private static IGenericObjectPool<ILegoRxStruct<float>> floatPool;

        private static IGenericObjectPool<ILegoRxStruct<float>> FloatPool
        {
            get
            {
                if (floatPool != null)
                {
                    return floatPool;
                }

                floatPool = new GenericObjectPool<ILegoRxStruct<float>>(
                    () => new YuLegoRxFloat(), 100
                );

                return floatPool;
            }
        }

        private static IGenericObjectPool<ILegoRxStruct<double>> doublePool;

        private static IGenericObjectPool<ILegoRxStruct<double>> DoublePool
        {
            get
            {
                if (doublePool != null)
                {
                    return doublePool;
                }

                doublePool = new GenericObjectPool<ILegoRxStruct<double>>(
                    () => new YuLegoRxDouble(), 10
                );

                return doublePool;
            }
        }

        private static IGenericObjectPool<ILegoRxStruct<bool>> boolPool;

        private static IGenericObjectPool<ILegoRxStruct<bool>> BoolPool
        {
            get
            {
                if (boolPool != null)
                {
                    return boolPool;
                }

                boolPool = new GenericObjectPool<ILegoRxStruct<bool>>(
                    () => new YuLegoRxBool(), 20
                );

                return boolPool;
            }
        }

        #endregion

        #region 存取基础响应数据实例

        public static ILegoRxStruct<byte> GetRxByte()
        {
            return BytePool.Take();
        }

        public static ILegoRxStruct<short> GetRxShort()
        {
            return ShortPool.Take();
        }

        public static ILegoRxStruct<int> GetRxInt()
        {
            return IntPool.Take();
        }

        public static ILegoRxStruct<long> GetRxLong()
        {
            return LongPool.Take();
        }

        public static ILegoRxStruct<string> GetRxStr()
        {
            return StrPool.Take();
        }

        public static ILegoRxStruct<float> GetRxFloat()
        {
            return FloatPool.Take();
        }

        public static ILegoRxStruct<double> GetRxDouble()
        {
            return DoublePool.Take();
        }

        public static ILegoRxStruct<bool> GetRxBool()
        {
            return BoolPool.Take();
        }

        public static void RestoreRxByte(ILegoRxStruct<byte> rxByte)
        {
            BytePool.Restore(rxByte);
        }

        public static void RestoreRxShort(ILegoRxStruct<short> rxShort)
        {
            ShortPool.Restore(rxShort);
        }

        public static void RestoreRxInt(ILegoRxStruct<int> rxInt)
        {
            IntPool.Restore(rxInt);
        }

        public static void RestoreRxLong(ILegoRxStruct<long> rxLong)
        {
            LongPool.Restore(rxLong);
        }

        public static void RestoreRxStr(ILegoRxStruct<string> rxStr)
        {
            StrPool.Restore(rxStr);
        }

        public static void RestoreRxFloat(ILegoRxStruct<float> rxFloat)
        {
            FloatPool.Restore(rxFloat);
        }

        public static void RestoreRxDouble(ILegoRxStruct<double> rxDouble)
        {
            DoublePool.Restore(rxDouble);
        }

        public static void RestoreRxBool(ILegoRxStruct<bool> rxBool)
        {
            BoolPool.Restore(rxBool);
        }

        #endregion
    }
}