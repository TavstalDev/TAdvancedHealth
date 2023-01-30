using Rocket.API.Serialisation;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TAdvancedHealth.Compatibility;
using Tavstal.TAdvancedHealth.Managers;
using Tavstal.TAdvancedHealth.Modules;
using UnityEngine;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.API;
using Logger = Tavstal.TAdvancedHealth.Helpers.LoggerHelper;
using SDG.Framework.Utilities;
using Rocket.Unturned;
using System.Reflection;
using Tavstal.TAdvancedHealth.Helpers;

namespace Tavstal.TAdvancedHealth.Handlers
{
    public static class UnturnedEventHandler
    {
        private static DatabaseManager Database => TAdvancedHealthMain.Database;
        private static TAdvancedHealthConfig Config => TAdvancedHealthMain.Instance.Configuration.Instance;

        internal static void OnLoad()
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

        internal static void OnUnload()
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

        // Status Events

        private static void Event_OnPlayerHealthUpdate(UnturnedPlayer p, byte b)
        {
            string voidname = "Health void";
            try
            {
                TAdvancedHealthComponent comp = p.GetComponent<TAdvancedHealthComponent>();
                PlayerHealth health = Database.GetPlayerHealth(p.Id);
                EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, p.CSteamID, true, EProgressBarType.Health_Simple, (int)((System.Math.Round(health.BaseHealth, 2) / Config.CustomHealtSystemAndComponentSettings.BaseHealth) * 100), 0);
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerFoodUpdate(UnturnedPlayer p, byte b)
        {
            string voidname = "Food void";
            try
            {
                var c = Config.CustomHealtSystemAndComponentSettings;
                TAdvancedHealthComponent comp = p.GetComponent<TAdvancedHealthComponent>();

                EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, p.CSteamID, true, EProgressBarType.Food, p.Player.life.food, (int)comp.progressBarData.LastFood);
                comp.progressBarData.LastFood = b;

                if (b <= 0)
                    comp.TryAddState(EPlayerStates.NOFOOD);
                else
                    comp.TryRemoveState(EPlayerStates.NOFOOD);
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerStaminaUpdate(UnturnedPlayer p, byte b)
        {
            string voidname = "Stamina void";
            try
            {
                TAdvancedHealthComponent comp = p.GetComponent<TAdvancedHealthComponent>();
                EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, p.CSteamID, true, EProgressBarType.Stamina, p.Player.life.stamina, (int)comp.progressBarData.LastStamina);
                comp.progressBarData.LastStamina = b;
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerWaterUpdate(UnturnedPlayer p, byte b)
        {
            string voidname = "Water void";
            try
            {
                var c = Config.CustomHealtSystemAndComponentSettings;
                TAdvancedHealthComponent comp = p.GetComponent<TAdvancedHealthComponent>();

                EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, p.CSteamID, true, EProgressBarType.Water, p.Player.life.water, (int)comp.progressBarData.LastWater);
                comp.progressBarData.LastWater = b;

                if (b <= 0)
                    comp.TryAddState(EPlayerStates.NOWATER);
                else
                    comp.TryRemoveState(EPlayerStates.NOWATER);
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerVirusUpdate(UnturnedPlayer p, byte b)
        {
            string voidname = "Virus void";
            try
            {
                var c = Config.CustomHealtSystemAndComponentSettings;
                TAdvancedHealthComponent comp = p.GetComponent<TAdvancedHealthComponent>();

                EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, p.CSteamID, true, EProgressBarType.Radiation, p.Player.life.virus, (int)comp.progressBarData.LastVirus);
                comp.progressBarData.LastVirus = b;

                if (b <= 0)
                    comp.TryAddState(EPlayerStates.NOVIRUS);
                else
                    comp.TryRemoveState(EPlayerStates.NOVIRUS);
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerOxygenUpdate(UnturnedPlayer p, byte b)
        {
            string voidname = "Oxygen void";
            try
            {
                var c = Config.CustomHealtSystemAndComponentSettings;
                TAdvancedHealthComponent comp = p.GetComponent<TAdvancedHealthComponent>();
                EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, p.CSteamID, true, EProgressBarType.Oxygen, p.Player.life.oxygen, (int)comp.progressBarData.LastOxygen);
                comp.progressBarData.LastOxygen = b;

                if (b <= 0)
                    comp.TryAddState(EPlayerStates.NOOXYGEN);
                else
                    comp.TryRemoveState(EPlayerStates.NOOXYGEN);
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerBleedingUpdate(UnturnedPlayer p, bool state)
        {
            string voidname = "Bleeding void";
            try
            {
                var c = Config.CustomHealtSystemAndComponentSettings;
                TAdvancedHealthComponent comp = p.GetComponent<TAdvancedHealthComponent>();
                if (state)
                {
                    if (!c.CanStartBleeding && state)
                    {
                        p.Bleeding = false;
                        return;
                    }

                    int val = MathHelper.GenerateRandomNumber(1, 100);

                    if (val <= c.HeavyBleedingChance)
                        comp.hasHeavyBleeding = true;

                    comp.TryAddState(EPlayerStates.BLEEDING);
                }
                else
                {
                    comp.hasHeavyBleeding = false;
                    comp.TryRemoveState(EPlayerStates.BLEEDING);
                }

            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerBrokenUpdate(UnturnedPlayer p, bool state)
        {
            string voidname = "BrokenBone void";
            try
            {
                var c = Config.CustomHealtSystemAndComponentSettings;
                TAdvancedHealthComponent comp = p.GetComponent<TAdvancedHealthComponent>();
                PlayerHealth health = Database.GetPlayerHealth(p.Id);
                if (state)
                {
                    if (c.CanHavePainEffect)
                    {
                        int painChance = MathHelper.GenerateRandomNumber(1, 100);

                        if (painChance <= c.PainEffectChance)
                        {
                            EffectManager.sendUIEffect(c.PainEffectID, (short)c.PainEffectID, comp.transCon, true);
                            if (c.PainEffectDuration > 0)
                                TAdvancedHealthMain.Instance.StartCoroutine(TAdvancedHealthMain.Instance.DelayedInvoke(c.PainEffectDuration, () => { SDG.Unturned.EffectManager.askEffectClearByID(c.PainEffectID, p.SteamPlayer().transportConnection); }));

                        }
                    }

                    if (health.LeftLegHealth == 0 && health.RightLegHealth == 0)
                    {
                        if (!c.CanWalkWithBrokenLegs)
                        {
                            p.Player.stance.checkStance(EPlayerStance.PRONE, true);
                        }
                    }
                    else if (health.LeftLegHealth == 0 || health.RightLegHealth == 0)
                    {
                        if (!c.CanWalkWithOneBrokenLeg)
                        {
                            p.Player.stance.checkStance(EPlayerStance.PRONE, true);
                        }
                    }

                    comp.TryAddState(EPlayerStates.BROKENBONE);
                }
                else
                    comp.TryRemoveState(EPlayerStates.BROKENBONE);

            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        internal static void Event_OnMoonUpdated(bool isFullMoon)
        {
            string voidname = "MoonUpdate void";
            try
            {
                if (isFullMoon != TAdvancedHealthMain.Instance.hasFullMoon)
                    TAdvancedHealthMain.Instance.hasFullMoon = isFullMoon;
                foreach (SteamPlayer steamPlayer in Provider.clients)
                {
                    UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                    TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();

                    if (isFullMoon)
                        comp.TryAddState(EPlayerStates.FULLMOON);
                    else
                        comp.TryRemoveState(EPlayerStates.FULLMOON);
                }

            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerDeadzoneUpdated(UnturnedPlayer player, bool isActive)
        {
            string voidname = "DeadZone void";
            try
            {
                TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();
                if (isActive)
                    comp.TryAddState(EPlayerStates.DEADZONE);
                else
                    comp.TryRemoveState(EPlayerStates.DEADZONE);

            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerSafezoneUpdated(UnturnedPlayer player, bool isActive)
        {
            string voidname = "SafeZone void";
            try
            {
                TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();
                if (isActive)
                    comp.TryAddState(EPlayerStates.SAFEZONE);
                else
                    comp.TryRemoveState(EPlayerStates.SAFEZONE);

            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerTemperatureUpdate(UnturnedPlayer player, EPlayerTemperature newTemperature)
        {
            string voidname = "Temperature void";
            try
            {
                TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();

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

                comp.TryAddState(state);

            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        private static void Event_OnPlayerStanceUpdate(UnturnedPlayer player, byte stance)
        {
            //TAdvancedHealthComponent cp = player.GetComponent<TAdvancedHealthComponent>();
            PlayerHealth health = Database.GetPlayerHealth(player.Id);
            if (health.isInjured)
                player.Player.stance.checkStance(EPlayerStance.PRONE, true);
            else
            {
                var c = Config.CustomHealtSystemAndComponentSettings;
                if (health.LeftLegHealth == 0 && health.RightLegHealth == 0)
                    if (!c.CanWalkWithBrokenLegs)
                        player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                    else if (health.LeftLegHealth == 0 || health.RightLegHealth == 0)
                        if (!c.CanWalkWithOneBrokenLeg)
                            player.Player.stance.checkStance(EPlayerStance.PRONE, true);
            }
        }

        // Other Events

        private static void Event_OnPlayerUseMedicine(Player instigatingPlayer, ItemConsumeableAsset consumeableAsset)
        {
            string voidname = "UseMedicine";
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(instigatingPlayer);

                TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();
                var c = Config.CustomHealtSystemAndComponentSettings;
                if (comp.lastEquipedItem == 0 || !player.Player.equipment.isEquipped) { return; }

                if (Config.Medicines != null)
                {
                    Medicine med = Config.Medicines.FirstOrDefault(x => x.itemID == comp.lastEquipedItem);
                    if (med != null)
                    {
                        PlayerHealth health = Database.GetPlayerHealth(player.Id);
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
                            TAdvancedHealthMain.Instance.StartCoroutine(TAdvancedHealthMain.Instance.DelayedInvoke(0.5f, () =>
                            {
                                player.Broken = false;
                                player.Player.movement.sendPluginJumpMultiplier(1f);
                            }));
                        }
                        else
                        {
                            health.LeftLegHealth = c.LeftLegHealth;
                            TAdvancedHealthMain.Instance.StartCoroutine(TAdvancedHealthMain.Instance.DelayedInvoke(0.5f, () =>
                            {
                                player.Broken = false;
                                player.Player.movement.sendPluginJumpMultiplier(1f);
                            }));
                        }

                        if (health.RightLegHealth + med.HealsRightLegHP <= c.RightLegHealth)
                        {
                            health.RightLegHealth += med.HealsRightLegHP;
                            TAdvancedHealthMain.Instance.StartCoroutine(TAdvancedHealthMain.Instance.DelayedInvoke(0.5f, () =>
                            {
                                player.Broken = false;
                                player.Player.movement.sendPluginJumpMultiplier(1f);
                            }));
                        }
                        else
                        {
                            health.RightLegHealth = c.RightLegHealth;
                            TAdvancedHealthMain.Instance.StartCoroutine(TAdvancedHealthMain.Instance.DelayedInvoke(0.5f, () =>
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

                        Database.Update(player.Id, health);
                    }
                    comp.lastEquipedItem = 0;
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
                var c = Config.CustomHealtSystemAndComponentSettings;
                PlayerHealth health = Database.GetPlayerHealth(player.Id);
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
                var c = Config.CustomHealtSystemAndComponentSettings;
                PlayerHealth h = Database.GetPlayerHealth(player.Id);
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
                PlayerHealth health = Database.GetPlayerHealth(player.Id);
                if (health == null)
                {
                    var c = Config;
                    var s = c.CustomHealtSystemAndComponentSettings;
                    HUDStyle style = Config.HUDStyles.FirstOrDefault(x => x.Enabled);
                    Database.Add(player.Id, new PlayerHealth { PlayerId = player.Id, HUDEffectID = style.EffectID, BaseHealth = s.BaseHealth, BodyHealth = s.BodyHealth, HeadHealth = s.HeadHealth, LeftArmHealth = s.LeftArmHealth, LeftLegHealth = s.LeftLegHealth, RightArmHealth = s.RightArmHealth, RightLegHealth = s.RightLegHealth, isInjured = false, isHUDEnabled = true, dieDate = DateTime.Now });
                    health = Database.GetPlayerHealth(player.Id);
                }
                else
                {
                    var c = Config;
                    var s = c.CustomHealtSystemAndComponentSettings;
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
                        Database.Update(player.Id, health);
                    }
                }

                TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();
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

                if (TAdvancedHealthMain.Instance.hasFullMoon)
                    comp.TryAddState(EPlayerStates.FULLMOON);

                player.Player.equipment.onEquipRequested += Event_OnPlayerEquipRequested;
                player.Player.equipment.onDequipRequested += Event_OnPlayerDequipRequested;
                player.Player.life.onHurt += Event_OnPlayerLifeDamaged;
                player.Player.life.onOxygenUpdated += (byte b) => Event_OnPlayerOxygenUpdate(player, b);
                player.Player.life.onTemperatureUpdated += (EPlayerTemperature newTemperature) => Event_OnPlayerTemperatureUpdate(player, newTemperature);
                player.Player.movement.onSafetyUpdated += (bool isSafe) => Event_OnPlayerSafezoneUpdated(player, isSafe);
                player.Player.movement.onRadiationUpdated += (bool isRadio) => Event_OnPlayerDeadzoneUpdated(player, isRadio);
                player.Player.life.onVirusUpdated += (byte virus) => Event_OnPlayerVirusUpdate(player, virus);
                #region HideHealth HUD
                if (health.isHUDEnabled)
                {
                    EffectManager.sendUIEffect(comp.EffectID, (short)comp.EffectID, comp.transCon, true);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowFood, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowHealth, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowOxygen, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStamina, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVirus, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowWater, false);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons, false);
                    UnturnedHelper.UpdateHealthAllUI(player);
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

                TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();
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
                TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();
                bool isMedicine = false;
                if (Config.Medicines.FirstOrDefault(x => x.itemID == jar.item.id) != null)
                {
                    comp.lastEquipedItem = jar.item.id;
                    isMedicine = true;
                }

                var c = Config.CustomHealtSystemAndComponentSettings;
                PlayerHealth h = Database.GetPlayerHealth(player.Id);
                if (h.RightArmHealth == 0 && h.LeftArmHealth == 0)
                {
                    if (!isMedicine)
                    {
                        if (!c.CanHoldOneHandItemsWithBrokenArms)
                            if (Config.oneHandedItems.ItemID.Contains(jar.item.id) || Config.oneHandedItems.ItemTypes.Contains(asset.type))
                            {
                                shouldAllow = false;
                                if (player.Player.equipment.isEquipped)
                                    player.Player.equipment.dequip();
                            }
                        if (!c.CanHoldTwoHandItemsWithBrokenArms)
                            if (Config.twoHandedItems.ItemID.Contains(jar.item.id) || Config.twoHandedItems.ItemTypes.Contains(asset.type))
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
                            if (Config.oneHandedItems.ItemID.Contains(jar.item.id) || Config.oneHandedItems.ItemTypes.Contains(asset.type))
                            {
                                shouldAllow = false;
                                if (player.Player.equipment.isEquipped)
                                    player.Player.equipment.dequip();
                            }
                        if (!c.CanHoldTwoHandItemsWithOneBrokenArm)
                            if (Config.twoHandedItems.ItemID.Contains(jar.item.id) || Config.twoHandedItems.ItemTypes.Contains(asset.type))
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
                TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();
                comp.lastEquipedItem = 0;
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
                TAdvancedHealthComponent cp = player.GetComponent<TAdvancedHealthComponent>();
                var c = Config.CustomHealtSystemAndComponentSettings;
                var ag = Config.AntiGroupFriendlyFireSettings;
                bool allow = true;

                player.Player.life.askHeal(100, false, false);
                PlayerHealth health = Database.GetPlayerHealth(player.Id);
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
                    if (health.isInjured && !cp.allowDamage)
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

                cp.allowDamage = false;

                if (!allow || player.Features.GodMode)
                    totaldamage = 0;

                Vector3 ragdoll = player.Position.normalized;
                if (limb == ELimb.SKULL)
                {
                    if (UnturnedHelper.CanBleed(health.HeadHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.HeadHealth - totaldamage > 0)
                    {
                        health.HeadHealth -= totaldamage;
                        Database.UpdateHeadHealth(player.Id, health.HeadHealth);
                    }
                    else
                    {
                        health.HeadHealth = 0;
                        Database.UpdateHeadHealth(player.Id, 0);
                    }

                    if (health.HeadHealth == 0)
                    {
                        if (c.CanBeInjured && !health.isInjured)
                        {
                            int chanc = MathHelper.GenerateRandomNumber(1, 100);
                            if (c.InjuredChance >= chanc)
                            {
                                UnturnedHelper.SetPlayerDowned(player);
                                return;
                            }
                        }

                        if (c.DieWhenHeadHealthIsZero)
                        {
                            cp.allowDamage = true;
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
                    if (UnturnedHelper.CanBleed(health.LeftLegHealth, totaldamage) || UnturnedHelper.CanBleed(health.RightLegHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.RightLegHealth - totaldamage > 0)
                    {
                        health.RightLegHealth -= totaldamage;
                        Database.UpdateRightLegHealth(player.Id, health.RightLegHealth);
                    }
                    else
                    {
                        health.RightLegHealth = 0;
                        Database.UpdateRightLegHealth(player.Id, health.RightLegHealth);
                    }

                    if (health.RightLegHealth + health.RightLegHealth == 0)
                    {
                        if (c.DieWhenLegsHealthIsZero)
                        {
                            cp.allowDamage = true;
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
                        Database.UpdateLeftLegHealth(player.Id, health.LeftArmHealth);
                    }
                    else
                    {
                        health.LeftLegHealth = 0;
                        Database.UpdateLeftLegHealth(player.Id, health.LeftArmHealth);
                    }

                    if (health.LeftLegHealth + health.RightLegHealth == 0)
                    {
                        if (c.DieWhenLegsHealthIsZero)
                        {
                            cp.allowDamage = true;
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
                    if (UnturnedHelper.CanBleed(health.BodyHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.BodyHealth - totaldamage > 0)
                    {
                        health.BodyHealth -= totaldamage;
                        Database.UpdateBodyHealth(player.Id, health.BodyHealth);
                    }
                    else
                    {
                        health.BodyHealth = 0;
                        Database.UpdateBodyHealth(player.Id, health.BodyHealth);
                    }

                    if (health.BodyHealth == 0)
                    {
                        if (c.CanBeInjured && !health.isInjured)
                        {
                            int chanc = MathHelper.GenerateRandomNumber(1, 100);
                            if (c.InjuredChance >= chanc)
                            {
                                UnturnedHelper.SetPlayerDowned(player);
                                return;
                            }
                        }

                        if (c.DieWhenBodyHealthIsZero)
                        {
                            cp.allowDamage = true;
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
                    if (UnturnedHelper.CanBleed(health.LeftArmHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.LeftArmHealth - totaldamage > 0)
                    {
                        health.LeftArmHealth -= totaldamage;
                        Database.UpdateLeftArmHealth(player.Id, health.LeftArmHealth);
                    }
                    else
                    {
                        health.LeftArmHealth = 0;
                        Database.UpdateLeftArmHealth(player.Id, health.LeftArmHealth);
                    }

                    if (health.LeftArmHealth + health.RightArmHealth == 0)
                    {
                        if (c.DieWhenArmsHealthIsZero)
                        {
                            cp.allowDamage = true;
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
                    if (UnturnedHelper.CanBleed(health.RightArmHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.RightArmHealth - totaldamage > 0)
                    {
                        health.RightArmHealth -= totaldamage;
                        Database.UpdateRightArmHealth(player.Id, health.RightArmHealth);
                    }
                    else
                    {
                        health.RightArmHealth = 0;
                        Database.UpdateRightArmHealth(player.Id, health.RightArmHealth);
                    }

                    if (health.RightArmHealth + health.RightArmHealth == 0)
                    {
                        if (c.DieWhenArmsHealthIsZero)
                        {
                            cp.allowDamage = true;
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
                    if (UnturnedHelper.CanBleed(health.LeftLegHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.LeftLegHealth - totaldamage > 0)
                    {
                        health.LeftLegHealth -= totaldamage;
                        Database.UpdateLeftLegHealth(player.Id, health.LeftLegHealth);
                    }
                    else
                    {
                        health.LeftLegHealth = 0;
                        Database.UpdateLeftLegHealth(player.Id, health.LeftLegHealth);
                    }

                    if (health.LeftLegHealth + health.RightLegHealth == 0)
                    {
                        if (c.DieWhenLegsHealthIsZero)
                        {
                            cp.allowDamage = true;
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
                    if (UnturnedHelper.CanBleed(health.RightLegHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.RightLegHealth - totaldamage > 0)
                    {
                        health.RightLegHealth -= totaldamage;
                        Database.UpdateRightLegHealth(player.Id, health.RightLegHealth);
                    }
                    else
                    {
                        health.RightLegHealth = 0;
                        Database.UpdateRightLegHealth(player.Id, health.RightLegHealth);
                    }

                    if (health.RightLegHealth + health.RightLegHealth == 0)
                    {
                        if (c.DieWhenLegsHealthIsZero)
                        {
                            cp.allowDamage = true;
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
                    if (UnturnedHelper.CanBleed(health.BodyHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.BodyHealth - totaldamage > 0)
                    {
                        health.BodyHealth -= totaldamage;
                        Database.UpdateBodyHealth(player.Id, health.BodyHealth);
                    }
                    else
                    {
                        health.BodyHealth = 0;
                        Database.UpdateBodyHealth(player.Id, health.BodyHealth);
                    }

                    if (health.BodyHealth == 0)
                    {
                        if (c.CanBeInjured && !health.isInjured)
                        {
                            int chanc = MathHelper.GenerateRandomNumber(1, 100);
                            if (c.InjuredChance >= chanc)
                            {
                                UnturnedHelper.SetPlayerDowned(player);
                                return;
                            }
                        }

                        if (c.DieWhenBodyHealthIsZero)
                        {
                            cp.allowDamage = true;
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
                TAdvancedHealthComponent cp = player.GetComponent<TAdvancedHealthComponent>();
                var c = Config.CustomHealtSystemAndComponentSettings;
                var ag = Config.AntiGroupFriendlyFireSettings;
                bool allow = true;
                shouldAllow = false;

                /*if (parameters.killer != CSteamID.Nil)
                {
                    UnturnedPlayer killerPlayer = UnturnedPlayer.FromCSteamID(parameters.killer);
                    if (killerPlayer != null && Config.DefibrillatorSettings.Enabled)
                    {
                        TAdvancedHealthComponent killerComp = killerPlayer.GetComponent<TAdvancedHealthComponent>();
                        if (!Config.DefibrillatorSettings.EnablePermission || (Config.DefibrillatorSettings.EnablePermission && killerPlayer.HasPermission(Config.DefibrillatorSettings.PermissionForUseDefiblirator)))
                        {
                            Defibrillator defibrillator = Config.DefibrillatorSettings.DefibrillatorItems.Find(x => x.ItemID == killerPlayer.Player.equipment.itemID);
                            if (defibrillator != null)
                            {
                                if (killerComp.lastDefibliratorUses.TryGetValue(defibrillator.ItemID, out DateTime value))
                                {
                                    if (value > DateTime.Now)
                                    {
                                        Helper.SendChatMessage(killerPlayer.SteamPlayer(), TAdvancedHealthMain.Instance.Translate(true, "error_defiblirator_cooldown", (value - DateTime.Now).TotalSeconds.ToString("0.00")));
                                        return;
                                    }
                                    killerComp.lastDefibliratorUses.Remove(defibrillator.ItemID);
                                }

                                int chance = MathHelper.GenerateRandomNumber(1, 100);
                                if (chance != 0 && chance <= defibrillator.ReviveChance)
                                    cp.Revive();
                                killerComp.lastDefibliratorUses.Add(defibrillator.ItemID, DateTime.Now.AddSeconds(defibrillator.RechargeTimeSecs));
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

                                List<RocketPermissionsGroup> mutualGroups = UnturnedHelper.GetMutualGroups(victim, attacker);
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
                PlayerHealth health = Database.GetPlayerHealth(player.Id);
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
                    if (health.isInjured && !cp.allowDamage)
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
                    totaldamage = Mathf.Min((int)byte.MaxValue, b);
                }
                else
                    totaldamage = parameters.times * parameters.damage;

                if (!allow || player.Features.GodMode)
                    totaldamage = 0;

                Vector3 ragdoll = player.Position.normalized;
                if (limb == ELimb.SKULL)
                {
                    if (UnturnedHelper.CanBleed(health.HeadHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.HeadHealth - totaldamage > 0)
                    {
                        health.HeadHealth -= totaldamage;
                        Database.UpdateHeadHealth(player.Id, health.HeadHealth);
                    }
                    else
                    {
                        health.HeadHealth = 0;
                        Database.UpdateHeadHealth(player.Id, health.HeadHealth);
                    }

                    if (health.HeadHealth == 0)
                    {
                        if (c.CanBeInjured && !health.isInjured)
                        {
                            int chanc = MathHelper.GenerateRandomNumber(1, 100);
                            if (c.InjuredChance >= chanc)
                            {
                                UnturnedHelper.SetPlayerDowned(player);
                                return;
                            }
                        }

                        if (c.DieWhenHeadHealthIsZero)
                        {
                            cp.allowDamage = true;
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
                    if (UnturnedHelper.CanBleed(health.LeftLegHealth, totaldamage) || UnturnedHelper.CanBleed(health.RightLegHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.RightLegHealth - totaldamage > 0)
                    {
                        health.RightLegHealth -= totaldamage;
                        Database.UpdateRightLegHealth(player.Id, health.RightLegHealth);
                    }
                    else
                    {
                        health.RightLegHealth = 0;
                        Database.UpdateRightLegHealth(player.Id, health.RightLegHealth);
                    }

                    if (health.RightLegHealth + health.RightLegHealth == 0)
                    {
                        if (c.DieWhenLegsHealthIsZero)
                        {
                            cp.allowDamage = true;
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
                        Database.UpdateLeftLegHealth(player.Id, health.LeftLegHealth);
                    }
                    else
                    {
                        health.LeftLegHealth = 0;
                        Database.UpdateLeftLegHealth(player.Id, health.LeftLegHealth);
                    }

                    if (health.LeftLegHealth + health.RightLegHealth == 0)
                    {
                        if (c.DieWhenLegsHealthIsZero)
                        {
                            cp.allowDamage = true;
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
                    if (UnturnedHelper.CanBleed(health.BodyHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.BodyHealth - totaldamage > 0)
                    {
                        health.BodyHealth -= totaldamage;
                        Database.UpdateBodyHealth(player.Id, health.BodyHealth);
                    }
                    else
                    {
                        health.BodyHealth = 0;
                        Database.UpdateBodyHealth(player.Id, health.BodyHealth);
                    }

                    if (health.BodyHealth == 0)
                    {
                        if (c.CanBeInjured && !health.isInjured)
                        {
                            int chanc = MathHelper.GenerateRandomNumber(1, 100);
                            if (c.InjuredChance >= chanc)
                            {
                                UnturnedHelper.SetPlayerDowned(player);
                                return;
                            }
                        }

                        if (c.DieWhenBodyHealthIsZero)
                        {
                            cp.allowDamage = true;
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
                    if (UnturnedHelper.CanBleed(health.LeftArmHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.LeftArmHealth - totaldamage > 0)
                    {
                        health.LeftArmHealth -= totaldamage;
                        Database.UpdateLeftArmHealth(player.Id, health.LeftArmHealth);
                    }
                    else
                    {
                        health.LeftArmHealth = 0;
                        Database.UpdateLeftArmHealth(player.Id, health.LeftArmHealth);
                    }

                    if (health.LeftArmHealth + health.RightArmHealth == 0)
                    {
                        if (c.DieWhenArmsHealthIsZero)
                        {
                            cp.allowDamage = true;
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
                    if (UnturnedHelper.CanBleed(health.RightArmHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.RightArmHealth - totaldamage > 0)
                    {
                        health.RightArmHealth -= totaldamage;
                        Database.UpdateRightArmHealth(player.Id, health.RightArmHealth);
                    }
                    else
                    {
                        health.RightArmHealth = 0;
                        Database.UpdateRightArmHealth(player.Id, health.RightArmHealth);
                    }

                    if (health.RightArmHealth + health.RightArmHealth == 0)
                    {
                        if (c.DieWhenArmsHealthIsZero)
                        {
                            cp.allowDamage = true;
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
                    if (UnturnedHelper.CanBleed(health.LeftLegHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.LeftLegHealth - totaldamage > 0)
                    {
                        health.LeftLegHealth -= totaldamage;
                        Database.UpdateLeftLegHealth(player.Id, health.LeftLegHealth);
                    }
                    else
                    {
                        health.LeftLegHealth = 0;
                        Database.UpdateLeftLegHealth(player.Id, health.LeftLegHealth);
                    }

                    if (health.LeftLegHealth + health.RightLegHealth == 0)
                    {
                        if (c.DieWhenLegsHealthIsZero)
                        {
                            cp.allowDamage = true;
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
                    if (UnturnedHelper.CanBleed(health.RightLegHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.RightLegHealth - totaldamage > 0)
                    {
                        health.RightLegHealth -= totaldamage;
                        Database.UpdateRightLegHealth(player.Id, health.RightLegHealth);
                    }
                    else
                    {
                        health.RightLegHealth = 0;
                        Database.UpdateRightLegHealth(player.Id, health.RightLegHealth);
                    }

                    if (health.RightLegHealth + health.RightLegHealth == 0)
                    {
                        if (c.DieWhenLegsHealthIsZero)
                        {
                            cp.allowDamage = true;
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
                    if (UnturnedHelper.CanBleed(health.BodyHealth, totaldamage))
                        player.Bleeding = true;

                    if (health.BodyHealth - totaldamage > 0)
                    {
                        health.BodyHealth -= totaldamage;
                        Database.UpdateBodyHealth(player.Id, health.BodyHealth);
                    }
                    else
                    {
                        health.BodyHealth = 0;
                        Database.UpdateBodyHealth(player.Id, health.BodyHealth);
                    }

                    if (health.BodyHealth == 0)
                    {
                        if (c.CanBeInjured && !health.isInjured)
                        {
                            int chanc = MathHelper.GenerateRandomNumber(1, 100);
                            if (c.InjuredChance >= chanc)
                            {
                                UnturnedHelper.SetPlayerDowned(player);
                                return;
                            }
                        }

                        if (c.DieWhenBodyHealthIsZero)
                        {
                            cp.allowDamage = true;
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
                        TAdvancedHealthComponent playerComp = player.GetComponent<TAdvancedHealthComponent>();
                        playerComp.Drag(targetPlayer);
                    }
                }
                else if (gesture == UnturnedPlayerEvents.PlayerGesture.SurrenderStop)
                {
                    TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();

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
                TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();
                comp.Revive();
                if (comp.DragState != EDragState.NONE)
                    comp.UnDrag();


                EffectManager.sendUIEffectVisibility((short)comp.EffectID, comp.transCon, true, "RevivePanel", false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);

                TAdvancedHealthMain.Instance.StartCoroutine(TAdvancedHealthMain.Instance.DelayedInvoke(0.1f, () =>
                {
                    if (Config.HospitalSettings.EnableRespawnInHospital)
                    {
                        if (Config.HospitalSettings.hospitals != null)
                            if (Config.HospitalSettings.RandomSpawn)
                            {
                                int i = MathHelper.GenerateRandomNumber(0, Config.HospitalSettings.hospitals.Count - 1);
                                Hospital h = Config.HospitalSettings.hospitals.ElementAt(i);
                                if (h.Position != null)
                                {
                                    i = MathHelper.GenerateRandomNumber(0, h.Position.Count - 1);
                                    Vector3 p = h.Position.ElementAt(i).GetVector3();
                                    player.Teleport(p, player.Rotation);
                                }
                            }
                            else
                            {
                                Hospital hospital = Config.HospitalSettings.hospitals.FirstOrDefault(x => player.HasPermission(x.SpawnPermission.ToLower()));
                                if (hospital != null)
                                    if (hospital.Position != null)
                                    {
                                        int index = MathHelper.GenerateRandomNumber(0, hospital.Position.Count - 1);
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
                TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();

                UnturnedHelper.UpdateHealthUI(player);
                if (comp.DragState != EDragState.NONE)
                    comp.UnDrag();

                EffectManager.sendUIEffectVisibility((short)comp.EffectID, comp.transCon, true, "RevivePanel", false);
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
                TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();
                if (buttonName == "bt_suicide" || buttonName == "bt_suicide2")
                {
                    PlayerHealth health = Database.GetPlayerHealth(player.Id);
                    if (health.isInjured)
                    {
                        comp.allowDamage = true;
                        player.Player.life.askDamage(100, player.Position.normalized, EDeathCause.BLEEDING, ELimb.SKULL, CSteamID.Nil, out EPlayerKill outKill);

                        if (player.Player.movement.pluginSpeedMultiplier == 0)
                            player.Player.movement.sendPluginSpeedMultiplier(1);
                        health.isInjured = false;
                        if (comp.DragState != EDragState.NONE)
                            comp.UnDrag();

                        player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                        EffectManager.sendUIEffectVisibility((short)comp.EffectID, comp.transCon, true, "RevivePanel", false);
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