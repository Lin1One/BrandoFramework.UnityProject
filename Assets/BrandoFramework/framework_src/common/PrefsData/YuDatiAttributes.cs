#region Head

// Author:            liuruoyu1981
// CreateDate:        2019/1/19 11:22:41
// Email:             35490136@qq.com || liuruoyu1981@gmail.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System;

namespace Common.PrefsData
{
    /// <summary>
    /// 资料柜原生（非unity3d可序列化脚本）文件后缀名。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class YuDatiSuffixAttribute : Attribute
    {
        public string Suffix { get; private set; }

        public YuDatiSuffixAttribute(string suffix)
        {
            Suffix = suffix;
        }
    }

    /// <summary>
    /// 资料文件类型。
    /// </summary>
    [Serializable]
    public enum YuDatiSaveType
    {
        /// <summary>
        /// 单例资料。
        /// 全局只有一份资料实体。
        /// </summary>
        Single,

        /// <summary>
        /// 多实例资料。
        /// 依据给定的资料Id可以有多份资料存在。
        /// </summary>
        Multi,

        /// <summary>
        /// 自动日期命名资料
        /// 该类型资料会在获取实例的时候自动按照当前日期创建资料文件。
        /// </summary>
        AutoDate,
    }

    /// <summary>
    /// 描述资料文件的用途。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class YuDatiDescAttribute : Attribute
    {
        /// <summary>
        /// 资料文件类型。
        /// </summary>
        public YuDatiSaveType DatiSaveType { get; private set; }

        public Type DatiType { get; private set; }

        /// <summary>
        /// 简短描述。
        /// 用于绘制动态创建资料文件的菜单项。
        /// </summary>
        public string Title { get; private set; }

        public YuDatiDescAttribute(YuDatiSaveType datiSaveType, Type datiType,
            string title)
        {
            DatiSaveType = datiSaveType;
            DatiType = datiType;
            Title = title;
        }
    }

    /// <summary>
    /// 用于给出资料文件详情用途、使用说明的附加特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class YuDatiDetailHelpDescAttribute : Attribute
    {
        public string DetailHelperDesc { get; private set; }

        public YuDatiDetailHelpDescAttribute(string helpDesc) => DetailHelperDesc = helpDesc;
    }
}

