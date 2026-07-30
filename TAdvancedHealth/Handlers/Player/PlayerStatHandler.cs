using System;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TAdvancedHealth.Utils.Helpers;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Helpers.Unturned;

namespace Tavstal.TAdvancedHealth.Handlers.Player
{
    public static class PlayerStatHandler
    {
        private static AdvancedHealthConfig _config => AdvancedHealth.Instance.Config;
        
        internal static void Attach()
        {
            UnturnedPlayerEvents.OnPlayerUpdateHealth += OnPlayerHealthUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateFood += OnPlayerFoodUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateWater += OnPlayerWaterUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateStamina += OnPlayerStaminaUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateBleeding += OnPlayerBleedingUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateBroken += OnPlayerBrokenUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateVirus += OnPlayerVirusUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateStance += OnPlayerStanceUpdate;
        }

        internal static void Detach()
        {
            UnturnedPlayerEvents.OnPlayerUpdateHealth -= OnPlayerHealthUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateFood -= OnPlayerFoodUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateWater -= OnPlayerWaterUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateStamina -= OnPlayerStaminaUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateBleeding -= OnPlayerBleedingUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateBroken -= OnPlayerBrokenUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateVirus -= OnPlayerVirusUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateStance -= OnPlayerStanceUpdate;
        }
        
        internal static void OnPlayerHealthUpdate(UnturnedPlayer player, byte value)
        {
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                var health = comp.HealthData;
                if (health == null)
                    return;
                
                EffectHelper.SendUIEffectProgressBar((short)_config.EffectId, player.CSteamID, true, EProgressBar.SimpleHealth, (int)(Math.Round(health.BaseHealth, 2) / _config.HealthSystemSettings.BaseHealth * 100), 0);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerHealthUpdate)}.", ex);
            }
        }
        
        internal static void OnPlayerFoodUpdate(UnturnedPlayer player, byte value)
        {
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                var health = comp.HealthData;
                if (health == null)
                    return;
                
                EffectHelper.SendUIEffectProgressBar((short)_config.EffectId, player.CSteamID, true, EProgressBar.Food, player.Player.life.food, (int)comp.ProgressBarData.LastFood);
                comp.ProgressBarData.LastFood = value;

                if (value <= 0)
                    comp.TryAddState(EPlayerState.NO_FOOD);
                else
                    comp.TryRemoveState(EPlayerState.NO_FOOD);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerFoodUpdate)}.", ex);
            }
        }
        
        internal static void OnPlayerStaminaUpdate(UnturnedPlayer player, byte value)
        {
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                EffectHelper.SendUIEffectProgressBar((short)_config.EffectId, player.CSteamID, true, EProgressBar.Stamina,
                    player.Player.life.stamina, (int)comp.ProgressBarData.LastStamina);
                comp.ProgressBarData.LastStamina = value;
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerStaminaUpdate)}.", ex);
            }
        }
        
        internal static void OnPlayerWaterUpdate(UnturnedPlayer player, byte value)
        {
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                EffectHelper.SendUIEffectProgressBar((short)_config.EffectId, player.CSteamID, true, EProgressBar.Water, player.Player.life.water, (int)comp.ProgressBarData.LastWater);
                comp.ProgressBarData.LastWater = value;

                if (value <= 0)
                    comp.TryAddState(EPlayerState.NO_WATER);
                else
                    comp.TryRemoveState(EPlayerState.NO_WATER);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerWaterUpdate)}.", ex);
            }
        }

        internal static void OnPlayerVirusUpdate(UnturnedPlayer player, byte value)
        {
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                EffectHelper.SendUIEffectProgressBar((short)_config.EffectId, player.CSteamID, true, EProgressBar.Radiation, player.Player.life.virus, (int)comp.ProgressBarData.LastVirus);
                comp.ProgressBarData.LastVirus = value;

                if (value <= 0)
                    comp.TryAddState(EPlayerState.NO_VIRUS);
                else
                    comp.TryRemoveState(EPlayerState.NO_VIRUS);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerVirusUpdate)}.", ex);
            }
        }


        internal static void OnPlayerOxygenUpdate(UnturnedPlayer player, byte value)
        {
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                EffectHelper.SendUIEffectProgressBar((short)_config.EffectId, player.CSteamID, true, EProgressBar.Oxygen, player.Player.life.oxygen, (int)comp.ProgressBarData.LastOxygen);
                comp.ProgressBarData.LastOxygen = value;

                if (value <= 0)
                    comp.TryAddState(EPlayerState.NO_OXYGEN);
                else
                    comp.TryRemoveState(EPlayerState.NO_OXYGEN);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerOxygenUpdate)}.", ex);
            }
        }


        internal static void OnPlayerBleedingUpdate(UnturnedPlayer player, bool state)
        {
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                if (!state)
                {
                    comp.hasHeavyBleeding = false;
                    comp.TryRemoveState(EPlayerState.BLEEDING);
                    return;
                }

                if (!_config.HealthSystemSettings.Combat.CanStartBleeding)
                {
                    player.Bleeding = false;
                    return;
                }

                if (MathHelper.Next(1, 100) <= _config.HealthSystemSettings.Combat.HeavyBleedingChance)
                    comp.hasHeavyBleeding = true;

                comp.TryAddState(EPlayerState.BLEEDING);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerBleedingUpdate)}.", ex);
            }
        }
        
        internal static void OnPlayerBrokenUpdate(UnturnedPlayer player, bool state)
        {
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                if (!state)
                {
                    comp.TryRemoveState(EPlayerState.BROKEN_BONES);
                    return;
                }
                
                if (_config.HealthSystemSettings.Combat.CanHavePainEffect)
                {
                    int painChance = MathHelper.Next(1, 100);

                    if (painChance <= _config.HealthSystemSettings.Combat.PainEffectChance)
                    {
                       UEffectHelper.SendUIEffect(_config.HealthSystemSettings.PainEffectID,
                            (short)_config.HealthSystemSettings.PainEffectID, comp.TranspConnection, true);
                        if (_config.HealthSystemSettings.Combat.PainEffectDuration > 0)
                            AdvancedHealth.Instance.InvokeAction(_config.HealthSystemSettings.Combat.PainEffectDuration,
                                () =>
                                {
                                    EffectManager.askEffectClearByID(_config.HealthSystemSettings.PainEffectID,
                                        player.SteamPlayer().transportConnection);
                                });

                    }
                }

                var health = comp.HealthData;
                if (health != null)
                {
                    if (health.LeftLegHealth == 0 && health.RightLegHealth == 0)
                    {
                        if (!_config.HealthSystemSettings.Movement.CanWalkWithBrokenLegs)
                            player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                    }
                    else if (health.LeftLegHealth == 0 || health.RightLegHealth == 0)
                    {
                        if (!_config.HealthSystemSettings.Movement.CanWalkWithOneBrokenLeg)
                            player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                    }
                }

                comp.TryAddState(EPlayerState.BROKEN_BONES);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerBrokenUpdate)}.", ex);
            }
        }
        
        internal static void OnMoonUpdated(bool isFullMoon)
        {
            try
            {
                foreach (SteamPlayer steamPlayer in Provider.clients)
                {
                    UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                    AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                    if (isFullMoon)
                        comp.TryAddState(EPlayerState.FULL_MOON);
                    else
                        comp.TryRemoveState(EPlayerState.FULL_MOON);
                }
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnMoonUpdated)}.", ex);
            }
        }

        internal static void OnPlayerDeadzoneUpdated(UnturnedPlayer player, bool isActive)
        {
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                if (isActive)
                    comp.TryAddState(EPlayerState.DEATH_ZONE);
                else
                    comp.TryRemoveState(EPlayerState.DEATH_ZONE);
            }
            catch (Exception ex)
            {
               AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerDeadzoneUpdated)}.", ex);
            }
        }
        
        internal static void OnPlayerSafezoneUpdated(UnturnedPlayer player, bool isActive)
        {
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                if (isActive)
                    comp.TryAddState(EPlayerState.SAFE_ZONE);
                else
                    comp.TryRemoveState(EPlayerState.SAFE_ZONE);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerSafezoneUpdated)}.", ex);
            }
        }
        
        internal static void OnPlayerTemperatureUpdate(UnturnedPlayer player, EPlayerTemperature newTemperature)
        {
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                comp.TryRemoveState(EffectHelper.GetPlayerState(comp.currentTemperature), false);
                comp.TryAddState(EffectHelper.GetPlayerState(newTemperature));
                comp.currentTemperature = newTemperature;
                
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerTemperatureUpdate)}.", ex);
            }
        }
        
        internal static void OnPlayerStanceUpdate(UnturnedPlayer player, byte stance)
        {
            try
            {
                var comp = player.GetComponent<AdvancedHealthComponent>();
                var health = comp.HealthData;
                if (health == null)
                    return;
                
                if (health.IsInjured)
                {
                    player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                    return;
                }

                if (health.LeftLegHealth == 0 || health.RightLegHealth == 0)
                {
                    if (!_config.HealthSystemSettings.Movement.CanWalkWithOneBrokenLeg)
                        player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                    else if (!_config.HealthSystemSettings.Movement.CanWalkWithBrokenLegs && health.LeftLegHealth == 0 &&
                             health.RightLegHealth == 0)
                        player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                }
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerStanceUpdate)}.", ex);
            }
        }
    }
}