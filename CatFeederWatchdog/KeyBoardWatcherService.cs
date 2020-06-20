﻿using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Device.Gpio;

namespace CatFeederWatchdog
{
    public class KeyBoardWatcherService : IHostedService
    {
        private readonly FeederWatcherLogic watcherLogic;

        private GpioController gpio;

        public KeyBoardWatcherService(FeederWatcherLogic watcherLogic)
        {
            this.watcherLogic = watcherLogic ?? throw new ArgumentNullException(nameof(watcherLogic));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (gpio == null)
                gpio = new GpioController();

            gpio.OpenPin(5); gpio.SetPinMode(5, PinMode.InputPullUp);
            gpio.OpenPin(6); gpio.SetPinMode(6, PinMode.InputPullUp);
            gpio.OpenPin(13); gpio.SetPinMode(13, PinMode.InputPullUp);
            gpio.OpenPin(19); gpio.SetPinMode(19, PinMode.InputPullUp);

            gpio.RegisterCallbackForPinValueChangedEvent(5, PinEventTypes.Rising, Pin5ChangeCallback);
            gpio.RegisterCallbackForPinValueChangedEvent(6, PinEventTypes.Rising, Pin6ChangeCallback);
            //gpio.RegisterCallbackForPinValueChangedEvent(13, PinEventTypes.Rising, PinChangeCallback);
            //gpio.RegisterCallbackForPinValueChangedEvent(19, PinEventTypes.Rising, PinChangeCallback);

            return Task.CompletedTask;
        }

        public void Pin5ChangeCallback(object sender, PinValueChangedEventArgs e) 
        {
            watcherLogic.Feed();
        }

        public void Pin6ChangeCallback(object sender, PinValueChangedEventArgs e)
        {
            Console.WriteLine($"now is {DateTime.Now}");
            Console.WriteLine($"Pin {e.PinNumber} is {e.ChangeType}");            
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            gpio.ClosePin(5);
            gpio.ClosePin(6);
            gpio.ClosePin(13);
            gpio.ClosePin(19);
            gpio.Dispose();
            gpio = null;
            return Task.CompletedTask;
        }
    }
}