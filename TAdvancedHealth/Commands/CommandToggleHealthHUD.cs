using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tavstal.TAdvancedHealth.Models.Database;
using Tavstal.TAdvancedHealth.Utils.Helpers;
using Tavstal.TLibrary;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TLibrary.Models.Commands;
using Tavstal.TLibrary.Models.Plugin;

namespace Tavstal.TAdvancedHealth.Commands
{
    public class CommandToggleHealthHUD : CommandBase
    {
        protected override IPlugin Plugin => AdvancedHealth.Instance;
        public override AllowedCaller AllowedCaller => AllowedCaller.Player;
        public override string Name => "togglehealthhud";
        public override string Help => "Toggles the custom hud.";
        public override string Syntax => "/togglehealthhud";
        public override List<string> Aliases => new List<string> { "thealthhud", "togglehhud", "togglehh" };
        public override List<string> Permissions => new List<string> { "tadvancedhealth.commands.togglehud" };
        protected override List<SubCommand> SubCommands => new List<SubCommand>();

        protected override async Task<bool> ExecutionRequested(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            HealthData health = await AdvancedHealth.DatabaseManager.GetPlayerHealthAsync(player.Id);

            if (!health.IsHUDEnabled)
            {
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowFood, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowHealth, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowOxygen, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStamina, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVirus, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowWater, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons, false);
                });
                health.IsHUDEnabled = true;
                UEffectHelper.SendUIEffect(health.HUDEffectID, (short)health.HUDEffectID, player.SteamPlayer().transportConnection, true);
                await EffectHelper.UpdateWholeHealthUIAsync(player);
            }
            else
            {
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowFood, true);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowHealth, true);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowOxygen, true);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStamina, true);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVirus, true);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowWater, true);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons, true);
                });
                health.IsHUDEnabled = false;
                UEffectHelper.AskEffectClearByID(health.HUDEffectID, player.SteamPlayer().transportConnection);

            }

            await AdvancedHealth.DatabaseManager.UpdateHUDEnabledAsync(player.Id, health.IsHUDEnabled);
            return true;
        }
    }
}
