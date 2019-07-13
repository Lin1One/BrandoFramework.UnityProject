

using Common;
using Common.DataStruct;

namespace YuU3dPlay
{
    /// <summary>
    /// 基础类型响应式数据对象池。
    /// </summary>
    public static class YuRxStrcutPool
    {
        #region 延迟构建的基础响应数据对象池

        private static IObjectPool<IRxStruct<byte>> bytePool;

        private static IObjectPool<IRxStruct<byte>> BytePool
        {
            get
            {
                if (bytePool != null)
                {
                    return bytePool;
                }

                bytePool = new ObjectPool<IRxStruct<byte>>(
                    () => new RxByte(), 10
                );

                return bytePool;
            }
        }

        private static IObjectPool<IRxStruct<short>> shortPool;

        private static IObjectPool<IRxStruct<short>> ShortPool
        {
            get
            {
                if (shortPool != null)
                {
                    return shortPool;
                }

                shortPool = new ObjectPool<IRxStruct<short>>(
                    () => new YuRxShort(), 10
                );

                return shortPool;
            }
        }

        private static IObjectPool<IRxStruct<int>> intPool;

        private static IObjectPool<IRxStruct<int>> IntPool
        {
            get
            {
                if (intPool != null)
                {
                    return intPool;
                }

                intPool = new ObjectPool<IRxStruct<int>>(
                    () => new YuRxInt(), 100
                );

                return intPool;
            }
        }

        private static IObjectPool<IRxStruct<long>> longPool;

        private static IObjectPool<IRxStruct<long>> LongPool
        {
            get
            {
                if (longPool != null)
                {
                    return longPool;
                }

                longPool = new ObjectPool<IRxStruct<long>>(
                    () => new YuRxLong(), 10
                );

                return longPool;
            }
        }

        private static IObjectPool<IRxStruct<string>> strPool;

        private static IObjectPool<IRxStruct<string>> StrPool
        {
            get
            {
                if (strPool != null)
                {
                    return strPool;
                }

                strPool = new ObjectPool<IRxStruct<string>>(
                    () => new YuRxString(), 100
                );

                return strPool;
            }
        }

        private static IObjectPool<IRxStruct<float>> floatPool;

        private static IObjectPool<IRxStruct<float>> FloatPool
        {
            get
            {
                if (floatPool != null)
                {
                    return floatPool;
                }

                floatPool = new ObjectPool<IRxStruct<float>>(
                    () => new YuRxFloat(), 100
                );

                return floatPool;
            }
        }

        private static IObjectPool<IRxStruct<double>> doublePool;

        private static IObjectPool<IRxStruct<double>> DoublePool
        {
            get
            {
                if (doublePool != null)
                {
                    return doublePool;
                }

                doublePool = new ObjectPool<IRxStruct<double>>(
                    () => new YuRxDouble(), 10
                );

                return doublePool;
            }
        }

        private static IObjectPool<IRxStruct<bool>> boolPool;

        private static IObjectPool<IRxStruct<bool>> BoolPool
        {
            get
            {
                if (boolPool != null)
                {
                    return boolPool;
                }

                boolPool = new ObjectPool<IRxStruct<bool>>(
                    () => new RxBool(), 20
                );

                return boolPool;
            }
        }

        #endregion

        #region 存取基础响应数据实例

        public static IRxStruct<byte> GetRxByte()
        {
            return BytePool.Take();
        }

        public static IRxStruct<short> GetRxShort()
        {
            return ShortPool.Take();
        }

        public static IRxStruct<int> GetRxInt()
        {
            return IntPool.Take();
        }

        public static IRxStruct<long> GetRxLong()
        {
            return LongPool.Take();
        }

        public static IRxStruct<string> GetRxStr()
        {
            return StrPool.Take();
        }

        public static IRxStruct<float> GetRxFloat()
        {
            return FloatPool.Take();
        }

        public static IRxStruct<double> GetRxDouble()
        {
            return DoublePool.Take();
        }

        public static IRxStruct<bool> GetRxBool()
        {
            return BoolPool.Take();
        }

        public static void RestoreRxByte(IRxStruct<byte> rxByte)
        {
            BytePool.Restore(rxByte);
        }

        public static void RestoreRxShort(IRxStruct<short> rxShort)
        {
            ShortPool.Restore(rxShort);
        }

        public static void RestoreRxInt(IRxStruct<int> rxInt)
        {
            IntPool.Restore(rxInt);
        }

        public static void RestoreRxLong(IRxStruct<long> rxLong)
        {
            LongPool.Restore(rxLong);
        }

        public static void RestoreRxStr(IRxStruct<string> rxStr)
        {
            StrPool.Restore(rxStr);
        }

        public static void RestoreRxFloat(IRxStruct<float> rxFloat)
        {
            FloatPool.Restore(rxFloat);
        }

        public static void RestoreRxDouble(IRxStruct<double> rxDouble)
        {
            DoublePool.Restore(rxDouble);
        }

        public static void RestoreRxBool(IRxStruct<bool> rxBool)
        {
            BoolPool.Restore(rxBool);
        }

        #endregion
    }
}