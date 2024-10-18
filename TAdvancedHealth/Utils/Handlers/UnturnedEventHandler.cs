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
using System.Threading.Tasks;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TAdvancedHealth.Models.Database;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TAdvancedHealth.Utils.Helpers;
using Tavstal.TAdvancedHealth.Utils.Managers;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Helpers.Unturned;
using UnityEngine;

namespace Tavstal.TAdvancedHealth.Utils.Handlers
{
    public static class UnturnedEventHandler
    {
        private static DatabaseManager _database => TAdvancedHealth.DatabaseManager;
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
                await EffectHelper.SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Health_Simple, (int)((Math.Round(health.BaseHealth, 2) / _config.HealthSystemSettings.BaseHealth) * 100), 0);
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
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
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                await EffectHelper.SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Food, player.Player.life.food, (int)comp.ProgressBarData.LastFood);
                comp.ProgressBarData.LastFood = value;

                if (value <= 0)
                   await comp.TryAddStateAsync(EPlayerState.NOFOOD);
                else
                   await comp.TryRemoveStateAsync(EPlayerState.NOFOOD);
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
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
                await EffectHelper.SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Stamina, player.Player.life.stamina, (int)comp.ProgressBarData.LastStamina);
                comp.ProgressBarData.LastStamina = value;
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
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
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                await EffectHelper.SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Water, player.Player.life.water, (int)comp.ProgressBarData.LastWater);
                comp.ProgressBarData.LastWater = value;

                if (value <= 0)
                   await comp.TryAddStateAsync(EPlayerState.NOWATER);
                else
                   await comp.TryRemoveStateAsync(EPlayerState.NOWATER);
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
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
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                await EffectHelper.SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Radiation, player.Player.life.virus, (int)comp.ProgressBarData.LastVirus);
                comp.ProgressBarData.LastVirus = value;

                if (value <= 0)
                   await comp.TryAddStateAsync(EPlayerState.NOVIRUS);
                else
                   await comp.TryRemoveStateAsync(EPlayerState.NOVIRUS);
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
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
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                await EffectHelper.SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Oxygen, player.Player.life.oxygen, (int)comp.ProgressBarData.LastOxygen);
                comp.ProgressBarData.LastOxygen = value;

                if (value <= 0)
                   await comp.TryAddStateAsync(EPlayerState.NOOXYGEN);
                else
                   await comp.TryRemoveStateAsync(EPlayerState.NOOXYGEN);
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
            }
        }

        /// <summary>
        /// Event handler for updates to the bleeding state of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose bleeding state is being updated.</param>
        /// <param name="state">A boolean indicating whether the player is bleeding.</param>
        private static async void Event_OnPlayerBleedingUpdate(UnturnedPlayer player, bool state)
        {
            string voidname = "OnPlayerBleedingUpdate";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                if (state)
                {
                    if (!_config.HealthSystemSettings.CanStartBleeding)
                    {
                        player.Bleeding = false;
                        return;
                    }

                    if (MathHelper.Next(1, 100) <= _config.HealthSystemSettings.HeavyBleedingChance)
                        comp.HasHeavyBleeding = true;

                    await comp.TryAddStateAsync(EPlayerState.BLEEDING);
                }
                else
                {
                    comp.HasHeavyBleeding = false;
                    await comp.TryRemoveStateAsync(EPlayerState.BLEEDING);
                }

            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
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
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                HealthData health = await _database.GetPlayerHealthAsync(player.Id);
                if (state)
                {
                    if (_config.HealthSystemSettings.CanHavePainEffect)
                    {
                        int painChance = MathHelper.Next(1, 100);

                        if (painChance <= _config.HealthSystemSettings.PainEffectChance)
                        {
                            EffectManager.sendUIEffect(_config.HealthSystemSettings.PainEffectID, (short)_config.HealthSystemSettings.PainEffectID, comp.TranspConnection, true);
                            if (_config.HealthSystemSettings.PainEffectDuration > 0)
                                TAdvancedHealth.Instance.InvokeAction(_config.HealthSystemSettings.PainEffectDuration, () => { EffectManager.askEffectClearByID(_config.HealthSystemSettings.PainEffectID, player.SteamPlayer().transportConnection); });

                        }
                    }

                    if (health.LeftLegHealth == 0 && health.RightLegHealth == 0)
                    {
                        if (!_config.HealthSystemSettings.CanWalkWithBrokenLegs)
                            player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                    }
                    else if (health.LeftLegHealth == 0 || health.RightLegHealth == 0)
                    {
                        if (!_config.HealthSystemSettings.CanWalkWithOneBrokenLeg)
                            player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                    }

                    await comp.TryAddStateAsync(EPlayerState.BROKENBONE);
                }
                else
                   await comp.TryRemoveStateAsync(EPlayerState.BROKENBONE);

            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
            }
        }

        /// <summary>
        /// Event handler for updates to the moon state.
        /// </summary>
        /// <param name="isFullMoon">A boolean indicating whether the moon is in a full state.</param>
        internal static async void Event_OnMoonUpdated(bool isFullMoon)
        {
            string voidname = "OnMoonUpdated";
            try
            {
                foreach (SteamPlayer steamPlayer in Provider.clients)
                {
                    UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                    AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                    if (isFullMoon)
                       await comp.TryAddStateAsync(EPlayerState.FULLMOON);
                    else
                       await comp.TryRemoveStateAsync(EPlayerState.FULLMOON);
                }

            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
            }
        }

        /// <summary>
        /// Event handler for updates to the deadzone status of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose deadzone status is being updated.</param>
        /// <param name="isActive">A boolean indicating whether the player is currently in a deadzone.</param>
        private static async void Event_OnPlayerDeadzoneUpdated(UnturnedPlayer player, bool isActive)
        {
            string voidname = "OnPlayerDeadzoneUpdated";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                if (isActive)
                   await comp.TryAddStateAsync(EPlayerState.DEADZONE);
                else
                   await comp.TryRemoveStateAsync(EPlayerState.DEADZONE);

            }
            catch (Exception e)
            {
               TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
            }
        }

        /// <summary>
        /// Event handler for updates to the safezone status of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose safezone status is being updated.</param>
        /// <param name="isActive">A boolean indicating whether the player is currently in a safezone.</param>
        private static async void Event_OnPlayerSafezoneUpdated(UnturnedPlayer player, bool isActive)
        {
            string voidname = "OnPlayerSafezoneUpdated";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                if (isActive)
                    await comp.TryAddStateAsync(EPlayerState.SAFEZONE);
                else
                   await comp.TryRemoveStateAsync(EPlayerState.SAFEZONE);

            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
            }
        }

        /// <summary>
        /// Event handler for updates to the temperature of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose temperature is being updated.</param>
        /// <param name="newTemperature">The new temperature state of the player.</param>
        private static async void Event_OnPlayerTemperatureUpdate(UnturnedPlayer player, EPlayerTemperature newTemperature)
        {
            string voidname = "OnPlayerTemperatureUpdate";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                EPlayerState state = EPlayerState.ACID;

                if (newTemperature == EPlayerTemperature.ACID)
                    state = EPlayerState.ACID;
                else if (newTemperature == EPlayerTemperature.BURNING)
                    state = EPlayerState.BURNING;
                else if (newTemperature == EPlayerTemperature.COLD)
                    state = EPlayerState.COLD;
                else if (newTemperature == EPlayerTemperature.COVERED)
                    state = EPlayerState.COVERED;
                else if (newTemperature == EPlayerTemperature.FREEZING)
                    state = EPlayerState.FREEZING;
                else if (newTemperature == EPlayerTemperature.WARM)
                    state = EPlayerState.WARM;
                else if (newTemperature == EPlayerTemperature.NONE)
                    state = EPlayerState.NONE_TEMPERATURE;

                await comp.TryAddStateAsync(state);

            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
            }
        }

        /// <summary>
        /// Event handler for updates to the stance of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose stance is being updated.</param>
        /// <param name="stance">The new stance value of the player.</param>
        private static async void Event_OnPlayerStanceUpdate(UnturnedPlayer player, byte stance)
        {
            string voidname = "OnPlayerStanceUpdate";
            try
            {
                HealthData health = await _database.GetPlayerHealthAsync(player.Id);
                if (health.IsInjured)
                    player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                else
                {
                    if (health.LeftLegHealth == 0 || health.RightLegHealth == 0)
                    {
                        if (!_config.HealthSystemSettings.CanWalkWithOneBrokenLeg)
                            player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                        else if (!_config.HealthSystemSettings.CanWalkWithBrokenLegs && health.LeftLegHealth == 0 && health.RightLegHealth == 0)
                            player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                    }
                }
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
            }
        }
        #endregion
        #region Item Events
        /// <summary>
        /// Event handler for when a player requests to equip an item.
        /// </summary>
        /// <param name="equipment">The player's equipment manager.</param>
        /// <param name="jar">The item jar containing the item being equipped.</param>
        /// <param name="asset">The asset of the item being equipped.</param>
        /// <param name="shouldAllow">A reference boolean indicating whether the player should be allowed to equip the item.</param>
        private static void Event_OnPlayerEquipRequested(PlayerEquipment equipment, ItemJar jar, ItemAsset asset, ref bool shouldAllow)
        {
            string voidname = "OnPlayerEquipRequested";
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

                HealthData healthData = _database.GetPlayerHealth(player.Id);
                if (healthData.RightArmHealth == 0 && healthData.LeftArmHealth == 0)
                {
                    if (!isMedicine)
                    {
                        if (!_config.HealthSystemSettings.CanHoldOneHandItemsWithBrokenArms)
                            if (_config.OneHandedItems.ItemID.Contains(jar.item.id) || _config.OneHandedItems.ItemTypes.Contains(asset.type))
                            {
                                shouldAllow = false;
                                if (player.Player.equipment.itemID != 0)
                                    player.Player.equipment.dequip();
                            }
                        if (!_config.HealthSystemSettings.CanHoldTwoHandItemsWithBrokenArms)
                            if (_config.TwoHandedItems.ItemID.Contains(jar.item.id) || _config.TwoHandedItems.ItemTypes.Contains(asset.type))
                            {
                                shouldAllow = false;
                                if (player.Player.equipment.itemID != 0)
                                    player.Player.equipment.dequip();
                            }
                    }
                }
                else if (healthData.RightArmHealth == 0 || healthData.LeftArmHealth == 0)
                    if (!isMedicine)
                    {
                        if (!_config.HealthSystemSettings.CanHoldOneHandItemsWithOneBrokenArm)
                            if (_config.OneHandedItems.ItemID.Contains(jar.item.id) || _config.OneHandedItems.ItemTypes.Contains(asset.type))
                            {
                                shouldAllow = false;
                                if (player.Player.equipment.itemID != 0)
                                    player.Player.equipment.dequip();
                            }
                        if (!_config.HealthSystemSettings.CanHoldTwoHandItemsWithOneBrokenArm)
                            if (_config.TwoHandedItems.ItemID.Contains(jar.item.id) || _config.TwoHandedItems.ItemTypes.Contains(asset.type))
                            {
                                shouldAllow = false;
                                if (player.Player.equipment.itemID != 0)
                                    player.Player.equipment.dequip();
                            }
                    }
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
            }
        }

        /// <summary>
        /// Event handler for when a player requests to dequip an item.
        /// </summary>
        /// <param name="equipment">The player's equipment manager.</param>
        /// <param name="shouldAllow">A reference boolean indicating whether the player should be allowed to dequip the item.</param>
        private static void Event_OnPlayerDequipRequested(PlayerEquipment equipment, ref bool shouldAllow)
        {
            string voidname = "OnPlayerDequipRequested";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(equipment.player);
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                comp.LastEquipedItem = 0;
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
            }
        }

        /// <summary>
        /// Event handler for when a player uses medicine.
        /// </summary>
        /// <param name="instigatingPlayer">The player who initiated the use of medicine.</param>
        /// <param name="consumeableAsset">The consumable asset representing the medicine being used.</param>
        private static async void Event_OnPlayerUseMedicine(Player instigatingPlayer, ItemConsumeableAsset consumeableAsset)
        {
            string voidname = "OnPlayerUseMedicine";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(instigatingPlayer);

                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                if (comp.LastEquipedItem == 0) { return; }

                if (_config.Medicines != null)
                {
                    Medicine med = _config.Medicines.FirstOrDefault(x => x.ItemID == comp.LastEquipedItem);
                    if (med != null)
                    {
                        HealthData health = await _database.GetPlayerHealthAsync(player.Id);
                        if (health.BodyHealth + med.HealsBodyHP <= _config.HealthSystemSettings.BodyHealth)
                            health.BodyHealth += med.HealsBodyHP;
                        else
                            health.BodyHealth = _config.HealthSystemSettings.BodyHealth;

                        if (health.HeadHealth + med.HealsHeadHP <= _config.HealthSystemSettings.HeadHealth)
                            health.HeadHealth += med.HealsHeadHP;
                        else
                            health.HeadHealth = _config.HealthSystemSettings.HeadHealth;

                        if (med.CuresPain)
                            EffectManager.askEffectClearByID(_config.HealthSystemSettings.PainEffectID, player.SteamPlayer().transportConnection);

                        if (health.LeftLegHealth + med.HealsLeftLegHP <= _config.HealthSystemSettings.LeftLegHealth)
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
                            health.LeftLegHealth = _config.HealthSystemSettings.LeftLegHealth;
                            TAdvancedHealth.Instance.InvokeAction(0.5f, () =>
                            {
                                player.Broken = false;
                                player.Player.movement.sendPluginJumpMultiplier(1f);
                            });
                        }

                        if (health.RightLegHealth + med.HealsRightLegHP <= _config.HealthSystemSettings.RightLegHealth)
                        {
                            health.RightLegHealth += med.HealsRightLegHP;
                            TAdvancedHealth.Instance.InvokeAction(0.5f, () =>
                            {
                                player.Broken = false;
                                player.Player.movement.sendPluginJumpMultiplier(1f);
                            });
                        }
                        else
                        {
                            health.RightLegHealth = _config.HealthSystemSettings.RightLegHealth;
                            TAdvancedHealth.Instance.InvokeAction(0.5f, () =>
                            {
                                player.Broken = false;
                                player.Player.movement.sendPluginJumpMultiplier(1f);
                            });
                        }

                        if (health.LeftArmHealth + med.HealsLeftArmHP <= _config.HealthSystemSettings.LeftArmHealth)
                            health.LeftArmHealth += med.HealsLeftArmHP;
                        else
                            health.LeftArmHealth = _config.HealthSystemSettings.LeftArmHealth;

                        if (health.RightArmHealth + med.HealsRightArmHP <= _config.HealthSystemSettings.RightArmHealth)
                            health.RightArmHealth += med.HealsRightArmHP;
                        else
                            health.RightArmHealth = _config.HealthSystemSettings.RightArmHealth;

                        await _database.UpdateHealthAsync(player.Id, health);
                    }
                    comp.LastEquipedItem = 0;
                }
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
            }
        }
        #endregion
        #region Vehicle Events
        /// <summary>
        /// Event handler for when a player requests to enter a vehicle.
        /// </summary>
        /// <param name="p">The player who requested to enter the vehicle.</param>
        /// <param name="vehicle">The vehicle that the player is trying to enter.</param>
        /// <param name="shouldAllow">A reference boolean indicating whether the player should be allowed to enter the vehicle.</param>
        private static void Event_OnPlayerVehicleEnterRequested(Player p, InteractableVehicle vehicle, ref bool shouldAllow)
        {
            string voidname = "OnPlayerVehicleEnterRequested";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(p);
                HealthData health = _database.GetPlayerHealth(player.Id);
                if (vehicle.passengers[0].player == null)
                {
                    if (health.LeftLegHealth == 0 && health.RightLegHealth == 0 || player.Broken)
                    {
                        if (!_config.HealthSystemSettings.CanDriveWithBrokenLegs)
                            shouldAllow = false;
                    }
                    else if (health.LeftLegHealth == 0 || health.RightLegHealth == 0)
                        if (!_config.HealthSystemSettings.CanDriveWithOneBrokenLeg)
                            shouldAllow = false;

                    if (health.LeftArmHealth == 0 && health.RightArmHealth == 0 || player.Broken)
                    {
                        if (!_config.HealthSystemSettings.CanDriveWithBrokenArms)
                            shouldAllow = false;
                    }
                    else if (health.LeftArmHealth == 0 || health.RightArmHealth == 0)
                        if (!_config.HealthSystemSettings.CanDriveWithOneBrokenLeg)
                            shouldAllow = false;
                }
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
            }
        }

        /// <summary>
        /// Event handler for when a player requests to swap seats in a vehicle.
        /// </summary>
        /// <param name="p">The player who requested to swap seats.</param>
        /// <param name="vehicle">The vehicle in which the seat swap is requested.</param>
        /// <param name="shouldAllow">A reference boolean indicating whether the player should be allowed to swap seats.</param>
        /// <param name="fromSeatIndex">The index of the seat the player is swapping from.</param>
        /// <param name="toSeatIndex">A reference to the index of the seat the player is swapping to.</param>
        private static void Event_OnPlayerSwapSeatRequested(Player p, InteractableVehicle vehicle, ref bool shouldAllow, byte fromSeatIndex, ref byte toSeatIndex)
        {
            string voidname = "SwapSeat";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(p);
                HealthData healthData = _database.GetPlayerHealth(player.Id);
                if (healthData.LeftLegHealth == 0 && healthData.RightLegHealth == 0 || player.Broken)
                {
                    if (toSeatIndex == 0 && !_config.HealthSystemSettings.CanDriveWithBrokenLegs)
                        shouldAllow = false;
                }
                else if (healthData.LeftLegHealth == 0 || healthData.RightLegHealth == 0)
                    if (toSeatIndex == 0 && !_config.HealthSystemSettings.CanDriveWithOneBrokenLeg)
                        shouldAllow = false;

                if (healthData.LeftArmHealth == 0 && healthData.RightArmHealth == 0 || player.Broken)
                {
                    if (!_config.HealthSystemSettings.CanDriveWithBrokenArms && toSeatIndex == 0)
                        shouldAllow = false;
                }
                else if (healthData.LeftArmHealth == 0 || healthData.RightArmHealth == 0)
                    if (!_config.HealthSystemSettings.CanDriveWithOneBrokenLeg && toSeatIndex == 0)
                        shouldAllow = false;
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
            }
        }
        #endregion
        #region Player Events
        #region General
        /// <summary>
        /// Event handler for when a player joins the server.
        /// </summary>
        /// <param name="player">The Unturned player who joined the server.</param>
        private static async void Event_OnPlayerJoin(UnturnedPlayer player)
        {
            string voidname = "OnPlayerJoin";
            try
            {
                #region Check Health Data
                HealthData health = await _database.GetPlayerHealthAsync(player.Id);
                if (health == null)
                {
                    HUDStyle style = _config.HUDStyles.FirstOrDefault(x => x.Enabled);
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
                comp.EffectID = health.HUDEffectID;
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


                if (LightingManager.isFullMoon)
                    await comp.TryAddStateAsync(EPlayerState.FULLMOON);
                #endregion
                #region Attach Events
                player.Player.equipment.onEquipRequested += Event_OnPlayerEquipRequested;
                player.Player.equipment.onDequipRequested += Event_OnPlayerDequipRequested;
                player.Player.life.onHurt += Event_OnPlayerLifeDamaged;
                player.Player.life.onOxygenUpdated += b => Event_OnPlayerOxygenUpdate(player, b);
                player.Player.life.onTemperatureUpdated += newTemperature => Event_OnPlayerTemperatureUpdate(player, newTemperature);
                player.Player.movement.onSafetyUpdated += isSafe => Event_OnPlayerSafezoneUpdated(player, isSafe);
                player.Player.movement.onRadiationUpdated += isRadio => Event_OnPlayerDeadzoneUpdated(player, isRadio);
                player.Player.life.onVirusUpdated += virus => Event_OnPlayerVirusUpdate(player, virus);
                #endregion
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
                    await EffectHelper.UpdateWholeHealthUIAsync(player);
                }
                #endregion
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
            }
        }

        /// <summary>
        /// Event handler for when a player leaves the server.
        /// </summary>
        /// <param name="player">The Unturned player who left the server.</param>
        private static void Event_OnPlayerLeave(UnturnedPlayer player)
        {
            string voidname = "OnPlayerLeave";
            try
            {
                #region Dettach Events
                player.Player.equipment.onEquipRequested -= Event_OnPlayerEquipRequested;
                player.Player.equipment.onDequipRequested -= Event_OnPlayerDequipRequested;
                player.Player.life.onHurt -= Event_OnPlayerLifeDamaged;
                player.Player.life.onOxygenUpdated -= b => Event_OnPlayerOxygenUpdate(player, b);
                player.Player.life.onTemperatureUpdated -= newTemperature => Event_OnPlayerTemperatureUpdate(player, newTemperature);
                player.Player.movement.onSafetyUpdated -= isSafe => Event_OnPlayerSafezoneUpdated(player, isSafe);
                player.Player.movement.onRadiationUpdated -= isRadio => Event_OnPlayerDeadzoneUpdated(player, isRadio);
                player.Player.life.onVirusUpdated -= virus => Event_OnPlayerVirusUpdate(player, virus);
                #endregion

                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                if (comp.DragState != EDragState.NONE)
                    comp.UnDrag();
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
            }
        }
        #endregion
        #region Life Events
        /// <summary>
        /// Handles incoming damage inflicted on a player.
        /// </summary>
        /// <param name="player">The player receiving the damage.</param>
        /// <param name="health">The health data of the player.</param>
        /// <param name="killer">The Steam ID of the entity responsible for the damage.</param>
        /// <param name="totalDamage">The total amount of damage inflicted on the player.</param>
        /// <param name="limb">The limb affected by the damage.</param>
        /// <param name="cause">The cause of death or damage.</param>
        /// <param name="ragdoll">The position of the ragdoll after the damage.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task HandleIncomingDamageAsync(UnturnedPlayer player, HealthData health, CSteamID killer, float totalDamage, ELimb limb, EDeathCause cause, Vector3 ragdoll)
        {
            AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
            switch (limb)
            {
                // HEAD
                case ELimb.SKULL:
                    {

                        if (HealthHelper.CanBleed(health.HeadHealth, totalDamage))
                            player.Bleeding = true;

                        if (health.HeadHealth - totalDamage > 0)
                        {
                            health.HeadHealth -= totalDamage;
                            await _database.UpdateHealthAsync(player.Id, health.HeadHealth, EHealth.HEAD);
                        }
                        else
                        {
                            health.HeadHealth = 0;
                            await _database.UpdateHealthAsync(player.Id, 0, EHealth.HEAD);
                        }

                        if (health.HeadHealth == 0)
                        {
                            if (_config.HealthSystemSettings.CanBeInjured && !health.IsInjured)
                            {
                                int chanc = MathHelper.Next(1, 100);
                                if (_config.HealthSystemSettings.InjuredChance >= chanc)
                                {
                                    await HealthHelper.SetPlayerDownedAsync(player);
                                    return;
                                }
                            }

                            if (_config.HealthSystemSettings.DieWhenHeadHealthIsZero)
                            {
                                comp.AllowDamage = true;
                                CSteamID id = CSteamID.Nil;
                                if (EDeathCause.ZOMBIE != cause)
                                {
                                    if (killer != CSteamID.Nil)
                                        id = killer;
                                }
                                player.Player.life.askDamage(100, ragdoll, cause, limb, id, out _);
                            }
                        }
                        break;
                    }
                // BODY
                case ELimb.LEFT_BACK:
                case ELimb.LEFT_FRONT:
                case ELimb.RIGHT_BACK:
                case ELimb.RIGHT_FRONT:
                case ELimb.SPINE:
                    {
                        if (HealthHelper.CanBleed(health.BodyHealth, totalDamage))
                            player.Bleeding = true;

                        if (health.BodyHealth - totalDamage > 0)
                        {
                            health.BodyHealth -= totalDamage;
                            await _database.UpdateHealthAsync(player.Id, health.BodyHealth, EHealth.BODY);
                        }
                        else
                        {
                            health.BodyHealth = 0;
                            await _database.UpdateHealthAsync(player.Id, health.BodyHealth, EHealth.BODY);
                        }

                        if (health.BodyHealth == 0)
                        {
                            if (_config.HealthSystemSettings.CanBeInjured && !health.IsInjured)
                            {
                                int chanc = MathHelper.Next(1, 100);
                                if (_config.HealthSystemSettings.InjuredChance >= chanc)
                                {
                                    await HealthHelper.SetPlayerDownedAsync(player);
                                    return;
                                }
                            }

                            if (_config.HealthSystemSettings.DieWhenBodyHealthIsZero)
                            {
                                comp.AllowDamage = true;
                                CSteamID id = CSteamID.Nil;
                                if (EDeathCause.ZOMBIE != cause)
                                {
                                    if (killer != CSteamID.Nil)
                                        id = killer;
                                }
                                player.Player.life.askDamage(100, ragdoll, cause, limb, id, out _);
                            }
                        }
                        break;
                    }
                // LEFT ARM
                case ELimb.LEFT_ARM:
                case ELimb.LEFT_HAND:
                    {
                        if (HealthHelper.CanBleed(health.LeftArmHealth, totalDamage))
                            player.Bleeding = true;

                        if (health.LeftArmHealth - totalDamage > 0)
                        {
                            health.LeftArmHealth -= totalDamage;
                            await _database.UpdateHealthAsync(player.Id, health.LeftArmHealth, EHealth.LEFT_ARM);
                        }
                        else
                        {
                            health.LeftArmHealth = 0;
                            await _database.UpdateHealthAsync(player.Id, health.LeftArmHealth, EHealth.LEFT_ARM);
                        }

                        if (health.LeftArmHealth + health.RightArmHealth == 0)
                        {
                            if (_config.HealthSystemSettings.DieWhenArmsHealthIsZero)
                            {
                                comp.AllowDamage = true;
                                CSteamID id = CSteamID.Nil;
                                if (EDeathCause.ZOMBIE != cause)
                                {
                                    if (killer != CSteamID.Nil)
                                        id = killer;
                                }
                                player.Player.life.askDamage(100, ragdoll, cause, limb, id, out _);
                            }
                        }
                        break;
                    }
                // RIGHT ARM
                case ELimb.RIGHT_ARM:
                case ELimb.RIGHT_HAND:
                    {
                        if (HealthHelper.CanBleed(health.RightArmHealth, totalDamage))
                            player.Bleeding = true;

                        if (health.RightArmHealth - totalDamage > 0)
                        {
                            health.RightArmHealth -= totalDamage;
                            await _database.UpdateHealthAsync(player.Id, health.RightArmHealth, EHealth.RIGHT_ARM);
                        }
                        else
                        {
                            health.RightArmHealth = 0;
                            await _database.UpdateHealthAsync(player.Id, health.RightArmHealth, EHealth.RIGHT_ARM);
                        }

                        if (health.RightArmHealth + health.RightArmHealth == 0)
                        {
                            if (_config.HealthSystemSettings.DieWhenArmsHealthIsZero)
                            {
                                comp.AllowDamage = true;
                                CSteamID id = CSteamID.Nil;
                                if (EDeathCause.ZOMBIE != cause)
                                {
                                    if (killer != CSteamID.Nil)
                                        id = killer;
                                }
                                player.Player.life.askDamage(100, ragdoll, cause, limb, id, out _);
                            }
                        }
                        break;
                    }
                // LEFT LEG
                case ELimb.LEFT_LEG:
                case ELimb.LEFT_FOOT:
                    {
                        if (HealthHelper.CanBleed(health.LeftLegHealth, totalDamage))
                            player.Bleeding = true;

                        if (health.LeftLegHealth - totalDamage > 0)
                        {
                            health.LeftLegHealth -= totalDamage;
                        }
                        else
                        {
                            health.LeftLegHealth = 0;
                        }

                        await _database.UpdateHealthAsync(player.Id, health.LeftLegHealth, EHealth.LEFT_LEG);

                        if (health.LeftLegHealth + health.RightLegHealth == 0)
                        {
                            if (_config.HealthSystemSettings.DieWhenLegsHealthIsZero)
                            {
                                comp.AllowDamage = true;
                                CSteamID id = CSteamID.Nil;
                                if (EDeathCause.ZOMBIE != cause)
                                {
                                    if (killer != CSteamID.Nil)
                                        id = killer;
                                }
                                player.Player.life.askDamage(100, ragdoll, cause, limb, id, out _);
                            }
                        }
                        break;
                    }
                // RIGHT LEG
                case ELimb.RIGHT_LEG:
                case ELimb.RIGHT_FOOT:
                    {
                        if (HealthHelper.CanBleed(health.RightLegHealth, totalDamage))
                            player.Bleeding = true;

                        if (health.RightLegHealth - totalDamage > 0)
                        {
                            health.RightLegHealth -= totalDamage;
                            await _database.UpdateHealthAsync(player.Id, health.RightLegHealth, EHealth.RIGHT_LEG);
                        }
                        else
                        {
                            health.RightLegHealth = 0;
                            await _database.UpdateHealthAsync(player.Id, health.RightLegHealth, EHealth.RIGHT_LEG);
                        }

                        if (health.RightLegHealth + health.RightLegHealth == 0)
                        {
                            if (_config.HealthSystemSettings.DieWhenLegsHealthIsZero)
                            {
                                comp.AllowDamage = true;
                                CSteamID id = CSteamID.Nil;
                                if (EDeathCause.ZOMBIE != cause)
                                {
                                    if (killer != CSteamID.Nil)
                                        id = killer;
                                }
                                player.Player.life.askDamage(100, ragdoll, cause, limb, id, out _);
                            }
                        }
                        break;
                    }
                default:
                    {
                        switch (cause)
                        {
                            case EDeathCause.BONES:
                                {
                                    if (HealthHelper.CanBleed(health.LeftLegHealth, totalDamage) || HealthHelper.CanBleed(health.RightLegHealth, totalDamage))
                                        player.Bleeding = true;

                                    if (health.RightLegHealth - totalDamage > 0)
                                    {
                                        health.RightLegHealth -= totalDamage;
                                        await _database.UpdateHealthAsync(player.Id, health.RightLegHealth, EHealth.RIGHT_LEG);
                                    }
                                    else
                                    {
                                        health.RightLegHealth = 0;
                                        await _database.UpdateHealthAsync(player.Id, health.RightLegHealth, EHealth.RIGHT_LEG);
                                    }

                                    if (health.RightLegHealth + health.RightLegHealth == 0)
                                    {
                                        if (_config.HealthSystemSettings.DieWhenLegsHealthIsZero)
                                        {
                                            comp.AllowDamage = true;
                                            CSteamID id = CSteamID.Nil;
                                            if (killer != CSteamID.Nil)
                                                id = killer;
                                            player.Player.life.askDamage(100, ragdoll, cause, limb, id, out _);
                                        }
                                    }

                                    if (health.LeftLegHealth - totalDamage > 0)
                                    {
                                        health.LeftLegHealth -= totalDamage;
                                        await _database.UpdateHealthAsync(player.Id, health.LeftArmHealth, EHealth.LEFT_LEG);
                                    }
                                    else
                                    {
                                        health.LeftLegHealth = 0;
                                        await _database.UpdateHealthAsync(player.Id, health.LeftArmHealth, EHealth.LEFT_LEG);
                                    }

                                    if (health.LeftLegHealth + health.RightLegHealth == 0)
                                    {
                                        if (_config.HealthSystemSettings.DieWhenLegsHealthIsZero)
                                        {
                                            comp.AllowDamage = true;
                                            CSteamID id = CSteamID.Nil;
                                            if (killer != CSteamID.Nil)
                                                id = killer;
                                            player.Player.life.askDamage(100, ragdoll, cause, limb, id, out EPlayerKill _);
                                        }
                                    }
                                    break;
                                }
                            case EDeathCause.INFECTION:
                            case EDeathCause.BREATH:
                                {

                                    break;
                                }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Event handler for when a player's life is damaged.
        /// </summary>
        /// <param name="p">The player whose life is damaged.</param>
        /// <param name="damage">The amount of damage inflicted on the player.</param>
        /// <param name="force">The force applied to the player as a result of the damage.</param>
        /// <param name="cause">The cause of death or damage.</param>
        /// <param name="limb">The limb affected by the damage.</param>
        /// <param name="killer">The Steam ID of the entity responsible for the damage.</param>
        private static async void Event_OnPlayerLifeDamaged(Player p, byte damage, Vector3 force, EDeathCause cause, ELimb limb, CSteamID killer)
        {
            string voidname = "OnPlayerLifeDamaged";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(p);
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                player.Player.life.askHeal(100, false, false);
                HealthData health = await _database.GetPlayerHealthAsync(player.Id);
                float totaldamage = damage;

                if (cause == EDeathCause.PUNCH)
                    totaldamage = 1.0f;

                if (cause == EDeathCause.BONES)
                {
                    totaldamage = 10.0f;
                    player.Broken = true;
                }

                if (EDeathCause.BLEEDING == cause)
                {
                    if (health.IsInjured && !comp.AllowDamage)
                    {
                        totaldamage = 0;
                        player.Bleeding = false;
                    }
                    else
                    {
                        if (comp.HasHeavyBleeding)
                            totaldamage = _config.HealthSystemSettings.HeavyBleedingDamage;
                        else
                            totaldamage = _config.HealthSystemSettings.BleedingDamage;
                        player.Bleeding = true;
                    }
                }

                if (cause == EDeathCause.ANIMAL || cause == EDeathCause.ZOMBIE)
                    limb = ELimb.LEFT_FRONT;

                comp.AllowDamage = false;

                if (player.Features.GodMode)
                    totaldamage = 0;

                await HandleIncomingDamageAsync(player, health, killer, totaldamage, limb, cause, player.Position.normalized);
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
            }
        }

        /// <summary>
        /// Event handler for when a player is damaged.
        /// </summary>
        /// <param name="parameters">The parameters related to the damage inflicted on the player.</param>
        /// <param name="shouldAllow">A reference boolean indicating whether the damage should be allowed.</param>
        private static void Event_OnPlayerDamaged(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            string voidname = "OnPlayerDamaged";
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
                                    comp.ReviveAsync();
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
                                //List<Permission> victimPerms = victim.GetPermissions();
                                //List<Permission> attackerPerms = attacker.GetPermissions();

                                List<RocketPermissionsGroup> mutualGroups = UPlayerHelper.GetMutualGroups(victim, attacker);
                                List<string> ffGroups = ag.Groups;

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
                                            UChatHelper.SendPlainChatMessage(attacker.SteamPlayer(), ag.Message);
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
                float totaldamage;

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
                        if (cp.HasHeavyBleeding)
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
                    totaldamage = Mathf.Min(byte.MaxValue, b);
                }
                else
                    totaldamage = parameters.times * parameters.damage;

                if (!allow || player.Features.GodMode)
                    totaldamage = 0;

                Task.Run(async () => await HandleIncomingDamageAsync(player, health, killer, totaldamage, limb, cause, player.Position.normalized));
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
            }
        }

        /// <summary>
        /// Event handler for when a player is revived.
        /// </summary>
        /// <param name="player">The Unturned player who was revived.</param>
        /// <param name="position">The position where the player was revived.</param>
        /// <param name="angle">The angle at which the player was revived.</param>
        private static async void Event_OnPlayerRevived(UnturnedPlayer player, Vector3 position, byte angle)
        {
            string voidname = "OnPlayerRevived";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                await comp.ReviveAsync();
                if (comp.DragState != EDragState.NONE)
                    comp.UnDrag();


                EffectManager.sendUIEffectVisibility((short)comp.EffectID, comp.TranspConnection, true, "RevivePanel", false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);

                TAdvancedHealth.Instance.InvokeAction(0.1f, () =>
                {
                    if (_config.HospitalSettings.EnableRespawnInHospital)
                    {
                        if (_config.HospitalSettings.Hospitals != null)
                            if (_config.HospitalSettings.RandomSpawn)
                            {
                                int i = MathHelper.Next(0, _config.HospitalSettings.Hospitals.Count - 1);
                                Hospital h = _config.HospitalSettings.Hospitals.ElementAt(i);
                                if (h.Position != null)
                                {
                                    i = MathHelper.Next(0, h.Position.Count - 1);
                                    Vector3 p = h.Position.ElementAt(i).GetVector3();
                                    player.Teleport(p, player.Rotation);
                                }
                            }
                            else
                            {
                                Hospital hospital = _config.HospitalSettings.Hospitals.FirstOrDefault(x => player.HasPermission(x.SpawnPermission.ToLower()));
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
                });
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
            }
        }

        /// <summary>
        /// Event handler for when a player dies.
        /// </summary>
        /// <param name="player">The Unturned player who died.</param>
        /// <param name="cause">The cause of death.</param>
        /// <param name="limb">The limb affected by the fatal injury.</param>
        /// <param name="murderer">The Steam ID of the entity responsible for the death.</param>
        private static async void Event_OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            string voidname = "OnPlayerDeath";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                await EffectHelper.UpdateWholeHealthUIAsync(player);
                if (comp.DragState != EDragState.NONE)
                    comp.UnDrag();

                EffectManager.sendUIEffectVisibility((short)comp.EffectID, comp.TranspConnection, true, "RevivePanel", false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
            }
        }
        #endregion
        #region Character Events
        /// <summary>
        /// Event handler for updates to a player's gesture.
        /// </summary>
        /// <param name="player">The Unturned player whose gesture is being updated.</param>
        /// <param name="gesture">The gesture being performed by the player.</param>
        private static async void Event_OnPlayerGestureUpdated(UnturnedPlayer player, UnturnedPlayerEvents.PlayerGesture gesture)
        {
            string voidname = "OnPlayerGestureUpdated";
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
                        await playerComp.DragAsync(targetPlayer);
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
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
            }
        }
        #endregion
        #endregion
        #region UI
        /// <summary>
        /// Event handler for when a player clicks a button.
        /// </summary>
        /// <param name="Player">The player who clicked the button.</param>
        /// <param name="buttonName">The name of the button that was clicked.</param>
        private static async void Event_OnButtonClickded(Player Player, string buttonName)
        {
            string voidname = "OnButtonClicked";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(Player);
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                if (buttonName == "bt_suicide" || buttonName == "bt_suicide2")
                {
                    HealthData health = await _database.GetPlayerHealthAsync(player.Id);
                    if (health.IsInjured)
                    {
                        comp.AllowDamage = true;
                        player.Player.life.askDamage(100, player.Position.normalized, EDeathCause.BLEEDING, ELimb.SKULL, CSteamID.Nil, out _);

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
                TAdvancedHealth.Logger.LogError($"Error in {voidname}: {e}");
            }
        }

        #endregion
    }

}