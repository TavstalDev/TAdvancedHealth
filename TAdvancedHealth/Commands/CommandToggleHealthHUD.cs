using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Utils.Helpers;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TLibrary.Models.Commands;
using Tavstal.TLibrary.Models.Plugin;
// ReSharper disable UnusedType.Global

namespace Tavstal.TAdvancedHealth.Commands
{
    public class CommandToggleHealthHUD : CustomCommandBase
    {
        public override IPlugin Plugin => AdvancedHealth.Instance;
        public override bool UseBackgroundThread => false;
        public override AllowedCaller AllowedCaller => AllowedCaller.Player;
        public override string Name => "togglehealthhud";
        public override string Help => "Toggles the custom hud.";
        public override string Syntax => "/togglehealthhud";
        public override List<string> Aliases => new List<string> { "thealthhud", "togglehhud", "togglehh" };
        public override List<string> Permissions => new List<string> { "tadvancedhealth.commands.togglehud" };
        public override List<ISubcommand> SubCommands => new List<ISubcommand>();

        protected override bool HandleExecute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
            var health = comp.HealthData;
            if (health == null)
                return true;
            
            if (!health.IsHUDEnabled)
            {
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowFood, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowHealth, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowOxygen, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStamina, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVirus, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowWater, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons, false);
                health.SetHUDEnabled(true, health.HUDEffectID);
                UEffectHelper.SendUIEffect(health.HUDEffectID, (short)health.HUDEffectID, player.SteamPlayer().transportConnection, true);
                EffectHelper.UpdateWholeHealthUI(player);
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
                health.SetHUDEnabled(false, health.HUDEffectID);
                EffectManager.askEffectClearByID(health.HUDEffectID, player.SteamPlayer().transportConnection);

            }
            return true;
        }
    }
}
