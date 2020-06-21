using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EPaperDriver
{
    public class Eink2In7B
    {
        private const int width = 264;
        private const int height = 176;

        private readonly DisplaySpiDriver displayDriver;
        private readonly ILogger<Eink2In7B> logger;
        private bool isInitialized;

        public bool IsInitialized { get => isInitialized; private set => isInitialized = value; }

        private static byte[] lut_vcom_dc = new byte[]{
            0x00, 0x00,
            0x00, 0x1A, 0x1A, 0x00, 0x00, 0x01,
            0x00, 0x0A, 0x0A, 0x00, 0x00, 0x08,
            0x00, 0x0E, 0x01, 0x0E, 0x01, 0x10,
            0x00, 0x0A, 0x0A, 0x00, 0x00, 0x08,
            0x00, 0x04, 0x10, 0x00, 0x00, 0x05,
            0x00, 0x03, 0x0E, 0x00, 0x00, 0x0A,
            0x00, 0x23, 0x00, 0x00, 0x00, 0x01
        };

        //R21H
        private static byte[] lut_ww = new byte[]{
            0x90, 0x1A, 0x1A, 0x00, 0x00, 0x01,
            0x40, 0x0A, 0x0A, 0x00, 0x00, 0x08,
            0x84, 0x0E, 0x01, 0x0E, 0x01, 0x10,
            0x80, 0x0A, 0x0A, 0x00, 0x00, 0x08,
            0x00, 0x04, 0x10, 0x00, 0x00, 0x05,
            0x00, 0x03, 0x0E, 0x00, 0x00, 0x0A,
            0x00, 0x23, 0x00, 0x00, 0x00, 0x01
        };

        //R22H r
        private static byte[] lut_bw = new byte[]{
            0xA0, 0x1A, 0x1A, 0x00, 0x00, 0x01,
            0x00, 0x0A, 0x0A, 0x00, 0x00, 0x08,
            0x84, 0x0E, 0x01, 0x0E, 0x01, 0x10,
            0x90, 0x0A, 0x0A, 0x00, 0x00, 0x08,
            0xB0, 0x04, 0x10, 0x00, 0x00, 0x05,
            0xB0, 0x03, 0x0E, 0x00, 0x00, 0x0A,
            0xC0, 0x23, 0x00, 0x00, 0x00, 0x01
        };

        //R23H w
        private static byte[] lut_bb = new byte[] {
            0x90, 0x1A, 0x1A, 0x00, 0x00, 0x01,
            0x40, 0x0A, 0x0A, 0x00, 0x00, 0x08,
            0x84, 0x0E, 0x01, 0x0E, 0x01, 0x10,
            0x80, 0x0A, 0x0A, 0x00, 0x00, 0x08,
            0x00, 0x04, 0x10, 0x00, 0x00, 0x05,
            0x00, 0x03, 0x0E, 0x00, 0x00, 0x0A,
            0x00, 0x23, 0x00, 0x00, 0x00, 0x01
        };

        //R24H b
        private static byte[] lut_wb = new byte[]{
            0x90, 0x1A, 0x1A, 0x00, 0x00, 0x01,
            0x20, 0x0A, 0x0A, 0x00, 0x00, 0x08,
            0x84, 0x0E, 0x01, 0x0E, 0x01, 0x10,
            0x10, 0x0A, 0x0A, 0x00, 0x00, 0x08,
            0x00, 0x04, 0x10, 0x00, 0x00, 0x05,
            0x00, 0x03, 0x0E, 0x00, 0x00, 0x0A,
            0x00, 0x23, 0x00, 0x00, 0x00, 0x01
        };

        private void SetLut()
        {
            
            displayDriver.SendCommand(EinkDisplayCommand.LutForVcom);                            //vcom
            displayDriver.SendData(lut_vcom_dc);
            
            displayDriver.SendCommand(EinkDisplayCommand.LutBlue);                      //ww --
            displayDriver.SendData(lut_ww);

            displayDriver.SendCommand(EinkDisplayCommand.LutWhite);                      //bw r
            displayDriver.SendData(lut_bw);

            displayDriver.SendCommand(EinkDisplayCommand.LutGray1);                      //wb w
            displayDriver.SendData(lut_bb);

            displayDriver.SendCommand(EinkDisplayCommand.LutGray2);                      //bb b
            displayDriver.SendData(lut_wb);
        }

        public Eink2In7B(DisplaySpiDriver displayDriver, ILogger<Eink2In7B> logger)
        {
            this.displayDriver = displayDriver ?? throw new ArgumentNullException(nameof(displayDriver));
            this.logger = logger;
            isInitialized = false;
        }

        public async Task InitAsync()
        {
            if (isInitialized) return;
            isInitialized = true;
            logger.LogTrace("Initializing display");
            await displayDriver.ResetAsync();
            logger.LogTrace("Display resetted");

            

            logger.LogTrace("booster soft start");
            displayDriver.SendCommand(EinkDisplayCommand.BoosterSoftStart);
            displayDriver.SendData(0x07, 0x07, 0x04);

            logger.LogTrace("power optimization");
            displayDriver.SendCommand(EinkDisplayCommand.PowerOptimization);
            displayDriver.SendData(0x60, 0xA5);

            displayDriver.SendCommand(EinkDisplayCommand.PowerOptimization);
            displayDriver.SendData(0x89, 0xA5);

            displayDriver.SendCommand(EinkDisplayCommand.PowerOptimization);
            displayDriver.SendData(0x90, 0x00);

            displayDriver.SendCommand(EinkDisplayCommand.PowerOptimization);
            displayDriver.SendData(0x93, 0x2A);

            logger.LogTrace("Reset DFV_EN ");
            displayDriver.SendCommand(EinkDisplayCommand.PartialDisplayRefresh);
            displayDriver.SendData(0x00);

            logger.LogTrace("Power settings");
            displayDriver.SendCommand(EinkDisplayCommand.PowerSetting);
            displayDriver.SendData(0x03, 0x00, 0x2b, 0x2b, 0x09);

            
            logger.LogTrace("Power on");
            displayDriver.SendCommand(EinkDisplayCommand.PowerOn);
            logger.LogTrace("Waiting for display...");
            //await displayDriver.WaitDisplayAsync();
            displayDriver.WaitDisplay();
            logger.LogTrace("Display started");
            displayDriver.SendCommand(EinkDisplayCommand.PanelSetting);
            displayDriver.SendData(0xaf);

            logger.LogTrace("PLL control");
            displayDriver.SendCommand(EinkDisplayCommand.PllControl);
            displayDriver.SendData(0x3a);

            
            logger.LogTrace("VcmDcSetting");
            displayDriver.SendCommand(EinkDisplayCommand.VcmDcSetting);
            displayDriver.SendData(0x12);

            logger.LogTrace("VcomAndDataIntervalSetting");
            displayDriver.SendCommand(EinkDisplayCommand.VcomAndDataIntervalSetting);
            displayDriver.SendData(0x87);


            SetLut();
        }

        public Task ClearDisplay()
        {
            logger.LogTrace("Clear black screen");
            displayDriver.SendCommand(EinkDisplayCommand.DataStartTransmission1);
            displayDriver.SendData(Enumerable.Repeat((byte)0x00, width * height / 8).ToArray());
            displayDriver.SendCommand(EinkDisplayCommand.DataStop);

            logger.LogTrace("Clear red screen");
            displayDriver.SendCommand(EinkDisplayCommand.DataStartTransmission2);
            displayDriver.SendData(Enumerable.Repeat((byte)0x00, width * height / 8).ToArray());
            displayDriver.SendCommand(EinkDisplayCommand.DataStop);

            logger.LogTrace("refresh");
            displayDriver.SendCommand(EinkDisplayCommand.DisplayRefresh);
            logger.LogTrace("refreshed");
            //return displayDriver.WaitDisplayAsync().ContinueWith(_ => logger.LogTrace("Display is cleared"), TaskContinuationOptions.OnlyOnRanToCompletion);
            displayDriver.WaitDisplay();
            logger.LogTrace("Display is cleared");
            return Task.CompletedTask;
        }

        public Task DisplayData(byte[] black, byte[] red)
        {
            logger.LogTrace("Sending black data");
            displayDriver.SendCommand(EinkDisplayCommand.DataStartTransmission1);
            displayDriver.SendData(black);
            displayDriver.SendCommand(EinkDisplayCommand.DataStop);
            logger.LogTrace("Black data sent");
            logger.LogTrace("Sending red data");
            displayDriver.SendCommand(EinkDisplayCommand.DataStartTransmission2);
            displayDriver.SendData(red);
            displayDriver.SendCommand(EinkDisplayCommand.DataStop);
            logger.LogTrace("Red data send");

            displayDriver.SendCommand(EinkDisplayCommand.DisplayRefresh);
            //return displayDriver.WaitDisplayAsync().ContinueWith(_ => logger.LogTrace("B&R data displayed"), TaskContinuationOptions.OnlyOnRanToCompletion);
            displayDriver.WaitDisplay();
            logger.LogTrace("B&R data displayed");
            return Task.CompletedTask;
        }

        public Task DisplayBlack(byte[] black)
        {
            logger.LogTrace("Sending black data");
            displayDriver.SendCommand(EinkDisplayCommand.DataStartTransmission1);
            displayDriver.SendData(black);
            displayDriver.SendCommand(EinkDisplayCommand.DataStop);
            logger.LogTrace("Sent black data");

            displayDriver.SendCommand(EinkDisplayCommand.DisplayRefresh);
            //return displayDriver.WaitDisplayAsync().ContinueWith(_ => logger.LogTrace("B data displayed"), TaskContinuationOptions.OnlyOnRanToCompletion);
            displayDriver.WaitDisplay();
            logger.LogTrace("B data displayed");
            return Task.CompletedTask;
        }
    }
}
