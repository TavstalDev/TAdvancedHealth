using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Reflection;
using Tavstal.TAdvancedHealth.Compatibility;
using Logger = Tavstal.TAdvancedHealth.Helpers.LoggerHelper;
using Tavstal.TAdvancedHealth.Helpers;

namespace Tavstal.TAdvancedHealth
{
    public class CommandVersion : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => ("v" + Assembly.GetExecutingAssembly().GetName().Name);
        public string Help => "Gets the version of the plugin";
        public string Syntax => "";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "TAdvancedHealth.version" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            Logger.Log("#########################################");
            Logger.Log(string.Format("# Build Version: {0}", TAdvancedHealthMain.Instance._Version));
            Logger.Log(string.Format("# Build Date: {0}", TAdvancedHealthMain.Instance._BuildDate));
            Logger.Log("#########################################");
        }
    }
}