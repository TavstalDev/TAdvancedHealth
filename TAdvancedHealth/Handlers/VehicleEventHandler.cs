using System;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Database;
using Tavstal.TLibrary.Extensions;

namespace Tavstal.TAdvancedHealth.Handlers
{
    public static class VehicleEventHandler
    {
        private static AdvancedHealthConfig _config => AdvancedHealth.Instance.Config;
        
        internal static void Attach()
        {
            VehicleManager.onEnterVehicleRequested += OnPlayerVehicleEnterRequested;
            VehicleManager.onSwapSeatRequested += OnPlayerSwapSeatRequested;
        }

        internal static void Detach()
        {
            VehicleManager.onEnterVehicleRequested -= OnPlayerVehicleEnterRequested;
            VehicleManager.onSwapSeatRequested -= OnPlayerSwapSeatRequested;
        }

        private static void OnPlayerVehicleEnterRequested(SDG.Unturned.Player p, InteractableVehicle vehicle, ref bool shouldAllow)
        {
            string methodName = "OnPlayerVehicleEnterRequested";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(p);
                var comp = player.GetComponent<AdvancedHealthComponent>();
                HealthData? healthData = comp.HealthData;
                if (healthData == null)
                    return;
                
                if (vehicle.passengers[0].player != null)
                    return;

                if (!_config.HealthSystemSettings.CanDriveWithBrokenLegs)
                    if (healthData is { LeftLegHealth: 0, RightLegHealth: 0 } || player.Broken)
                    {
                        shouldAllow = false;
                        return;
                    }

                if (!_config.HealthSystemSettings.CanDriveWithOneBrokenLeg)
                    if (healthData.LeftLegHealth == 0 || healthData.RightLegHealth == 0 || player.Broken)
                    {
                        shouldAllow = false;
                        return;
                    }
                
                if (!_config.HealthSystemSettings.CanDriveWithBrokenArms)
                    if (healthData is { LeftArmHealth: 0, RightArmHealth: 0 })
                    {
                        shouldAllow = false;
                        return;
                    }
                
                if (!_config.HealthSystemSettings.CanDriveWithOneBrokenArm)
                    if (healthData.LeftArmHealth == 0 || healthData.RightArmHealth == 0)
                        shouldAllow = false;
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {methodName}.", ex);
            }
        }
        
        private static void OnPlayerSwapSeatRequested(SDG.Unturned.Player p, InteractableVehicle vehicle, ref bool shouldAllow, byte fromSeatIndex, ref byte toSeatIndex)
        {
            string methodName = "SwapSeat";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(p);
                var comp = player.GetComponent<AdvancedHealthComponent>();
                HealthData? healthData = comp.HealthData;
                if (healthData == null)
                    return;
                
                if (healthData is { LeftLegHealth: 0, RightLegHealth: 0 } || player.Broken)
                {
                    if (toSeatIndex == 0 && !_config.HealthSystemSettings.CanDriveWithBrokenLegs)
                        shouldAllow = false;
                }
                else if (healthData.LeftLegHealth == 0 || healthData.RightLegHealth == 0)
                    if (toSeatIndex == 0 && !_config.HealthSystemSettings.CanDriveWithOneBrokenLeg)
                        shouldAllow = false;

                if (healthData is { LeftArmHealth: 0, RightArmHealth: 0 } || player.Broken)
                {
                    if (!_config.HealthSystemSettings.CanDriveWithBrokenArms && toSeatIndex == 0)
                        shouldAllow = false;
                }
                else if (healthData.LeftArmHealth == 0 || healthData.RightArmHealth == 0)
                    if (!_config.HealthSystemSettings.CanDriveWithOneBrokenLeg && toSeatIndex == 0)
                        shouldAllow = false;
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {methodName}.", ex);
            }
        }
    }
}