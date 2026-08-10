using System;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.RocketFlow.Attributes;
using Tavstal.RocketFlow.Core;
using Tavstal.RocketFlow.Events.Player;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TAdvancedHealth.Utils.Helpers;
using Tavstal.TAdvancedHealth.Utils.Managers;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.Unturned;
// ReSharper disable UnusedMember.Local

namespace Tavstal.TAdvancedHealth.Handlers.Player
{
    public class PlayerConnectionListener : EventListener
    {
        private static AdvancedHealthConfig _config => AdvancedHealth.Instance.Config;
        
        internal static void OnPlayerJoin(UnturnedPlayer player)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(player);
                if (comp == null)
                    return;
                
                var health = comp.HealthData;
                if (health == null)
                {
                    AdvancedHealth.Logger.Error($"Failed to retrieve health data for player {player.DisplayName} ({player.Id}).");
                    return;
                }

                #region Hide default HUD and show the custom one

                UEffectHelper.SendUIEffect(_config.EffectId, (short)_config.EffectId, comp.TranspConnection, true);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowFood, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowHealth, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowOxygen, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStamina, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVirus, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowWater, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons, false);
                EffectHelper.UpdateWholeHealthUI(player);
                
                #region Update States
                
                /* TODO
                PlayerStatHandler.OnPlayerBleedingUpdate(player, player.Bleeding);
                PlayerStatHandler.OnPlayerBrokenUpdate(player, player.Broken);
                PlayerStatHandler.OnSafezoneUpdated(player, player.Player.movement.isSafe);
                PlayerStatHandler.OnPlayerDeadzoneUpdated(player, player.Player.movement.isRadiated);
                PlayerStatHandler.OnPlayerTemperatureUpdate(player, player.Player.life.temperature);*/
                
                if (LightingManager.isFullMoon)
                    comp.TryAddState(EPlayerState.FULL_MOON);

                #endregion
                #endregion
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerJoin)}.", ex);
            }
        }

        [EventHandler]
        private void OnPlayerDisconnect(PlayerDisconnectEvent e)
        {
            AdvancedHealthComponent? comp = ComponentManager.Get(e.Player);
            if (comp != null && comp.dragState != EDragState.None)
                comp.UnDrag();
                
            ComponentManager.Invalidate(e.Player.Id);
        }
        
        private static void OnPlayerLeave(UnturnedPlayer player)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(player);
                if (comp != null && comp.dragState != EDragState.None)
                    comp.UnDrag();
                
                ComponentManager.Invalidate(player.Id);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerLeave)}.", ex);
            }
        }
    }
}