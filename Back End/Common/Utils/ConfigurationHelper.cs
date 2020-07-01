using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Utils
{
    public class ConfigurationHelper
    {
        public static IConfigurationRoot Init()
        {
            return new ConfigurationBuilder().AddEnvironmentVariables()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{EnvironmentName}.json", true, true)
                .Build();
        }
        public static string EnvironmentName
        {
            get
            {
                return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            }
        }
    }
}
