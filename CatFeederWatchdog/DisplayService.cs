using BetaSoft.EPaperHatCore;
using BetaSoft.EPaperHatCore.GUI;
using BetaSoft.EPaperHatCore.GUI.Fonts;
using System;
using System.Collections.Generic;
using System.Text;
using static BetaSoft.EPaperHatCore.GUI.Enums;

namespace CatFeederWatchdog
{
    public class DisplayService
    {
        private DateTime lastDisplay;
        private readonly EpaperBase eid;
        public DisplayService()
        {
            eid = new Epaper27(176, 264);
            eid.Initialize();
        }

        public void DisplayFeed()
        {
            lastDisplay = DateTime.Now;

            //create black and red screens
            var blackScreen = new Screen(176, 264, Rotate.ROTATE_270, Color.WHITE);
            var redScreen = new Screen(176, 264, Rotate.ROTATE_270, Color.WHITE);

            //draw something on screen using a font
            var font = new Font8();
            blackScreen.DrawString(10, 20, $"Displayed at {lastDisplay}", font, Color.WHITE, Color.BLACK);
            blackScreen.DrawString(10, 50, "Red text", font, Color.WHITE, Color.RED);

            eid.DisplayScreens(blackScreen, redScreen);
        }
    }
}
