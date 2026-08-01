using System;
using System.Collections.Generic;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TAdvancedHealth.Utils.Helpers;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.Unturned;

namespace Tavstal.TAdvancedHealth.Handlers.Player
{
    public static class PlayerConnectionHandler
    {
        private static AdvancedHealthConfig _config => AdvancedHealth.Instance.Config;
        private static readonly Dictionary<string, PlayerStatSubscriptions> _playerStats = new  Dictionary<string, PlayerStatSubscriptions>();
        
        internal static void Attach()
        {
            U.Events.OnPlayerConnected += OnPlayerJoin;
            U.Events.OnPlayerDisconnected += OnPlayerLeave;
        }

        internal static void Detach()
        {
            U.Events.OnPlayerConnected -= OnPlayerJoin;
            U.Events.OnPlayerDisconnected -= OnPlayerLeave;
        }
        
        private static void OnPlayerJoin(UnturnedPlayer player)
        {
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                var health = comp.HealthData;
                if (health == null)
                    return;
                
                #region Set ProgressBarData

                comp.ProgressBarData.LastHealthHead = health.HeadHealth;
                comp.ProgressBarData.LastHealthBody = health.BodyHealth;
                comp.ProgressBarData.LastHealthLeftArm = health.LeftArmHealth;
                comp.ProgressBarData.LastHealthLeftLeg = health.LeftLegHealth;
                comp.ProgressBarData.LastHealthRightArm = health.RightArmHealth;
                comp.ProgressBarData.LastHealthRightLeg = health.RightLegHealth;
                comp.ProgressBarData.LastFood = player.Player.life.food;
                comp.ProgressBarData.LastWater = player.Player.life.water;
                comp.ProgressBarData.LastVirus = player.Player.life.virus;
                comp.ProgressBarData.LastOxygen = player.Player.life.oxygen;
                comp.ProgressBarData.LastStamina = player.Player.life.stamina;

                #endregion

                #region Update States

                PlayerStatHandler.OnPlayerFoodUpdate(player, player.Player.life.food);
                PlayerStatHandler.OnPlayerWaterUpdate(player, player.Player.life.water);
                PlayerStatHandler.OnPlayerVirusUpdate(player, player.Player.life.virus);
                PlayerStatHandler.OnPlayerOxygenUpdate(player, player.Player.life.oxygen);
                PlayerStatHandler.OnPlayerStaminaUpdate(player, player.Player.life.stamina);
                PlayerStatHandler.OnPlayerBleedingUpdate(player, player.Bleeding);
                PlayerStatHandler.OnPlayerBrokenUpdate(player, player.Broken);
                PlayerStatHandler.OnPlayerSafezoneUpdated(player, player.Player.movement.isSafe);
                PlayerStatHandler.OnPlayerDeadzoneUpdated(player, player.Player.movement.isRadiated);
                PlayerStatHandler.OnPlayerTemperatureUpdate(player, player.Player.life.temperature);


                if (LightingManager.isFullMoon)
                    comp.TryAddState(EPlayerState.FULL_MOON);

                #endregion

                #region Attach Events

                player.Player.equipment.onEquipRequested += PlayerInventoryHandler.OnPlayerEquipRequested;
                player.Player.equipment.onDequipRequested += PlayerInventoryHandler.OnPlayerDequipRequested;
                player.Player.life.onHurt += PlayerLifeHandler.OnPlayerLifeDamaged;

                var statSubscriptions = new PlayerStatSubscriptions(player);
                _playerStats.Add(player.Id, statSubscriptions);
                player.Player.life.onOxygenUpdated += statSubscriptions.OxygenCallback;
                player.Player.life.onTemperatureUpdated += statSubscriptions.TemperatureCallback;
                player.Player.movement.onSafetyUpdated += statSubscriptions.SafetyCallback;
                player.Player.movement.onRadiationUpdated += statSubscriptions.RadiationCallback;
                player.Player.life.onVirusUpdated += statSubscriptions.VirusCallback;

                #endregion

                #region HideHealth HUD

                UEffectHelper.SendUIEffect(_config.EffectId, (short)_config.EffectId, comp.TranspConnection, true);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowFood, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowHealth, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowOxygen, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStamina, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVirus, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowWater, false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons, false);
                EffectHelper.UpdateWholeHealthUI(player);
                #endregion
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerJoin)}.", ex);
            }
        }
        
        private static void OnPlayerLeave(UnturnedPlayer player)
        {
            try
            {
                player.Player.equipment.onEquipRequested -= PlayerInventoryHandler.OnPlayerEquipRequested;
                player.Player.equipment.onDequipRequested -= PlayerInventoryHandler.OnPlayerDequipRequested;
                player.Player.life.onHurt -= PlayerLifeHandler.OnPlayerLifeDamaged;
                
                if (_playerStats.TryGetValue(player.Id, out PlayerStatSubscriptions statSubscriptions))
                {
                    player.Player.life.onOxygenUpdated -= statSubscriptions.OxygenCallback;
                    player.Player.life.onTemperatureUpdated -= statSubscriptions.TemperatureCallback;
                    player.Player.movement.onSafetyUpdated -= statSubscriptions.SafetyCallback;
                    player.Player.movement.onRadiationUpdated -= statSubscriptions.RadiationCallback;
                    player.Player.life.onVirusUpdated -= statSubscriptions.VirusCallback;
                    _playerStats.Remove(player.Id);
                }

                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                if (comp.dragState != EDragState.None)
                    comp.UnDrag();
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerLeave)}.", ex);
            }
        }
    }
}