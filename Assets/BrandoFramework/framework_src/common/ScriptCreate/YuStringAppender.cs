using client_common;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// 字符串追加器。
    /// 提供常用的字符串动态构建方法封装。
    /// 提供各种语言脚本创建的API封装。
    /// </summary>
    public class YuStringAppender : IDisposable
    {
        private readonly StringBuilder m_Builder;

        /// <summary>
        /// 缩进宽度。
        /// </summary>
        private int m_SpaceWidth;

        private void AppendSpace()
        {
            for (var i = 0; i < m_SpaceWidth; i++)
            {
                m_Builder.Append(" ");
            }
        }

        public void AppendSingleComment(string comment)
        {
            AppendLine($"// {comment}");
        }

        public void AppendMultiComment(params string[] comments)
        {
            AppendLine("/*");
            foreach (var s in comments)
            {
                AppendLine("* " + s);
            }

            AppendLine("*/");
            AppendLine();
        }

        /// <summary>
        /// 忽视当前的缩进长度直接在起始位置写入一行。
        /// </summary>
        /// <param name="content"></param>
        public void AppendLineAtStart(string content)
        {
            m_Builder.AppendLine(content);
        }

        /// <summary>
        /// 忽视当前的缩进长度直接在起始位置写入。
        /// </summary>
        /// <param name="content"></param>
        public void AppendAtStart(string content)
        {
            m_Builder.Append(content);
        }

        /// <summary>
        /// 向左缩进4个空格
        /// </summary>
        public void ToLeft()
        {
            var result = m_SpaceWidth - 4;
            if (result < 0)
            {
                Debug.Log("缩进不能为负数！");
                return;
            }

            m_SpaceWidth = result;
        }

        /// <summary>
        /// 向右缩进4个空格。
        /// </summary>
        public void ToRight()
        {
            m_SpaceWidth += 4;
        }

        public string Content
        {
            get { return m_Builder.ToString(); }
        }

        public YuStringAppender()
        {
            m_Builder = YuCommonFactory.StringBuilderPool.Take();
            m_Builder.Clear();
        }

        public void Dispose()
        {
            m_Builder.Clear();
            YuCommonFactory.StringBuilderPool.Restore(m_Builder);
        }

        public override string ToString()
        {
            var content = m_Builder.ToString();
            var bytes = Encoding.Default.GetBytes(content);
            var utf8Str = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Default, Encoding.UTF8, bytes));
            return utf8Str;
        }

        public void Clean()
        {
            m_Builder.Clear();
        }

        #region Append方法

        public static YuStringAppender StartAppend(string format, params object[] args)
        {
            var builder = new YuStringAppender();
            builder.m_Builder.AppendFormat(format, args);
            return builder;
        }

        public static YuStringAppender StartAppend(string format)
        {
            var builder = new YuStringAppender();
            builder.m_Builder.Append(format);
            return builder;
        }

        public static YuStringAppender StartAppendLine(string value)
        {
            var builder = new YuStringAppender();
            builder.m_Builder.AppendLine(value);
            return builder;
        }

        public static YuStringAppender StartAppendLine(string format, params object[] args)
        {
            var builder = new YuStringAppender();
            builder.m_Builder.AppendFormat(format, args);
            builder.m_Builder.AppendLine();
            return builder;
        }

        public YuStringAppender Append(string format, params object[] args)
        {
            m_Builder.AppendFormat(format, args);
            return this;
        }

        public YuStringAppender Append(string format)
        {
            m_Builder.Append(format);
            return this;
        }

        public YuStringAppender AppendLine()
        {
            AppendSpace();
            m_Builder.AppendLine();
            return this;
        }

        public YuStringAppender AppendRightLine(string value)
        {
            ToRight();
            AppendLine(value);
            return this;
        }

        #region 添加花括号并修改缩进

        public void AppendLeftBracketsAndToRight()
        {
            AppendLine("{");
            ToRight();
        }

        public void AppendRightBracketsAndToLeft()
        {
            ToLeft();
            AppendLine("}");
        }

        #endregion

        public YuStringAppender AppendLine(string value)
        {
            AppendSpace();
            m_Builder.AppendLine(value);
            return this;
        }

        public void AppendPrecomplie(params string[] pars)
        {
            for (int i = 0; i < pars.Length; i++)
            {
                var args = pars[i];

                Append(i == 0 ? $"#if {args}" : $" && {args}");
            }

            AppendLine();
        }

        private YuStringAppender AppendLine(string value, params object[] args)
        {
            AppendSpace();
            m_Builder.AppendFormat(value, args);
            m_Builder.AppendLine();
            return this;
        }

        #endregion

        #region Cs

        public void AppendUsingNamespace(params string[] namespaces)
        {
            foreach (var ns in namespaces)
            {
                AppendLine($"using {ns};");
            }

            AppendLine();
        }

        /// <summary>
        /// 追加Cs注释。
        /// </summary>
        /// <param name="bodyComment">主体注释文本。</param>
        /// <param name="paramNames">参数名注释列表。</param>
        /// <param name="paramComments">参数注释列表。</param>
        public void AppendCsComment
        (
            string bodyComment,
            List<string> paramNames = null,
            List<string> paramComments = null
        )
        {
            AppendLine("/// <summary>");
            AppendLine("/// " + bodyComment);
            AppendLine("/// </summary>");

            if (paramNames == null) return;

            if (paramComments == null)
            {
                foreach (var paramName in paramNames)
                {
                    AppendLine("/// <param name=\"" + paramName + "\"></param>");
                }
            }
            else
            {
                for (var i = 0; i < paramNames.Count; i++)
                {
                    var name = paramNames[i];
                    var comment = paramComments[i];
                    AppendLine("/// <param name=\"{0}\">{1}</param>", name, comment);
                }
            }
        }

        public void AppCsNoteHeader
        (
            string coderName = "Yu",
            string coderEmail = "Yu@gmail.com || 35490136@qq.com"
        )
        {
            AppendLine("#region Head");
            AppendLine();
            AppendLine($"// Author:                {coderName}");
            //AppendLine("// CreateDate:            {0}", YuDateUtility.DateAndTimeForNow);
            AppendLine($"// Email:                 {coderEmail}");
            AppendLine();
            AppendLine("#endregion");
            AppendLine();
        }

        public void AppendCsFooter()
        {
            ToLeft();
            AppendLine("}");
            ToLeft();
            AppendLine("}");
            AppendLine();
        }

        #endregion
    }
}