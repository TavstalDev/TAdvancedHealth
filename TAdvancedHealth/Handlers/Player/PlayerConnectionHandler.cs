using System;
using System.Linq;
using System.Threading.Tasks;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TAdvancedHealth.Models.Database;
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
                Task.Run(async () =>
                {
                    #region Check Health Data
                    HealthData health = await _database.GetPlayerHealthAsync(player.Id);
                    if (health == null)
                    {
                        HUDStyle? style = _config.HUDStyles.FirstOrDefault(x => x.Enabled);
                        if (style == null)
                            style = _config.HUDStyles[0];

                        await _database.AddHealthDataAsync(player.Id, new HealthData
                        {
                            PlayerId = player.Id,
                            HUDEffectID = style.EffectID,
                            BaseHealth = _config.HealthSystemSettings.BaseHealth,
                            BodyHealth = _config.HealthSystemSettings.BodyHealth,
                            HeadHealth = _config.HealthSystemSettings.HeadHealth,
                            LeftArmHealth = _config.HealthSystemSettings.LeftArmHealth,
                            LeftLegHealth = _config.HealthSystemSettings.LeftLegHealth,
                            RightArmHealth = _config.HealthSystemSettings.RightArmHealth,
                            RightLegHealth = _config.HealthSystemSettings.RightLegHealth,
                            IsInjured = false,
                            IsHUDEnabled = true,
                            DeathDate = DateTime.Now
                        });
                        health = await _database.GetPlayerHealthAsync(player.Id);
                    }
                    else
                    {
                        if (health.BaseHealth > _config.HealthSystemSettings.BaseHealth)
                            health.BaseHealth = _config.HealthSystemSettings.BaseHealth;
                        if (_config.HealthSystemSettings.EnableTarkovLikeHealth)
                        {
                            if (health.HeadHealth > _config.HealthSystemSettings.HeadHealth)
                                health.HeadHealth = _config.HealthSystemSettings.HeadHealth;
                            if (health.BodyHealth > _config.HealthSystemSettings.BodyHealth)
                                health.BodyHealth = _config.HealthSystemSettings.BodyHealth;
                            if (health.RightArmHealth > _config.HealthSystemSettings.RightArmHealth)
                                health.RightArmHealth = _config.HealthSystemSettings.RightArmHealth;
                            if (health.LeftArmHealth > _config.HealthSystemSettings.LeftArmHealth)
                                health.LeftArmHealth = _config.HealthSystemSettings.LeftArmHealth;
                            if (health.RightLegHealth > _config.HealthSystemSettings.RightLegHealth)
                                health.RightLegHealth = _config.HealthSystemSettings.RightLegHealth;
                            if (health.LeftLegHealth > _config.HealthSystemSettings.LeftLegHealth)
                                health.LeftLegHealth = _config.HealthSystemSettings.LeftLegHealth;
                            await _database.UpdateHealthAsync(player.Id, health);
                        }
                    }

                    #endregion

                    AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
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
                    OnPlayerFoodUpdate(player, player.Player.life.food);
                    OnPlayerWaterUpdate(player, player.Player.life.water);
                    OnPlayerVirusUpdate(player, player.Player.life.virus);
                    OnPlayerOxygenUpdate(player, player.Player.life.oxygen);
                    OnPlayerStaminaUpdate(player, player.Player.life.stamina);
                    OnPlayerBleedingUpdate(player, player.Bleeding);
                    OnPlayerBrokenUpdate(player, player.Broken);
                    OnPlayerSafezoneUpdated(player, player.Player.movement.isSafe);
                    OnPlayerDeadzoneUpdated(player, player.Player.movement.isRadiated);
                    OnPlayerTemperatureUpdate(player, player.Player.life.temperature);


                    if (LightingManager.isFullMoon)
                        await comp.TryAddStateAsync(EPlayerState.FullMoon);

                    #endregion

                    #region Attach Events
                    player.Player.equipment.onEquipRequested += OnPlayerEquipRequested;
                    player.Player.equipment.onDequipRequested += OnPlayerDequipRequested;
                    player.Player.life.onHurt += OnPlayerLifeDamaged;
                    player.Player.life.onOxygenUpdated += b => OnPlayerOxygenUpdate(player, b);
                    player.Player.life.onTemperatureUpdated +=
                        newTemperature => OnPlayerTemperatureUpdate(player, newTemperature);
                    player.Player.movement.onSafetyUpdated += isSafe => OnPlayerSafezoneUpdated(player, isSafe);
                    player.Player.movement.onRadiationUpdated += isRadio => OnPlayerDeadzoneUpdated(player, isRadio);
                    player.Player.life.onVirusUpdated += virus => OnPlayerVirusUpdate(player, virus);
                    #endregion

                    #region HideHealth HUD
                    if (health.IsHUDEnabled)
                    {
                        EffectManager.sendUIEffect(comp.effectId, (short)comp.effectId, comp.TranspConnection, true);
                        player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowFood, false);
                        player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowHealth, false);
                        player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowOxygen, false);
                        player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStamina, false);
                        player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVirus, false);
                        player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowWater, false);
                        player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons, false);
                        await EffectHelper.UpdateWholeHealthUIAsync(player);
                    }
                    #endregion
                });
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Error in {methodName}.", ex);
            }
        }
        
        private static void OnPlayerLeave(UnturnedPlayer player)
        {
            string methodName = "OnPlayerLeave";
            try
            {
                #region Dettach Events
                player.Player.equipment.onEquipRequested -= OnPlayerEquipRequested;
                player.Player.equipment.onDequipRequested -= OnPlayerDequipRequested;
                player.Player.life.onHurt -= OnPlayerLifeDamaged;
                player.Player.life.onOxygenUpdated -= b => OnPlayerOxygenUpdate(player, b);
                player.Player.life.onTemperatureUpdated -= newTemperature => OnPlayerTemperatureUpdate(player, newTemperature);
                player.Player.movement.onSafetyUpdated -= isSafe => OnPlayerSafezoneUpdated(player, isSafe);
                player.Player.movement.onRadiationUpdated -= isRadio => OnPlayerDeadzoneUpdated(player, isRadio);
                player.Player.life.onVirusUpdated -= virus => OnPlayerVirusUpdate(player, virus);
                #endregion

                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                if (comp.dragState != EDragState.None)
                    comp.UnDrag();
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Error in {methodName}.", ex);
            }
        }
    }
}