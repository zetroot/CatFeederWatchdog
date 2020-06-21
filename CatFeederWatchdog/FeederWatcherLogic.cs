using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CatFeederWatchdog
{
    public class FeederWatcherLogic
    {
        private readonly DisplayService displayService;
        private readonly TimeSpan feedPeriod = TimeSpan.FromHours(4);

        public DateTime LastFeed { get; private set; }
        public DateTime NextFeed { get; private set; }

        public FeederWatcherLogic(DisplayService displayService)
        {
            LastFeed = DateTime.Now;
            NextFeed = DateTime.Now;
            this.displayService = displayService;
        }

        public async Task Feed()
        {
            LastFeed = DateTime.Now;
            NextFeed = DateTime.Now.Add(feedPeriod);
            await displayService.DisplayFeedInfoAsync(LastFeed, NextFeed, false, "Кошки ели по 1/2");
        }

        public async Task FeedDouble()
        {
            LastFeed = DateTime.Now;
            NextFeed = DateTime.Now.Add(feedPeriod).Add(feedPeriod);
            await displayService.DisplayFeedInfoAsync(LastFeed, NextFeed, false, "Кошки ели по целой");
        }

        public bool IsCatStraving()
        {         
            return NextFeed < DateTime.Now;
        }

        internal string GenerateMessage()
        {
            if (IsCatStraving())
                return "Покорми кошек!";
            else
                return "Не верь кошкам!";
        }

        internal Task Refresh()
        {
            return displayService.DisplayFeedInfoAsync(LastFeed, NextFeed, IsCatStraving(), IsCatStraving() ? "Покорми!" : "Кошки сыты");
        }
    }
}
