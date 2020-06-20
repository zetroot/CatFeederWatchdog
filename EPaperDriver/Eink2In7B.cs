using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPaperDriver
{
    public class Eink2In7B
    {
        private const int width = 264;
        private const int height = 176;
        

        private readonly DisplaySpiDriver displayDriver;

        public Eink2In7B(DisplaySpiDriver displayDriver)
        {
            this.displayDriver = displayDriver ?? throw new ArgumentNullException(nameof(displayDriver));
        }

        private void log(string s) => Console.WriteLine(s);

        public void Init()
        {
            log("init start");
            displayDriver.Reset();
            log("resetted");

            log("booster soft start");
            displayDriver.SendCommand(EinkDisplayCommand.BoosterSoftStart);
            displayDriver.SendData(0x07, 0x07, 0x04);

            log("power optimization");
            displayDriver.SendCommand(EinkDisplayCommand.PowerOptimization);
            displayDriver.SendData(0x60, 0xA5);

            displayDriver.SendCommand(EinkDisplayCommand.PowerOptimization);
            displayDriver.SendData(0x89, 0xA5);

            displayDriver.SendCommand(EinkDisplayCommand.PowerOptimization);
            displayDriver.SendData(0x90, 0x00);

            displayDriver.SendCommand(EinkDisplayCommand.PowerOptimization);
            displayDriver.SendData(0x93, 0x2A);

            log("Reset DFV_EN ");
            displayDriver.SendCommand(EinkDisplayCommand.PartialDisplayRefresh);
            displayDriver.SendData(0x00);

            log("power on");
            displayDriver.SendCommand(EinkDisplayCommand.PowerOn);
            log("send poweron, waiting...");
            displayDriver.WaitUntilReady();

            displayDriver.SendCommand(EinkDisplayCommand.PanelSetting);
            displayDriver.SendData(0x0f);
        }

        public void ClearDisplay()
        {
            log("transferring 1");
            displayDriver.SendCommand(EinkDisplayCommand.DataStartTransmission1);           
            displayDriver.SendData(Enumerable.Repeat((byte)0x00, width * height / 8).ToArray());
            displayDriver.SendCommand(EinkDisplayCommand.DataStop);

            log("transferring 2");
            displayDriver.SendCommand(EinkDisplayCommand.DataStartTransmission2);
            displayDriver.SendData(Enumerable.Repeat((byte)0x00, width * height / 8).ToArray());
            displayDriver.SendCommand(EinkDisplayCommand.DataStop);

            log("refresh");
            displayDriver.SendCommand(EinkDisplayCommand.DisplayRefresh);
            log("refreshed");
            displayDriver.WaitUntilReady();
        }

        public void DisplayData(byte[] black, byte[] red)
        {
            displayDriver.SendCommand(EinkDisplayCommand.DataStartTransmission1);
            displayDriver.SendData(black);
            displayDriver.SendCommand(EinkDisplayCommand.DataStop);

            displayDriver.SendCommand(EinkDisplayCommand.DataStartTransmission2);
            displayDriver.SendData(red);
            displayDriver.SendCommand(EinkDisplayCommand.DataStop);

            displayDriver.SendCommand(EinkDisplayCommand.DisplayRefresh);
            displayDriver.WaitUntilReady();
        }

        public void DisplayBlack(byte[] black)
        {
            displayDriver.SendCommand(EinkDisplayCommand.DataStartTransmission1);
            displayDriver.SendData(black);
            displayDriver.SendCommand(EinkDisplayCommand.DataStop);

            displayDriver.SendCommand(EinkDisplayCommand.DisplayRefresh);
            displayDriver.WaitUntilReady();
        }
    }
}
