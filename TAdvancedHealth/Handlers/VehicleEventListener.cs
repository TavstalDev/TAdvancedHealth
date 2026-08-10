using System;
using Rocket.Unturned.Player;
using Tavstal.RocketFlow.Attributes;
using Tavstal.RocketFlow.Core;
using Tavstal.RocketFlow.Events.Vehicle;
using Tavstal.TAdvancedHealth.Utils.Managers;
using Tavstal.TLibrary.Extensions;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace Tavstal.TAdvancedHealth.Handlers
{
    public class VehicleEventListener : EventListener
    {
        private AdvancedHealthConfig _config => AdvancedHealth.Instance.Config;
        
        [EventHandler]
        private void OnPlayerVehicleEnterRequested(VehicleEnterEvent e)
        {
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(e.Player);
                var comp = ComponentManager.Get(player);
                if (comp == null)
                    return;

                var healthData = comp.HealthData;
                if (healthData == null)
                    return;

                if (e.Vehicle.passengers[0].player != null)
                    return;
                
                if (player.Broken && !_config.HealthSystemSettings.Restrictions.CanDriveWithBrokenBones)
                {
                    e.ShouldAllow = false;
                    e.IsCancelled = true;
                    return;
                }
                
                bool bothLegsBroken = _config.HealthSystemSettings.Restrictions.CanDriveWithBrokenLegs;
                bool anyLegBroken = _config.HealthSystemSettings.Restrictions.CanDriveWithOneBrokenLeg;
                
                if (healthData is { LeftLegHealth: 0, RightLegHealth: 0 } && !bothLegsBroken || 
                    (healthData.LeftLegHealth == 0 || healthData.RightLegHealth == 0) && !anyLegBroken)
                {
                    e.ShouldAllow = false;
                    e.IsCancelled = true;
                    return;
                }

                bool bothArmBroken = _config.HealthSystemSettings.Restrictions.CanDriveWithOneBrokenArm;
                bool anyArmBroken = _config.HealthSystemSettings.Restrictions.CanDriveWithOneBrokenArm;
                
                if (healthData is { LeftArmHealth: 0, RightArmHealth: 0 } && !bothArmBroken ||
                    (healthData.LeftArmHealth == 0 || healthData.RightArmHealth == 0) && !anyArmBroken)
                {
                    e.ShouldAllow = false;
                    e.IsCancelled = true;
                }
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerVehicleEnterRequested)}.",
                    ex);
            }
        }
        
        [EventHandler]
        private void OnPlayerSwapSeatRequested(VehicleSwapSeatEvent e)
        {
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(e.Player);
                var comp = ComponentManager.Get(player);
                if (comp == null)
                    return;
                
                var healthData = comp.HealthData;
                if (healthData == null)
                    return;

                if (e.ToSeatIndex != 0)
                    return;
                
                if (player.Broken && !_config.HealthSystemSettings.Restrictions.CanDriveWithBrokenBones)
                {
                    e.ShouldAllow = false;
                    e.IsCancelled = true;
                    return;
                }
                
                bool bothLegsBroken = _config.HealthSystemSettings.Restrictions.CanDriveWithBrokenLegs;
                bool anyLegBroken = _config.HealthSystemSettings.Restrictions.CanDriveWithOneBrokenLeg;
                
                if (healthData is { LeftLegHealth: 0, RightLegHealth: 0 } && !bothLegsBroken || 
                    (healthData.LeftLegHealth == 0 || healthData.RightLegHealth == 0) && !anyLegBroken)
                {
                    e.ShouldAllow = false;
                    e.IsCancelled = true;
                    return;
                }

                bool bothArmBroken = _config.HealthSystemSettings.Restrictions.CanDriveWithOneBrokenArm;
                bool anyArmBroken = _config.HealthSystemSettings.Restrictions.CanDriveWithOneBrokenArm;
                
                if (healthData is { LeftArmHealth: 0, RightArmHealth: 0 } && !bothArmBroken ||
                    (healthData.LeftArmHealth == 0 || healthData.RightArmHealth == 0) && !anyArmBroken)
                {
                    e.ShouldAllow = false;
                    e.IsCancelled = true;
                }
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerSwapSeatRequested)}.", ex);
            }
        }
    }
}