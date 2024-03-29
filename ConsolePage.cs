using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.AndroidExtensions;
using StardewValley;
using StardewValley.Menus;

namespace ConsoleMobile
{
    internal class ConsolePage : IClickableMenu
    {
        MobileScrollbar scrollbar;
        public float scrollPercent;
        public ConsolePage()
        {
            initializeUpperRightCloseButton();

            var scrollbarPaddingHeight = 100;
            scrollbar = new MobileScrollbar(viewport.Width - 100, scrollbarPaddingHeight / 2, 0, viewport.Height - scrollbarPaddingHeight);
            scrollbar.setPercentage(0);
        }
        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);
            var distFromY = y - scrollbar.Bounds.Y;
            scrollPercent = (distFromY / (float)scrollbar.Bounds.Height) * 100;
            scrollPercent = Math.Clamp(scrollPercent, 0, 100);
            scrollbar.setPercentage(scrollPercent);
        }
        public override void draw(SpriteBatch b)
        {
            //draw BG
            const int backgroundDark = 50;
            drawTextureBox(b, 0, 0, viewport.Width, viewport.Height, new Color(backgroundDark, backgroundDark, backgroundDark));
            base.draw(b); //draw close button
            var st = System.Diagnostics.Stopwatch.StartNew();
            drawLines(b);
            st.Stop();
            //1.2ms
            ModEntry.Instance.Monitor.Log("use time: " + st.Elapsed.TotalMilliseconds + "ms");

            scrollbar.draw(b);
        }
        Color InfoColor = Color.White;
        Color DebugColor = new Color(220, 220, 220);
        Color ErrorColor = new Color(255, 60, 60);
        Color WarnColor = new Color(255, 120, 0);
        Color AlertColor = new Color(200, 0, 255);
        Vector2 ExampleLineSize = Vector2.Zero;
        void drawLines(SpriteBatch batch)
        {
            var font = Game1.smallFont;
            if (ExampleLineSize == Vector2.Zero)
                ExampleLineSize = font.MeasureString("example line");

            int paddingTextRenderY = (int)ExampleLineSize.Y * 4;
            var lastTextPosRender = Vector2.Zero;
            lastTextPosRender.X = 50;
            lastTextPosRender.Y = paddingTextRenderY;

            var logLines = ConsoleHook.Lines;
            logLines.Add("[>> End Line <<]");

            int totalRenderLineHeight = 0;
            //adjust with scrollbar
            foreach (var line in logLines)
            {
                var lineSize = font.MeasureString(line);
                totalRenderLineHeight += (int)lineSize.Y;
            }
            lastTextPosRender.Y -= (int)((scrollPercent / 100f) * (totalRenderLineHeight - Game1.viewport.Height + (paddingTextRenderY * 2)));

            foreach (var line in logLines)
            {
                var textColor = InfoColor;
                var textLogLevel = line.Substring(10).Split(" ")[0];
                textColor = textLogLevel switch
                {
                    "DEBUG" => DebugColor,
                    "ERROR" => ErrorColor,
                    "WARN" => WarnColor,
                    "ALERT" => AlertColor,
                    "TRACT" => DebugColor,
                    _ => InfoColor,
                };

                var lineSize = font.MeasureString(line);
                batch.DrawString(font, line, lastTextPosRender, textColor);

                lastTextPosRender.Y += (int)lineSize.Y;
            }
        }
    }
}
