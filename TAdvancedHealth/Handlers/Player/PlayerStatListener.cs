using System;
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

namespace Tavstal.TAdvancedHealth.Handlers.Player
{
    public class PlayerStatListener : EventListener
    {
        private AdvancedHealthConfig _config => AdvancedHealth.Instance.Config;

        [EventHandler]
        private void OnHealthUpdate(PlayerHealthEvent e)
        {
            try
            {
                if (_config.HealthSystemSettings.EnableLimbHealthSystem)
                    return;
                
                AdvancedHealthComponent? comp = ComponentManager.Get(e.Player);
                if (comp == null)
                    return;
                
                var health = comp.HealthData;
                if (health == null)
                    return;
                
                EffectHelper.SendUIEffectProgressBar(e.Player, (short)_config.EffectId, true, EProgressBar.SimpleHealth, (int)(Math.Round(health.BaseHealth, 2) / _config.HealthSystemSettings.BaseHealth * 100), 0);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnHealthUpdate)}.", ex);
            }
        }

        [EventHandler]
        private void OnFoodUpdate(PlayerFoodEvent e)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(e.Player);
                if (comp == null)
                    return;
                
                var health = comp.HealthData;
                if (health == null)
                    return;
                
                EffectHelper.SendUIEffectProgressBar(e.Player, (short)_config.EffectId, true, EProgressBar.Food, e.Value, (int)comp.ProgressbarData.Food.Value);
                comp.ProgressbarData.Food.Value = e.Value;

                if (e.Value <= 0)
                    comp.TryAddState(EPlayerState.NO_FOOD);
                else
                    comp.TryRemoveState(EPlayerState.NO_FOOD);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnFoodUpdate)}.", ex);
            }
        }
        
        [EventHandler]
        private void OnStaminaUpdate(PlayerStaminaEvent e)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(e.Player);
                if (comp == null)
                    return;
                
                EffectHelper.SendUIEffectProgressBar(e.Player, (short)_config.EffectId, true, EProgressBar.Stamina,
                    e.Value, (int)comp.ProgressbarData.Stamina.Value);
                comp.ProgressbarData.Stamina.Value = e.Value;
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnStaminaUpdate)}.", ex);
            }
        }
        
        [EventHandler]
        private void OnWaterUpdate(PlayerWaterEvent e)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(e.Player);
                if (comp == null)
                    return;
                
                EffectHelper.SendUIEffectProgressBar(e.Player, (short)_config.EffectId, true, EProgressBar.Water, e.Value, (int)comp.ProgressbarData.Water.Value);
                comp.ProgressbarData.Water.Value = e.Value;

                if (e.Value <= 0)
                    comp.TryAddState(EPlayerState.NO_WATER);
                else
                    comp.TryRemoveState(EPlayerState.NO_WATER);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnWaterUpdate)}.", ex);
            }
        }

        [EventHandler]
        private void OnVirusUpdate(PlayerVirusEvent e)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(e.Player);
                if (comp == null)
                    return;
                
                EffectHelper.SendUIEffectProgressBar(e.Player, (short)_config.EffectId, true, EProgressBar.Radiation, e.Value, (int)comp.ProgressbarData.Virus.Value);
                comp.ProgressbarData.Virus.Value = e.Value;

                if (e.Value <= 0)
                    comp.TryAddState(EPlayerState.NO_VIRUS);
                else
                    comp.TryRemoveState(EPlayerState.NO_VIRUS);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnVirusUpdate)}.", ex);
            }
        }

        [EventHandler]
        private void OnOxygenUpdate(PlayerOxygenEvent e)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(e.Player);
                if (comp == null)
                    return;
                
                EffectHelper.SendUIEffectProgressBar(e.Player, (short)_config.EffectId, true, EProgressBar.Oxygen, e.NewOxygen, (int)comp.ProgressbarData.Oxygen.Value);
                comp.ProgressbarData.Oxygen.Value = e.NewOxygen;

                if (e.NewOxygen <= 0)
                    comp.TryAddState(EPlayerState.NO_OXYGEN);
                else
                    comp.TryRemoveState(EPlayerState.NO_OXYGEN);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnOxygenUpdate)}.", ex);
            }
        }

        [EventHandler]
        private void OnBleedingUpdate(PlayerBleedingEvent e)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(e.Player);
                if (comp == null)
                    return;
                
                if (!e.IsBleeding)
                {
                    comp.hasHeavyBleeding = false;
                    comp.TryRemoveState(EPlayerState.BLEEDING);
                    return;
                }

                if (!_config.HealthSystemSettings.Combat.CanStartBleeding)
                {
                    e.Player.Bleeding = false;
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
        
        [EventHandler]
        private void OnBonesUpdate(PlayerBonesEvent e)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(e.Player);
                if (comp == null)
                    return;
                
                if (!e.IsBroken)
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
                                        e.Player.SteamPlayer().transportConnection);
                                });

                    }
                }

                var health = comp.HealthData;
                if (health != null)
                {
                    if (health.LeftLegHealth == 0 && health.RightLegHealth == 0)
                    {
                        if (!_config.HealthSystemSettings.Movement.CanWalkWithBrokenLegs)
                            e.Player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                    }
                    else if (health.LeftLegHealth == 0 || health.RightLegHealth == 0)
                    {
                        if (!_config.HealthSystemSettings.Movement.CanWalkWithOneBrokenLeg)
                            e.Player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                    }
                }

                comp.TryAddState(EPlayerState.BROKEN_BONES);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnBonesUpdate)}.", ex);
            }
        }

        [EventHandler]
        private void OnDeadzoneUpdated(PlayerDeadzoneUpdatedEvent e)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(e.Player);
                if (comp == null)
                    return;
                
                if (e.IsInDeadzone)
                    comp.TryAddState(EPlayerState.DEATH_ZONE);
                else
                    comp.TryRemoveState(EPlayerState.DEATH_ZONE);
            }
            catch (Exception ex)
            {
               AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnDeadzoneUpdated)}.", ex);
            }
        }
        
        [EventHandler]
        private void OnSafezoneUpdated(PlayerSafezoneUpdatedEvent e)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(e.Player);
                if (comp == null)
                    return;
                
                if (e.IsSafe)
                    comp.TryAddState(EPlayerState.SAFE_ZONE);
                else
                    comp.TryRemoveState(EPlayerState.SAFE_ZONE);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnSafezoneUpdated)}.", ex);
            }
        }
        
        [EventHandler]
        private void OnTemperatureUpdate(PlayerTemperatureUpdatedEvent e)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(e.Player);
                if (comp == null)
                    return;
                
                comp.TryRemoveState(EffectHelper.GetPlayerState(comp.currentTemperature), false);
                comp.TryAddState(EffectHelper.GetPlayerState(e.NewTemperature));
                comp.currentTemperature = e.NewTemperature;
                
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnTemperatureUpdate)}.", ex);
            }
        }
        
        [EventHandler]
        private void OnStanceUpdate(PlayerStanceEvent e)
        {
            try
            {
                var comp = ComponentManager.Get(e.Player);
                if (comp == null)
                    return;
                
                var health = comp.HealthData;
                if (health == null)
                    return;
                
                if (health.IsInjured)
                {
                    e.Player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                    return;
                }

                if (health.LeftLegHealth == 0 || health.RightLegHealth == 0)
                {
                    if (!_config.HealthSystemSettings.Movement.CanWalkWithOneBrokenLeg || !_config.HealthSystemSettings.Movement.CanWalkWithBrokenLegs && health.LeftLegHealth == 0 &&
                        health.RightLegHealth == 0)
                        e.Player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                }
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnStanceUpdate)}.", ex);
            }
        }
    }
}