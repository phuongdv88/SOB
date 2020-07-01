using System;
using Serilog;
using Serilog.Core;
using Utils.Settings;

namespace Utils
{
    public class Log
    {
        //Tham khảo thêm cách log ở https://github.com/serilog/serilog/wiki/Structured-Data
        public static Logger LogInstance { get; set; }
        static Log()
        {
            var configuration = AppSettings.Configuration;
            LogInstance = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
        /// <summary>
        /// Log.Info("/Member/Register {@request}", request);
        /// </summary>
        /// <param name="msg">string</param>
        /// <param name="props">object</param>
        public static void Info(string msg, params object[] props)
        {
            LogInstance.Information(msg, props);
        }
        public static void Debug(string msg, params object[] props)
        {
            LogInstance.Debug(msg, props);
        }
        public static void Error(string msg, params object[] props)
        {
            LogInstance.Error(msg, props);
        }
        public static void Error(Exception ex, string msg, params object[] props)
        {
            LogInstance.Error(ex, msg, props);
        }
        public static void Fatal(string msg, params object[] props)
        {
            LogInstance.Fatal(msg, props);
        }
        public static void Fatal(Exception ex, string msg, params object[] props)
        {
            LogInstance.Fatal(ex, msg, props);
        }
    }
}
