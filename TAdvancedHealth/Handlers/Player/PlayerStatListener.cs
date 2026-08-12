using System;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.RocketFlow.Attributes;
using Tavstal.RocketFlow.Core;
using Tavstal.RocketFlow.Events.Player;
using Tavstal.RocketFlow.Events.Player.Life;
using Tavstal.RocketFlow.Events.Player.Movement;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TAdvancedHealth.Utils.Helpers;
using Tavstal.TAdvancedHealth.Utils.Managers;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Helpers.Unturned;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace Tavstal.TAdvancedHealth.Handlers.Player
{
    public class PlayerStatListener : EventListener
    {
        private static AdvancedHealthConfig _config => AdvancedHealth.Instance.Config;

        [EventHandler]
        private void OnHealthUpdate(PlayerHealthEvent e) =>
            HealthUpdate(e.Player, e.Value);

        [EventHandler]
        private void OnFoodUpdate(PlayerFoodEvent e) =>
            FoodUpdate(e.Player, e.Value);

        [EventHandler]
        private void OnStaminaUpdate(PlayerStaminaEvent e) =>
            StaminaUpdate(e.Player, e.Value);

        [EventHandler]
        private void OnWaterUpdate(PlayerWaterEvent e) =>
            WaterUpdate(e.Player, e.Value);

        [EventHandler]
        private void OnVirusUpdate(PlayerVirusEvent e) =>
            VirusUpdate(e.Player, e.Value);

        [EventHandler]
        private void OnOxygenUpdate(PlayerOxygenEvent e) =>
            OxygenUpdate(e.Player, e.NewOxygen);

        [EventHandler]
        private void OnBleedingUpdate(PlayerBleedingEvent e) =>
            BleedingUpdate(e.Player, e.IsBleeding);

        [EventHandler]
        private void OnBonesUpdate(PlayerBonesEvent e) =>
            BonesUpdate(e.Player, e.IsBroken);

        [EventHandler]
        private void OnDeadzoneUpdated(PlayerDeadzoneUpdatedEvent e) =>
            DeadzoneUpdated(e.Player, e.IsInDeadzone);

        [EventHandler]
        private void OnSafezoneUpdated(PlayerSafezoneUpdatedEvent e) =>
            SafezoneUpdated(e.Player, e.IsSafe);

        [EventHandler]
        private void OnTemperatureUpdate(PlayerTemperatureUpdatedEvent e) =>
            TemperatureUpdate(e.Player, e.NewTemperature);

        [EventHandler]
        private void OnStanceUpdate(PlayerStanceEvent e) => 
            StanceUpdate(e.Player, e.Stance);
        
        internal static void HealthUpdate(UnturnedPlayer player, byte newHealth)
        {
            try
            {
                if (_config.HealthSystemSettings.EnableLimbHealthSystem)
                    return;
                
                AdvancedHealthComponent? comp = ComponentManager.Get(player);
                if (comp == null)
                    return;
                
                var health = comp.HealthData;
                if (health == null)
                    return;
                
                EffectHelper.SendUIEffectProgressBar(player, (short)_config.EffectId, true, EProgressBar.SimpleHealth, (int)(Math.Round(health.BaseHealth, 2) / _config.HealthSystemSettings.BaseHealth * 100), 0);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnHealthUpdate)}.", ex);
            }
        }
        
        internal static void FoodUpdate(UnturnedPlayer player, byte food)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(player);
                if (comp == null)
                    return;
                
                var health = comp.HealthData;
                if (health == null)
                    return;
                
                EffectHelper.SendUIEffectProgressBar(player, (short)_config.EffectId, true, EProgressBar.Food, food, (int)comp.ProgressbarData.Food.Value);
                comp.ProgressbarData.Food.Value = food;

                if (food <= 0)
                    comp.TryAddState(EPlayerState.NO_FOOD);
                else
                    comp.TryRemoveState(EPlayerState.NO_FOOD);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnFoodUpdate)}.", ex);
            }
        }
        
        internal static void StaminaUpdate(UnturnedPlayer player, byte stamina)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(player);
                if (comp == null)
                    return;
                
                EffectHelper.SendUIEffectProgressBar(player, (short)_config.EffectId, true, EProgressBar.Stamina,
                    stamina, (int)comp.ProgressbarData.Stamina.Value);
                comp.ProgressbarData.Stamina.Value = stamina;
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnStaminaUpdate)}.", ex);
            }
        }
        
        internal static void WaterUpdate(UnturnedPlayer player, byte water)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(player);
                if (comp == null)
                    return;
                
                EffectHelper.SendUIEffectProgressBar(player, (short)_config.EffectId, true, EProgressBar.Water, water, (int)comp.ProgressbarData.Water.Value);
                comp.ProgressbarData.Water.Value = water;

                if (water <= 0)
                    comp.TryAddState(EPlayerState.NO_WATER);
                else
                    comp.TryRemoveState(EPlayerState.NO_WATER);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnWaterUpdate)}.", ex);
            }
        }
        
        internal static void VirusUpdate(UnturnedPlayer player, byte virus)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(player);
                if (comp == null)
                    return;
                
                EffectHelper.SendUIEffectProgressBar(player, (short)_config.EffectId, true, EProgressBar.Radiation, virus, (int)comp.ProgressbarData.Virus.Value);
                comp.ProgressbarData.Virus.Value = virus;

                if (virus <= 0)
                    comp.TryAddState(EPlayerState.NO_VIRUS);
                else
                    comp.TryRemoveState(EPlayerState.NO_VIRUS);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnVirusUpdate)}.", ex);
            }
        }
        
        internal static void OxygenUpdate(UnturnedPlayer player, byte oxygen)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(player);
                if (comp == null)
                    return;
                
                EffectHelper.SendUIEffectProgressBar(player, (short)_config.EffectId, true, EProgressBar.Oxygen, oxygen, (int)comp.ProgressbarData.Oxygen.Value);
                comp.ProgressbarData.Oxygen.Value = oxygen;

                if (oxygen <= 0)
                    comp.TryAddState(EPlayerState.NO_OXYGEN);
                else
                    comp.TryRemoveState(EPlayerState.NO_OXYGEN);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnOxygenUpdate)}.", ex);
            }
        }
        
        internal static void BleedingUpdate(UnturnedPlayer player, bool isBleeding)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(player);
                if (comp == null)
                    return;
                
                if (!isBleeding)
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
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnBleedingUpdate)}.", ex);
            }
        }
        
        internal static void BonesUpdate(UnturnedPlayer player, bool isBroken)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(player);
                if (comp == null)
                    return;
                
                if (!isBroken)
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
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnBonesUpdate)}.", ex);
            }
        }

        
        internal static void DeadzoneUpdated(UnturnedPlayer player, bool isInDeadZone)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(player);
                if (comp == null)
                    return;
                
                if (isInDeadZone)
                    comp.TryAddState(EPlayerState.DEATH_ZONE);
                else
                    comp.TryRemoveState(EPlayerState.DEATH_ZONE);
            }
            catch (Exception ex)
            {
               AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnDeadzoneUpdated)}.", ex);
            }
        }
        
        internal static void SafezoneUpdated(UnturnedPlayer player, bool isSafe)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(player);
                if (comp == null)
                    return;
                
                if (isSafe)
                    comp.TryAddState(EPlayerState.SAFE_ZONE);
                else
                    comp.TryRemoveState(EPlayerState.SAFE_ZONE);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnSafezoneUpdated)}.", ex);
            }
        }
        
        internal static void TemperatureUpdate(UnturnedPlayer player, EPlayerTemperature temperature)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(player);
                if (comp == null)
                    return;
                
                comp.TryRemoveState(EffectHelper.GetPlayerState(comp.currentTemperature), false);
                comp.TryAddState(EffectHelper.GetPlayerState(temperature));
                comp.currentTemperature = temperature;
                
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnTemperatureUpdate)}.", ex);
            }
        }
        
        internal static void StanceUpdate(UnturnedPlayer player, byte stance)
        {
            try
            {
                var comp = ComponentManager.Get(player);
                if (comp == null)
                    return;
                
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
                    if (!_config.HealthSystemSettings.Movement.CanWalkWithOneBrokenLeg || !_config.HealthSystemSettings.Movement.CanWalkWithBrokenLegs && health.LeftLegHealth == 0 &&
                        health.RightLegHealth == 0)
                        player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                }
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnStanceUpdate)}.", ex);
            }
        }
    }
}