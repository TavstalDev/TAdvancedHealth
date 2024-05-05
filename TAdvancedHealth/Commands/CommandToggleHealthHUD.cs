using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using SDG.Unturned;
using Tavstal.TAdvancedHealth.Compatibility;
using Tavstal.TAdvancedHealth.Helpers;

namespace Tavstal.TAdvancedHealth.Commands
{
    public class CommandToggleHealthHUD : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "togglehealthhud";
        public string Help => "Toggles the custom hud.";
        public string Syntax => "/togglehealthhud";
        public List<string> Aliases => new List<string> { "thealthhud", "togglehhud", "togglehh" };
        public List<string> Permissions => new List<string> { "TAdvancedHealth.command.togglehud" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            HealthData health = TAdvancedHealth.Database.GetPlayerHealth(player.Id);

            if (!health.IsHUDEnabled)
            {
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowFood, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowHealth, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowOxygen, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStamina, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVirus, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowWater, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons, false);
                health.IsHUDEnabled = true;
                EffectManager.sendUIEffect(health.HUDEffectID, (short)health.HUDEffectID, player.SteamPlayer().transportConnection, true);
                HealthHelper.UpdateHealthAllUI(player);
            }
            else
            {
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowFood, true);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowHealth, true);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowOxygen, true);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStamina, true);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVirus, true);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowWater, true);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons, true);
                health.IsHUDEnabled = false;
                EffectManager.askEffectClearByID(health.HUDEffectID, player.SteamPlayer().transportConnection);

            }

            TAdvancedHealth.Database.UpdateHUDEnabledAsync(player.Id, health.IsHUDEnabled);
        }
    }
}
