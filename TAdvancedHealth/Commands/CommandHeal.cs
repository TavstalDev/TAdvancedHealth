using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TLibrary.Helpers.Unturned;

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
            var chsettings = TAdvancedHealth.Instance.CHSCSettings;
            var main = TAdvancedHealth.Instance;
            if (args.Length == 0)
            {
                AdvancedHealthComponent comp = callerPlayer.GetComponent<AdvancedHealthComponent>();
                comp.ReviveAsync();

                TAdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_cure_success", callerPlayer.CharacterName);
            }
            else if (args.Length == 1)
            {
                UnturnedPlayer targetPlayer = UnturnedPlayer.FromName(args[0]);
                if (targetPlayer == null)
                {
                    TAdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "error_playet_not_found");
                    return;
                }

                AdvancedHealthComponent targetComp = targetPlayer.GetComponent<AdvancedHealthComponent>();
                targetComp.ReviveAsync();

                TAdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_cure_success", targetPlayer.CharacterName);
            }
            else
                TAdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), $"<color=yellow>{Syntax}</color>");
        }
    }
}