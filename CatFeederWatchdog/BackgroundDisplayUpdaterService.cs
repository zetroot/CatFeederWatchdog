using Microsoft.Extensions.Hosting;
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
        private Timer timer;

        public BackgroundDisplayUpdaterService(FeederWatcherLogic watcherLogic, DisplayService displayService)
        {
            this.watcherLogic = watcherLogic ?? throw new ArgumentNullException(nameof(watcherLogic));
            this.displayService = displayService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(UpdateDisplay, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
            return Task.CompletedTask;
        }

        private void UpdateDisplay(object state)
        {
            Console.WriteLine($"Is cat straving? {watcherLogic.IsCatStraving()}");
            Console.WriteLine($"Time from last feed {watcherLogic.GetTimeFromLastFeed()}");
            displayService.DisplayFeed();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            return Task.CompletedTask;
        }
    }
}
