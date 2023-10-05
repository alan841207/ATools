using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATools
{
    /// <summary>
    /// AppSetting 帮助类
    /// </summary>
    public class AppSettingHelper
    {

        public static IConfiguration configuration;

        /// <summary>
        /// 
        /// </summary>
        static AppSettingHelper()
        {
            configuration = new ConfigurationBuilder()
                                //.SetBasePath(Environment.CurrentDirectory)
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

            //var configuration = new ConfigurationBuilder()
            //                         .SetBasePath(Directory.GetCurrentDirectory())
            //                         .AddJsonFile($"appsettings.json")
            //                         .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true);

        }
    }
}
