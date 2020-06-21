﻿using EPaperDriver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CatFeederWatchdog
{
    class Program
    {
        public async static Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host
            .CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, configApp) => 
                {
                    configApp.AddJsonFile("appsettings.json", true, true);
                }
            )
            .ConfigureServices(services => 
                {
                    services.TryAddSingleton<FeederWatcherLogic>();
                    services.AddHostedService<KeyBoardWatcherService>();
                    services.AddHostedService<BackgroundDisplayUpdaterService>();
                    services.TryAddSingleton<DisplayService>();
                    services.TryAddSingleton<Eink2In7B>();
                    services.TryAddSingleton<DisplaySpiDriver>();
                }
            )
            .ConfigureLogging(logbuilder => logbuilder.AddConsole().SetMinimumLevel(LogLevel.Trace));
                
                
    }
}
