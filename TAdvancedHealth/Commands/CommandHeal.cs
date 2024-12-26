using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TLibrary.Models.Commands;
using Tavstal.TLibrary.Models.Plugin;

namespace Tavstal.TAdvancedHealth.Commands
{
    public class CommandCure : CommandBase
    {
        protected override IPlugin Plugin => AdvancedHealth.Instance;
        public override AllowedCaller AllowedCaller => AllowedCaller.Player;
        public override string Name => "cure";
        public override string Help => "Heals yourself or somebody else.";
        public override string Syntax => "/cure <player>";
        public override List<string> Aliases => new List<string> { "heal" };
        public override List<string> Permissions => new List<string> { "tadvancedhealth.commands.heal", "tadvancedhealth.commands.cure" };
        protected override List<SubCommand> SubCommands => new List<SubCommand>();
        
        protected override async Task<bool> ExecutionRequested(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
            if (args.Length == 0)
            {
                AdvancedHealthComponent comp = callerPlayer.GetComponent<AdvancedHealthComponent>();
                await comp.ReviveAsync();

                AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_cure_success", callerPlayer.CharacterName);
            }
            else if (args.Length == 1)
            {
                UnturnedPlayer targetPlayer = UnturnedPlayer.FromName(args[0]);
                if (targetPlayer == null)
                {
                    AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "error_playet_not_found");
                    return true;
                }

                AdvancedHealthComponent targetComp = targetPlayer.GetComponent<AdvancedHealthComponent>();
                await targetComp.ReviveAsync();

                AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_cure_success", targetPlayer.CharacterName);
            }
            else
                AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), $"<color=yellow>{Syntax}</color>");
            return true;
        }
    }
}