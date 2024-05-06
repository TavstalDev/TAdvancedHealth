using Rocket.API;
using System.Collections.Generic;
using System.Reflection;

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
            TAdvancedHealth.Logger.Log("#########################################");
            TAdvancedHealth.Logger.Log(string.Format("# Build Version: {0}", TAdvancedHealth.Version));
            TAdvancedHealth.Logger.Log(string.Format("# Build Date: {0}", TAdvancedHealth.BuildDate));
            TAdvancedHealth.Logger.Log("#########################################");
        }
    }
}