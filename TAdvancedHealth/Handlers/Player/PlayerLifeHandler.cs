using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TAdvancedHealth.Utils.Helpers;
using Tavstal.TAdvancedHealth.Utils.Managers;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Helpers.Unturned;
using UnityEngine;

namespace Tavstal.TAdvancedHealth.Handlers.Player
{
    public static class PlayerLifeHandler
    {
        private static AdvancedHealthConfig _config => AdvancedHealth.Instance.Config;
        
        internal static void Attach()
        {
            UnturnedPlayerEvents.OnPlayerRevive += OnPlayerRevived;
            DamageTool.damagePlayerRequested += OnPlayerDamaged;
            UnturnedPlayerEvents.OnPlayerDeath += OnPlayerDeath;
        }

        internal static void Detach()
        {
            UnturnedPlayerEvents.OnPlayerRevive -= OnPlayerRevived;
            DamageTool.damagePlayerRequested -= OnPlayerDamaged;
            UnturnedPlayerEvents.OnPlayerDeath -= OnPlayerDeath;
        }
        
        private static void OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            try
            {
                AdvancedHealthComponent comp = ComponentManager.Get(player);

                EffectHelper.UpdateWholeHealthUI(player);
                if (comp.dragState != EDragState.None)
                    comp.UnDrag();

                EffectManager.sendUIEffectVisibility((short)_config.EffectId, comp.TranspConnection, true, "RevivePanel", false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerDeath)}.", ex);
            }
        }
        
        internal static void OnPlayerRevived(UnturnedPlayer player, Vector3 position, byte angle)
        {
            try
            {
                AdvancedHealthComponent comp = ComponentManager.Get(player);
                comp.Revive();
                if (comp.dragState != EDragState.None)
                    comp.UnDrag();


                EffectManager.sendUIEffectVisibility((short)_config.EffectId, comp.TranspConnection, true, "RevivePanel", false);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);

                AdvancedHealth.Instance.InvokeAction(0.1f, () =>
                {
                    
                    if (_config.HospitalSettings.EnableRespawnInHospital)
                    {
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
                            Hospital? hospital = _config.HospitalSettings.Hospitals.FirstOrDefault(x => player.HasPermission(x.Permission.ToLower()));
                            if (hospital is { Position: { } })
                            {
                                int index = MathHelper.Next(0, hospital.Position.Count - 1);
                                Vector3 hPosition = hospital.Position.ElementAt(index).GetVector3();
                                player.Teleport(hPosition, player.Rotation);
                            }
                        }
                    }

                    PlayerStatHandler.OnPlayerFoodUpdate(player, player.Player.life.food);
                    PlayerStatHandler.OnPlayerWaterUpdate(player, player.Player.life.water);
                    PlayerStatHandler.OnPlayerVirusUpdate(player, player.Player.life.virus);
                    PlayerStatHandler.OnPlayerOxygenUpdate(player, player.Player.life.oxygen);
                    PlayerStatHandler.OnPlayerStaminaUpdate(player, player.Player.life.stamina);
                    PlayerStatHandler.OnPlayerBleedingUpdate(player, player.Bleeding);
                    PlayerStatHandler.OnPlayerBrokenUpdate(player, player.Broken);
                    PlayerStatHandler.OnPlayerSafezoneUpdated(player, player.Player.movement.isSafe);
                    PlayerStatHandler.OnPlayerDeadzoneUpdated(player, player.Player.movement.isRadiated);
                    PlayerStatHandler.OnPlayerTemperatureUpdate(player, player.Player.life.temperature);
                });
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerRevived)}.", ex);
            }
        }
        
         private static void OnPlayerDamaged(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(parameters.player);
                AdvancedHealthComponent comp = ComponentManager.Get(player);
                var health = comp.HealthData;
                if (health == null)
                    return;
                var healthSettings = _config.HealthSystemSettings;
                var friendlyFireSettings = _config.AntiGroupFriendlyFireSettings;
                bool allow = true;
                shouldAllow = false;

                /*if (parameters.killer != CSteamID.Nil)
                {
                    UnturnedPlayer killerPlayer = UnturnedPlayer.FromCSteamID(parameters.killer);
                    if (killerPlayer != null && _config.DefibrillatorSettings.Enabled)
                    {
                        AdvancedHealthComponent killerComp = killerPlayer.
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

                if (friendlyFireSettings.Enable)
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
                                List<string> ffGroups = friendlyFireSettings.Groups;

                                foreach (var group in mutualGroups)
                                {
                                    if (!ffGroups.Contains(group.Id.ToLower()))
                                        continue;
                                    shouldAllow = false;
                                    if (!string.IsNullOrEmpty(friendlyFireSettings.MessageIcon))
                                        ChatManager.serverSendMessage(
                                            friendlyFireSettings.Message.Replace('{', '<').Replace('}', '>'),
                                            Color.white, null, attacker.SteamPlayer(), EChatMode.LOCAL,
                                            friendlyFireSettings.MessageIcon, true);
                                    else
                                        UChatHelper.SendPlainChatMessage(attacker.SteamPlayer(),
                                            friendlyFireSettings.Message);
                                    allow = false;
                                    break;
                                }
                            }
                        }
                    }
                }

                player.Player.life.askHeal(100, false, false);
                float totaldamage;

                EDeathCause cause = parameters.cause;
                ELimb limb = parameters.limb;
                CSteamID killer = parameters.killer;

                switch (cause)
                {
                    case EDeathCause.PUNCH:
                        parameters.damage = 1.0f;
                        break;
                    case EDeathCause.BONES:
                        parameters.damage = 10.0f;
                        player.Broken = true;
                        break;
                    case EDeathCause.BLEEDING when health.IsInjured && !comp.allowDamage:
                        parameters.damage = 0;
                        player.Bleeding = false;
                        break;
                    case EDeathCause.BLEEDING:
                    {
                        parameters.damage = comp.hasHeavyBleeding ? healthSettings.Combat.HeavyBleedingDamage : healthSettings.Combat.BleedingDamage;
                        player.Bleeding = true;
                        break;
                    }
                    case EDeathCause.ANIMAL:
                    case EDeathCause.ZOMBIE:
                        limb = ELimb.LEFT_FRONT;
                        break;
                }

                comp.allowDamage = false;

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

                HandleIncomingDamage(player, health, killer, totaldamage, limb, cause, player.Position.normalized);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerDamaged)}.", ex);
            }
        }
         
        internal static void OnPlayerLifeDamaged(SDG.Unturned.Player p, byte damage, Vector3 force, EDeathCause cause, ELimb limb, CSteamID killer)
        {
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(p);
                AdvancedHealthComponent comp = ComponentManager.Get(player);

                player.Player.life.askHeal(100, false, false);
                var health = comp.HealthData;
                if (health == null)
                    return;
                
                float totalDamage = damage;

                switch (cause)
                {
                    case EDeathCause.PUNCH:
                        totalDamage = 1.0f;
                        break;
                    case EDeathCause.BONES:
                        totalDamage = 10.0f;
                        player.Broken = true;
                        break;
                    case EDeathCause.BLEEDING when health.IsInjured && !comp.allowDamage:
                        totalDamage = 0;
                        player.Bleeding = false;
                        break;
                    case EDeathCause.BLEEDING:
                    {
                        totalDamage = comp.hasHeavyBleeding ? _config.HealthSystemSettings.Combat.HeavyBleedingDamage : _config.HealthSystemSettings.Combat.BleedingDamage;
                        player.Bleeding = true;
                        break;
                    }
                    case EDeathCause.ANIMAL:
                    case EDeathCause.ZOMBIE:
                        limb = ELimb.LEFT_FRONT;
                        break;
                }

                comp.allowDamage = false;

                if (player.Features.GodMode)
                    totalDamage = 0;

                HandleIncomingDamage(player, health, killer, totalDamage, limb, cause, player.Position.normalized);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerLifeDamaged)}.", ex);
            }
        }
        
        private static void HandleIncomingDamage(UnturnedPlayer player, Health health, CSteamID killer, float totalDamage, ELimb limb, EDeathCause cause, Vector3 ragdoll)
        {
            AdvancedHealthComponent comp = ComponentManager.Get(player);
            switch (limb)
            {
                // HEAD
                case ELimb.SKULL:
                {

                    if (HealthHelper.CanBleed(health.HeadHealth, totalDamage))
                        player.Bleeding = true;

                    health.SetHeadHealth(health.HeadHealth - totalDamage);

                    if (health.HeadHealth > 0)
                        break;

                    if (_config.HealthSystemSettings.Combat.CanBeInjured && !health.IsInjured)
                    {
                        int chanc = MathHelper.Next(1, 100);
                        if (_config.HealthSystemSettings.Combat.InjuredChance >= chanc)
                        {
                            HealthHelper.SetPlayerDowned(player);
                            return;
                        }
                    }

                    if (!_config.HealthSystemSettings.Restrictions.DieWhenHeadHealthIsZero)
                        break;

                    comp.allowDamage = true;
                    CSteamID id = CSteamID.Nil;
                    if (EDeathCause.ZOMBIE != cause)
                    {
                        if (killer != CSteamID.Nil)
                            id = killer;
                    }

                    player.Player.life.askDamage(100, ragdoll, cause, limb, id, out _);
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

                    health.SetBodyHealth(health.BodyHealth - totalDamage);

                    if (health.BodyHealth > 0)
                        break;

                    if (_config.HealthSystemSettings.Combat.CanBeInjured && !health.IsInjured)
                    {
                        int chanc = MathHelper.Next(1, 100);
                        if (_config.HealthSystemSettings.Combat.InjuredChance >= chanc)
                        {
                            HealthHelper.SetPlayerDowned(player);
                            return;
                        }
                    }

                    if (!_config.HealthSystemSettings.Restrictions.DieWhenBodyHealthIsZero)
                        break;
                    
                    comp.allowDamage = true;
                    CSteamID id = CSteamID.Nil;
                    if (EDeathCause.ZOMBIE != cause)
                    {
                        if (killer != CSteamID.Nil)
                            id = killer;
                    }

                    player.Player.life.askDamage(100, ragdoll, cause, limb, id, out _);
                    break;
                }
                // LEFT ARM
                case ELimb.LEFT_ARM:
                case ELimb.LEFT_HAND:
                {
                    if (HealthHelper.CanBleed(health.LeftArmHealth, totalDamage))
                        player.Bleeding = true;

                    health.SetLeftArmHealth(health.LeftArmHealth - totalDamage);

                    if (health.LeftArmHealth + health.RightArmHealth > 0)
                        break;

                    if (!_config.HealthSystemSettings.Restrictions.DieWhenArmsHealthIsZero)
                        break;
                    
                    comp.allowDamage = true;
                    CSteamID id = CSteamID.Nil;
                    if (EDeathCause.ZOMBIE != cause)
                    {
                        if (killer != CSteamID.Nil)
                            id = killer;
                    }

                    player.Player.life.askDamage(100, ragdoll, cause, limb, id, out _);
                    break;
                }
                // RIGHT ARM
                case ELimb.RIGHT_ARM:
                case ELimb.RIGHT_HAND:
                {
                    if (HealthHelper.CanBleed(health.RightArmHealth, totalDamage))
                        player.Bleeding = true;

                    health.SetRightArmHealth(health.RightArmHealth - totalDamage);

                    if (health.RightArmHealth + health.RightArmHealth > 0)
                        break;

                    if (!_config.HealthSystemSettings.Restrictions.DieWhenArmsHealthIsZero)
                        break;
                    
                    comp.allowDamage = true;
                    CSteamID id = CSteamID.Nil;
                    if (EDeathCause.ZOMBIE != cause)
                    {
                        if (killer != CSteamID.Nil)
                            id = killer;
                    }

                    player.Player.life.askDamage(100, ragdoll, cause, limb, id, out _);
                    break;
                }
                // LEFT LEG
                case ELimb.LEFT_LEG:
                case ELimb.LEFT_FOOT:
                {
                    if (HealthHelper.CanBleed(health.LeftLegHealth, totalDamage))
                        player.Bleeding = true;

                    health.SetLeftLegHealth(health.LeftLegHealth - totalDamage);

                    if (health.LeftLegHealth + health.RightLegHealth > 0)
                        break;

                    if (!_config.HealthSystemSettings.Restrictions.DieWhenLegsHealthIsZero)
                        break;
                    
                    comp.allowDamage = true;
                    CSteamID id = CSteamID.Nil;
                    if (EDeathCause.ZOMBIE != cause)
                    {
                        if (killer != CSteamID.Nil)
                            id = killer;
                    }

                    player.Player.life.askDamage(100, ragdoll, cause, limb, id, out _);
                    break;
                }
                // RIGHT LEG
                case ELimb.RIGHT_LEG:
                case ELimb.RIGHT_FOOT:
                {
                    if (HealthHelper.CanBleed(health.RightLegHealth, totalDamage))
                        player.Bleeding = true;

                    health.SetRightLegHealth(health.RightLegHealth - totalDamage);

                    if (health.RightLegHealth + health.RightLegHealth > 0)
                        break;
                    
                    if (!_config.HealthSystemSettings.Restrictions.DieWhenLegsHealthIsZero)
                        break;
                    
                    comp.allowDamage = true;
                    CSteamID id = CSteamID.Nil;
                    if (EDeathCause.ZOMBIE != cause)
                    {
                        if (killer != CSteamID.Nil)
                            id = killer;
                    }

                    player.Player.life.askDamage(100, ragdoll, cause, limb, id, out _);
                    break;
                }
                default:
                {
                    if (cause == EDeathCause.BONES)
                    {
                        if (HealthHelper.CanBleed(health.LeftLegHealth, totalDamage) ||
                            HealthHelper.CanBleed(health.RightLegHealth, totalDamage))
                            player.Bleeding = true;

                        health.SetRightLegHealth(health.RightLegHealth - totalDamage);
                        health.SetLeftLegHealth(health.LeftLegHealth - totalDamage);

                        if (health.LeftLegHealth + health.RightLegHealth > 0)
                            return;
                        if (!_config.HealthSystemSettings.Restrictions.DieWhenLegsHealthIsZero)
                            return;
                        comp.allowDamage = true;
                        CSteamID id = CSteamID.Nil;
                        if (killer != CSteamID.Nil)
                            id = killer;
                        player.Player.life.askDamage(100, ragdoll, cause, limb, id, out EPlayerKill _);
                        return;
                    }
                    
                    if (HealthHelper.CanBleed(health.BodyHealth, totalDamage))
                        player.Bleeding = true;

                    health.SetBodyHealth(health.BodyHealth - totalDamage);

                    if (health.BodyHealth > 0)
                        break;

                    if (_config.HealthSystemSettings.Combat.CanBeInjured && !health.IsInjured)
                    {
                        int chanc = MathHelper.Next(1, 100);
                        if (_config.HealthSystemSettings.Combat.InjuredChance >= chanc)
                        {
                            HealthHelper.SetPlayerDowned(player);
                            return;
                        }
                    }

                    if (!_config.HealthSystemSettings.Restrictions.DieWhenBodyHealthIsZero)
                        break;
                    
                    comp.allowDamage = true;
                    CSteamID killerId = CSteamID.Nil;
                    if (EDeathCause.ZOMBIE != cause)
                    {
                        if (killer != CSteamID.Nil)
                            killerId = killer;
                    }

                    player.Player.life.askDamage(100, ragdoll, cause, limb, killerId, out _);
                    break;
                }
            }
        }
    }
}