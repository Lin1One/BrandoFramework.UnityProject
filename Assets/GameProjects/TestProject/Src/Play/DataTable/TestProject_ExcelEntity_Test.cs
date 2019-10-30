using System;
using Client.DataTable;
using System.Collections.Generic;
using System.Linq;
using Common.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Client.DataTable;


// Excel本地数据表脚本，该脚本为自动创建请勿手动修改！

namespace TestProject
{
    /// <summary>
    /// Excel数据表_Test
    /// </summary>
    [Serializable]
    [ProtoBuf.ProtoContract]
    public class TestProject_ExcelEntity_Test
        : TestProject_IExcelEntity_Test, IExcelEntity<TestProject_ExcelEntity_Test>
    {
        [ProtoBuf.ProtoMember(1)]
        [LabelText("步骤的大执行ID")]
        [ShowInInspector]
        [SerializeField]
        public int id;
        
        /// <summary>
        /// 步骤的大执行ID
        /// </summary>
        public int ID => id;
        
        [ProtoBuf.ProtoMember(2)]
        [LabelText("权重ID")]
        [ShowInInspector]
        [SerializeField]
        public int sortid;
        
        /// <summary>
        /// 权重ID
        /// </summary>
        public int sortID => sortid;
        
        [ProtoBuf.ProtoMember(3)]
        [LabelText("小执行的ID集合")]
        [ShowInInspector]
        [SerializeField]
        public float childidlist;
        
        /// <summary>
        /// 小执行的ID集合
        /// </summary>
        public float childIDList => childidlist;
        
        [ProtoBuf.ProtoMember(4)]
        [LabelText("大执行的触发条件集合")]
        [ShowInInspector]
        [SerializeField]
        public string startconditionlist;
        
        /// <summary>
        /// 大执行的触发条件集合
        /// </summary>
        public string startConditionList => startconditionlist;
        
        [ProtoBuf.ProtoMember(5)]
        [LabelText("提前完成条件，满足之后直接跳过这个大步骤，不触发这个引导")]
        [ShowInInspector]
        [SerializeField]
        public string finishconditionlist;
        
        /// <summary>
        /// 提前完成条件，满足之后直接跳过这个大步骤，不触发这个引导
        /// </summary>
        public string finishConditionList => finishconditionlist;
        
        /// <summary>
        /// 将excel的一行txt数据源转换为数据实例。
        /// </summary>
        public void InitEntity(List<string> cells)
        {
            id = Convert.ToInt32(cells[0]);
            sortid = Convert.ToInt32(cells[1]);
            childidlist = float.Parse(cells[2]);
            startconditionlist = cells[3];
            finishconditionlist = cells[4];
        }
        
        /// <summary>
        /// 将excel的txt数据源转换为数据列表。
        /// </summary>
        public List<IExcelEntity> InitEntitys(List<string> rows)
        {
            var entitys = new List<IExcelEntity>();
             int i = 0;
            try
            {
                for (; i < rows.Count; i++)
                {
                    var strList = rows[i].Split(ExcelDataConstant.Separator,
                        StringSplitOptions.None).ToList();
                    var entity = new TestProject_ExcelEntity_Test();
                    entity.InitEntity(strList);
                    entitys.Add(entity);
                }
            }
            catch
            {
                Debug.Log($"生成该表格二进制文件出错，出错行数为第 {i}行");
            }
            
            return entitys;
        }
        
        /// <summary>
        /// 获得本地数据的一份深度复制副本
        /// </summary>
        public TestProject_ExcelEntity_Test DeepCopy()
        {
            var buff = SerializeUtility.Serialize(this);
            var entity = SerializeUtility.DeSerialize<TestProject_ExcelEntity_Test>(buff);
            return entity;
        }
        /// <summary>
        /// 将本地数据对象转换为txt源数据字符串
        /// </summary>
        public string ToTxt()
        {
            var entityStr = string.Empty;
            entityStr = entityStr + ID + "[__]";
            entityStr = entityStr + sortID + "[__]";
            entityStr = entityStr + childIDList + "[__]";
            entityStr = entityStr + startConditionList + "[__]";
            entityStr = entityStr + finishConditionList + "[__]";
            entityStr = entityStr.Remove(entityStr.Length - 1);
            return entityStr;
        }
        
    }
}

