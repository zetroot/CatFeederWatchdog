using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CatFeederWatchdog
{
    public class DisplayUpdaterService : IObserver<FeedingState>, IHostedService
    {
        private readonly DisplayService displayService;
        private readonly ILogger<DisplayUpdaterService> logger;
        private readonly IDisposable sub;

        public DisplayUpdaterService(DisplayService displayService, ILogger<DisplayUpdaterService> logger, FeederWatcherLogic feederWatcher)
        {
            this.displayService = displayService;
            this.logger = logger;
            sub = feederWatcher.Subscribe(this);
        }

        public void OnCompleted()
        {
            logger.LogWarning("BL is completed");
        }

        public void OnError(Exception error)
        {
            logger.LogError(error, "BL has an error");
        }

        public async void OnNext(FeedingState value)
        {
            logger.LogTrace("Next recieved {0}", value.ToString());
            await displayService.DisplayFeedInfoAsync(value.LastFeed, value.NextFeed, value.IsCatStraving, value.Message);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogTrace("Starting display hosted service");
            return displayService.Init();            
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            sub.Dispose();
            return displayService.ClearDisplay();            
        }
    }
}
