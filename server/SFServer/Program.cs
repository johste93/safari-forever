using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SFServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((builderContext, config) =>
            {
                config.AddEnvironmentVariables();
                config.AddJsonFile("Properties/rewardConfig.json", optional: false, reloadOnChange: false);
                config.AddJsonFile("Properties/versionConfig.json", optional: false, reloadOnChange: false);
                config.AddJsonFile("Properties/priceConfig.json", optional: false, reloadOnChange: false);
                config.AddJsonFile("Properties/gameConfig.json", optional: false, reloadOnChange: false);
                config.AddJsonFile("Properties/slotMachineConfig.json", optional: false, reloadOnChange: false);
                config.AddJsonFile("Properties/cardsConfig.json", optional: false, reloadOnChange: false);
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddConsole();
                logging.AddDebug();
                logging.AddEventSourceLogger();
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.SetMinimumLevel(LogLevel.Debug);
            })
                .UseStartup<Startup>();
    }
}
