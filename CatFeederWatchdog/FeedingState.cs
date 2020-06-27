using System;
using System.Text;

namespace CatFeederWatchdog
{
    public struct FeedingState
    {
        public DateTime LastFeed { get; }
        public FeedQuantity LastFeedQuantity { get; }
        public DateTime NextFeed { get; }
        public bool IsCatStraving => DateTime.Now >= NextFeed;
        public string Message
        {
            get
            {
                if (NextFeed > DateTime.Now)
                {
                    var left = NextFeed - DateTime.Now;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Кошки ели по ");
                    sb.AppendLine(LastFeedQuantity switch { FeedQuantity.Half => "1/2", FeedQuantity.Full => "целой", _ => "много" });
                    sb.AppendLine("Осталось");
                    if (left.Hours > 0)
                    {
                        sb.Append($"больших лап - {left.Hours}\nмаленьких - {left.Minutes}");
                    }
                    else
                    {
                        sb.Append($"маленьких лап-{left.Minutes}");
                    }
                    return sb.ToString();
                }
                else
                {
                    return "Пора кормить!";
                }

            }
        }

        public FeedingState(DateTime lastFeed, DateTime nextFeed, FeedQuantity feedQuantity)
        {
            LastFeed = lastFeed;
            NextFeed = nextFeed;
            LastFeedQuantity = feedQuantity;
        }
    }
}