using System;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Database;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TAdvancedHealth.Utils.Managers;
using Tavstal.TLibrary.Extensions;

namespace Tavstal.TAdvancedHealth.Handlers
{
    public static class UIEventHandler
    {
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
            string methodName = "OnButtonClicked";
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);
                AdvancedHealthComponent comp = uPlayer.GetComponent<AdvancedHealthComponent>();
                if (buttonName == "bt_suicide" || buttonName == "bt_suicide2")
                {
                    HealthData? health = HealthManager.Get(uPlayer.CSteamID.m_SteamID);
                    if (health == null)
                        return;
                    
                    if (health.IsInjured)
                    {
                        comp.allowDamage = true;
                        uPlayer.Player.life.askDamage(100, uPlayer.Position.normalized, EDeathCause.BLEEDING, ELimb.SKULL, CSteamID.Nil, out _);

                        if (uPlayer.Player.movement.pluginSpeedMultiplier == 0)
                            uPlayer.Player.movement.sendPluginSpeedMultiplier(1);
                        health.IsInjured = false;
                        if (comp.dragState != EDragState.None)
                            comp.UnDrag();

                        uPlayer.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                        EffectManager.sendUIEffectVisibility((short)comp.effectId, comp.TranspConnection, true, "RevivePanel", false);
                    }
                }
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Error in {methodName}.", ex);
            }
        }
    }
}