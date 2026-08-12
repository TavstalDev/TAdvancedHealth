using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using Tavstal.RocketFlow.Attributes;
using Tavstal.RocketFlow.Core;
using Tavstal.RocketFlow.Events.Damage;
using Tavstal.RocketFlow.Events.Player.Life;
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
// ReSharper disable UnusedMember.Local

namespace Tavstal.TAdvancedHealth.Handlers.Player
{
    public class PlayerLifeListener : EventListener
    {
        private static AdvancedHealthConfig _config => AdvancedHealth.Instance.Config;
        
        [EventHandler]
        private void OnDeath(PlayerDeathEvent e)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(e.Player);
                if (comp == null)
                    return;

                EffectHelper.UpdateWholeHealthUI(e.Player);
                if (comp.dragState != EDragState.None)
                    comp.UnDrag();

                EffectManager.sendUIEffectVisibility((short)_config.EffectId, comp.TranspConnection, true, "RevivePanel", false);
                e.Player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnDeath)}.", ex);
            }
        }
        
        [EventHandler]
        private void OnRevived(PlayerReviveEvent e)
        {
            try
            {
                var player = e.Player;
                AdvancedHealthComponent? comp = ComponentManager.Get(player);
                if (comp == null)
                    return;
                
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
                    
                    PlayerStatListener.FoodUpdate(player, player.Player.life.food);
                    PlayerStatListener.WaterUpdate(player, player.Player.life.water);
                    PlayerStatListener.VirusUpdate(player, player.Player.life.virus);
                    PlayerStatListener.OxygenUpdate(player, player.Player.life.oxygen);
                    PlayerStatListener.StaminaUpdate(player, player.Player.life.stamina);
                    PlayerStatListener.BleedingUpdate(player, player.Bleeding);
                    PlayerStatListener.BonesUpdate(player, player.Broken);
                    PlayerStatListener.SafezoneUpdated(player, player.Player.movement.isSafe);
                    PlayerStatListener.DeadzoneUpdated(player, player.Player.movement.isRadiated);
                    PlayerStatListener.TemperatureUpdate(player, player.Player.life.temperature);
                });
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnRevived)}.", ex);
            }
        }
        
        [EventHandler(priority: EEventPriority.LOWEST)]
        private void OnDamaged(PlayerDamageEvent e)
        {
            try
            {
                UnturnedPlayer victim = UnturnedPlayer.FromPlayer(e.Parameters.player);
                if (victim == null)
                    return;
                
                AdvancedHealthComponent? comp = ComponentManager.Get(victim);
                if (comp == null)
                    return;
                
                var health = comp.HealthData;
                if (health == null)
                    return;
                var healthSettings = _config.HealthSystemSettings;
                var friendlyFireSettings = _config.AntiGroupFriendlyFireSettings;
                bool allow = true;
                bool shouldAllow = false;

                try
                {
                    if (friendlyFireSettings.Enable)
                    {
                        UnturnedPlayer attacker = UnturnedPlayer.FromCSteamID(e.Parameters.killer);
                        if (attacker != null)
                        {
                            if (attacker.CSteamID != victim.CSteamID)
                            {
                                EDeathCause cause2 = e.Parameters.cause;
                                if (cause2 == EDeathCause.CHARGE || cause2 == EDeathCause.GRENADE ||
                                    cause2 == EDeathCause.GUN || cause2 == EDeathCause.LANDMINE ||
                                    cause2 == EDeathCause.MELEE || cause2 == EDeathCause.MISSILE ||
                                    cause2 == EDeathCause.PUNCH || cause2 == EDeathCause.ROADKILL ||
                                    cause2 == EDeathCause.SENTRY)
                                {
                                    List<RocketPermissionsGroup> mutualGroups =
                                        UPlayerHelper.GetMutualGroups(victim, attacker);
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

                    victim.Player.life.askHeal(100, false, false);
                    float totaldamage;

                    EDeathCause cause = e.Parameters.cause;
                    ELimb limb = e.Parameters.limb;
                    CSteamID killer = e.Parameters.killer;
                    var param = e.Parameters;

                    switch (cause)
                    {
                        case EDeathCause.PUNCH:
                            param.damage = 1.0f;
                            break;
                        case EDeathCause.BONES:
                            param.damage = 10.0f;
                            victim.Broken = true;
                            break;
                        case EDeathCause.BLEEDING when health.IsInjured && !comp.allowDamage:
                            param.damage = 0;
                            victim.Bleeding = false;
                            break;
                        case EDeathCause.BLEEDING:
                        {
                            param.damage = comp.hasHeavyBleeding
                                ? healthSettings.Combat.HeavyBleedingDamage
                                : healthSettings.Combat.BleedingDamage;
                            victim.Bleeding = true;
                            break;
                        }
                        case EDeathCause.ANIMAL:
                        case EDeathCause.ZOMBIE:
                            limb = ELimb.LEFT_FRONT;
                            break;
                    }

                    comp.allowDamage = false;

                    if (e.Parameters.respectArmor)
                    {
                        param.times *= DamageTool.getPlayerArmor(e.Parameters.limb, victim.Player);
                        if (e.Parameters.applyGlobalArmorMultiplier)
                            param.times *= Provider.modeConfigData.Players.Armor_Multiplier;
                        int b = Mathf.FloorToInt(e.Parameters.damage * e.Parameters.times);
                        totaldamage = Mathf.Min(byte.MaxValue, b);
                    }
                    else
                        totaldamage = e.Parameters.times * e.Parameters.damage;

                    if (!allow || victim.Features.GodMode)
                        totaldamage = 0;

                    e.Parameters = param;
                    HandleIncomingDamage(victim, health, killer, totaldamage, limb, cause, victim.Position.normalized);
                }
                finally
                {
                    e.ShouldAllow = shouldAllow;
                    e.IsCancelled = !shouldAllow;
                }
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnDamaged)}.", ex);
            }
        }
         
        [EventHandler]
        private void OnLifeDamaged(PlayerHurtEvent e)
        {
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(e.Player);
                AdvancedHealthComponent? comp = ComponentManager.Get(player);
                if (comp == null)
                    return;

                e.Player.life.askHeal(100, false, false);
                var health = comp.HealthData;
                if (health == null)
                    return;
                
                float totalDamage = e.Damage;
                var limb = e.Limb;
                
                switch (e.Cause)
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

                HandleIncomingDamage(player, health, e.Killer, totalDamage, limb, e.Cause, player.Position.normalized);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnLifeDamaged)}.", ex);
            }
        }
        
        private static void HandleIncomingDamage(UnturnedPlayer player, Health health, CSteamID killer, float totalDamage, ELimb limb, EDeathCause cause, Vector3 ragdoll)
        {
            AdvancedHealthComponent? comp = ComponentManager.Get(player);
            if (comp == null)
                return;
            
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