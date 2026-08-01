using System;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TAdvancedHealth.Utils.Managers;
using Tavstal.TLibrary.Extensions;

namespace Tavstal.TAdvancedHealth.Handlers
{
    public static class UIEventHandler
    {
        private static AdvancedHealthConfig _config => AdvancedHealth.Instance.Config;
        
        internal static void Attach()
        {
            EffectManager.onEffectButtonClicked += OnButtonClickded;
        }

        internal static void Detach()
        {
            EffectManager.onEffectButtonClicked -= OnButtonClickded;
        }
        
        private static void OnButtonClickded(SDG.Unturned.Player player, string buttonName)
        {
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);
                AdvancedHealthComponent comp = ComponentManager.Get(uPlayer);
                if (buttonName == "bt_revive_suicide")
                {
                    var health = comp.HealthData;
                    if (health == null)
                        return;
                    
                    if (health.IsInjured)
                    {
                        comp.allowDamage = true;
                        uPlayer.Player.life.askDamage(100, uPlayer.Position.normalized, EDeathCause.BLEEDING, ELimb.SKULL, CSteamID.Nil, out _);

                        if (uPlayer.Player.movement.pluginSpeedMultiplier == 0)
                            uPlayer.Player.movement.sendPluginSpeedMultiplier(1);
                        health.SetInjured(false);
                        if (comp.dragState != EDragState.None)
                            comp.UnDrag();

                        uPlayer.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                        EffectManager.sendUIEffectVisibility((short)_config.EffectId, comp.TranspConnection, true, "RevivePanel", false);
                    }
                }
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnButtonClickded)}.", ex);
            }
        }
    }
}