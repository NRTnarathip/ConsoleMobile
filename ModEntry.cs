using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ConsoleMobile
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; }
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Monitor.Log("on entry Console Mobile");
            Helper.ConsoleCommands.Add("openconsole", "Open console", OnCMDOpenConsole);
            Helper.Events.Display.Rendered += OnRendered;
        }

        private void OnRendered(object? sender, RenderedEventArgs e)
        {
            var page = Game1.activeClickableMenu as ConsolePage;
            if (page == null) return;

        }

        private void OnCMDOpenConsole(string arg1, string[] arg2)
        {
            Monitor.Log("On cmd opensoncole.");

            ToggleConsole(true);

        }
        bool isOpen => Game1.activeClickableMenu?.GetType() == typeof(ConsolePage);
        public void ToggleConsole(bool toggle)
        {
            if (toggle)
            {
                if (isOpen) return;

                Game1.activeClickableMenu = new ConsolePage();
                return;
            }

            if (isOpen == false) return;

            Game1.activeClickableMenu = null;
        }
    }
}
