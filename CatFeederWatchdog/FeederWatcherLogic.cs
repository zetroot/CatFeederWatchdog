using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace CatFeederWatchdog
{
    public class FeederWatcherLogic : IObservable<FeedingState>
    {
        private readonly TimeSpan feedPeriod = TimeSpan.FromHours(4);
        private readonly ILogger<FeederWatcherLogic> logger;

        private BehaviorSubject<FeedingState> subject;
        private FeedQuantity lastFeedQuantity;
        
        public DateTime LastFeed { get; private set; }
        public DateTime NextFeed { get; private set; }

        public FeederWatcherLogic(ILogger<FeederWatcherLogic> logger)
        {
            LastFeed = DateTime.Now;
            NextFeed = DateTime.Now;
            subject = new BehaviorSubject<FeedingState>(new FeedingState());            
            
            Observable
                .Interval(TimeSpan.FromMinutes(20))
                .Subscribe(_ => { subject.OnNext(new FeedingState(LastFeed, NextFeed, lastFeedQuantity)); logger.LogInformation("Regular refresh"); });
            this.logger = logger;
        }

        public void Feed()
        {
            logger.LogTrace("Feed is called");
            LastFeed = DateTime.Now;
            NextFeed = DateTime.Now.Add(feedPeriod);
            lastFeedQuantity = FeedQuantity.Half;
            subject.OnNext(new FeedingState(LastFeed, NextFeed, lastFeedQuantity));
            Observable
                .Timer(feedPeriod)
                .Subscribe(_ => { subject.OnNext(new FeedingState(LastFeed, NextFeed, lastFeedQuantity)); logger.LogInformation("Next feed alert"); }) ;
        }

        public void FeedDouble()
        {
            LastFeed = DateTime.Now;
            NextFeed = DateTime.Now.Add(feedPeriod).Add(feedPeriod);
            lastFeedQuantity = FeedQuantity.Full;
            subject.OnNext(new FeedingState(LastFeed, NextFeed, lastFeedQuantity));
            Observable
                .Timer(feedPeriod * 2)
                .Subscribe(_ => { subject.OnNext(new FeedingState(LastFeed, NextFeed, lastFeedQuantity)); logger.LogInformation("Next feed alert"); });
        }

        public IDisposable Subscribe(IObserver<FeedingState> observer)
        {
            logger.LogTrace("Observer is subsribing: {0}", observer);
            return subject.Subscribe(observer);
        }

        internal void Refresh()
        {
            logger.LogTrace("Refresh called");
            subject.OnNext(new FeedingState(LastFeed, NextFeed, lastFeedQuantity));
        }
    }
}
