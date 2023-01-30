using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Tavstal.TAdvancedHealth.Compatibility;
using Tavstal.TAdvancedHealth.Helpers;
using Tavstal.TAdvancedHealth.Modules;

namespace Tavstal.TAdvancedHealth
{
    public class CommandCure : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "cure";
        public string Help => "Heals yourself or somebody else.";
        public string Syntax => "/cure <player>";
        public List<string> Aliases => new List<string> { "heal" };
        public List<string> Permissions => new List<string> { "TAdvancedHealth.command.heal" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
            var chsettings = TAdvancedHealthMain.Instance.CHSCSettings;
            var main = TAdvancedHealthMain.Instance;
            if (args.Length == 0)
            {
                TAdvancedHealthComponent comp = callerPlayer.GetComponent<TAdvancedHealthComponent>();
                comp.Revive();

                UnturnedHelper.SendChatMessage(callerPlayer.SteamPlayer(), main.Translate(true, "command_cure_success", callerPlayer.CharacterName));
            }
            else if (args.Length == 1)
            {
                UnturnedPlayer targetPlayer = UnturnedPlayer.FromName(args[0]);
                if (targetPlayer == null)
                {
                    UnturnedHelper.SendChatMessage(callerPlayer.SteamPlayer(), "error_playet_not_found");
                    return;
                }

                TAdvancedHealthComponent targetComp = targetPlayer.GetComponent<TAdvancedHealthComponent>();
                targetComp.Revive();

                UnturnedHelper.SendChatMessage(callerPlayer.SteamPlayer(), main.Translate(true, "command_cure_success", targetPlayer.CharacterName));
            }
            else
                UnturnedHelper.SendChatMessage(callerPlayer.SteamPlayer(), "<color=yellow>" + Syntax + "</color>");
        }
    }
}