using EPaperDriver;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;

namespace CatFeederWatchdog
{
    public class DisplayService
    {
     
        private const int einkWidth = 264;
        private const int einkHeight = 176;
        private const int lineHeight = 25;//(einkHeight - padding * 2) / 5;
        private const int padding = 3;
        private readonly Eink2In7B display;
        private readonly ILogger<DisplayService> logger;

        public DisplayService(Eink2In7B display, ILogger<DisplayService> logger)
        {
            this.display = display;
            this.logger = logger;
        }

        public Task ClearDisplay() => display.ClearDisplay();

        public async Task DisplayFeedInfoAsync(DateTime lastFeed, DateTime nextFeed, bool isAlert, string message)
        {
                if (!display.IsInitialized)
                {
                    logger.LogDebug("Not initialized. Initializing!");
                    await display.InitAsync();
                }
                logger.LogTrace("Initialized");
                logger.LogTrace("Clearing");
                //var clearTask = Task.Run(() => display.ClearDisplay());

                Bitmap regular_img = new Bitmap(einkWidth, einkHeight);
                Bitmap alert_img = new Bitmap(einkWidth, einkHeight);

                RectangleF fullBody = new RectangleF(0, 0, einkWidth, einkHeight);
                RectangleF prevFeedLine = new RectangleF(padding, padding, einkWidth - 75, lineHeight);
                RectangleF nextFeedLine = new RectangleF(padding, padding + lineHeight, einkWidth - 75, lineHeight);
                RectangleF displayLine = new RectangleF(padding, padding + 2 * lineHeight, einkWidth - 75, lineHeight);
                RectangleF msgLine = new RectangleF(padding, padding + 3 * lineHeight, einkWidth - 75, einkHeight - padding*2 - lineHeight*3);

                Graphics regular_g = Graphics.FromImage(regular_img);
                Graphics alert_g = Graphics.FromImage(alert_img);

                regular_g.SmoothingMode = SmoothingMode.HighSpeed;
                regular_g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                regular_g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                regular_g.FillRectangle(Brushes.White, fullBody);
                alert_g.FillRectangle(Brushes.White, fullBody);

                regular_g.DrawString($"Кошки ели:{lastFeed.ToString("HH:mm")}", new Font(FontFamily.GenericMonospace, 14, FontStyle.Bold), Brushes.Black, prevFeedLine);
                if (isAlert)
                {
                    alert_g.DrawString($"Покорми в:{nextFeed.ToString("HH:mm")}", new Font(FontFamily.GenericMonospace, 14, FontStyle.Bold), Brushes.Black, nextFeedLine);
                    alert_g.DrawString(message, new Font(FontFamily.GenericMonospace, 14, FontStyle.Bold), Brushes.Black, msgLine);
                }
                else
                {
                    regular_g.DrawString($"Покорми в:{nextFeed.ToString("HH:mm")}", new Font(FontFamily.GenericMonospace, 14, FontStyle.Bold), Brushes.Black, nextFeedLine);
                    regular_g.DrawString(message, new Font(FontFamily.GenericMonospace, 14, FontStyle.Bold), Brushes.Black, msgLine);
                }
                regular_g.DrawString($"Сейчас:{DateTime.Now.ToString("HH:mm")}", new Font(FontFamily.GenericMonospace, 14, FontStyle.Bold), Brushes.Black, displayLine);

                regular_g.Flush();
                alert_g.Flush();

                regular_img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                alert_img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                logger.LogTrace("bitmap generated");

                var zipped_regular = ZipBytes(GetBytes(regular_img).ToArray()).ToArray();
                var zipped_alert = ZipBytes(GetBytes(alert_img).ToArray()).ToArray();
                logger.LogTrace("bitmap zipped");
                //await clearTask;
                logger.LogTrace("clear finished");
                await display.DisplayData(zipped_regular, zipped_alert);
                logger.LogInformation("bitmap displayed");
            
        }

        internal async Task Init()
        {
            if (!display.IsInitialized)
            {
                logger.LogDebug("Initializing display");
                await display.InitAsync();
            }
        }

        static IEnumerable<byte> GetBytes(Bitmap image)
        {
            var height = image.Height;
            var width = image.Width;
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    var p = image.GetPixel(i, j);
                    if (p.G > 64)
                        yield return 0x00;
                    else
                        yield return 0xff;
                }
            }
        }

        static IEnumerable<byte> ZipBytes(IEnumerable<byte> bytes)
        {
            var cnt = bytes.Count();
            var enumerator = bytes.GetEnumerator();
            enumerator.Reset();
            for (int i = 0; i < cnt; i += 8)
            {

                enumerator.MoveNext();
                var byte8 = enumerator.Current == 0 ? 0 : 1 << 7;
                enumerator.MoveNext();
                var byte7 = enumerator.Current == 0 ? 0 : 1 << 6;
                enumerator.MoveNext();
                var byte6 = enumerator.Current == 0 ? 0 : 1 << 5;
                enumerator.MoveNext();
                var byte5 = enumerator.Current == 0 ? 0 : 1 << 4;
                enumerator.MoveNext();
                var byte4 = enumerator.Current == 0 ? 0 : 1 << 3;
                enumerator.MoveNext();
                var byte3 = enumerator.Current == 0 ? 0 : 1 << 2;
                enumerator.MoveNext();
                var byte2 = enumerator.Current == 0 ? 0 : 1 << 1;
                enumerator.MoveNext();
                var byte1 = enumerator.Current == 0 ? 0 : 1 << 0;

                byte result = (byte)(byte1 | byte2 | byte3 | byte4 | byte5 | byte6 | byte7 | byte8);
                yield return result;
            }
        }

    }
}
