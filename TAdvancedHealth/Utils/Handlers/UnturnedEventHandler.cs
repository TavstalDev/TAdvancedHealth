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
        // ReSharper disable once InconsistentNaming
        private static DatabaseManager _database => AdvancedHealth.DatabaseManager;
        // ReSharper disable once InconsistentNaming
        private static AdvancedHealthConfig _config => AdvancedHealth.Instance.Config;

        /// <summary>
        /// Attaches event listeners too Unturned to enable event handling.
        /// </summary>
        internal static void Attach()
        {
            U.Events.OnPlayerConnected += OnPlayerJoin;
            U.Events.OnPlayerDisconnected += OnPlayerLeave;
            UnturnedPlayerEvents.OnPlayerRevive += OnPlayerRevived;
            UnturnedPlayerEvents.OnPlayerUpdateGesture += OnPlayerGestureUpdated;
            EffectManager.onEffectButtonClicked += OnButtonClickded;
            VehicleManager.onEnterVehicleRequested += OnPlayerVehicleEnterRequested;
            VehicleManager.onSwapSeatRequested += OnPlayerSwapSeatRequested;
            UnturnedPlayerEvents.OnPlayerUpdateHealth += OnPlayerHealthUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateFood += OnPlayerFoodUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateWater += OnPlayerWaterUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateStamina += OnPlayerStaminaUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateVirus += OnPlayerVirusUpdate;
            DamageTool.damagePlayerRequested += OnPlayerDamaged;
            UseableConsumeable.onConsumePerformed += OnPlayerUseMedicine;
            UnturnedPlayerEvents.OnPlayerDeath += OnPlayerDeath;
            UnturnedPlayerEvents.OnPlayerUpdateBleeding += OnPlayerBleedingUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateBroken += OnPlayerBrokenUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateStance += OnPlayerStanceUpdate;
            
        }

        /// <summary>
        /// Detaches event listeners from Unturned to disable event handling.
        /// </summary>
        internal static void Detach()
        {
            U.Events.OnPlayerConnected -= OnPlayerJoin;
            U.Events.OnPlayerDisconnected -= OnPlayerLeave;
            UnturnedPlayerEvents.OnPlayerRevive -= OnPlayerRevived;
            UnturnedPlayerEvents.OnPlayerUpdateGesture -= OnPlayerGestureUpdated;
            EffectManager.onEffectButtonClicked -= OnButtonClickded;
            VehicleManager.onEnterVehicleRequested -= OnPlayerVehicleEnterRequested;
            VehicleManager.onSwapSeatRequested -= OnPlayerSwapSeatRequested;
            UnturnedPlayerEvents.OnPlayerUpdateHealth -= OnPlayerHealthUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateFood -= OnPlayerFoodUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateWater -= OnPlayerWaterUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateStamina -= OnPlayerStaminaUpdate;
            DamageTool.damagePlayerRequested -= OnPlayerDamaged;
            UseableConsumeable.onConsumePerformed -= OnPlayerUseMedicine;
            UnturnedPlayerEvents.OnPlayerDeath -= OnPlayerDeath;
            UnturnedPlayerEvents.OnPlayerUpdateBleeding -= OnPlayerBleedingUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateBroken -= OnPlayerBrokenUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateVirus -= OnPlayerVirusUpdate;
            UnturnedPlayerEvents.OnPlayerUpdateStance -= OnPlayerStanceUpdate;
        }

        #region Status Events
        /// <summary>
        /// Event handler for updates to the health of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose health is being updated.</param>
        /// <param name="value">The new health value of the player.</param>
        private static void OnPlayerHealthUpdate(UnturnedPlayer player, byte value)
        {
            string methodName = "OnPlayerHealthUpdate";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                Task.Run(async () =>
                {
                    HealthData health = await _database.GetPlayerHealthAsync(player.Id);
                    await EffectHelper.SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.SimpleHealth, (int)((Math.Round(health.BaseHealth, 2) / _config.HealthSystemSettings.BaseHealth) * 100), 0);
                });
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }

        /// <summary>
        /// Event handler for updates to the food level of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose food level is being updated.</param>
        /// <param name="value">The new food level value of the player.</param>
        private static void OnPlayerFoodUpdate(UnturnedPlayer player, byte value)
        {
            string methodName = "OnPlayerFoodUpdate";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                Task.Run(async () =>
                {
                    await EffectHelper.SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.Food, player.Player.life.food, (int)comp.ProgressBarData.LastFood);
                    comp.ProgressBarData.LastFood = value;

                    if (value <= 0)
                        await comp.TryAddStateAsync(EPlayerState.NoFood);
                    else
                        await comp.TryRemoveStateAsync(EPlayerState.NoFood);
                });
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }

        /// <summary>
        /// Event handler for updates to the stamina level of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose stamina level is being updated.</param>
        /// <param name="value">The new stamina level value of the player.</param>
        private static void OnPlayerStaminaUpdate(UnturnedPlayer player, byte value)
        {
            string methodName = "OnPlayerStaminaUpdate";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                Task.Run(async () => await EffectHelper.SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.Stamina, player.Player.life.stamina, (int)comp.ProgressBarData.LastStamina));
                comp.ProgressBarData.LastStamina = value;
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }

        /// <summary>
        /// Event handler for updates to the water level of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose water level is being updated.</param>
        /// <param name="value">The new water level value of the player.</param>
        private static void OnPlayerWaterUpdate(UnturnedPlayer player, byte value)
        {
            string methodName = "OnPlayerWaterUpdate";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                Task.Run(async () =>
                {
                    await EffectHelper.SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.Water, player.Player.life.water, (int)comp.ProgressBarData.LastWater);
                    comp.ProgressBarData.LastWater = value;

                    if (value <= 0)
                        await comp.TryAddStateAsync(EPlayerState.NoWater);
                    else
                        await comp.TryRemoveStateAsync(EPlayerState.NoWater);
                });
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }

        /// <summary>
        /// Event handler for updates to the virus level of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose virus level is being updated.</param>
        /// <param name="value">The new virus level value of the player.</param>
        private static void OnPlayerVirusUpdate(UnturnedPlayer player, byte value)
        {
            string methodName = "OnPlayerVirusUpdate";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                Task.Run(async () =>
                {
                    await EffectHelper.SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.Radiation, player.Player.life.virus, (int)comp.ProgressBarData.LastVirus);
                    comp.ProgressBarData.LastVirus = value;

                    if (value <= 0)
                        await comp.TryAddStateAsync(EPlayerState.NoVirus);
                    else
                        await comp.TryRemoveStateAsync(EPlayerState.NoVirus);
                });
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }

        /// <summary>
        /// Event handler for updates to the oxygen level of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose oxygen level is being updated.</param>
        /// <param name="value">The new oxygen level value of the player.</param>
        private static void OnPlayerOxygenUpdate(UnturnedPlayer player, byte value)
        {
            string methodName = "OnPlayerOxygenUpdate";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                Task.Run(async () =>
                {
                    await EffectHelper.SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.Oxygen, player.Player.life.oxygen, (int)comp.ProgressBarData.LastOxygen);
                    comp.ProgressBarData.LastOxygen = value;

                    if (value <= 0)
                        await comp.TryAddStateAsync(EPlayerState.NoOxygen);
                    else
                        await comp.TryRemoveStateAsync(EPlayerState.NoOxygen);
                });
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }

        /// <summary>
        /// Event handler for updates to the bleeding state of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose bleeding state is being updated.</param>
        /// <param name="state">A boolean indicating whether the player is bleeding.</param>
        private static void OnPlayerBleedingUpdate(UnturnedPlayer player, bool state)
        {
            string methodName = "OnPlayerBleedingUpdate";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                Task.Run(async () =>
                {
                    if (!state)
                    {
                        comp.hasHeavyBleeding = false;
                        await comp.TryRemoveStateAsync(EPlayerState.Bleeding);
                        return;
                    }

                    if (!_config.HealthSystemSettings.CanStartBleeding)
                    {
                        player.Bleeding = false;
                        return;
                    }

                    if (MathHelper.Next(1, 100) <= _config.HealthSystemSettings.HeavyBleedingChance)
                        comp.hasHeavyBleeding = true;

                    await comp.TryAddStateAsync(EPlayerState.Bleeding);
                });
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }

        /// <summary>
        /// Event handler for updates to the broken state of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose broken state is being updated.</param>
        /// <param name="state">A boolean indicating whether the player is in a broken state.</param>
        private static void OnPlayerBrokenUpdate(UnturnedPlayer player, bool state)
        {
            string methodName = "OnPlayerBrokenUpdate";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                Task.Run(async () =>
                {
                    if (!state)
                    {
                        await comp.TryRemoveStateAsync(EPlayerState.BrokenBone);
                        return;
                    }
                    
                    HealthData health = await _database.GetPlayerHealthAsync(player.Id);
                    if (_config.HealthSystemSettings.CanHavePainEffect)
                    {
                        int painChance = MathHelper.Next(1, 100);

                        if (painChance <= _config.HealthSystemSettings.PainEffectChance)
                        {
                            EffectManager.sendUIEffect(_config.HealthSystemSettings.PainEffectID,
                                (short)_config.HealthSystemSettings.PainEffectID, comp.TranspConnection, true);
                            if (_config.HealthSystemSettings.PainEffectDuration > 0)
                                AdvancedHealth.Instance.InvokeAction(_config.HealthSystemSettings.PainEffectDuration,
                                    () =>
                                    {
                                        EffectManager.askEffectClearByID(_config.HealthSystemSettings.PainEffectID,
                                            player.SteamPlayer().transportConnection);
                                    });

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

                    await comp.TryAddStateAsync(EPlayerState.BrokenBone);
                });
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }

        /// <summary>
        /// Event handler for updates to the moon state.
        /// </summary>
        /// <param name="isFullMoon">A boolean indicating whether the moon is in a full state.</param>
        internal static void OnMoonUpdated(bool isFullMoon)
        {
            string methodName = "OnMoonUpdated";
            try
            {
                Task.Run(async () =>
                {
                    foreach (SteamPlayer steamPlayer in Provider.clients)
                    {
                        UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                        AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                        if (isFullMoon)
                            await comp.TryAddStateAsync(EPlayerState.FullMoon);
                        else
                            await comp.TryRemoveStateAsync(EPlayerState.FullMoon);
                    }
                });
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }

        /// <summary>
        /// Event handler for updates to the deadzone status of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose deadzone status is being updated.</param>
        /// <param name="isActive">A boolean indicating whether the player is currently in a deadzone.</param>
        private static void OnPlayerDeadzoneUpdated(UnturnedPlayer player, bool isActive)
        {
            string methodName = "OnPlayerDeadzoneUpdated";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                Task.Run(async () =>
                {
                    if (isActive)
                        await comp.TryAddStateAsync(EPlayerState.DeadZone);
                    else
                        await comp.TryRemoveStateAsync(EPlayerState.DeadZone);
                });
            }
            catch (Exception e)
            {
               AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }

        /// <summary>
        /// Event handler for updates to the safezone status of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose safezone status is being updated.</param>
        /// <param name="isActive">A boolean indicating whether the player is currently in a safezone.</param>
        private static void OnPlayerSafezoneUpdated(UnturnedPlayer player, bool isActive)
        {
            string methodName = "OnPlayerSafezoneUpdated";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                Task.Run(async () =>
                {
                    if (isActive)
                        await comp.TryAddStateAsync(EPlayerState.SafeZone);
                    else
                        await comp.TryRemoveStateAsync(EPlayerState.SafeZone);
                });

            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }

        /// <summary>
        /// Event handler for updates to the temperature of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose temperature is being updated.</param>
        /// <param name="newTemperature">The new temperature state of the player.</param>
        private static void OnPlayerTemperatureUpdate(UnturnedPlayer player, EPlayerTemperature newTemperature)
        {
            string methodName = "OnPlayerTemperatureUpdate";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                EPlayerState state = EPlayerState.Acid;

                if (newTemperature == EPlayerTemperature.ACID)
                    state = EPlayerState.Acid;
                else if (newTemperature == EPlayerTemperature.BURNING)
                    state = EPlayerState.Burning;
                else if (newTemperature == EPlayerTemperature.COLD)
                    state = EPlayerState.Cold;
                else if (newTemperature == EPlayerTemperature.COVERED)
                    state = EPlayerState.Covered;
                else if (newTemperature == EPlayerTemperature.FREEZING)
                    state = EPlayerState.Freezing;
                else if (newTemperature == EPlayerTemperature.WARM)
                    state = EPlayerState.Warm;
                else if (newTemperature == EPlayerTemperature.NONE)
                    state = EPlayerState.NoneTemperature;

                Task.Run(async () => await comp.TryAddStateAsync(state));
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }

        /// <summary>
        /// Event handler for updates to the stance of an Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose stance is being updated.</param>
        /// <param name="stance">The new stance value of the player.</param>
        private static void OnPlayerStanceUpdate(UnturnedPlayer player, byte stance)
        {
            string methodName = "OnPlayerStanceUpdate";
            try
            {

                Task.Run(async () =>
                {
                    HealthData health = await _database.GetPlayerHealthAsync(player.Id);
                    if (health.IsInjured)
                    {
                        player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                        return;
                    }

                    if (health.LeftLegHealth == 0 || health.RightLegHealth == 0)
                    {
                        if (!_config.HealthSystemSettings.CanWalkWithOneBrokenLeg)
                            player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                        else if (!_config.HealthSystemSettings.CanWalkWithBrokenLegs && health.LeftLegHealth == 0 &&
                                 health.RightLegHealth == 0)
                            player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                    }
                });
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
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
        private static void OnPlayerEquipRequested(PlayerEquipment equipment, ItemJar jar, ItemAsset asset, ref bool shouldAllow)
        {
            string methodName = "OnPlayerEquipRequested";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(equipment.player);
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                bool isMedicine = false;
                if (_config.Medicines.FirstOrDefault(x => x.ItemID == jar.item.id) != null)
                {
                    comp.lastEquipedItem = jar.item.id;
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
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }

        /// <summary>
        /// Event handler for when a player requests to dequip an item.
        /// </summary>
        /// <param name="equipment">The player's equipment manager.</param>
        /// <param name="shouldAllow">A reference boolean indicating whether the player should be allowed to dequip the item.</param>
        private static void OnPlayerDequipRequested(PlayerEquipment equipment, ref bool shouldAllow)
        {
            string methodName = "OnPlayerDequipRequested";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(equipment.player);
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                comp.lastEquipedItem = 0;
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }

        /// <summary>
        /// Event handler for when a player uses medicine.
        /// </summary>
        /// <param name="instigatingPlayer">The player who initiated the use of medicine.</param>
        /// <param name="consumeableAsset">The consumable asset representing the medicine being used.</param>
        private static void OnPlayerUseMedicine(Player instigatingPlayer, ItemConsumeableAsset consumeableAsset)
        {
            string methodName = "OnPlayerUseMedicine";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(instigatingPlayer);

                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                if (comp.lastEquipedItem == 0) { return; }

                Task.Run(async () =>
                {
                    if (_config.Medicines == null)
                        return;
                    
                    Medicine med = _config.Medicines.FirstOrDefault(x => x.ItemID == comp.lastEquipedItem);
                    if (med != null)
                    {
                        HealthData health = await _database.GetPlayerHealthAsync(player.Id);
                        if (health.BodyHealth + med.HealsBodyHp <= _config.HealthSystemSettings.BodyHealth)
                            health.BodyHealth += med.HealsBodyHp;
                        else
                            health.BodyHealth = _config.HealthSystemSettings.BodyHealth;

                        if (health.HeadHealth + med.HealsHeadHp <= _config.HealthSystemSettings.HeadHealth)
                            health.HeadHealth += med.HealsHeadHp;
                        else
                            health.HeadHealth = _config.HealthSystemSettings.HeadHealth;

                        if (med.CuresPain)
                            EffectManager.askEffectClearByID(_config.HealthSystemSettings.PainEffectID,
                                player.SteamPlayer().transportConnection);

                        if (health.LeftLegHealth + med.HealsLeftLegHp <= _config.HealthSystemSettings.LeftLegHealth)
                        {
                            health.LeftLegHealth += med.HealsLeftLegHp;
                            AdvancedHealth.Instance.InvokeAction(0.5f, () =>
                            {
                                player.Broken = false;
                                player.Player.movement.sendPluginJumpMultiplier(1f);
                            });
                        }
                        else
                        {
                            health.LeftLegHealth = _config.HealthSystemSettings.LeftLegHealth;
                            AdvancedHealth.Instance.InvokeAction(0.5f, () =>
                            {
                                player.Broken = false;
                                player.Player.movement.sendPluginJumpMultiplier(1f);
                            });
                        }

                        if (health.RightLegHealth + med.HealsRightLegHp <=
                            _config.HealthSystemSettings.RightLegHealth)
                        {
                            health.RightLegHealth += med.HealsRightLegHp;
                            AdvancedHealth.Instance.InvokeAction(0.5f, () =>
                            {
                                player.Broken = false;
                                player.Player.movement.sendPluginJumpMultiplier(1f);
                            });
                        }
                        else
                        {
                            health.RightLegHealth = _config.HealthSystemSettings.RightLegHealth;
                            AdvancedHealth.Instance.InvokeAction(0.5f, () =>
                            {
                                player.Broken = false;
                                player.Player.movement.sendPluginJumpMultiplier(1f);
                            });
                        }

                        if (health.LeftArmHealth + med.HealsLeftArmHp <= _config.HealthSystemSettings.LeftArmHealth)
                            health.LeftArmHealth += med.HealsLeftArmHp;
                        else
                            health.LeftArmHealth = _config.HealthSystemSettings.LeftArmHealth;

                        if (health.RightArmHealth + med.HealsRightArmHp <=
                            _config.HealthSystemSettings.RightArmHealth)
                            health.RightArmHealth += med.HealsRightArmHp;
                        else
                            health.RightArmHealth = _config.HealthSystemSettings.RightArmHealth;

                        await _database.UpdateHealthAsync(player.Id, health);
                    }

                    comp.lastEquipedItem = 0;
                });
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
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
        private static void OnPlayerVehicleEnterRequested(Player p, InteractableVehicle vehicle, ref bool shouldAllow)
        {
            string methodName = "OnPlayerVehicleEnterRequested";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(p);
                HealthData health = _database.GetPlayerHealth(player.Id);
                if (vehicle.passengers[0].player != null)
                    return;

                if (!_config.HealthSystemSettings.CanDriveWithBrokenLegs)
                    if (health.LeftLegHealth == 0 && health.RightLegHealth == 0 || player.Broken)
                    {
                        shouldAllow = false;
                        return;
                    }

                if (!_config.HealthSystemSettings.CanDriveWithOneBrokenLeg)
                    if (health.LeftLegHealth == 0 || health.RightLegHealth == 0 || player.Broken)
                    {
                        shouldAllow = false;
                        return;
                    }
                
                if (!_config.HealthSystemSettings.CanDriveWithBrokenArms)
                    if (health.LeftArmHealth == 0 && health.RightArmHealth == 0)
                    {
                        shouldAllow = false;
                        return;
                    }
                
                if (!_config.HealthSystemSettings.CanDriveWithOneBrokenArm)
                    if (health.LeftArmHealth == 0 || health.RightArmHealth == 0)
                        shouldAllow = false;
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
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
        private static void OnPlayerSwapSeatRequested(Player p, InteractableVehicle vehicle, ref bool shouldAllow, byte fromSeatIndex, ref byte toSeatIndex)
        {
            string methodName = "SwapSeat";
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
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }
        #endregion
        #region Player Events
        #region General
        /// <summary>
        /// Event handler for when a player joins the server.
        /// </summary>
        /// <param name="player">The Unturned player who joined the server.</param>
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
                        HUDStyle style = _config.HUDStyles.FirstOrDefault(x => x.Enabled);
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
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }

        /// <summary>
        /// Event handler for when a player leaves the server.
        /// </summary>
        /// <param name="player">The Unturned player who left the server.</param>
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
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
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
                            await _database.UpdateHealthAsync(player.Id, health.HeadHealth, EHealth.Head);
                        }
                        else
                        {
                            health.HeadHealth = 0;
                            await _database.UpdateHealthAsync(player.Id, 0, EHealth.Head);
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
                                comp.allowDamage = true;
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
                            await _database.UpdateHealthAsync(player.Id, health.BodyHealth, EHealth.Body);
                        }
                        else
                        {
                            health.BodyHealth = 0;
                            await _database.UpdateHealthAsync(player.Id, health.BodyHealth, EHealth.Body);
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
                                comp.allowDamage = true;
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
                            await _database.UpdateHealthAsync(player.Id, health.LeftArmHealth, EHealth.LeftARM);
                        }
                        else
                        {
                            health.LeftArmHealth = 0;
                            await _database.UpdateHealthAsync(player.Id, health.LeftArmHealth, EHealth.LeftARM);
                        }

                        if (health.LeftArmHealth + health.RightArmHealth == 0)
                        {
                            if (_config.HealthSystemSettings.DieWhenArmsHealthIsZero)
                            {
                                comp.allowDamage = true;
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
                            await _database.UpdateHealthAsync(player.Id, health.RightArmHealth, EHealth.RightARM);
                        }
                        else
                        {
                            health.RightArmHealth = 0;
                            await _database.UpdateHealthAsync(player.Id, health.RightArmHealth, EHealth.RightARM);
                        }

                        if (health.RightArmHealth + health.RightArmHealth == 0)
                        {
                            if (_config.HealthSystemSettings.DieWhenArmsHealthIsZero)
                            {
                                comp.allowDamage = true;
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

                        await _database.UpdateHealthAsync(player.Id, health.LeftLegHealth, EHealth.LeftLeg);

                        if (health.LeftLegHealth + health.RightLegHealth == 0)
                        {
                            if (_config.HealthSystemSettings.DieWhenLegsHealthIsZero)
                            {
                                comp.allowDamage = true;
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
                            await _database.UpdateHealthAsync(player.Id, health.RightLegHealth, EHealth.RightLeg);
                        }
                        else
                        {
                            health.RightLegHealth = 0;
                            await _database.UpdateHealthAsync(player.Id, health.RightLegHealth, EHealth.RightLeg);
                        }

                        if (health.RightLegHealth + health.RightLegHealth == 0)
                        {
                            if (_config.HealthSystemSettings.DieWhenLegsHealthIsZero)
                            {
                                comp.allowDamage = true;
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
                                        await _database.UpdateHealthAsync(player.Id, health.RightLegHealth, EHealth.RightLeg);
                                    }
                                    else
                                    {
                                        health.RightLegHealth = 0;
                                        await _database.UpdateHealthAsync(player.Id, health.RightLegHealth, EHealth.RightLeg);
                                    }

                                    if (health.RightLegHealth + health.RightLegHealth == 0)
                                    {
                                        if (_config.HealthSystemSettings.DieWhenLegsHealthIsZero)
                                        {
                                            comp.allowDamage = true;
                                            CSteamID id = CSteamID.Nil;
                                            if (killer != CSteamID.Nil)
                                                id = killer;
                                            player.Player.life.askDamage(100, ragdoll, cause, limb, id, out _);
                                        }
                                    }

                                    if (health.LeftLegHealth - totalDamage > 0)
                                    {
                                        health.LeftLegHealth -= totalDamage;
                                        await _database.UpdateHealthAsync(player.Id, health.LeftArmHealth, EHealth.LeftLeg);
                                    }
                                    else
                                    {
                                        health.LeftLegHealth = 0;
                                        await _database.UpdateHealthAsync(player.Id, health.LeftArmHealth, EHealth.LeftLeg);
                                    }

                                    if (health.LeftLegHealth + health.RightLegHealth == 0)
                                    {
                                        if (_config.HealthSystemSettings.DieWhenLegsHealthIsZero)
                                        {
                                            comp.allowDamage = true;
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
        private static void OnPlayerLifeDamaged(Player p, byte damage, Vector3 force, EDeathCause cause, ELimb limb, CSteamID killer)
        {
            string methodName = "OnPlayerLifeDamaged";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(p);
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                Task.Run(async () =>
                {
                    player.Player.life.askHeal(100, false, false);
                    HealthData health = await _database.GetPlayerHealthAsync(player.Id);
                    float totalDamage = damage;

                    if (cause == EDeathCause.PUNCH)
                        totalDamage = 1.0f;

                    if (cause == EDeathCause.BONES)
                    {
                        totalDamage = 10.0f;
                        player.Broken = true;
                    }

                    if (EDeathCause.BLEEDING == cause)
                    {
                        if (health.IsInjured && !comp.allowDamage)
                        {
                            totalDamage = 0;
                            player.Bleeding = false;
                        }
                        else
                        {
                            if (comp.hasHeavyBleeding)
                                totalDamage = _config.HealthSystemSettings.HeavyBleedingDamage;
                            else
                                totalDamage = _config.HealthSystemSettings.BleedingDamage;
                            player.Bleeding = true;
                        }
                    }

                    if (cause == EDeathCause.ANIMAL || cause == EDeathCause.ZOMBIE)
                        limb = ELimb.LEFT_FRONT;

                    comp.allowDamage = false;

                    if (player.Features.GodMode)
                        totalDamage = 0;

                    await HandleIncomingDamageAsync(player, health, killer, totalDamage, limb, cause, player.Position.normalized);
                });
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }

        /// <summary>
        /// Event handler for when a player is damaged.
        /// </summary>
        /// <param name="parameters">The parameters related to the damage inflicted on the player.</param>
        /// <param name="shouldAllow">A reference boolean indicating whether the damage should be allowed.</param>
        private static void OnPlayerDamaged(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            string methodName = "OnPlayerDamaged";
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
                    if (health.IsInjured && !cp.allowDamage)
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
                cp.allowDamage = false;

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
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }

        /// <summary>
        /// Event handler for when a player is revived.
        /// </summary>
        /// <param name="player">The Unturned player who was revived.</param>
        /// <param name="position">The position where the player was revived.</param>
        /// <param name="angle">The angle at which the player was revived.</param>
        private static void OnPlayerRevived(UnturnedPlayer player, Vector3 position, byte angle)
        {
            string methodName = "OnPlayerRevived";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                Task.Run(async () => await comp.ReviveAsync());
                if (comp.dragState != EDragState.None)
                    comp.UnDrag();


                EffectManager.sendUIEffectVisibility((short)comp.effectId, comp.TranspConnection, true, "RevivePanel", false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);

                AdvancedHealth.Instance.InvokeAction(0.1f, () =>
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
                });
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }

        /// <summary>
        /// Event handler for when a player dies.
        /// </summary>
        /// <param name="player">The Unturned player who died.</param>
        /// <param name="cause">The cause of death.</param>
        /// <param name="limb">The limb affected by the fatal injury.</param>
        /// <param name="murderer">The Steam ID of the entity responsible for the death.</param>
        private static void OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            string methodName = "OnPlayerDeath";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                Task.Run(async () => await EffectHelper.UpdateWholeHealthUIAsync(player));
                if (comp.dragState != EDragState.None)
                    comp.UnDrag();

                EffectManager.sendUIEffectVisibility((short)comp.effectId, comp.TranspConnection, true, "RevivePanel", false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }
        #endregion
        #region Character Events
        /// <summary>
        /// Event handler for updates to a player's gesture.
        /// </summary>
        /// <param name="player">The Unturned player whose gesture is being updated.</param>
        /// <param name="gesture">The gesture being performed by the player.</param>
        private static void OnPlayerGestureUpdated(UnturnedPlayer player, UnturnedPlayerEvents.PlayerGesture gesture)
        {
            string methodName = "OnPlayerGestureUpdated";
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
                        Task.Run(async () => await playerComp.DragAsync(targetPlayer));
                    }
                }
                else if (gesture == UnturnedPlayerEvents.PlayerGesture.SurrenderStop)
                {
                    AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                    if (comp.dragState == EDragState.Dragger)
                        comp.UnDrag();
                }
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }
        #endregion
        #endregion
        #region UI
        /// <summary>
        /// Event handler for when a player clicks a button.
        /// </summary>
        /// <param name="player">The player who clicked the button.</param>
        /// <param name="buttonName">The name of the button that was clicked.</param>
        private static void OnButtonClickded(Player player, string buttonName)
        {
            string methodName = "OnButtonClicked";
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);
                AdvancedHealthComponent comp = uPlayer.GetComponent<AdvancedHealthComponent>();
                if (buttonName == "bt_suicide" || buttonName == "bt_suicide2")
                {
                    Task.Run(async () =>
                    {
                        HealthData health = await _database.GetPlayerHealthAsync(uPlayer.Id);
                        if (health.IsInjured)
                        {
                            comp.allowDamage = true;
                            uPlayer.Player.life.askDamage(100, uPlayer.Position.normalized, EDeathCause.BLEEDING, ELimb.SKULL, CSteamID.Nil, out _);

                            if (uPlayer.Player.movement.pluginSpeedMultiplier == 0)
                                uPlayer.Player.movement.sendPluginSpeedMultiplier(1);
                            health.IsInjured = false;
                            if (comp.dragState != EDragState.None)
                                comp.UnDrag();

                            uPlayer.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                            EffectManager.sendUIEffectVisibility((short)comp.effectId, comp.TranspConnection, true, "RevivePanel", false);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError($"Error in {methodName}: {e}");
            }
        }

        #endregion
    }

}