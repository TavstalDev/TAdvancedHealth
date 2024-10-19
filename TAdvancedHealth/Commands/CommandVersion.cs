using Rocket.API;
using System.Collections.Generic;
using System.Reflection;
using Tavstal.TLibrary.Helpers.Unturned;

namespace Tavstal.TAdvancedHealth.Commands
{
    public class CommandVersion : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => ("v" + Assembly.GetExecutingAssembly().GetName().Name);
        public string Help => "Gets the version of the plugin";
        public string Syntax => "";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "tadvancedhealth.commands.version" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            AdvancedHealth.Instance.SendPlainCommandReply(caller, "#########################################");
            AdvancedHealth.Instance.SendPlainCommandReply(caller, $"# Build Version: {AdvancedHealth.Version}");
            AdvancedHealth.Instance.SendPlainCommandReply(caller, $"# Build Date: {AdvancedHealth.BuildDate}");
            AdvancedHealth.Instance.SendPlainCommandReply(caller, "#########################################");
        }
    }
}