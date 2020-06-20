using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.Spi;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EPaperDriver
{
    public class DisplaySpiDriver : IDisposable
    {
        /// <summary>
        /// GPIO Reset Pin Number
        /// </summary>
        private const int PIN_RESET = 17;

        /// <summary>
        /// GPIO SPI DC Pin Number
        /// </summary>
        private const int PIN_DC = 25;

        /// <summary>
        /// GPIO SPI CS Pin Number
        /// </summary>
        private const int PIN_CS = 8;

        /// <summary>
        /// GPIO Busy Pin Number
        /// </summary>
        private const int PIN_BUSY = 24;

        private readonly SpiDevice spiDevice;
        private readonly GpioController gpioController;

        public DisplaySpiDriver()
        {
            gpioController = new GpioController();
            var spiConnectionSettings = new SpiConnectionSettings(0, 0);
            spiDevice = SpiDevice.Create(spiConnectionSettings);

            // open pins
            gpioController.OpenPin(PIN_RESET);
            gpioController.OpenPin(PIN_DC);
            gpioController.OpenPin(PIN_CS);
            gpioController.OpenPin(PIN_BUSY);

            gpioController.SetPinMode(PIN_RESET, PinMode.Output);
            gpioController.SetPinMode(PIN_DC, PinMode.Output);
            gpioController.SetPinMode(PIN_CS, PinMode.Output);
            gpioController.SetPinMode(PIN_BUSY, PinMode.Input);
            gpioController.Write(PIN_CS, PinValue.High);
        }

        /// <summary>
        /// Отправить команду
        /// </summary>
        /// <param name="displayCommand">Команда для отправки</param>
        public void SendCommand(EinkDisplayCommand displayCommand)
        {
            gpioController.Write(PIN_DC, PinValue.Low);
            SendByte((byte)displayCommand);
        }

        /// <summary>
        /// Отправить данные
        /// </summary>
        /// <param name="data">один байт данных</param>
        public void SendData(byte data)
        {
            gpioController.Write(PIN_DC, PinValue.High);
            SendByte(data);
        }

        /// <summary>
        /// Отправить массив данных
        /// </summary>
        /// <param name="data">массив данных</param>
        public void SendData(params byte[] data)
        {
            gpioController.Write(PIN_DC, PinValue.High);
            foreach (var b in data)
                SendByte(b);
        }

        /// <summary>
        /// Внутренний хелпер отправки данных
        /// </summary>
        /// <param name="data"></param>
        private void SendByte(byte data)
        {
            gpioController.Write(PIN_CS, PinValue.Low);
            spiDevice.WriteByte(data);
            gpioController.Write(PIN_CS, PinValue.High);
        }

        ///// <summary>
        ///// Reset display
        ///// </summary>
        //public void Reset()
        //{
        //    gpioController.Write(PIN_RESET, PinValue.High);
        //    Thread.Sleep(200);
        //    gpioController.Write(PIN_RESET, PinValue.Low);
        //    Thread.Sleep(2);
        //    gpioController.Write(PIN_RESET, PinValue.High);
        //    Thread.Sleep(200);
        //}

        /// <summary>
        /// Асинхронный сброс дисплея
        /// </summary>
        /// <returns></returns>
        public async Task ResetAsync()
        {
            gpioController.Write(PIN_RESET, PinValue.High);
            await Task.Delay(200);
            gpioController.Write(PIN_RESET, PinValue.Low);
            await Task.Delay(2);
            gpioController.Write(PIN_RESET, PinValue.High);
            await Task.Delay(200);
        }

        ///// <summary>
        ///// Wait while display is busy
        ///// </summary>
        //public void WaitUntilReady()
        //{
        //    bool busy;
        //    do
        //    {
        //        var busyPinValue = gpioController.Read(PIN_BUSY);
        //        busy = busyPinValue != PinValue.High;
        //    } while (busy);
        //}

        /// <summary>
        /// Получить задачу, которая завершится, когда дисплей будет готов
        /// </summary>
        /// <returns>Задача ожидания готовности дисплея</returns>
        public Task WaitDisplayAsync()
        {
            var tcs = new TaskCompletionSource<object>();
            PinChangeEventHandler pinChange = (s, e) => 
            {
                if (e.ChangeType == PinEventTypes.Rising)
                    tcs.SetResult(null);
            };

            gpioController.RegisterCallbackForPinValueChangedEvent(PIN_BUSY, PinEventTypes.Rising, pinChange);
            return tcs.Task.ContinueWith(t => gpioController.UnregisterCallbackForPinValueChangedEvent(PIN_BUSY, pinChange));            
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    gpioController.Write(PIN_RESET, PinValue.Low);
                    gpioController.Write(PIN_DC, PinValue.Low);
                    gpioController.Write(PIN_CS, PinValue.Low);
                    spiDevice.Dispose();
                    gpioController.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {            
            Dispose(true);
        }
        #endregion


    }
}
