using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomizeLogging.Infrastructure;

namespace CustomizeLogging
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext,logging) => {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
               
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.ClearProviders();
                    var config = new ColoredConsoleLoggerConfiguration
                    {
                        Color = ConsoleColor.Red,
                        LogLevel = LogLevel.Information

                    };
                    logging.AddProvider(new ColoredConsoleLoggerProvider(config));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
