#region Head

// Author:            LinYuzhou
// CreateDate:        2020/1/19 11:22:41

#endregion

using System;

namespace Common.PrefsData
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DatiOnlyInEditorAttribute : Attribute
    {

    }
    /// <summary>
    /// 资料文件序列化数据（非unity3d可序列化脚本）文件后缀名Attr
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DatiSuffixAttribute : Attribute
    {
        public string Suffix { get; private set; }

        public DatiSuffixAttribute(string suffix)
        {
            Suffix = suffix;
        }
    }

    /// <summary>
    /// 描述资料文件的用途。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DatiDescAttribute : Attribute
    {
        /// <summary>
        /// 资料文件类型。
        /// </summary>
        public DatiSaveType DatiSaveType { get; private set; }

        /// <summary>
        /// 简短描述。
        /// 用于绘制动态创建资料文件的菜单项。
        /// </summary>
        public string Title { get; private set; }

        public DatiDescAttribute(DatiSaveType datiSaveType,string title)
        {
            DatiSaveType = datiSaveType;
            Title = title;
        }
    }

    /// <summary>
    /// 资料文件类型。
    /// </summary>
    [Serializable]
    public enum DatiSaveType
    {
        /// <summary>
        /// 单例资料。
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
    /// 用于给出资料文件详情用途、使用说明的附加特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DatiDetailHelpDescAttribute : Attribute
    {
        public string DetailHelperDesc { get; private set; }

        public DatiDetailHelpDescAttribute(string helpDesc) => DetailHelperDesc = helpDesc;
    }
}

