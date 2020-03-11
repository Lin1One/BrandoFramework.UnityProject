using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Common.Utility
{
    /// <summary>
    /// IO工具。
    /// 1. 目录操作。
    /// 2. 文件操作。
    /// 3. 路径操作。
    /// </summary>
    public static class IOUtility
    {
        #region 目录操作

        /// <summary>
        /// 获得目标目录的所有子目录及自身。
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="selectFunc"></param>
        /// <returns></returns>
        public static List<string> GetAllDir(string dir, Func<string, bool> selectFunc = null, 
            bool onlyTopDir = false, bool includeRoot = true)
        {
            //  新建返回结果列表并添加自身。
            var dirs = new List<string>();
            if (includeRoot) dirs.Add(dir);
            GetAllDirs(dir, dirs, onlyTopDir);
            if (selectFunc != null)
            {
                dirs = dirs.Where(selectFunc).ToList();
            }

            return dirs;
        }

        /// <summary>
        /// 获得指定目录下所有子目录。
        /// </summary>
        /// <param name="dir">目标目录。</param>
        /// <param name="dirs">存放结果列表。</param>
        /// <returns></returns>
        private static void GetAllDirs(string dir, List<string> dirs, bool onlyTopDir = false)
        {
            if (string.IsNullOrEmpty(dir))
            {
                //Debug.LogError("目标目录为空！");
                return;
            }

            if (!Directory.Exists(dir))
            {
                //Debug.LogError($"目标字符串{dir}不是一个有效目录");
                return;
            }

            var sonDirs = onlyTopDir ? Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly) :
                 Directory.GetDirectories(dir);
                
            if (sonDirs.Length <= 0) return;
            foreach (var sonDir in sonDirs)
            {
                var tempDir = string.Empty;
                if (!sonDir.EndsWith("/"))
                {
                    tempDir = sonDir + "/";
                }

                tempDir = tempDir.Replace("\\", "/");
                dirs.Add(tempDir);
                if(!onlyTopDir)
                {
                    GetAllDirs(tempDir, dirs);
                }
                
            }
        }


        /// <summary>
        /// 获得指定目录下的所有子目录中同名目录列表。
        /// </summary>
        /// <param name="targetDirs"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        public static List<string> GetSameIdDirsAtDirTree(List<string> targetDirs,
            bool isLower = true)
        {
            var idPathMap = new Dictionary<string, string>();
            var sameIdPaths = new List<string>();

            foreach (var targetDir in targetDirs)
            {
                var dirs = GetAllDir(targetDir,null,false,false);

                foreach (var d in dirs)
                {
                    var dirId = GetLastDir(d);
                    if (isLower)
                    {
                        dirId = dirId.ToLower();
                    }

                    if (!idPathMap.ContainsKey(dirId))
                    {
                        idPathMap.Add(dirId, d);
                    }
                    else
                    {
                        var d1 = idPathMap[dirId];
                        sameIdPaths.Add(d1);
                        sameIdPaths.Add(d);
                    }
                }
            }

            return sameIdPaths;
        }

        /// <summary>
        /// 确保指定的目录或路径上的所有目录的存在性。
        /// </summary>
        /// <param name="targetPath"></param>
        public static void EnsureDirExist(string targetPath)
        {
            var lastIndex = targetPath.LastIndexOf("/", StringComparison.Ordinal);
            var lastDir = targetPath.Substring(0, lastIndex);
            CreateDirectory(lastDir);
        }

        /// <summary>
        /// 如果指定的目录不存在则创建一个新目录。
        /// </summary>
        /// <param name="targetDir">目标目录。</param>
        /// <param name="isHide">是否隐藏刚创建的目录。</param>
        /// <returns></returns>
        public static void CreateDirectory(string targetDir, bool isHide = false)
        {
            if (Directory.Exists(targetDir)) return;

            Directory.CreateDirectory(targetDir);
            if (isHide)
            {
                File.SetAttributes(targetDir, FileAttributes.Hidden);
            }
        }

        /// <summary>
        /// 使用系统的默认应用打开指定路径上的目录或者文件。
        /// </summary>
        /// <param name="path"></param>
        public static void OpenFolderOrFile(string path)
        {
            System.Diagnostics.Process.Start(path);
        }

        /// <summary>
        /// 克隆目标目录及其下所有子目录的目录树结构到目标目录(不包括文件)
        /// </summary>
        /// <param name="leftDir">目录一。</param>
        /// <param name="rightDir">目录二。</param>
        private static void CloneDirectoryTree(string leftDir, string rightDir)
        {
            var dirs = GetAllDir(leftDir);
            foreach (var dir in dirs)
            {
                var newDir = dir.Replace(leftDir, rightDir);
                CreateDirectory(newDir);
            }
        }

        /// <summary>
        /// 同步两个目录及其子目录下的所有文件。
        /// </summary>
        /// <param name="leftDir">目录一。</param>
        /// <param name="righyDir">目录二。</param>
        /// <param name="filter">文件过滤器委托。</param>
        /// <param name="beforeCopy">拷贝操作前委托。</param>
        public static void SyncDirectory(string leftDir, string righyDir,
            Func<string, bool> filter = null, Func<string, string> beforeCopy = null)
        {
            EnsureDirExist(leftDir);
            EnsureDirExist(righyDir);

            CloneDirectoryTree(leftDir, righyDir);
            var allFiles = filter == null ? GetPathsContainSonDir(leftDir) : GetPathsContainSonDir(leftDir, filter);
            allFiles.ForEach(p => TryCopy(p, p.Replace(leftDir, righyDir), true, beforeCopy));
        }

        /// <summary>
        /// 拷贝一个目录及其所有子目录及文件到目标目录下.
        /// </summary>
        /// <param name="leftDir">待拷贝源目录。</param>
        /// <param name="rightDir">拷贝目标目录。</param>
        public static void CopyDirectory(string leftDir, string rightDir)
        {
            var files = GetPathsContainSonDir(leftDir);
            CloneDirectoryTree(leftDir, rightDir);
            foreach (var path in files)
            {
                var newPath = path.Replace(leftDir, rightDir);
                TryCopy(path, newPath, true);
            }
        }

        /// <summary>
        /// 删除一个目录及其所有子目录和文件。
        /// 如果目录不存在则操作取消。
        /// </summary>
        /// <param name="dir">目标目录</param>
        public static void DeleteDirectory(string dir)
        {
            if (!Directory.Exists(dir)) return;

            var di = new DirectoryInfo(dir);
            di.Delete(true);
        }

        #endregion

        #region 文件操作

        /// <summary>
        /// 将指定文件移动到目标目录中。
        /// 如果目标目录不存在将会自动创建新目录。
        /// </summary>
        /// <param name="sourcePath">源文件。</param>
        /// <param name="targetPath">目标路径。</param>
        public static void Move(string sourcePath, string targetPath)
        {
            EnsureDirExist(targetPath);
            File.Move(sourcePath, targetPath);
        }

        /// <summary>
        /// 如果指定的路径上有文件存在则删除。
        /// </summary>
        /// <param name="path">目标路径。</param>
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// 将指定的文件拷贝到指定目录
        /// 如果目标路径上已有文件存在则删除已有的然后新建
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="targetDir"></param>
        public static void CopyFile(string sourceFilePath, string targetDir)
        {
            EnsureDirExist(targetDir);
            var filename = Path.GetFileName(sourceFilePath);
            if (!targetDir.EndsWith("/"))
            {
                targetDir = targetDir + "/";
            }

            var newPath = targetDir + filename;
            if (File.Exists(newPath))
            {
                File.Delete(newPath);
            }

            File.Copy(sourceFilePath, newPath);
        }

        /// <summary>
        /// 将指定的文件拷贝到目标路径上。
        /// 如果目标路径上已有文件并且isDelete参数为真时则会比较两个文件的Md5值，
        /// 如果不想等则会覆盖目标路径上的文件，如果相等则拷贝操作取消。
        /// </summary>
        /// <param name="sourcePath">源文件路径。</param>
        /// <param name="newPath">目标文件路径。</param>
        /// <param name="isDelete">当目标文件路径上已经存在文件是，是否删除目标文件。</param>
        /// <param name="beforeFunc">拷贝开始前用于修正目标路径的委托。</param>
        public static void TryCopy(string sourcePath, string newPath, bool isDelete = false,
            Func<string, string> beforeFunc = null)
        {
            if (!File.Exists(sourcePath))
            {
                //Debug.Log($"源路径{sourcePath}文件不存在！");
                return;
            }

            if (beforeFunc != null)
            {
                newPath = beforeFunc(newPath);
            }

            if (!File.Exists(newPath))
            {
                EnsureDirExist(newPath);
                File.Copy(sourcePath, newPath);
            }

            if (!isDelete) return;
            if (YuMd5Utility.CompareTwoFileMd5(sourcePath, newPath)) return;

            File.Delete(newPath);
            File.Copy(sourcePath, newPath);
        }

        public static void Rename(string sourcePath, string targetPath)
        {
            var fielInfo = new FileInfo(sourcePath);
            fielInfo.MoveTo(Path.Combine(sourcePath, targetPath));
        }

        /// <summary>
        /// 获得目标目录下所有文件的文件信息列表。
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static List<FileInfo> GetFileInfosAtDir(string dir, bool allFile = false)
        {
            if (!Directory.Exists(dir))
            {
                return null;
            }

            var di = new DirectoryInfo(dir);
            var fileInfos = allFile ? di.GetFiles("*", SearchOption.AllDirectories).ToList() :
                di.GetFiles().ToList();
            return fileInfos;
        }

        /// <summary>
        /// 删除目录下的所有文件。
        /// </summary>
        /// <param name="dir">目标目录。</param>
        public static void DeleteAllFile(string dir)
        {
            if (!Directory.Exists(dir))
            {
                //Debug.LogError($"给定的字符串{dir}不是一个有效目录！");
                return;
            }

            var paths = GetPathsContainSonDir(dir);
            paths.ForEach(DeleteFile);
        }

        /// <summary>
        /// 在指定的路径上创建文本文件并写入指定内容。
        /// 该方法会自动创建对应的目录树结构。
        /// </summary>
        /// <param name="path">目标路径。</param>
        /// <param name="content">要写入的文本内容。</param>
        public static void WriteAllText(string path, string content,bool forceWrite = true)
        {
            EnsureDirExist(path);
            if (File.Exists(path))
            {
                if (!forceWrite)
                {//Debug.Log($"目标路径{path}已存在文件，创建取消！");
                    return;
                }
                File.Delete(path);
            }

            File.WriteAllText(path, content);
        }

        public static void WriteAllBytes(string path, byte[] bytes, bool forceWrite = false)
        {
            EnsureDirExist(path);
            if (File.Exists(path))
            {
                if (!forceWrite)
                {
                    return;
                }
                File.Delete(path);
            }

            File.WriteAllBytes(path, bytes);
        }

        public static void SerializeAndWriteBytes(string path, object obj, bool isDeleteExist = false)
        {
            var bytes = SerializeUtility.Serialize(obj);
            WriteAllBytes(path, bytes, isDeleteExist);
        }

        /// <summary>
        /// 使用指定的字节数组创建一个二进制文件。
        /// </summary>
        /// <param name="path">目标文件路径。</param>
        /// <param name="buffer">字节数组。</param>
        public static void CreateBinaryFile(string path, byte[] buffer)
        {
            EnsureDirExist(path);
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                using (var bw = new BinaryWriter(fs))
                {
                    bw.Write(buffer);
                }
            }
        }

        #region 异步文件操作

        private static FileStream fs;
        private static Action<byte[]> readCallback;

        private static List<byte> buffer;
        private static List<byte> Buffer => buffer ?? (buffer = new List<byte>(1024 * 1024));

        public static void ReadFileAsync(string path, Action<byte[]> callback)
        {
            if (!File.Exists(path))
            {
                return;
            }

            readCallback = callback;
            var tmpBuffer = new byte[1024 * 1024];
            fs = File.OpenRead(path);
            fs.BeginRead(tmpBuffer, 0, tmpBuffer.Length, OnReadCompleted, tmpBuffer);
        }

        private static void OnReadCompleted(IAsyncResult ar)
        {
            var tempBuffer = (byte[])ar.AsyncState;
            int readedLength = fs.EndRead(ar);
            if (readedLength > 0)
            {
                for (int i = 0; i < readedLength; i++)
                {
                    var b = tempBuffer[i];
                    Buffer.Add(b);
                }

                fs.BeginRead(tempBuffer, 0, tempBuffer.Length, OnReadCompleted, tempBuffer);
            }
            else
            {
                var bytes = Buffer.ToArray();
                readCallback(bytes);
            }
        }

        #endregion

        #endregion

        #region 路径操作

        /// <summary>
        /// 获得目标路径上最后一个目录的字符串。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetLastDir(string path)
        {
            string last;
            path = path.Replace('\\', '/');

            if (path.EndsWith("/"))
            {
                var array = path.Split('/');
                last = array[array.Length - 2];
            }
            else
            {
                last = path.Split('/').Last();
            }

            return last;
        }

        /// <summary>
        /// 获得目标路径上的去除前 n 个目录的字符串。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetSomeDirPath(string path, int n)
        {
            string someDirPath = "";
            path = path.Replace('\\', '/');

            if (path.EndsWith("/"))
            {
                var array = path.Split('/');
                for (int i = n; i < array.Length - 1; i++)
                {
                    someDirPath += array[i];
                    if (i != array.Length - 2)
                    {
                        someDirPath += "_";
                    }
                }
            }
            else
            {
                var array = path.Split('/');
                for (int i = n; i < array.Length; i++)
                {
                    someDirPath += array[n];
                    if (i != array.Length - 1)
                    {
                        someDirPath += "_";
                    }
                }
            }
            return someDirPath;
        }

        /// <summary>
        /// 获得指定目录及所有子目录下所有文件路径。
        /// </summary>
        /// <param name="dir">目标目录。</param>
        /// <param name="fileFilter">文件过滤委托。</param>
        /// <param name="dirFilter">目录过滤委托。</param>
        /// <returns></returns>
        public static List<string> GetPathsContainSonDir(string dir, Func<string, bool> fileFilter = null,
            Func<string, bool> dirFilter = null)
        {
            EnsureDirExist(dir);
            var dirs = GetAllDir(dir, dirFilter);
            var paths = new List<string>();

            foreach (var d in dirs)
            {
                var files = Directory.GetFiles(d).ToList();
                files = files.Select(p => p.Replace("\\", "/")).ToList();
                paths.AddRange(files);
            }

            if (fileFilter != null)
            {
                paths = paths.Where(fileFilter).ToList();
            }

            return paths;
        }

        /// <summary>
        /// 获得目标目录下所有的文件路径。
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static List<string> GetPaths(string dir)
        {
            var paths = Directory.GetFiles(dir).ToList();
            return paths;
        }

        /// <summary>
        /// 获得目标目录下所有的文件路径。
        /// 包含子目录
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static List<string> GetAllFilesPaths(string dir)
        {
            var paths = Directory.GetFiles(dir).ToList();
            foreach(var str in paths)
            {
                str.Replace("\\", "/");
            }
            var childDirPaths = Directory.GetDirectories(dir);

            foreach (var childDirPath in childDirPaths)
            {
                paths.AddRange(GetAllFilesPaths(childDirPath));
            }
            return paths;
        }


        /// <summary>
        /// 返回目标目录下所有的文件名。
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="selecter"></param>
        /// <returns></returns>
        public static List<string> GetFileIds(string dir, Func<string, bool> selecter = null)
        {
            var paths = Directory.GetFiles(dir).ToList();
            var fileIds = selecter == null
                ? paths.Select(Path.GetFileNameWithoutExtension).ToList()
                : paths.Where(selecter).Select(Path.GetFileNameWithoutExtension).ToList();

            return fileIds;
        }

        /// <summary>
        /// 获得指定目录下及其所有子目录中所有文件的文件名及文件路径字典。
        /// </summary>
        /// <param name="dir">指定目录</param>
        /// <param name="fileFilter">文件过滤器。</param>
        /// <param name="dirFilter">目录过滤器。</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetPathDictionary(string dir,
            Func<string, bool> fileFilter = null, Func<string, bool> dirFilter = null)
        {
            if (!Directory.Exists(dir))
            {
                CreateDirectory(dir);
            }

            var pathDictionary = new Dictionary<string, string>();

            var paths = GetPathsContainSonDir(dir, fileFilter, dirFilter);
            foreach (var path in paths)
            {
                var fileName = Path.GetFileNameWithoutExtension(path);
                if (fileName != null && pathDictionary.ContainsKey(fileName))
                {
                    //Debug.LogWarning($"目标目录及其子目录下存在同名文件，文件名为：{fileName}");
                    continue;
                }

                if (fileName != null)
                {
                    pathDictionary.Add(fileName, path);
                }
            }

            return pathDictionary;
        }

        #endregion
    }
}