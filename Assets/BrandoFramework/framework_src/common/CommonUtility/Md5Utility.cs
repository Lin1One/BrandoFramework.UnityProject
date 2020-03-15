using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Common.Utility
{
    public static class Md5Utility
    {
        public static void CopyByMd5Compare(string source, string target)
        {
            if (!File.Exists(source) || !File.Exists(target))
            {
                return;
            }

            if (CompareTwoFileMd5(source, target)) return;
            File.Delete(source);
            IOUtility.CopyFile(target, source);
            Debug.Log($"目标路径{source}上的文件已和路径{target}同步完毕！");
        }

        /// <summary>
        /// 比较两个文件的Md5值并返回比较的结果。
        /// </summary>
        /// <param name="leftFile">文件一。</param>
        /// <param name="rightFile">文件二。</param>
        /// <returns></returns>
        public static bool CompareTwoFileMd5(string leftFile, string rightFile)
        {
            var leftMd5 = GetFileMd5(leftFile);
            var rightMd5 = GetFileMd5(rightFile);
            var result = leftMd5 == rightMd5;
            return result;
        }

        /// <summary>
        /// 获得目标文件的MD5值。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileMd5(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"目标文件{path}不存在！");
                return null;
            }

            var sb = CommonPool.StringBuilderPool.Take();
            sb.Clear();

            try
            {
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    var md5Provider = new MD5CryptoServiceProvider();
                    var retVal = md5Provider.ComputeHash(fs);
                    foreach (var t in retVal)
                    {
                        sb.Append(t.ToString("x2"));
                    }

                    var md5 = sb.ToString();
                    return md5;
                }
            }
            catch (Exception exception)
            {
                Debug.LogError("创建assetbundle文件Md5值时发生异常，异常信息错误为：" + exception.Message);
                return null;
            }
            finally
            {
                CommonPool.StringBuilderPool.Restore(sb);
            }
        }

        /// <summary>
        /// 获得双倍强化后的Md值。
        /// 用于生成密码md5。
        /// </summary>
        /// <param name="input">原始字符串。</param>
        /// <param name="key">加密密钥字符串。</param>
        /// <returns></returns>
        public static string GetStrongMd5Hash(string input, string key = "AiukMd5@")
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var hashCode =
                    BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(input)))
                        .Replace("_", "") +
                    BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(key)))
                        .Replace("_", "");

                var result = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(hashCode)))
                    .Replace("_", "");
                return result;
            }
        }
    }
}