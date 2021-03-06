#region Head

// Author:            LinYuzhou
// CreateDate:        2/3/2019 10:07:15 AM
// Email:             836045613@qq.com

#endregion

using Common.PrefsData;
using System.Collections.Generic;

namespace Client.Core.Editor
{
    public static class U3dCommonSpannerUtility
    {
        #region 缓存类型和特性

        //private static List<Type> datiTypes;

        //public static List<Type> DatiTypes
        //{
        //    get
        //    {
        //        if (datiTypes != null)
        //        {
        //            return datiTypes;
        //        }

        //        datiTypes = new List<Type>();

        //        foreach (var id in YuSetting.Instance.AllAssemblyId)
        //        {
        //            var asm = YuUnityIOUtility.GetUnityAssembly(id);
        //            if (asm == null)
        //            {
        //                continue;
        //            }

        //            var types = YuReflectUtility.GetTypeList<IYuU3dDati>(
        //                false, false, asm);
        //            datiTypes.AddRange(types);
        //        }

        //        return datiTypes;
        //    }
        //}

        //private static List<YuDatiDescAttribute> datiDescs;

        //public static List<YuDatiDescAttribute> DatiDescs
        //{
        //    get
        //    {
        //        if (datiDescs != null)
        //        {
        //            return datiDescs;
        //        }

        //        datiDescs = new List<YuDatiDescAttribute>();
        //        foreach (var datiType in DatiTypes)
        //        {
        //            var descAttr = datiType.GetAttribute<YuDatiDescAttribute>();
        //            if (descAttr == null)
        //            {
        //                Debug.LogError($"资料夹类型{datiType.Name}没有附加用途描述特性！");
        //                continue;
        //            }

        //            datiDescs.Add(descAttr);
        //        }

        //        return datiDescs;
        //    }
        //}

        private static List<DatiDescAttribute> singleDescs;

        public static List<DatiDescAttribute> SingleDescs
        {
            get
            {
                if (singleDescs != null)
                {
                    return singleDescs;
                }

                //singleDescs = DatiDescs.FindAll(d => d.DatiSaveType == YuDatiSaveType.Single);
                return singleDescs;
            }
        }

        //private static List<YuDatiDescAttribute> multiDescs;

        //public static List<YuDatiDescAttribute> MultiDescs
        //{
        //    get
        //    {
        //        if (multiDescs != null)
        //        {
        //            return multiDescs;
        //        }

        //        multiDescs = DatiDescs.FindAll(d => d.DatiSaveType == YuDatiSaveType.Multi);
        //        return multiDescs;
        //    }
        //}

        #endregion
    }

}

