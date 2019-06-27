#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com


#endregion

using Common.ScriptCreate;

namespace Client.DataTable.Editor
{
    public class YuU3dExcelCsharpInterfaceScriptCreator : YuAbsU3dExcelScriptCreator
    {
        public override ScriptType ScriptType => ScriptType.Csharp;

        protected override void AppendScript()
        {
            AppendHead();
            AppendUsingNameSpace();
            AppendInterfaceHead();
            AppendPropertyDeclare();
            AppendFooter();
        }

        private void AppendFooter()
        {
            Appender.AppendCsFooter();
        }

        private void AppendPropertyDeclare()
        {
            var fieldInfos = SheetInfo.FieldInfos;

            foreach (var fieldInfo in fieldInfos)
            {
                if (fieldInfo.FieldType == ExcelFieldType.Ignore)
                {
                    continue;
                }
                
                Appender.AppendCsComment(fieldInfo.ChineseName);
                Appender.AppendLine($"{fieldInfo.CsType} " +
                                    $"{fieldInfo.EnglishName} " + "{ get; }");
                Appender.AppendLine();
            }
        }

        private void AppendInterfaceHead()
        {
            //Appender.AppendLine($"namespace {AppSetting.PlayAsmId}");
            Appender.AppendLeftBracketsAndToRight();
            Appender.AppendCsComment("Excel数据表_" + SheetInfo.ChineseId);
            //var interfaceName = ExcelUtility.EntityInterfaceName(AppSetting, SheetInfo);
            var interfaceName = ExcelUtility.EntityInterfaceName(/*AppSetting*/SheetInfo);
            Appender.AppendLine("public interface " + interfaceName);
            //Appender.AppendLeftBracketsAndToRight();
        }

        private void AppendUsingNameSpace()
        {
            Appender.AppendUsingNamespace(
                "System",
                "System.Collections.Generic"
            );
        }

        private void AppendHead()
        {
            ////Appender.AppendCsHeadComment();
        }
    }
}