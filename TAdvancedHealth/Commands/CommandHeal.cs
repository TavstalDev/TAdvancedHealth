using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TLibrary.Models.Commands;
using Tavstal.TLibrary.Models.Plugin;
// ReSharper disable UnusedType.Global

namespace Tavstal.TAdvancedHealth.Commands
{
    public class CommandCure : CustomCommandBase
    {
        public override IPlugin Plugin => AdvancedHealth.Instance;
        public override bool UseBackgroundThread => false;
        public override AllowedCaller AllowedCaller => AllowedCaller.Player;
        public override string Name => "cure";
        public override string Help => "Heals yourself or somebody else.";
        public override string Syntax => "/cure <player>";
        public override List<string> Aliases => new List<string> { "heal" };
        public override List<string> Permissions => new List<string> { "tadvancedhealth.commands.heal", "tadvancedhealth.commands.cure" };
        public override List<ISubcommand> SubCommands => new List<ISubcommand>();

        protected override bool HandleExecute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
            if (args.Length == 0)
            {
                AdvancedHealthComponent comp = callerPlayer.GetComponent<AdvancedHealthComponent>();
                comp.Revive();
                AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_cure_success", AdvancedHealth.Instance.Config.General.MessageIcon, callerPlayer.CharacterName);
                return true;
            }
            
            if (args.Length == 1)
            {
                UnturnedPlayer targetPlayer = UnturnedPlayer.FromName(args[0]);
                if (targetPlayer == null)
                {
                    AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "error_playet_not_found", AdvancedHealth.Instance.Config.General.MessageIcon);
                    return true;
                }

                AdvancedHealthComponent targetComp = targetPlayer.GetComponent<AdvancedHealthComponent>();
                targetComp.Revive();

                AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_cure_success", AdvancedHealth.Instance.Config.General.MessageIcon, targetPlayer.CharacterName);
                return true;
            }
            
            return false;
        }
    }
}