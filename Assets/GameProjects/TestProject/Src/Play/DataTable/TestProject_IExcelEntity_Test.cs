using System;
using System.Collections.Generic;

namespace TestProject
{
    /// <summary>
    /// Excel数据表_Test
    /// </summary>
    public interface TestProject_IExcelEntity_Test
    {
        /// <summary>
        /// 步骤的大执行ID
        /// </summary>
        int ID { get; }
        
        /// <summary>
        /// 权重ID
        /// </summary>
        int sortID { get; }
        
        /// <summary>
        /// 小执行的ID集合
        /// </summary>
        float childIDList { get; }
        
        /// <summary>
        /// 大执行的触发条件集合
        /// </summary>
        string startConditionList { get; }
        
        /// <summary>
        /// 提前完成条件，满足之后直接跳过这个大步骤，不触发这个引导
        /// </summary>
        string finishConditionList { get; }
        
    }
}

