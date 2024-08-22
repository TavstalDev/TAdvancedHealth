using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TLibrary.Helpers.Unturned;

namespace Tavstal.TAdvancedHealth.Commands
{
    public class CommandCure : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "cure";
        public string Help => "Heals yourself or somebody else.";
        public string Syntax => "/cure <player>";
        public List<string> Aliases => new List<string> { "heal" };
        public List<string> Permissions => new List<string> { "tadvancedhealth.commands.heal", "tadvancedhealth.commands.cure" };

        public async void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
            if (args.Length == 0)
            {
                AdvancedHealthComponent comp = callerPlayer.GetComponent<AdvancedHealthComponent>();
                await comp.ReviveAsync();

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
                await targetComp.ReviveAsync();

                TAdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_cure_success", targetPlayer.CharacterName);
            }
            else
                TAdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), $"<color=yellow>{Syntax}</color>");
        }
    }
}