using FluentFTP;
using FluentFTP.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATools
{
    /// <summary>
    /// FTP 助手
    /// </summary>
    public class FtpHelper
    {

        /// <summary>
        /// FTP 上传
        /// </summary>
        /// <param name="host">FTP主机</param>
        /// <param name="user">FTP用户名</param>
        /// <param name="pwd">FTP密码</param>
        /// <returns></returns>
        public static async Task<(bool, string)> PushAsync(string host, string user, string pwd, string localfile, string remotefile)
        {
            var flag = false;
            string msg = string.Empty;

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pwd)) { return (false, "FTP无法登录"); }
            if (string.IsNullOrWhiteSpace(localfile) || string.IsNullOrWhiteSpace(remotefile))
            {
                return (false, "本地路径或远程路径不能为空");
            }

            using (var ftp = new FtpClient(host, user, pwd))
            {
                ftp.Connect();

                if (!ftp.DirectoryExists(Path.GetDirectoryName(remotefile)))
                    ftp.CreateDirectory(Path.GetDirectoryName(remotefile));

                var result = ftp.UploadFile(localfile, remotefile,createRemoteDir:true);
                flag = result.IsSuccess();
            }

            msg = flag ? "" : "上传失败!!!";
            return (flag,msg);
        }

        /// <summary>
        /// FTP上传文件夹
        /// </summary>
        /// <param name="host"></param>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        /// <param name="localfile"></param>
        /// <param name="remotefile"></param>
        /// <returns></returns>
        public static async Task<(bool,string)> PushDirectory(string host, string user, string pwd, string localfile, string remotefile)
        {
            var result = new List<FtpResult>();
            using (var ftp = new FtpClient(host, user, pwd))
            {
                ftp.Connect();
                try
                {
                    result = ftp.UploadDirectory(localfile, remotefile);
                }
                catch (Exception ex)
                {
                    SerilogHelper.Log.Information($@"PushDirectory 方法执行异常,异常信息:{ex.Message}");
                }

            }

            return result.Where(t => t.IsSuccess != true).Count() > 0 ?
                    (false, "FTP上传异常!!") : (true, "");
        }
    }
}
