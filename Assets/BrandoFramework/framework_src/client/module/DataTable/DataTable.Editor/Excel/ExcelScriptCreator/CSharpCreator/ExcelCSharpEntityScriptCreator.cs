#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion

using Common;
using Common.ScriptCreate;
using Common.Utility;
using UnityEngine;

namespace Client.DataTable.Editor
{
    /// <summary>
    /// Excel 脚本 Csharp 实体类创建器
    /// </summary>
    public class ExcelCSharpEntityScriptCreator : ExcelScriptCreatorBase
    {
        public override ScriptType ScriptType => ScriptType.Csharp;
        public override string ScriptName => ExcelUtility.EntityScriptName(projectInfo, SheetInfo);
        protected override void AppendScript()
        {
            AppendHead();
            AppendUsingNamespace();
            AppendNote();
            AppendCsClassHead();
            AppendCsField();
            AppendCsInitEntity();
            AppendCsInitEntitys();
            AppendCsToTxt();
            AppendCsDeepCopy();
            AppendFooter();
        }

        /// <summary>
        /// 代码头
        /// </summary>
        private void AppendHead()
        {
            //Appender.AppendCsHeadComment();
        }

        /// <summary>
        /// 命名空间
        /// </summary>
        private void AppendUsingNamespace()
        {
            Appender.AppendUsingNamespace(
                "System",
                typeof(ExcelFieldType).Namespace,
                "System.Collections.Generic",
                "System.Linq",
                typeof(SerializeUtility).Namespace,
                "Sirenix.OdinInspector",
                "UnityEngine",
                typeof(IExcelEntity).Namespace
            );
        }

        /// <summary>
        /// 增加注释
        /// </summary>
        private void AppendNote()
        {
            Appender.AppendLine();
            Appender.AppendLine("// Excel本地数据表脚本，该脚本为自动创建请勿手动修改！");
            Appender.AppendLine();
        }

        private void AppendCsClassHead()
        {
            var classNameSpace = projectInfo.ProjectRuntimeScriptDefines;
            if (string.IsNullOrEmpty(classNameSpace))
            {
                Debug.LogError("项目脚本命名空间为空");
                return;
            }
            Appender.AppendLine($"namespace {projectInfo.ProjectRuntimeScriptDefines}");
            Appender.AppendLeftBracketsAndToRight();
            Appender.AppendCsComment("Excel数据表_" + SheetInfo.ChineseId);
            Appender.AppendLine("[Serializable]");
            switch (exportSetting.serializationType)
            {
                case SerializationType.ProtoBuff:
                    {
                        Appender.AppendLine("[ProtoBuf.ProtoContract]");
                    }
                    break;
                case SerializationType.Json:
                    break;
            }
            Appender.AppendLine($"public class {ScriptName}");
            var interfaceName = ExcelUtility.EntityInterfaceName(projectInfo,SheetInfo);
            Appender.AppendLine($"    : {interfaceName}, IExcelEntity<{ScriptName}>");
            Appender.AppendLeftBracketsAndToRight();
        }

        /// <summary>
        /// 字段
        /// </summary>
        private void AppendCsField()
        {
            var excelFieldInfos = SheetInfo.FieldInfos;
            var protobufMemberIndex = 1;

            foreach (var fieldInfo in excelFieldInfos)
            {
                if (fieldInfo.FieldType == ExcelFieldType.Ignore)
                {
                    continue;
                }

                if (exportSetting.serializationType == SerializationType.ProtoBuff)
                {
                    Appender.AppendLine($"[ProtoBuf.ProtoMember({protobufMemberIndex})]");
                    protobufMemberIndex++;
                }

                if (fieldInfo.EnglishName == "Id")
                {
                    Appender.AppendLine("[ReadOnly]");
                }

                Appender.AppendLine($"[LabelText(\"{fieldInfo.ChineseName}\")]");
                Appender.AppendLine("[ShowInInspector]");
                Appender.AppendLine("[SerializeField]");
                Appender.AppendLine($"public {fieldInfo.CsType} {fieldInfo.EnglishName.ToLower()};");
                Appender.AppendLine();
                Appender.AppendCsComment(fieldInfo.ChineseName);
                Appender.AppendLine($"public {fieldInfo.CsType} {fieldInfo.EnglishName} " +
                                    $"=> {fieldInfo.EnglishName.ToLower()};");
                Appender.AppendLine();
            }
        }

        /// <summary>
        /// 数据表转换实例初始化方法
        /// </summary>
        private void AppendCsInitEntity()
        {
            Appender.AppendCsComment("将excel的一行txt数据源转换为数据实例。");
            Appender.AppendLine("public void InitEntity(List<string> cells)");
            Appender.AppendLine("{");
            Appender.ToRight();
            foreach (var info in SheetInfo.FieldInfos)
            {
                if (info.FieldType == ExcelFieldType.Ignore)
                {
                    continue;
                }

                var handler = YuU3dExcelFieldHandlerFactory.GetFieldHandler(info.FieldType,
                    ScriptType.Csharp);
                var csEntityStr = handler.GetCodeStr(
                    ExcelDataConstant.CsLanguageType, info);
                Appender.AppendLine(csEntityStr);
            }

            Appender.ToLeft();
            Appender.AppendLine("}");
            Appender.AppendLine();
        }

        private void AppendCsInitEntitys()
        {
            Appender.AppendCsComment("将excel的txt数据源转换为数据列表。");
            Appender.AppendLine($"public List<{"IExcelEntity"}> InitEntitys(List<string> rows)");
            Appender.AppendLine("{");
            Appender.ToRight();
            Appender.AppendLine($"var entitys = new List<{"IExcelEntity"}>();");
            Appender.AppendLine(" int i = 0;");
            Appender.AppendLine("try");
            Appender.AppendLine("{");
            Appender.ToRight();

            Appender.AppendLine("for (; i < rows.Count; i++)");
            Appender.AppendLine("{");
            Appender.ToRight();
            Appender.AppendLine("var strList = rows[i].Split(ExcelDataConstant.Separator,");
            Appender.AppendLine("    StringSplitOptions.None).ToList();");
            Appender.AppendLine($"var entity = new {ScriptName}();");
            Appender.AppendLine("entity.InitEntity(strList);");
            Appender.AppendLine("entitys.Add(entity);");
            Appender.ToLeft();
            Appender.AppendLine("}");
            Appender.ToLeft();
            Appender.AppendLine("}");
            Appender.AppendLine("catch");
            Appender.AppendLine("{");
            Appender.ToRight();
            Appender.AppendLine("Debug.Log($\"生成该表格二进制文件出错，出错行数为第 {i}行\");");
            Appender.ToLeft();
            Appender.AppendLine("}");
            Appender.AppendLine();

            Appender.AppendLine("return entitys;");
            Appender.ToLeft();
            Appender.AppendLine("}");
            Appender.AppendLine();
        }

        private void AppendCsDeepCopy()
        {
            Appender.AppendCsComment("将本地数据对象转换为txt源数据字符串");
            Appender.AppendLine("public string ToTxt()");
            Appender.AppendLine("{");
            Appender.ToRight();
            Appender.AppendLine("var entityStr = string.Empty;");

            foreach (var fieldInfo in SheetInfo.FieldInfos)
            {
                if (fieldInfo.FieldType == ExcelFieldType.Ignore)
                {
                    continue;
                }

                Appender.AppendLine($"entityStr = entityStr + {fieldInfo.EnglishName} + "
                                    + "\"" + "[__]" + "\"" + ";");
            }

            Appender.AppendLine("entityStr = entityStr.Remove(entityStr.Length - 1);");
            Appender.AppendLine("return entityStr;");
            Appender.ToLeft();
            Appender.AppendLine("}");
            Appender.AppendLine();
        }

        private void AppendCsToTxt()
        {
            Appender.AppendCsComment("获得本地数据的一份深度复制副本");
            Appender.AppendLine($"public {ScriptName} DeepCopy()");
            Appender.AppendLine("{");
            Appender.ToRight();
            Appender.AppendLine("var buff = SerializeUtility.Serialize(this);");
            Appender.AppendLine($"var entity = SerializeUtility.DeSerialize<{ScriptName}>(buff);");
            Appender.AppendLine("return entity;");
            Appender.ToLeft();
            Appender.AppendLine("}");
        }

        private void AppendFooter()
        {
            Appender.AppendCsFooter();
        }
    }
}