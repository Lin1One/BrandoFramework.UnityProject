#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion


using Common.ScriptCreate;

namespace Client.DataTable.Editor
{
    public class YuU3dExcelEntityScriptCreator : YuAbsU3dExcelScriptCreator
    {
        public override ScriptType ScriptType => ScriptType.Csharp;
        private string ScriptName => ExcelUtility.EntityScriptName( SheetInfo);
        //private YuU3dCoreSetting CoreSetting => YuU3dCoreSettingDati.GetSingleDati().ActualSerializableObject;

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
            Appender.AppendLine("var buff = YuSerializeUtility.Serialize(this);");
            Appender.AppendLine($"var entity = YuSerializeUtility.DeSerialize<{ScriptName}>(buff);");
            Appender.AppendLine("return entity;");
            Appender.ToLeft();
            Appender.AppendLine("}");
        }

        private void AppendCsInitEntitys()
        {
            Appender.AppendCsComment("将excel的txt数据源转换为数据列表。");
            Appender.AppendLine($"public List<{ScriptName}> InitEntitys(List<string> rows)");
            Appender.AppendLine("{");
            Appender.ToRight();
            Appender.AppendLine($"var entitys = new List<{ScriptName}>();");
            Appender.AppendLine(" int i = 0;");
            Appender.AppendLine("try");
            Appender.AppendLine("{");
            Appender.ToRight();

            Appender.AppendLine("for (; i < rows.Count; i++)");
            Appender.AppendLine("{");
            Appender.ToRight();
            Appender.AppendLine("var strList = rows[i].Split(YuExcelConstant.Separator,");
            Appender.AppendLine("    StringSplitOptions.None).ToList();");
            Appender.AppendLine($"var entity = new {ScriptName}();");
            Appender.AppendLine("entity.InitEntity(strList);");
            Appender.AppendLine("entitys.Add(entity);");
            Appender.ToLeft();
            Appender.AppendLine("}");
            Appender.ToLeft();
            Appender.AppendLine("}");
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
                //var csEntityStr = handler.GetCodeStr(
                //    YuExcelConstant.CsLanguageType, info, AppSetting);
                //Appender.AppendLine(csEntityStr);
            }

            Appender.ToLeft();
            Appender.AppendLine("}");
            Appender.AppendLine();
        }

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

                ////if (YuSetting.Instance.IsUseProtobuf)
                ////{
                ////    Appender.AppendLine($"[ProtoBuf.ProtoMember({protobufMemberIndex})]");
                ////    protobufMemberIndex++;
                ////}

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

        private void AppendCsClassHead()
        {
            ////Appender.AppendLine($"namespace {AppSetting.PlayAsmId}");
            ////Appender.AppendLeftBracketsAndToRight();
            ////Appender.AppendCsComment("Excel数据表_" + SheetInfo.ChineseId);
            ////Appender.AppendLine("[Serializable]");
            ////if (CoreSetting.SerializeSetting.IsUseProtobuf)
            ////{
            ////    Appender.AppendLine("[ProtoBuf.ProtoContract]");
            ////}

            ////Appender.AppendLine($"public class {ScriptName}");
            ////var interfaceName = ExcelUtility.EntityInterfaceName(AppSetting, SheetInfo);
            ////Appender.AppendLine($"    : {interfaceName}, IYuExcelEntity<{ScriptName}>");
            ////Appender.AppendLeftBracketsAndToRight();
        }

        private void AppendNote()
        {
            Appender.AppendLine();
            Appender.AppendLine("// Excel本地数据表脚本，该脚本为自动创建请勿手动修改！");
            Appender.AppendLine();
        }

        private void AppendUsingNamespace()
        {
            Appender.AppendUsingNamespace(
                "System",
                typeof(ExcelFieldType).Namespace,
                "System.Collections.Generic",
                "System.Linq",
                //typeof(IYuInjector).Namespace,
                "Sirenix.OdinInspector",
                "UnityEngine"
            );
        }

        private void AppendHead()
        {
           // Appender.AppendCsHeadComment();
        }
    }
}