using System;
using System.Collections.Generic;
using System.Text;

namespace CatFeederWatchdog
{
    public class FeederWatcherLogic
    {
        private DateTime? lastFeed;
        private DateTime nextFeed;

        public FeederWatcherLogic()
        {
            lastFeed = null;
            nextFeed = DateTime.Now;
        }

        public void Feed()
        {
            lastFeed = DateTime.Now;
            nextFeed = DateTime.Now.Add(TimeSpan.FromSeconds(5));
        }

        public bool IsCatStraving()
        {
            if (!lastFeed.HasValue) return true;
            return nextFeed > DateTime.Now;
        }

        public TimeSpan GetTimeFromLastFeed()
        {
            if (!lastFeed.HasValue) return TimeSpan.Zero;
            return DateTime.Now - lastFeed.Value;
        }
    }
}
