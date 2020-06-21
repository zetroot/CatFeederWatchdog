using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CatFeederWatchdog
{
    public class BackgroundDisplayUpdaterService : IHostedService
    {
        private readonly FeederWatcherLogic watcherLogic;
        private readonly DisplayService displayService;
        private readonly ILogger<BackgroundDisplayUpdaterService> logger;
        private Timer timer;

        public BackgroundDisplayUpdaterService(FeederWatcherLogic watcherLogic, DisplayService displayService, ILogger<BackgroundDisplayUpdaterService> logger)
        {
            this.watcherLogic = watcherLogic ?? throw new ArgumentNullException(nameof(watcherLogic));
            this.displayService = displayService;
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(UpdateDisplay, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private async void UpdateDisplay(object state)
        {            
            await displayService.DisplayFeedInfoAsync(watcherLogic.LastFeed, watcherLogic.NextFeed, watcherLogic.IsCatStraving(), watcherLogic.GenerateMessage());
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            return Task.CompletedTask;
        }
    }
}
