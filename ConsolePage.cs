using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.AndroidExtensions;
using StardewValley;
using StardewValley.Menus;

namespace ConsoleMobile;

public class ConsolePage : IClickableMenu
{
    public class LogFilter
    {
        public LogFilter() { }
        public bool info = true;
        public bool trace;
        public bool debug = true;
        public bool error = true;
        public bool warn = true;
        public bool alert = true;
        public bool IsEnable(string levelName)
        {
            return levelName.ToLower() switch
            {
                "info" => info,
                "trace" => trace,
                "debug" => debug,
                "error" => error,
                "warn" => warn,
                "alert" => alert,
                _ => true
            };
        }
    }

    MobileScrollbar scrollbar;
    public float scrollPercent;
    Vector2 paddingLogRender = Vector2.Zero;
    Vector2 ExampleLineSize = Vector2.Zero;
    SpriteFont font;
    const int scrollbarPaddingHeight = 150;
    LogFilter logFilter = new();
    public ConsolePage()
    {
        initializeUpperRightCloseButton();
        upperRightCloseButton.bounds.X = 50;

        font = Game1.dialogueFont;
        ExampleLineSize = font.MeasureString("example line");
        //var scrollbarX = viewport.Width - 100;// Right Side
        var scrollbarX = 10; //Left Side
        paddingLogRender.Y = ExampleLineSize.Y * 1;

        scrollbar = new MobileScrollbar(scrollbarX, scrollbarPaddingHeight / 2, 40, viewport.Height - scrollbarPaddingHeight);
        paddingLogRender.X = scrollbar.slider.bounds.X + scrollbar.slider.bounds.Width + 20;
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
        drawLogs(b);
        scrollbar.draw(b);
        base.draw(b); //draw close button
    }
    Color InfoColor = Color.White;
    Color DebugColor = new Color(220, 220, 220);
    Color ErrorColor = new Color(255, 60, 60);
    Color WarnColor = new Color(255, 120, 0);
    Color AlertColor = new Color(200, 0, 255);
    void drawLogs(SpriteBatch batch)
    {

        var currentDrawTextPos = paddingLogRender;

        var LogLines = ConsoleHook.Lines;
        LogLines.Add("[>> End Line <<]");

        //adjust with scrollbar
        //filter logs
        int totalRenderLineHeight = 0;
        List<Tuple<string, string>> logs = new();
        foreach (var line in LogLines)
        {
            var splitLines = line.Split(" ");
            var textLogLevel = "";
            if (splitLines.Length >= 2)
            {
                //split result, 1="[04:02:08", 2="TRACT", 3=mod name
                textLogLevel = splitLines[1];
                bool isRender = logFilter.IsEnable(textLogLevel);
                if (!isRender)
                    continue;
            }
            var lineSize = font.MeasureString(line);
            totalRenderLineHeight += (int)lineSize.Y;
            logs.Add(new(textLogLevel, line));
        }

        currentDrawTextPos.Y -= (int)((scrollPercent / 100f) * (totalRenderLineHeight));
        var lastLineSize = Vector2.Zero;
        foreach (var log in logs)
        {
            var line = log.Item2;
            var textLogLevel = log.Item1;

            //check text is must render in viewport
            var lineSize = font.MeasureString(line);
            currentDrawTextPos.Y += (int)lastLineSize.Y;
            lastLineSize = lineSize;

            if (currentDrawTextPos.Y >= paddingLogRender.Y && currentDrawTextPos.Y + ExampleLineSize.Y <= Game1.viewport.Height)
            {
                var textColor = textLogLevel switch
                {
                    "DEBUG" => DebugColor,
                    "ERROR" => ErrorColor,
                    "WARN" => WarnColor,
                    "ALERT" => AlertColor,
                    "TRACE" => DebugColor,
                    _ => InfoColor,
                };

                batch.DrawString(font, line, currentDrawTextPos, textColor);
            }
        }
    }
}
