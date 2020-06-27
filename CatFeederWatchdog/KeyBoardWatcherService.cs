using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Device.Gpio;
using Microsoft.Extensions.Logging;

namespace CatFeederWatchdog
{
    public class KeyBoardWatcherService : IHostedService
    {
        private readonly FeederWatcherLogic watcherLogic;
        private readonly ILogger<KeyBoardWatcherService> logger;

        private GpioController gpio;

        public KeyBoardWatcherService(FeederWatcherLogic watcherLogic, ILogger<KeyBoardWatcherService> logger)
        {
            this.watcherLogic = watcherLogic ?? throw new ArgumentNullException(nameof(watcherLogic));
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (gpio == null)
                gpio = new GpioController();
            logger.LogTrace("Strating kbd watcher");
            gpio.OpenPin(5); gpio.SetPinMode(5, PinMode.InputPullUp);
            gpio.OpenPin(6); gpio.SetPinMode(6, PinMode.InputPullUp);
            gpio.OpenPin(13); gpio.SetPinMode(13, PinMode.InputPullUp);
            gpio.OpenPin(19); gpio.SetPinMode(19, PinMode.InputPullUp);

            gpio.RegisterCallbackForPinValueChangedEvent(5, PinEventTypes.Rising, Pin5ChangeCallback);
            gpio.RegisterCallbackForPinValueChangedEvent(6, PinEventTypes.Rising, Pin6ChangeCallback);
            //gpio.RegisterCallbackForPinValueChangedEvent(13, PinEventTypes.Rising, PinChangeCallback);
            gpio.RegisterCallbackForPinValueChangedEvent(19, PinEventTypes.Rising, Pin19ChangeCallback);
            logger.LogTrace("kbd watcher started");
            return Task.CompletedTask;
        }

        private void Pin19ChangeCallback(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            logger.LogTrace("Pin 19 Changed");
            watcherLogic.Refresh();
        }

        private void Pin5ChangeCallback(object sender, PinValueChangedEventArgs e) 
        {
            logger.LogTrace("Pin 5 changed");
            watcherLogic.Feed();
        }

        private void Pin6ChangeCallback(object sender, PinValueChangedEventArgs e)
        {
            logger.LogTrace("Pin 6 changed");
            watcherLogic.FeedDouble();
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogTrace("Stopping kbd watcher");
            gpio.ClosePin(5);
            gpio.ClosePin(6);
            gpio.ClosePin(13);
            gpio.ClosePin(19);
            gpio.Dispose();
            gpio = null;
            logger.LogTrace("Stopped kbd watcher");
            return Task.CompletedTask;
        }
    }
}
