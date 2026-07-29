using System;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TAdvancedHealth.Utils.Helpers;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.Unturned;

namespace Tavstal.TAdvancedHealth.Handlers.Player
{
    public static class PlayerConnectionHandler
    {
        private static AdvancedHealthConfig _config => AdvancedHealth.Instance.Config;
        
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
            string methodName = "OnPlayerJoin";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                var health = comp.HealthData;
                if (health == null)
                    return;

                comp.effectId = health.HUDEffectID;

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
                    comp.TryAddState(EPlayerState.FullMoon);

                #endregion

                #region Attach Events

                player.Player.equipment.onEquipRequested += PlayerInventoryHandler.OnPlayerEquipRequested;
                player.Player.equipment.onDequipRequested += PlayerInventoryHandler.OnPlayerDequipRequested;
                player.Player.life.onHurt += PlayerLifeHandler.OnPlayerLifeDamaged;
                player.Player.life.onOxygenUpdated += b => PlayerStatHandler.OnPlayerOxygenUpdate(player, b);
                player.Player.life.onTemperatureUpdated +=
                    newTemperature => PlayerStatHandler.OnPlayerTemperatureUpdate(player, newTemperature);
                player.Player.movement.onSafetyUpdated += isSafe => PlayerStatHandler.OnPlayerSafezoneUpdated(player, isSafe);
                player.Player.movement.onRadiationUpdated += isRadio => PlayerStatHandler.OnPlayerDeadzoneUpdated(player, isRadio);
                player.Player.life.onVirusUpdated += virus => PlayerStatHandler.OnPlayerVirusUpdate(player, virus);

                #endregion

                #region HideHealth HUD

                if (health.IsHUDEnabled)
                {
                    UEffectHelper.SendUIEffect(comp.effectId, (short)comp.effectId, comp.TranspConnection, true);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowFood, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowHealth, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowOxygen, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStamina, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVirus, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowWater, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons, false);
                    EffectHelper.UpdateWholeHealthUI(player);
                }

                #endregion
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {methodName}.", ex);
            }
        }
        
        private static void OnPlayerLeave(UnturnedPlayer player)
        {
            string methodName = "OnPlayerLeave";
            try
            {
                player.Player.equipment.onEquipRequested -= PlayerInventoryHandler.OnPlayerEquipRequested;
                player.Player.equipment.onDequipRequested -= PlayerInventoryHandler.OnPlayerDequipRequested;
                player.Player.life.onHurt -= PlayerLifeHandler.OnPlayerLifeDamaged;
                player.Player.life.onOxygenUpdated -= b => PlayerStatHandler.OnPlayerOxygenUpdate(player, b);
                player.Player.life.onTemperatureUpdated -= newTemperature => PlayerStatHandler.OnPlayerTemperatureUpdate(player, newTemperature);
                player.Player.movement.onSafetyUpdated -= isSafe => PlayerStatHandler.OnPlayerSafezoneUpdated(player, isSafe);
                player.Player.movement.onRadiationUpdated -= isRadio => PlayerStatHandler.OnPlayerDeadzoneUpdated(player, isRadio);
                player.Player.life.onVirusUpdated -= virus => PlayerStatHandler.OnPlayerVirusUpdate(player, virus);

                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                if (comp.dragState != EDragState.None)
                    comp.UnDrag();
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {methodName}.", ex);
            }
        }
    }
}