using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TAdvancedHealth.Models.Database;
using Tavstal.TAdvancedHealth.Models.Enums;
using Tavstal.TAdvancedHealth.Utils.Helpers;
using Tavstal.TAdvancedHealth.Utils.Managers;
using Tavstal.TLibrary.Helpers.General;
using UnityEngine;

namespace Tavstal.TAdvancedHealth.Handlers
{
    public static class UnturnedEventHandler
    {
        private static DatabaseManager _database => TAdvancedHealth.Database;
        private static TAdvancedHealthConfig _config => TAdvancedHealth.Instance.Config;

        /// <summary>
        /// Attaches event listeners to Unturned to enable event handling.
        /// </summary>
        internal static void Attach()
        {
            U.Events.OnPlayerConnected += Event_OnPlayerJoin;
            U.Events.OnPlayerDisconnected += Event_OnPlayerLeave;
            UnturnedPlayerEvents.OnPlayerRevive += Event_OnPlayerRevived;
            UnturnedPlayerEvents.OnPlayerUpdateGesture += Event_OnPlayerGestureUpdated;
            EffectManager.onEffectButtonClicked += Event_OnButtonClickded;
            VehicleManager.onEnterVehicleRequested += Event_OnPlayerVehicleEnterRequested;
            VehicleManager.onSwapSeatRequested += Event_OnPlayerSwapSeatRequested;
            UnturnedPlayerEvents.OnPlayerUpdateHealth += Event_OnPlayerHealthUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateFood += Event_OnPlayerFoodUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateWater += Event_OnPlayerWaterUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateStamina += Event_OnPlayerStaminaUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateVirus += Event_OnPlayerVirusUpdate;
            DamageTool.damagePlayerRequested += Event_OnPlayerDamaged;
            UseableConsumeable.onConsumePerformed += Event_OnPlayerUseMedicine;
            UnturnedPlayerEvents.OnPlayerDeath += Event_OnPlayerDeath;
            UnturnedPlayerEvents.OnPlayerUpdateBleeding += Event_OnPlayerBleedingUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateBroken += Event_OnPlayerBrokenUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateStance += Event_OnPlayerStanceUpdate;
            
        }

        /// <summary>
        /// Detaches event listeners from Unturned to disable event handling.
        /// </summary>
        internal static void Dettach()
        {
            U.Events.OnPlayerConnected -= Event_OnPlayerJoin;
            U.Events.OnPlayerDisconnected -= Event_OnPlayerLeave;
            UnturnedPlayerEvents.OnPlayerRevive -= Event_OnPlayerRevived;
            UnturnedPlayerEvents.OnPlayerUpdateGesture -= Event_OnPlayerGestureUpdated;
            EffectManager.onEffectButtonClicked -= Event_OnButtonClickded;
            VehicleManager.onEnterVehicleRequested -= Event_OnPlayerVehicleEnterRequested;
            VehicleManager.onSwapSeatRequested -= Event_OnPlayerSwapSeatRequested;
            UnturnedPlayerEvents.OnPlayerUpdateHealth -= Event_OnPlayerHealthUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateFood -= Event_OnPlayerFoodUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateWater -= Event_OnPlayerWaterUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateStamina -= Event_OnPlayerStaminaUpdate;
            DamageTool.damagePlayerRequested -= Event_OnPlayerDamaged;
            UseableConsumeable.onConsumePerformed -= Event_OnPlayerUseMedicine;
            UnturnedPlayerEvents.OnPlayerDeath -= Event_OnPlayerDeath;
            UnturnedPlayerEvents.OnPlayerUpdateBleeding -= Event_OnPlayerBleedingUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateBroken -= Event_OnPlayerBrokenUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateVirus -= Event_OnPlayerVirusUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateStance -= Event_OnPlayerStanceUpdate;
        }

        #region Status Events
        /// <summary>
        /// Event handler for updates to the health of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose health is being updated.</param>
        /// <param name="value">The new health value of the player.</param>
        private static async void Event_OnPlayerHealthUpdate(UnturnedPlayer player, byte value)
        {
            string voidname = "OnPlayerHealthUpdate";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                HealthData health = await _database.GetPlayerHealthAsync(player.Id);
                await EffectHelper.SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Health_Simple, (int)((System.Math.Round(health.BaseHealth, 2) / _config.HealthSystemSettings.BaseHealth) * 100), 0);
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        /// <summary>
        /// Event handler for updates to the food level of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose food level is being updated.</param>
        /// <param name="value">The new food level value of the player.</param>
        private static async void Event_OnPlayerFoodUpdate(UnturnedPlayer player, byte value)
        {
            string voidname = "OnPlayerFoodUpdate";
            try
            {
                var c = _config.HealthSystemSettings;
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                await EffectHelper.SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Food, player.Player.life.food, (int)comp.progressBarData.LastFood);
                comp.progressBarData.LastFood = value;

                if (value <= 0)
                    comp.TryAddStateAsync(EPlayerStates.NOFOOD);
                else
                    comp.TryRemoveStateAsync(EPlayerStates.NOFOOD);
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        /// <summary>
        /// Event handler for updates to the stamina level of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose stamina level is being updated.</param>
        /// <param name="value">The new stamina level value of the player.</param>
        private static async void Event_OnPlayerStaminaUpdate(UnturnedPlayer player, byte value)
        {
            string voidname = "OnPlayerStaminaUpdate";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                await EffectHelper.SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Stamina, player.Player.life.stamina, (int)comp.progressBarData.LastStamina);
                comp.progressBarData.LastStamina = value;
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        /// <summary>
        /// Event handler for updates to the water level of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose water level is being updated.</param>
        /// <param name="value">The new water level value of the player.</param>
        private static async void Event_OnPlayerWaterUpdate(UnturnedPlayer player, byte value)
        {
            string voidname = "OnPlayerWaterUpdate";
            try
            {
                var c = _config.HealthSystemSettings;
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                await EffectHelper.SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Water, player.Player.life.water, (int)comp.progressBarData.LastWater);
                comp.progressBarData.LastWater = value;

                if (value <= 0)
                    comp.TryAddStateAsync(EPlayerStates.NOWATER);
                else
                    comp.TryRemoveStateAsync(EPlayerStates.NOWATER);
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        /// <summary>
        /// Event handler for updates to the virus level of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose virus level is being updated.</param>
        /// <param name="value">The new virus level value of the player.</param>
        private static async void Event_OnPlayerVirusUpdate(UnturnedPlayer player, byte value)
        {
            string voidname = "OnPlayerVirusUpdate";
            try
            {
                var c = _config.HealthSystemSettings;
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                await EffectHelper.SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Radiation, player.Player.life.virus, (int)comp.progressBarData.LastVirus);
                comp.progressBarData.LastVirus = value;

                if (value <= 0)
                    comp.TryAddStateAsync(EPlayerStates.NOVIRUS);
                else
                    comp.TryRemoveStateAsync(EPlayerStates.NOVIRUS);
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        /// <summary>
        /// Event handler for updates to the oxygen level of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose oxygen level is being updated.</param>
        /// <param name="value">The new oxygen level value of the player.</param>
        private static async void Event_OnPlayerOxygenUpdate(UnturnedPlayer player, byte value)
        {
            string voidname = "OnPlayerOxygenUpdate";
            try
            {
                var c = _config.HealthSystemSettings;
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                await EffectHelper.SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Oxygen, player.Player.life.oxygen, (int)comp.progressBarData.LastOxygen);
                comp.progressBarData.LastOxygen = value;

                if (value <= 0)
                    comp.TryAddStateAsync(EPlayerStates.NOOXYGEN);
                else
                    comp.TryRemoveStateAsync(EPlayerStates.NOOXYGEN);
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        /// <summary>
        /// Event handler for updates to the bleeding state of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose bleeding state is being updated.</param>
        /// <param name="state">A boolean indicating whether the player is bleeding.</param>
        private static void Event_OnPlayerBleedingUpdate(UnturnedPlayer player, bool state)
        {
            string voidname = "OnPlayerBleedingUpdate";
            try
            {
                var c = _config.HealthSystemSettings;
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                if (state)
                {
                    if (!c.CanStartBleeding && state)
                    {
                        player.Bleeding = false;
                        return;
                    }

                    int val = MathHelper.Next(1, 100);

                    if (val <= c.HeavyBleedingChance)
                        comp.hasHeavyBleeding = true;

                    comp.TryAddStateAsync(EPlayerStates.BLEEDING);
                }
                else
                {
                    comp.hasHeavyBleeding = false;
                    comp.TryRemoveStateAsync(EPlayerStates.BLEEDING);
                }

            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        /// <summary>
        /// Event handler for updates to the broken state of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose broken state is being updated.</param>
        /// <param name="state">A boolean indicating whether the player is in a broken state.</param>
        private static async void Event_OnPlayerBrokenUpdate(UnturnedPlayer player, bool state)
        {
            string voidname = "OnPlayerBrokenUpdate";
            try
            {
                var c = _config.HealthSystemSettings;
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                HealthData health = await _database.GetPlayerHealthAsync(player.Id);
                if (state)
                {
                    if (c.CanHavePainEffect)
                    {
                        int painChance = MathHelper.Next(1, 100);

                        if (painChance <= c.PainEffectChance)
                        {
                            EffectManager.sendUIEffect(c.PainEffectID, (short)c.PainEffectID, comp.TranspConnection, true);
                            if (c.PainEffectDuration > 0)
                                TAdvancedHealth.Instance.InvokeAction(c.PainEffectDuration, () => { SDG.Unturned.EffectManager.askEffectClearByID(c.PainEffectID, player.SteamPlayer().transportConnection); });

                        }
                    }

                    if (health.LeftLegHealth == 0 && health.RightLegHealth == 0)
                    {
                        if (!c.CanWalkWithBrokenLegs)
                        {
                            player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                        }
                    }
                    else if (health.LeftLegHealth == 0 || health.RightLegHealth == 0)
                    {
                        if (!c.CanWalkWithOneBrokenLeg)
                        {
                            player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                        }
                    }

                    comp.TryAddStateAsync(EPlayerStates.BROKENBONE);
                }
                else
                    comp.TryRemoveStateAsync(EPlayerStates.BROKENBONE);

            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        /// <summary>
        /// Event handler for updates to the moon state.
        /// </summary>
        /// <param name="isFullMoon">A boolean indicating whether the moon is in a full state.</param>
        internal static void Event_OnMoonUpdated(bool isFullMoon)
        {
            string voidname = "OnMoonUpdated";
            try
            {
                foreach (SteamPlayer steamPlayer in Provider.clients)
                {
                    UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                    AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                    if (isFullMoon)
                        comp.TryAddStateAsync(EPlayerStates.FULLMOON);
                    else
                        comp.TryRemoveStateAsync(EPlayerStates.FULLMOON);
                }

            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        /// <summary>
        /// Event handler for updates to the deadzone status of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose deadzone status is being updated.</param>
        /// <param name="isActive">A boolean indicating whether the player is currently in a deadzone.</param>
        private static void Event_OnPlayerDeadzoneUpdated(UnturnedPlayer player, bool isActive)
        {
            string voidname = "OnPlayerDeadzoneUpdated";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                if (isActive)
                    comp.TryAddStateAsync(EPlayerStates.DEADZONE);
                else
                    comp.TryRemoveStateAsync(EPlayerStates.DEADZONE);

            }
            catch (Exception e)
            {
               TAdvancedHealth.Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        /// <summary>
        /// Event handler for updates to the safezone status of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose safezone status is being updated.</param>
        /// <param name="isActive">A boolean indicating whether the player is currently in a safezone.</param>
        private static void Event_OnPlayerSafezoneUpdated(UnturnedPlayer player, bool isActive)
        {
            string voidname = "OnPlayerSafezoneUpdated";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                if (isActive)
                    comp.TryAddStateAsync(EPlayerStates.SAFEZONE);
                else
                    comp.TryRemoveStateAsync(EPlayerStates.SAFEZONE);

            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        /// <summary>
        /// Event handler for updates to the temperature of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose temperature is being updated.</param>
        /// <param name="newTemperature">The new temperature state of the player.</param>
        private static void Event_OnPlayerTemperatureUpdate(UnturnedPlayer player, EPlayerTemperature newTemperature)
        {
            string voidname = "OnPlayerTemperatureUpdate";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                EPlayerStates state = EPlayerStates.ACID;

                if (newTemperature == EPlayerTemperature.ACID)
                    state = EPlayerStates.ACID;
                else if (newTemperature == EPlayerTemperature.BURNING)
                    state = EPlayerStates.BURNING;
                else if (newTemperature == EPlayerTemperature.COLD)
                    state = EPlayerStates.COLD;
                else if (newTemperature == EPlayerTemperature.COVERED)
                    state = EPlayerStates.COVERED;
                else if (newTemperature == EPlayerTemperature.FREEZING)
                    state = EPlayerStates.FREEZING;
                else if (newTemperature == EPlayerTemperature.WARM)
                    state = EPlayerStates.WARM;
                else if (newTemperature == EPlayerTemperature.NONE)
                    state = EPlayerStates.NONE_TEMPERATURE;

                comp.TryAddStateAsync(state);

            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        /// <summary>
        /// Event handler for updates to the stance of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose stance is being updated.</param>
        /// <param name="stance">The new stance value of the player.</param>
        private static async void Event_OnPlayerStanceUpdate(UnturnedPlayer player, byte stance)
        {
            HealthData health = await _database.GetPlayerHealthAsync(player.Id);
            if (health.IsInjured)
                player.Player.stance.checkStance(EPlayerStance.PRONE, true);
            else
            {
                var c = _config.HealthSystemSettings;
                if (health.LeftLegHealth == 0 || health.RightLegHealth == 0)
                {
                    if (!c.CanWalkWithOneBrokenLeg)
                        player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                    else if (!c.CanWalkWithBrokenLegs && health.LeftLegHealth == 0 && health.RightLegHealth == 0)
                        player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                }
            }
        }
        #endregion

        // Other Events

        private static async void Event_OnPlayerUseMedicine(Player instigatingPlayer, ItemConsumeableAsset consumeableAsset)
        {
            string voidname = "UseMedicine";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(instigatingPlayer);

                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                var c = _config.HealthSystemSettings;
                if (comp.LastEquipedItem == 0) { return; }

                if (_config.Medicines != null)
                {
                    Medicine med = _config.Medicines.FirstOrDefault(x => x.ItemID == comp.LastEquipedItem);
                    if (med != null)
                    {
                        HealthData health = await _database.GetPlayerHealthAsync(player.Id);
                        if (health.BodyHealth + med.HealsBodyHP <= c.BodyHealth)
                            health.BodyHealth += med.HealsBodyHP;
                        else
                            health.BodyHealth = c.BodyHealth;

                        if (health.HeadHealth + med.HealsHeadHP <= c.HeadHealth)
                            health.HeadHealth += med.HealsHeadHP;
                        else
                            health.HeadHealth = c.HeadHealth;

                        if (med.CuresPain)
                            EffectManager.askEffectClearByID(c.PainEffectID, player.SteamPlayer().transportConnection);

                        if (health.LeftLegHealth + med.HealsLeftLegHP <= c.LeftLegHealth)
                        {
                            health.LeftLegHealth += med.HealsLeftLegHP;
                            TAdvancedHealth.Instance.InvokeAction(0.5f, () =>
                            {
                                player.Broken = false;
                                player.Player.movement.sendPluginJumpMultiplier(1f);
                            });
                        }
                        else
                        {
                            health.LeftLegHealth = c.LeftLegHealth;
                            TAdvancedHealth.Instance.StartCoroutine(TAdvancedHealth.Instance.DelayedInvoke(0.5f, () =>
                            {
                                player.Broken = false;
                                player.Player.movement.sendPluginJumpMultiplier(1f);
                            }));
                        }

                        if (health.RightLegHealth + med.HealsRightLegHP <= c.RightLegHealth)
                        {
                            health.RightLegHealth += med.HealsRightLegHP;
                            TAdvancedHealth.Instance.StartCoroutine(TAdvancedHealth.Instance.DelayedInvoke(0.5f, () =>
                            {
                                player.Broken = false;
                                player.Player.movement.sendPluginJumpMultiplier(1f);
                            }));
                        }
                        else
                        {
                            health.RightLegHealth = c.RightLegHealth;
                            TAdvancedHealth.Instance.StartCoroutine(TAdvancedHealth.Instance.DelayedInvoke(0.5f, () =>
                            {
                                player.Broken = false;
                                player.Player.movement.sendPluginJumpMultiplier(1f);
                            }));
                        }

                        if (health.LeftArmHealth + med.HealsLeftArmHP <= c.LeftArmHealth)
                            health.LeftArmHealth += med.HealsLeftArmHP;
                        else
                            health.LeftArmHealth = c.LeftArmHealth;

                        if (health.RightArmHealth + med.HealsRightArmHP <= c.RightArmHealth)
                            health.RightArmHealth += med.HealsRightArmHP;
                        else
                            health.RightArmHealth = c.RightArmHealth;

                        _database.UpdateHealthAsync(player.Id, health, EDatabaseEvent.ALL);
                    }
                    comp.LastEquipedItem = 0;
                }
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerVehicleEnterRequested(Player p, InteractableVehicle vehicle, ref bool shouldAllow)
        {
            string voidname = "VehicleEnter";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(p);
                var c = _config.HealthSystemSettings;
                HealthData health = _database.GetPlayerHealth(player.Id);
                if (vehicle.passengers[0].player == null)
                {
                    if (health.LeftLegHealth == 0 && health.RightLegHealth == 0 || player.Broken)
                    {
                        if (!c.CanDriveWithBrokenLegs)
                            shouldAllow = false;
                    }
                    else if (health.LeftLegHealth == 0 || health.RightLegHealth == 0)
                        if (!c.CanDriveWithOneBrokenLeg)
                            shouldAllow = false;

                    if (health.LeftArmHealth == 0 && health.RightArmHealth == 0 || player.Broken)
                    {
                        if (!c.CanDriveWithBrokenArms)
                            shouldAllow = false;
                    }
                    else if (health.LeftArmHealth == 0 || health.RightArmHealth == 0)
                        if (!c.CanDriveWithOneBrokenLeg)
                            shouldAllow = false;
                }
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerSwapSeatRequested(Player p, InteractableVehicle vehicle, ref bool shouldAllow, byte fromSeatIndex, ref byte toSeatIndex)
        {
            string voidname = "SwapSeat";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(p);
                var c = _config.HealthSystemSettings;
                HealthData h = _database.GetPlayerHealth(player.Id);
                if (h.LeftLegHealth == 0 && h.RightLegHealth == 0 || player.Broken)
                {
                    if (toSeatIndex == 0 && !c.CanDriveWithBrokenLegs)
                        shouldAllow = false;
                }
                else if (h.LeftLegHealth == 0 || h.RightLegHealth == 0)
                    if (toSeatIndex == 0 && !c.CanDriveWithOneBrokenLeg)
                        shouldAllow = false;


                if (h.LeftArmHealth == 0 && h.RightArmHealth == 0 || player.Broken)
                {
                    if (!c.CanDriveWithBrokenArms && toSeatIndex == 0)
                        shouldAllow = false;
                }
                else if (h.LeftArmHealth == 0 || h.RightArmHealth == 0)
                    if (!c.CanDriveWithOneBrokenLeg && toSeatIndex == 0)
                        shouldAllow = false;
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerJoin(UnturnedPlayer player)
        {
            string voidname = "Join";
            try
            {
                HealthData health = _database.GetPlayerHealth(player.Id);
                if (health == null)
                {
                    var c = _config;
                    var s = c.HealthSystemSettings;
                    HUDStyle style = _config.HUDStyles.FirstOrDefault(x => x.Enabled);
                    _database.AddHealthDataAsync(player.Id, new HealthData { PlayerId = player.Id, HUDEffectID = style.EffectID, BaseHealth = s.BaseHealth, BodyHealth = s.BodyHealth, HeadHealth = s.HeadHealth, LeftArmHealth = s.LeftArmHealth, LeftLegHealth = s.LeftLegHealth, RightArmHealth = s.RightArmHealth, RightLegHealth = s.RightLegHealth, IsInjured = false, IsHUDEnabled = true, DeathDate = DateTime.Now });
                    health = _database.GetPlayerHealth(player.Id);
                }
                else
                {
                    var c = _config;
                    var s = c.HealthSystemSettings;
                    if (health.BaseHealth > s.BaseHealth)
                        health.BaseHealth = s.BaseHealth;
                    if (s.EnableTarkovLikeHealth)
                    {
                        if (health.HeadHealth > s.HeadHealth)
                            health.HeadHealth = s.HeadHealth;
                        if (health.BodyHealth > s.BodyHealth)
                            health.BodyHealth = s.BodyHealth;
                        if (health.RightArmHealth > s.RightArmHealth)
                            health.RightArmHealth = s.RightArmHealth;
                        if (health.LeftArmHealth > s.LeftArmHealth)
                            health.LeftArmHealth = s.LeftArmHealth;
                        if (health.RightLegHealth > s.RightLegHealth)
                            health.RightLegHealth = s.RightLegHealth;
                        if (health.LeftLegHealth > s.LeftLegHealth)
                            health.LeftLegHealth = s.LeftLegHealth;
                        _database.UpdateHealthAsync(player.Id, health, EDatabaseEvent.ALL);
                    }
                }

                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                comp.EffectID = health.HUDEffectID;
                comp.progressBarData.LastHealthHead = health.HeadHealth;
                comp.progressBarData.LastHealthBody = health.BodyHealth;
                comp.progressBarData.LastHealthLeftArm = health.LeftArmHealth;
                comp.progressBarData.LastHealthLeftLeg = health.LeftLegHealth;
                comp.progressBarData.LastHealthRightArm = health.RightArmHealth;
                comp.progressBarData.LastHealthRightLeg = health.RightLegHealth;
                comp.progressBarData.LastFood = player.Player.life.food;
                comp.progressBarData.LastWater = player.Player.life.water;
                comp.progressBarData.LastVirus = player.Player.life.virus;
                comp.progressBarData.LastOxygen = player.Player.life.oxygen;
                comp.progressBarData.LastStamina = player.Player.life.stamina;

                Event_OnPlayerFoodUpdate(player, player.Player.life.food);
                Event_OnPlayerWaterUpdate(player, player.Player.life.water);
                Event_OnPlayerVirusUpdate(player, player.Player.life.virus);
                Event_OnPlayerOxygenUpdate(player, player.Player.life.oxygen);
                Event_OnPlayerStaminaUpdate(player, player.Player.life.stamina);
                Event_OnPlayerBleedingUpdate(player, player.Bleeding);
                Event_OnPlayerBrokenUpdate(player, player.Broken);
                Event_OnPlayerSafezoneUpdated(player, player.Player.movement.isSafe);
                Event_OnPlayerDeadzoneUpdated(player, player.Player.movement.isRadiated);
                Event_OnPlayerTemperatureUpdate(player, player.Player.life.temperature);

                if (TAdvancedHealth.Instance._hasFullMoon)
                    comp.TryAddStateAsync(EPlayerStates.FULLMOON);

                player.Player.equipment.onEquipRequested += Event_OnPlayerEquipRequested;
                player.Player.equipment.onDequipRequested += Event_OnPlayerDequipRequested;
                player.Player.life.onHurt += Event_OnPlayerLifeDamaged;
                player.Player.life.onOxygenUpdated += (byte b) => Event_OnPlayerOxygenUpdate(player, b);
                player.Player.life.onTemperatureUpdated += (EPlayerTemperature newTemperature) => Event_OnPlayerTemperatureUpdate(player, newTemperature);
                player.Player.movement.onSafetyUpdated += (bool isSafe) => Event_OnPlayerSafezoneUpdated(player, isSafe);
                player.Player.movement.onRadiationUpdated += (bool isRadio) => Event_OnPlayerDeadzoneUpdated(player, isRadio);
                player.Player.life.onVirusUpdated += (byte virus) => Event_OnPlayerVirusUpdate(player, virus);
                #region HideHealth HUD
                if (health.IsHUDEnabled)
                {
                    EffectManager.sendUIEffect(comp.EffectID, (short)comp.EffectID, comp.TranspConnection, true);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowFood, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowHealth, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowOxygen, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStamina, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVirus, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowWater, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons, false);
                    HealthHelper.UpdateHealthAllUI(player);
                }
                #endregion
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerLeave(UnturnedPlayer player)
        {
            string voidname = "Leave";
            try
            {
                player.Player.equipment.onEquipRequested -= Event_OnPlayerEquipRequested;
                player.Player.equipment.onDequipRequested -= Event_OnPlayerDequipRequested;
                player.Player.life.onHurt -= Event_OnPlayerLifeDamaged;

                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                if (comp.DragState != EDragState.NONE)
                    comp.UnDrag();
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerEquipRequested(PlayerEquipment equipment, ItemJar jar, ItemAsset asset, ref bool shouldAllow)
        {
            string voidname = "Equip";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(equipment.player);
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                bool isMedicine = false;
                if (_config.Medicines.FirstOrDefault(x => x.ItemID == jar.item.id) != null)
                {
                    comp.LastEquipedItem = jar.item.id;
                    isMedicine = true;
                }

                var c = _config.HealthSystemSettings;
                HealthData h = _database.GetPlayerHealth(player.Id);
                if (h.RightArmHealth == 0 && h.LeftArmHealth == 0)
                {
                    if (!isMedicine)
                    {
                        if (!c.CanHoldOneHandItemsWithBrokenArms)
                            if (_config.OneHandedItems.ItemID.Contains(jar.item.id) || _config.OneHandedItems.ItemTypes.Contains(asset.type))
                            {
                                shouldAllow = false;
                                if (player.Player.equipment.isEquipped)
                                    player.Player.equipment.dequip();
                            }
                        if (!c.CanHoldTwoHandItemsWithBrokenArms)
                            if (_config.TwoHandedItems.ItemID.Contains(jar.item.id) || _config.TwoHandedItems.ItemTypes.Contains(asset.type))
                            {
                                shouldAllow = false;
                                if (player.Player.equipment.isEquipped)
                                    player.Player.equipment.dequip();
                            }
                    }
                }
                else if (h.RightArmHealth == 0 || h.LeftArmHealth == 0)
                    if (!isMedicine)
                    {
                        if (!c.CanHoldOneHandItemsWithOneBrokenArm)
                            if (_config.OneHandedItems.ItemID.Contains(jar.item.id) || _config.OneHandedItems.ItemTypes.Contains(asset.type))
                            {
                                shouldAllow = false;
                                if (player.Player.equipment.isEquipped)
                                    player.Player.equipment.dequip();
                            }
                        if (!c.CanHoldTwoHandItemsWithOneBrokenArm)
                            if (_config.TwoHandedItems.ItemID.Contains(jar.item.id) || _config.TwoHandedItems.ItemTypes.Contains(asset.type))
                            {
                                shouldAllow = false;
                                if (player.Player.equipment.isEquipped)
                                    player.Player.equipment.dequip();
                            }
                    }
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerDequipRequested(PlayerEquipment equipment, ref bool shouldAllow)
        {
            string voidname = "Dequip";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(equipment.player);
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                comp.LastEquipedItem = 0;
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerLifeDamaged(Player p, byte damage, Vector3 force, EDeathCause cause, ELimb limb, CSteamID killer)
        {
            string voidname = "PlayerLifeDamage";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(p);
                AdvancedHealthComponent cp = player.GetComponent<AdvancedHealthComponent>();
                var c = _config.HealthSystemSettings;
                var ag = _config.AntiGroupFriendlyFireSettings;
                bool allow = true;

                player.Player.life.askHeal(100, false, false);
                HealthData health = _database.GetPlayerHealth(player.Id);
                float totaldamage = damage;
                damage = 0;

                if (cause == EDeathCause.PUNCH)
                    totaldamage = 1.0f;

                if (cause == EDeathCause.BONES)
                {
                    totaldamage = 10.0f;
                    player.Broken = true;
                }

                if (EDeathCause.BLEEDING == cause)
                {
                    if (health.IsInjured && !cp.AllowDamage)
                    {
                        totaldamage = 0;
                        player.Bleeding = false;
                    }
                    else
                    {
                        if (cp.hasHeavyBleeding)
                            totaldamage = c.HeavyBleedingDamage;
                        else
                            totaldamage = c.BleedingDamage;
                        player.Bleeding = true;
                    }
                }

                if (cause == EDeathCause.ANIMAL || cause == EDeathCause.ZOMBIE)
                    limb = ELimb.LEFT_FRONT;

                cp.AllowDamage = false;

                if (!allow || player.Features.GodMode)
                    totaldamage = 0;

                Vector3 ragdoll = player.Position.normalized;
                if (limb == ELimb.SKULL)
                {
                    if (HealthHelper.CanBleed(health.HeadHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.HeadHealth - totaldamage > 0)
                    {
                        health.HeadHealth -= totaldamage;
                        _database.UpdateHeadHealth(player.Id, health.HeadHealth);
                    }
                    else
                    {
                        health.HeadHealth = 0;
                        _database.UpdateHeadHealth(player.Id, 0);
                    }

                    if (health.HeadHealth == 0)
                    {
                        if (c.CanBeInjured && !health.IsInjured)
                        {
                            int chanc = MathHelper.Next(1, 100);
                            if (c.InjuredChance >= chanc)
                            {
                                HealthHelper.SetPlayerDowned(player);
                                return;
                            }
                        }

                        if (c.DieWhenHeadHealthIsZero)
                        {
                            cp.AllowDamage = true;
                            CSteamID id = CSteamID.Nil;
                            if (EDeathCause.ZOMBIE != cause)
                            {
                                if (killer != CSteamID.Nil)
                                    id = killer;
                            }
                            player.Player.life.askDamage(100, ragdoll, cause, limb, id, out EPlayerKill outKill);
                        }
                    }
                }
                else if (cause == EDeathCause.BONES)
                {
                    if (HealthHelper.CanBleed(health.LeftLegHealth, totaldamage) || HealthHelper.CanBleed(health.RightLegHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.RightLegHealth - totaldamage > 0)
                    {
                        health.RightLegHealth -= totaldamage;
                        _database.UpdateRightLegHealth(player.Id, health.RightLegHealth);
                    }
                    else
                    {
                        health.RightLegHealth = 0;
                        _database.UpdateRightLegHealth(player.Id, health.RightLegHealth);
                    }

                    if (health.RightLegHealth + health.RightLegHealth == 0)
                    {
                        if (c.DieWhenLegsHealthIsZero)
                        {
                            cp.AllowDamage = true;
                            CSteamID id = CSteamID.Nil;
                            if (EDeathCause.ZOMBIE != cause)
                            {
                                if (killer != CSteamID.Nil)
                                    id = killer;
                            }
                            player.Player.life.askDamage(100, ragdoll, cause, limb, id, out EPlayerKill outKill);
                        }
                    }

                    if (health.LeftLegHealth - totaldamage > 0)
                    {
                        health.LeftLegHealth -= totaldamage;
                        _database.UpdateLeftLegHealth(player.Id, health.LeftArmHealth);
                    }
                    else
                    {
                        health.LeftLegHealth = 0;
                        _database.UpdateLeftLegHealth(player.Id, health.LeftArmHealth);
                    }

                    if (health.LeftLegHealth + health.RightLegHealth == 0)
                    {
                        if (c.DieWhenLegsHealthIsZero)
                        {
                            cp.AllowDamage = true;
                            CSteamID id = CSteamID.Nil;
                            if (EDeathCause.ZOMBIE != cause)
                            {
                                if (killer != CSteamID.Nil)
                                    id = killer;
                            }
                            player.Player.life.askDamage(100, ragdoll, cause, limb, id, out EPlayerKill outKill);
                        }
                    }
                }
                else if (limb == ELimb.LEFT_BACK || limb == ELimb.LEFT_FRONT || limb == ELimb.RIGHT_BACK || limb == ELimb.RIGHT_FRONT || limb == ELimb.SPINE || cause == EDeathCause.INFECTION || cause == EDeathCause.BREATH)
                {
                    if (HealthHelper.CanBleed(health.BodyHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.BodyHealth - totaldamage > 0)
                    {
                        health.BodyHealth -= totaldamage;
                        _database.UpdateBodyHealth(player.Id, health.BodyHealth);
                    }
                    else
                    {
                        health.BodyHealth = 0;
                        _database.UpdateBodyHealth(player.Id, health.BodyHealth);
                    }

                    if (health.BodyHealth == 0)
                    {
                        if (c.CanBeInjured && !health.IsInjured)
                        {
                            int chanc = MathHelper.Next(1, 100);
                            if (c.InjuredChance >= chanc)
                            {
                                HealthHelper.SetPlayerDowned(player);
                                return;
                            }
                        }

                        if (c.DieWhenBodyHealthIsZero)
                        {
                            cp.AllowDamage = true;
                            CSteamID id = CSteamID.Nil;
                            if (EDeathCause.ZOMBIE != cause)
                            {
                                if (killer != CSteamID.Nil)
                                    id = killer;
                            }
                            player.Player.life.askDamage(100, ragdoll, cause, limb, id, out EPlayerKill outKill);
                        }
                    }
                }
                else if (limb == ELimb.LEFT_ARM || limb == ELimb.LEFT_HAND)
                {
                    if (HealthHelper.CanBleed(health.LeftArmHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.LeftArmHealth - totaldamage > 0)
                    {
                        health.LeftArmHealth -= totaldamage;
                        _database.UpdateLeftArmHealth(player.Id, health.LeftArmHealth);
                    }
                    else
                    {
                        health.LeftArmHealth = 0;
                        _database.UpdateLeftArmHealth(player.Id, health.LeftArmHealth);
                    }

                    if (health.LeftArmHealth + health.RightArmHealth == 0)
                    {
                        if (c.DieWhenArmsHealthIsZero)
                        {
                            cp.AllowDamage = true;
                            CSteamID id = CSteamID.Nil;
                            if (EDeathCause.ZOMBIE != cause)
                            {
                                if (killer != CSteamID.Nil)
                                    id = killer;
                            }
                            player.Player.life.askDamage(100, ragdoll, cause, limb, id, out EPlayerKill outKill);
                        }
                    }
                }
                else if (limb == ELimb.RIGHT_ARM || limb == ELimb.RIGHT_HAND)
                {
                    if (HealthHelper.CanBleed(health.RightArmHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.RightArmHealth - totaldamage > 0)
                    {
                        health.RightArmHealth -= totaldamage;
                        _database.UpdateRightArmHealth(player.Id, health.RightArmHealth);
                    }
                    else
                    {
                        health.RightArmHealth = 0;
                        _database.UpdateRightArmHealth(player.Id, health.RightArmHealth);
                    }

                    if (health.RightArmHealth + health.RightArmHealth == 0)
                    {
                        if (c.DieWhenArmsHealthIsZero)
                        {
                            cp.AllowDamage = true;
                            CSteamID id = CSteamID.Nil;
                            if (EDeathCause.ZOMBIE != cause)
                            {
                                if (killer != CSteamID.Nil)
                                    id = killer;
                            }
                            player.Player.life.askDamage(100, ragdoll, cause, limb, id, out EPlayerKill outKill);
                        }
                    }
                }
                else if (limb == ELimb.LEFT_LEG || limb == ELimb.LEFT_FOOT)
                {
                    if (HealthHelper.CanBleed(health.LeftLegHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.LeftLegHealth - totaldamage > 0)
                    {
                        health.LeftLegHealth -= totaldamage;
                        _database.UpdateLeftLegHealth(player.Id, health.LeftLegHealth);
                    }
                    else
                    {
                        health.LeftLegHealth = 0;
                        _database.UpdateLeftLegHealth(player.Id, health.LeftLegHealth);
                    }

                    if (health.LeftLegHealth + health.RightLegHealth == 0)
                    {
                        if (c.DieWhenLegsHealthIsZero)
                        {
                            cp.AllowDamage = true;
                            CSteamID id = CSteamID.Nil;
                            if (EDeathCause.ZOMBIE != cause)
                            {
                                if (killer != CSteamID.Nil)
                                    id = killer;
                            }
                            player.Player.life.askDamage(100, ragdoll, cause, limb, id, out EPlayerKill outKill);
                        }
                    }
                }
                else if (limb == ELimb.RIGHT_LEG || limb == ELimb.RIGHT_FOOT)
                {
                    if (HealthHelper.CanBleed(health.RightLegHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.RightLegHealth - totaldamage > 0)
                    {
                        health.RightLegHealth -= totaldamage;
                        _database.UpdateRightLegHealth(player.Id, health.RightLegHealth);
                    }
                    else
                    {
                        health.RightLegHealth = 0;
                        _database.UpdateRightLegHealth(player.Id, health.RightLegHealth);
                    }

                    if (health.RightLegHealth + health.RightLegHealth == 0)
                    {
                        if (c.DieWhenLegsHealthIsZero)
                        {
                            cp.AllowDamage = true;
                            CSteamID id = CSteamID.Nil;
                            if (EDeathCause.ZOMBIE != cause)
                            {
                                if (killer != CSteamID.Nil)
                                    id = killer;
                            }
                            player.Player.life.askDamage(100, ragdoll, cause, limb, id, out EPlayerKill outKill);
                        }
                    }
                }
                else
                {
                    if (HealthHelper.CanBleed(health.BodyHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.BodyHealth - totaldamage > 0)
                    {
                        health.BodyHealth -= totaldamage;
                        _database.UpdateBodyHealth(player.Id, health.BodyHealth);
                    }
                    else
                    {
                        health.BodyHealth = 0;
                        _database.UpdateBodyHealth(player.Id, health.BodyHealth);
                    }

                    if (health.BodyHealth == 0)
                    {
                        if (c.CanBeInjured && !health.IsInjured)
                        {
                            int chanc = MathHelper.Next(1, 100);
                            if (c.InjuredChance >= chanc)
                            {
                                HealthHelper.SetPlayerDowned(player);
                                return;
                            }
                        }

                        if (c.DieWhenBodyHealthIsZero)
                        {
                            cp.AllowDamage = true;
                            CSteamID id = CSteamID.Nil;
                            if (EDeathCause.ZOMBIE != cause)
                            {
                                if (killer != CSteamID.Nil)
                                    id = killer;
                            }
                            player.Player.life.askDamage(100, ragdoll, cause, limb, id, out EPlayerKill outKill);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerDamaged(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            string voidname = "PlayerDamaged";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(parameters.player);
                AdvancedHealthComponent cp = player.GetComponent<AdvancedHealthComponent>();
                var c = _config.HealthSystemSettings;
                var ag = _config.AntiGroupFriendlyFireSettings;
                bool allow = true;
                shouldAllow = false;

                /*if (parameters.killer != CSteamID.Nil)
                {
                    UnturnedPlayer killerPlayer = UnturnedPlayer.FromCSteamID(parameters.killer);
                    if (killerPlayer != null && _config.DefibrillatorSettings.Enabled)
                    {
                        AdvancedHealthComponent killerComp = killerPlayer.GetComponent<AdvancedHealthComponent>();
                        if (!_config.DefibrillatorSettings.EnablePermission || (_config.DefibrillatorSettings.EnablePermission && killerPlayer.HasPermission(_config.DefibrillatorSettings.PermissionForUseDefiblirator)))
                        {
                            Defibrillator defibrillator = _config.DefibrillatorSettings.DefibrillatorItems.Find(x => x.ItemID == killerPlayer.Player.equipment.ItemID);
                            if (defibrillator != null)
                            {
                                if (killerComp.LastDefibliratorUses.TryGetValue(defibrillator.ItemID, out DateTime value))
                                {
                                    if (value > DateTime.Now)
                                    {
                                        Helper.SendChatMessage(killerPlayer.SteamPlayer(), TAdvancedHealthMain.Instance.Translate(true, "error_defiblirator_cooldown", (value - DateTime.Now).TotalSeconds.ToString("0.00")));
                                        return;
                                    }
                                    killerComp.LastDefibliratorUses.Remove(defibrillator.ItemID);
                                }

                                int chance = MathHelper.Next(1, 100);
                                if (chance != 0 && chance <= defibrillator.ReviveChance)
                                    cp.ReviveAsync();
                                killerComp.LastDefibliratorUses.Add(defibrillator.ItemID, DateTime.Now.AddSeconds(defibrillator.RechargeTimeSecs));
                                return;
                            }
                        }
                    }
                }*/
                
                if (ag.EnableAntiGroupFriendlyFire)
                {
                    UnturnedPlayer victim = UnturnedPlayer.FromPlayer(parameters.player);
                    UnturnedPlayer attacker = UnturnedPlayer.FromCSteamID(parameters.killer);

                    if (victim != null && attacker != null)
                    {
                        if (attacker.CSteamID != victim.CSteamID)
                        {
                            EDeathCause cause2 = parameters.cause;
                            if (cause2 == EDeathCause.CHARGE || cause2 == EDeathCause.GRENADE || cause2 == EDeathCause.GUN || cause2 == EDeathCause.LANDMINE || cause2 == EDeathCause.MELEE || cause2 == EDeathCause.MISSILE || cause2 == EDeathCause.PUNCH || cause2 == EDeathCause.ROADKILL || cause2 == EDeathCause.SENTRY)
                            {
                                List<Permission> victimPerms = victim.GetPermissions();
                                List<Permission> attackerPerms = attacker.GetPermissions();

                                List<RocketPermissionsGroup> mutualGroups = PlayerHelper.GetMutualGroups(victim, attacker);
                                List<string> ffGroups = ag.groups;

                                for (int i = 0; i < mutualGroups.Count; i++)
                                {
                                    if (ffGroups.Contains(mutualGroups[i].Id.ToLower()))
                                    {
                                        shouldAllow = false;
                                        if (ag.MessageIcon != null)
                                        {

                                            ChatManager.serverSendMessage(ag.Message.Replace('{', '<').Replace('}', '>'), Color.white, null, attacker.SteamPlayer(), EChatMode.LOCAL, ag.MessageIcon, true);
                                        }
                                        else
                                        {
                                            UnturnedHelper.SendChatMessage(attacker.SteamPlayer(), ag.Message);
                                        }
                                        allow = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                
                player.Player.life.askHeal(100, false, false);
                HealthData health = _database.GetPlayerHealth(player.Id);
                float totaldamage = 0;
                
                EDeathCause cause = parameters.cause;
                ELimb limb = parameters.limb;
                CSteamID killer = parameters.killer;

                if (cause == EDeathCause.PUNCH)
                    parameters.damage = 1.0f;

                if (cause == EDeathCause.BONES)
                {
                    parameters.damage = 10.0f;
                    player.Broken = true;
                }

                if (EDeathCause.BLEEDING == cause)
                {
                    if (health.IsInjured && !cp.AllowDamage)
                    {
                        parameters.damage = 0;
                        player.Bleeding = false;
                    }
                    else
                    {
                        if (cp.hasHeavyBleeding)
                            parameters.damage = c.HeavyBleedingDamage;
                        else
                            parameters.damage = c.BleedingDamage;
                        player.Bleeding = true;
                    }
                }

                if (cause == EDeathCause.ANIMAL || cause == EDeathCause.ZOMBIE)
                {
                    limb = ELimb.LEFT_FRONT;
                }
                cp.AllowDamage = false;

                if (parameters.respectArmor)
                {
                    parameters.times *= DamageTool.getPlayerArmor(parameters.limb, parameters.player);
                    if (parameters.applyGlobalArmorMultiplier)
                        parameters.times *= Provider.modeConfigData.Players.Armor_Multiplier;
                    int b = Mathf.FloorToInt(parameters.damage * parameters.times);
                    totaldamage = Mathf.Min((int)byte.MaxValue, b);
                }
                else
                    totaldamage = parameters.times * parameters.damage;

                if (!allow || player.Features.GodMode)
                    totaldamage = 0;

                Vector3 ragdoll = player.Position.normalized;
                if (limb == ELimb.SKULL)
                {
                    if (HealthHelper.CanBleed(health.HeadHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.HeadHealth - totaldamage > 0)
                    {
                        health.HeadHealth -= totaldamage;
                        _database.UpdateHeadHealth(player.Id, health.HeadHealth);
                    }
                    else
                    {
                        health.HeadHealth = 0;
                        _database.UpdateHeadHealth(player.Id, health.HeadHealth);
                    }

                    if (health.HeadHealth == 0)
                    {
                        if (c.CanBeInjured && !health.IsInjured)
                        {
                            int chanc = MathHelper.Next(1, 100);
                            if (c.InjuredChance >= chanc)
                            {
                                HealthHelper.SetPlayerDowned(player);
                                return;
                            }
                        }

                        if (c.DieWhenHeadHealthIsZero)
                        {
                            cp.AllowDamage = true;
                            CSteamID id = CSteamID.Nil;
                            if (EDeathCause.ZOMBIE != cause)
                            {
                                if (killer != CSteamID.Nil)
                                    id = killer;
                            }
                            player.Player.life.askDamage(100, ragdoll, cause, limb, id, out EPlayerKill outKill);
                        }
                    }
                }
                else if (cause == EDeathCause.BONES)
                {
                    if (HealthHelper.CanBleed(health.LeftLegHealth, totaldamage) || HealthHelper.CanBleed(health.RightLegHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.RightLegHealth - totaldamage > 0)
                    {
                        health.RightLegHealth -= totaldamage;
                        _database.UpdateRightLegHealth(player.Id, health.RightLegHealth);
                    }
                    else
                    {
                        health.RightLegHealth = 0;
                        _database.UpdateRightLegHealth(player.Id, health.RightLegHealth);
                    }

                    if (health.RightLegHealth + health.RightLegHealth == 0)
                    {
                        if (c.DieWhenLegsHealthIsZero)
                        {
                            cp.AllowDamage = true;
                            CSteamID id = CSteamID.Nil;
                            if (EDeathCause.ZOMBIE != cause)
                            {
                                if (killer != CSteamID.Nil)
                                    id = killer;
                            }
                            player.Player.life.askDamage(100, ragdoll, cause, limb, id, out EPlayerKill outKill);
                        }
                    }

                    if (health.LeftLegHealth - totaldamage > 0)
                    {
                        health.LeftLegHealth -= totaldamage;
                        _database.UpdateLeftLegHealth(player.Id, health.LeftLegHealth);
                    }
                    else
                    {
                        health.LeftLegHealth = 0;
                        _database.UpdateLeftLegHealth(player.Id, health.LeftLegHealth);
                    }

                    if (health.LeftLegHealth + health.RightLegHealth == 0)
                    {
                        if (c.DieWhenLegsHealthIsZero)
                        {
                            cp.AllowDamage = true;
                            CSteamID id = CSteamID.Nil;
                            if (EDeathCause.ZOMBIE != cause)
                            {
                                if (killer != CSteamID.Nil)
                                    id = killer;
                            }
                            player.Player.life.askDamage(100, ragdoll, cause, limb, id, out EPlayerKill outKill);
                        }
                    }
                }
                else if (limb == ELimb.LEFT_BACK || limb == ELimb.LEFT_FRONT || limb == ELimb.RIGHT_BACK || limb == ELimb.RIGHT_FRONT || limb == ELimb.SPINE || cause == EDeathCause.INFECTION || cause == EDeathCause.BREATH)
                {
                    if (HealthHelper.CanBleed(health.BodyHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.BodyHealth - totaldamage > 0)
                    {
                        health.BodyHealth -= totaldamage;
                        _database.UpdateBodyHealth(player.Id, health.BodyHealth);
                    }
                    else
                    {
                        health.BodyHealth = 0;
                        _database.UpdateBodyHealth(player.Id, health.BodyHealth);
                    }

                    if (health.BodyHealth == 0)
                    {
                        if (c.CanBeInjured && !health.IsInjured)
                        {
                            int chanc = MathHelper.Next(1, 100);
                            if (c.InjuredChance >= chanc)
                            {
                                HealthHelper.SetPlayerDowned(player);
                                return;
                            }
                        }

                        if (c.DieWhenBodyHealthIsZero)
                        {
                            cp.AllowDamage = true;
                            CSteamID id = CSteamID.Nil;
                            if (EDeathCause.ZOMBIE != cause)
                            {
                                if (killer != CSteamID.Nil)
                                    id = killer;
                            }
                            player.Player.life.askDamage(100, ragdoll, cause, limb, id, out EPlayerKill outKill);
                        }
                    }
                }
                else if (limb == ELimb.LEFT_ARM || limb == ELimb.LEFT_HAND)
                {
                    if (HealthHelper.CanBleed(health.LeftArmHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.LeftArmHealth - totaldamage > 0)
                    {
                        health.LeftArmHealth -= totaldamage;
                        _database.UpdateLeftArmHealth(player.Id, health.LeftArmHealth);
                    }
                    else
                    {
                        health.LeftArmHealth = 0;
                        _database.UpdateLeftArmHealth(player.Id, health.LeftArmHealth);
                    }

                    if (health.LeftArmHealth + health.RightArmHealth == 0)
                    {
                        if (c.DieWhenArmsHealthIsZero)
                        {
                            cp.AllowDamage = true;
                            CSteamID id = CSteamID.Nil;
                            if (EDeathCause.ZOMBIE != cause)
                            {
                                if (killer != CSteamID.Nil)
                                    id = killer;
                            }
                            player.Player.life.askDamage(100, ragdoll, cause, limb, id, out EPlayerKill outKill);
                        }
                    }
                }
                else if (limb == ELimb.RIGHT_ARM || limb == ELimb.RIGHT_HAND)
                {
                    if (HealthHelper.CanBleed(health.RightArmHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.RightArmHealth - totaldamage > 0)
                    {
                        health.RightArmHealth -= totaldamage;
                        _database.UpdateRightArmHealth(player.Id, health.RightArmHealth);
                    }
                    else
                    {
                        health.RightArmHealth = 0;
                        _database.UpdateRightArmHealth(player.Id, health.RightArmHealth);
                    }

                    if (health.RightArmHealth + health.RightArmHealth == 0)
                    {
                        if (c.DieWhenArmsHealthIsZero)
                        {
                            cp.AllowDamage = true;
                            CSteamID id = CSteamID.Nil;
                            if (EDeathCause.ZOMBIE != cause)
                            {
                                if (killer != CSteamID.Nil)
                                    id = killer;
                            }
                            player.Player.life.askDamage(100, ragdoll, cause, limb, id, out EPlayerKill outKill);
                        }
                    }
                }
                else if (limb == ELimb.LEFT_LEG || limb == ELimb.LEFT_FOOT)
                {
                    if (HealthHelper.CanBleed(health.LeftLegHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.LeftLegHealth - totaldamage > 0)
                    {
                        health.LeftLegHealth -= totaldamage;
                        _database.UpdateLeftLegHealth(player.Id, health.LeftLegHealth);
                    }
                    else
                    {
                        health.LeftLegHealth = 0;
                        _database.UpdateLeftLegHealth(player.Id, health.LeftLegHealth);
                    }

                    if (health.LeftLegHealth + health.RightLegHealth == 0)
                    {
                        if (c.DieWhenLegsHealthIsZero)
                        {
                            cp.AllowDamage = true;
                            CSteamID id = CSteamID.Nil;
                            if (EDeathCause.ZOMBIE != cause)
                            {
                                if (killer != CSteamID.Nil)
                                    id = killer;
                            }
                            player.Player.life.askDamage(100, ragdoll, cause, limb, id, out EPlayerKill outKill);
                        }
                    }
                }
                else if (limb == ELimb.RIGHT_LEG || limb == ELimb.RIGHT_FOOT)
                {
                    if (HealthHelper.CanBleed(health.RightLegHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.RightLegHealth - totaldamage > 0)
                    {
                        health.RightLegHealth -= totaldamage;
                        _database.UpdateRightLegHealth(player.Id, health.RightLegHealth);
                    }
                    else
                    {
                        health.RightLegHealth = 0;
                        _database.UpdateRightLegHealth(player.Id, health.RightLegHealth);
                    }

                    if (health.RightLegHealth + health.RightLegHealth == 0)
                    {
                        if (c.DieWhenLegsHealthIsZero)
                        {
                            cp.AllowDamage = true;
                            CSteamID id = CSteamID.Nil;
                            if (EDeathCause.ZOMBIE != cause)
                            {
                                if (killer != CSteamID.Nil)
                                    id = killer;
                            }
                            player.Player.life.askDamage(100, ragdoll, cause, limb, id, out EPlayerKill outKill);
                        }
                    }
                }
                else
                {
                    if (HealthHelper.CanBleed(health.BodyHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.BodyHealth - totaldamage > 0)
                    {
                        health.BodyHealth -= totaldamage;
                        _database.UpdateBodyHealth(player.Id, health.BodyHealth);
                    }
                    else
                    {
                        health.BodyHealth = 0;
                        _database.UpdateBodyHealth(player.Id, health.BodyHealth);
                    }

                    if (health.BodyHealth == 0)
                    {
                        if (c.CanBeInjured && !health.IsInjured)
                        {
                            int chanc = MathHelper.Next(1, 100);
                            if (c.InjuredChance >= chanc)
                            {
                                HealthHelper.SetPlayerDowned(player);
                                return;
                            }
                        }

                        if (c.DieWhenBodyHealthIsZero)
                        {
                            cp.AllowDamage = true;
                            CSteamID id = CSteamID.Nil;
                            if (EDeathCause.ZOMBIE != cause)
                            {
                                if (killer != CSteamID.Nil)
                                    id = killer;
                            }
                            player.Player.life.askDamage(100, ragdoll, cause, limb, id, out EPlayerKill outKill);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerGestureUpdated(UnturnedPlayer player, UnturnedPlayerEvents.PlayerGesture gesture)
        {
            string voidname = "Gesture";
            try
            {
                if (gesture == UnturnedPlayerEvents.PlayerGesture.SurrenderStart)
                {
                    PlayerLook look = player.Player.look;

                    Player victimPlayer = null;
                    if (Physics.Raycast(new Ray(look.aim.position, look.aim.forward), out RaycastHit hit, 2f, RayMasks.PLAYER))
                    {
                        Player victimPlayer2 = hit.transform.GetComponent<Player>();
                        if (victimPlayer2 != null && Vector3.Distance(victimPlayer2.transform.position, player.Position) <= 5f)
                            victimPlayer = victimPlayer2;
                    }

                    if (victimPlayer != null)
                    {
                        UnturnedPlayer targetPlayer = UnturnedPlayer.FromPlayer(victimPlayer);
                        AdvancedHealthComponent playerComp = player.GetComponent<AdvancedHealthComponent>();
                        playerComp.DragAsync(targetPlayer);
                    }
                }
                else if (gesture == UnturnedPlayerEvents.PlayerGesture.SurrenderStop)
                {
                    AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                    if (comp.DragState == EDragState.DRAGGER)
                        comp.UnDrag();
                }
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerRevived(UnturnedPlayer player, Vector3 position, byte angle)
        {
            string voidname = "OnRevivedPlayer";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                comp.ReviveAsync();
                if (comp.DragState != EDragState.NONE)
                    comp.UnDrag();


                EffectManager.sendUIEffectVisibility((short)comp.EffectID, comp.TranspConnection, true, "RevivePanel", false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);

                TAdvancedHealth.Instance.StartCoroutine(TAdvancedHealth.Instance.DelayedInvoke(0.1f, () =>
                {
                    if (_config.HospitalSettings.EnableRespawnInHospital)
                    {
                        if (_config.HospitalSettings.hospitals != null)
                            if (_config.HospitalSettings.RandomSpawn)
                            {
                                int i = MathHelper.Next(0, _config.HospitalSettings.hospitals.Count - 1);
                                Hospital h = _config.HospitalSettings.hospitals.ElementAt(i);
                                if (h.Position != null)
                                {
                                    i = MathHelper.Next(0, h.Position.Count - 1);
                                    Vector3 p = h.Position.ElementAt(i).GetVector3();
                                    player.Teleport(p, player.Rotation);
                                }
                            }
                            else
                            {
                                Hospital hospital = _config.HospitalSettings.hospitals.FirstOrDefault(x => player.HasPermission(x.SpawnPermission.ToLower()));
                                if (hospital != null)
                                    if (hospital.Position != null)
                                    {
                                        int index = MathHelper.Next(0, hospital.Position.Count - 1);
                                        Vector3 hPosition = hospital.Position.ElementAt(index).GetVector3();
                                        player.Teleport(hPosition, player.Rotation);
                                    }
                            }
                    }

                    Event_OnPlayerFoodUpdate(player, player.Player.life.food);
                    Event_OnPlayerWaterUpdate(player, player.Player.life.water);
                    Event_OnPlayerVirusUpdate(player, player.Player.life.virus);
                    Event_OnPlayerOxygenUpdate(player, player.Player.life.oxygen);
                    Event_OnPlayerStaminaUpdate(player, player.Player.life.stamina);
                    Event_OnPlayerBleedingUpdate(player, player.Bleeding);
                    Event_OnPlayerBrokenUpdate(player, player.Broken);
                    Event_OnPlayerSafezoneUpdated(player, player.Player.movement.isSafe);
                    Event_OnPlayerDeadzoneUpdated(player, player.Player.movement.isRadiated);
                    Event_OnPlayerTemperatureUpdate(player, player.Player.life.temperature);
                }));
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            string voidname = "OnDeath";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                HealthHelper.UpdateHealthUI(player);
                if (comp.DragState != EDragState.NONE)
                    comp.UnDrag();

                EffectManager.sendUIEffectVisibility((short)comp.EffectID, comp.TranspConnection, true, "RevivePanel", false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnButtonClickded(Player Player, string buttonName)
        {
            string voidname = "OnButtonClicked";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(Player);
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                if (buttonName == "bt_suicide" || buttonName == "bt_suicide2")
                {
                    HealthData health = _database.GetPlayerHealth(player.Id);
                    if (health.IsInjured)
                    {
                        comp.AllowDamage = true;
                        player.Player.life.askDamage(100, player.Position.normalized, EDeathCause.BLEEDING, ELimb.SKULL, CSteamID.Nil, out EPlayerKill outKill);

                        if (player.Player.movement.pluginSpeedMultiplier == 0)
                            player.Player.movement.sendPluginSpeedMultiplier(1);
                        health.IsInjured = false;
                        if (comp.DragState != EDragState.NONE)
                            comp.UnDrag();

                        player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                        EffectManager.sendUIEffectVisibility((short)comp.EffectID, comp.TranspConnection, true, "RevivePanel", false);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }
    }

}