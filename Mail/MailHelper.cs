using ATool.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATools
{
    /// <summary>
    /// Mail 帮助类
    /// </summary>
    public class MailHelper
    {

        private static MailOption mailOption;

        /// <summary>
        /// 构造函数
        /// </summary>
        static MailHelper()
        {
            //AppSettingHelper.configuration.GetSection("Mail").Bind(mailOption);
            mailOption = AppSettingHelper.configuration.GetValue<MailOption>("Mail");
        }


        public static async Task SendMail()
        {
            //if (string.IsNullOrEmpty(input.FromUserEmail))
            //{
            //    input.FromUserEmail = "alan.shi@luxshare-ict.com";
            //}

            //var message = new MimeMessage();
            //message.From.Add(new MailboxAddress("DefaultSendMail", mailOption.DefaultMailSendAddress));
            ////message.To.Add(new MailboxAddress("Test01", input.FromUserEmail));
            //input.ToUserEmail.ForEach(t =>
            //{
            //    message.To.Add(new MailboxAddress(t.Split('@')[0], t));
            //});

            //message.Subject = input.Subject;

            //var builder = new BodyBuilder();
            //builder.TextBody = input.Body;
            //message.Body = builder.ToMessageBody();

            //using (var client = new MailKit.Net.Smtp.SmtpClient())
            //{
            //    client.CheckCertificateRevocation = false;
            //    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            //    try
            //    {
            //        await client.ConnectAsync(mailOption.Host, mailOption.Port, SecureSocketOptions.Auto);
            //        await client.AuthenticateAsync(mailOption.UserName, mailOption.PassWord);
            //        await client.SendAsync(message);
            //        await client.DisconnectAsync(true);
            //    }
            //    catch (Exception ex)
            //    {
            //        return new ResponseResult<string>()
            //        {
            //            Code = 200,
            //            Msg = "邮件发送异常!!!"
            //        };
            //        await Console.Out.WriteLineAsync(ex.Message);
            //    }

            //}

            //return new ResponseResult<string>()
            //{
            //    Code = 200,
            //    Msg = "邮件发送成功!!!"
            //};
        }
    }
}
