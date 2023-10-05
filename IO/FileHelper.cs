using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ATools
{
    /// <summary>
    /// 文件帮助类
    /// </summary>
    public static class FileHelper
    {
        #region 日志写锁
        /// <summary>
        /// 日志写锁
        /// </summary>
        private static readonly ReaderWriterLockSlim logWriteLock = new ReaderWriterLockSlim();
        #endregion

        #region 创建文件
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileBytes"></param>
        public static void Create(string filePath, params byte[] fileBytes)
        {
            using var fs = File.Create(filePath);
            fs.Write(fileBytes);
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileStream"></param>
        public static void Create(string filePath, Stream fileStream)
        {
            using (fileStream)
            {
                using var fs = File.Create(filePath);
                var buffer = new byte[2048];
                var count = 0;
                //每次读取2kb数据，然后写入文件
                while ((count = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    fs.Write(buffer, 0, count);
                }
            }
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileBytes"></param>
        public static async Task CreateAsync(string filePath, params byte[] fileBytes)
        {
            using var fs = File.Create(filePath);
            await fs.WriteAsync(fileBytes, 0, fileBytes.Length);
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileStream"></param>
        public static async Task CreateAsync(string filePath, Stream fileStream)
        {
            using (fileStream)
            {
                using var fs = File.Create(filePath);
                var buffer = new byte[2048];
                var count = 0;
                //每次读取2kb数据，然后写入文件
                while ((count = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    await fs.WriteAsync(buffer, 0, count);
                }
            }
        }

        /// <summary>
        /// 读取嵌入资源创建指定文件
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="manifestResourcePath">嵌入资源路径</param>
        /// <param name="filePath">文件路径</param>
        public static void CreateFileFromManifestResource(Assembly assembly, string manifestResourcePath, string filePath)
        {
            if (!File.Exists(filePath))
            {
                //读取嵌入资源
                using var stream = assembly.GetManifestResourceStream(manifestResourcePath);
                using var fs = File.Create(filePath);
                var buffer = new byte[2048];
                var count = 0;
                //每次读取2kb数据，然后写入文件
                while ((count = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    fs.Write(buffer, 0, count);
                }
            }
        }

        /// <summary>
        /// 读取嵌入资源创建指定文件
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="manifestResourcePath">嵌入资源路径</param>
        /// <param name="filePath">文件路径</param>
        public static async Task CreateFileFromManifestResourceAsync(Assembly assembly, string manifestResourcePath, string filePath)
        {
            if (!File.Exists(filePath))
            {
                //读取嵌入资源
                using var stream = assembly.GetManifestResourceStream(manifestResourcePath);
                using var fs = File.Create(filePath);
                var buffer = new byte[2048];
                var count = 0;
                //每次读取2kb数据，然后写入文件
                while ((count = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    await fs.WriteAsync(buffer, 0, count);
                }
            }
        }
        #endregion



        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="filePath">物理路径</param>
        /// <returns>string</returns>
        public static string ReadFile(string filePath)
        {
            var sb = new StringBuilder();
            if (File.Exists(filePath))
            {
                using var sr = new StreamReader(filePath);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="filePath">物理路径</param>
        /// <returns>string</returns>
        public static async Task<string> ReadFileAsync(string filePath)
        {
            var sb = new StringBuilder();
            if (File.Exists(filePath))
            {
                using var sr = new StreamReader(filePath);
                string line;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    sb.AppendLine(line);
                }
            }
            return sb.ToString();
        }



        #region 写入文本
        /// <summary>
        /// 写入文本
        /// </summary>
        /// <param name="content">文本内容</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="isAppend">是否追加</param>
        /// <param name="encoding">编码格式</param>
        public static void WriteFile(string content, string filePath, bool isAppend = false, string encoding = "utf-8")
        {
            try
            {
                logWriteLock.EnterWriteLock();
                IsExist(filePath);
                using var sw = new StreamWriter(filePath, isAppend, Encoding.GetEncoding(encoding));
                sw.Write(content);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                logWriteLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 写入文本
        /// </summary>
        /// <param name="content">文本内容</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="isAppend">是否追加</param>
        /// <param name="encoding">编码格式</param>
        public static async Task WriteFileAsync(string content, string filePath, bool isAppend = false, string encoding = "utf-8")
        {
            try
            {
                logWriteLock.EnterWriteLock();
                IsExist(filePath);
                using var sw = new StreamWriter(filePath, isAppend, Encoding.GetEncoding(encoding));
                await sw.WriteAsync(content);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                logWriteLock.ExitWriteLock();
            }
        }
        #endregion

        #region 检测文件
        /// <summary>
        /// 判断文件是否存在 不存在则创建
        /// </summary>
        /// <param name="path">物理绝对路径</param>
        public static void IsExist(string path)
        {
            if (!(path is null) && !File.Exists(path))
            {
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                File.Create(path).Close();
            }
        }
        #endregion

        #region 复制文件
        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="sourceFileName">源路径</param>
        /// <param name="destFileName">目的路径</param>
        /// <param name="overwrite">是否覆盖，默认：否</param>
        /// <returns>bool</returns>
        public static bool FileCopy(string sourceFileName, string destFileName, bool overwrite = false)
        {
            var isExist = File.Exists(destFileName);

            if (!overwrite && isExist)
                return true;

            if (overwrite && isExist)
                File.Delete(destFileName);

            using var fStream = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var fs = File.Create(destFileName);
            var buffer = new byte[2048];
            var bytesRead = 0;
            //每次读取2kb数据，然后写入文件
            while ((bytesRead = fStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                fs.Write(buffer, 0, bytesRead);
            }
            return true;
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="sourceFileName">源路径</param>
        /// <param name="destFileName">目的路径</param>
        /// <param name="overwrite">是否覆盖，默认：否</param>
        /// <returns>bool</returns>
        public static async Task<bool> FileCopyAsync(string sourceFileName, string destFileName, bool overwrite = false)
        {
            var isExist = File.Exists(destFileName);

            if (!overwrite && isExist)
                return true;

            if (overwrite && isExist)
                File.Delete(destFileName);

            using var fStream = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var fs = File.Create(destFileName);
            var buffer = new byte[2048];
            var bytesRead = 0;
            //每次读取2kb数据，然后写入文件
            while ((bytesRead = await fStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                await fs.WriteAsync(buffer, 0, bytesRead);
            }
            return true;
        }
        #endregion

        #region 移动文件
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="sourceFileName">源路径</param>
        /// <param name="destFileName">目的路径</param>
        /// <param name="overwrite">是否覆盖，默认：否</param>
        /// <returns>bool</returns>
        public static bool FileMove(string sourceFileName, string destFileName, bool overwrite = false)
        {
            var isExist = File.Exists(destFileName);

            if (!overwrite && isExist)
                return true;

            if (overwrite && isExist)
                File.Delete(destFileName);

            using var fStream = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var fs = File.Create(destFileName);
            var buffer = new byte[2048];
            var bytesRead = 0;
            //每次读取2kb数据，然后写入文件
            while ((bytesRead = fStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                fs.Write(buffer, 0, bytesRead);
            }

            if (File.Exists(sourceFileName))
                File.Delete(sourceFileName);

            return true;
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="sourceFileName">源路径</param>
        /// <param name="destFileName">目的路径</param>
        /// <param name="overwrite">是否覆盖，默认：否</param>
        /// <returns>bool</returns>
        public static async Task<bool> FileMoveAsync(string sourceFileName, string destFileName, bool overwrite = false)
        {
            var isExist = File.Exists(destFileName);

            if (!overwrite && isExist)
                return true;

            if (overwrite && isExist)
                File.Delete(destFileName);

            using var fStream = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var fs = File.Create(destFileName);
            var buffer = new byte[2048];
            var bytesRead = 0;
            //每次读取2kb数据，然后写入文件
            while ((bytesRead = await fStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                await fs.WriteAsync(buffer, 0, bytesRead);
            }

            if (File.Exists(sourceFileName))
                File.Delete(sourceFileName);

            return true;
        }
        #endregion

        #region base64数据保存到文件
        /// <summary>
        /// html5 base64数据保存到文件
        /// </summary>
        /// <param name="data">base64数据</param>
        /// <param name="filePath">文件路径(包含文件扩展名)</param>
        /// <returns>bool</returns>
        public static bool SaveBase64ToFile(string data, string filePath)
        {
            var index = data?.ToLower().IndexOf("base64,") ?? -1;
            if (index > -1)
            {
                data = data.Substring(index + 7);
                using var fs = File.Create(filePath);
                var bytes = Convert.FromBase64String(data);
                fs.Write(bytes, 0, bytes.Length);
                return true;
            }
            return false;
        }

        /// <summary>
        /// html5 base64数据保存到文件
        /// </summary>
        /// <param name="data">base64数据</param>
        /// <param name="filePath">文件路径(包含文件扩展名)</param>
        /// <returns>bool</returns>
        public static async Task<bool> SaveBase64ToFileAsync(string data, string filePath)
        {
            var index = data?.ToLower().IndexOf("base64,") ?? -1;
            if (index > -1)
            {
                data = data.Substring(index + 7);
                using var fs = File.Create(filePath);
                var bytes = Convert.FromBase64String(data);
                await fs.WriteAsync(bytes, 0, bytes.Length);
                return true;
            }
            return false;
        }
        #endregion

    }
}
 