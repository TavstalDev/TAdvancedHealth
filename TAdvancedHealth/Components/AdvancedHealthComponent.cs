using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using Tavstal.TAdvancedHealth.Models;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TAdvancedHealth.Utils.Managers;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Threading;
using UnityEngine;

namespace Tavstal.TAdvancedHealth.Components
{
    public class AdvancedHealthComponent : UnturnedPlayerComponent
    {
        public ITransportConnection TranspConnection => Player.SteamPlayer().transportConnection;
        public ProgressbarData ProgressbarData { get; } = new ProgressbarData();
        public Health? HealthData { get; private set; }
        public bool hasHeavyBleeding { get; set; }
        public EDragState dragState { get; set; } = EDragState.None;
        public CSteamID dragPartnerId { get; set; } = CSteamID.Nil;

        public Dictionary<ushort, DateTime> LastDefibliratorUses { get; set; } = new Dictionary<ushort, DateTime>();
        private DateTime _nextHeadHealDate;
        private DateTime _nextBodyHealDate;
        private DateTime _nextArmHealDate;
        private DateTime _nextLegHealDate;

        
        public bool allowDamage;
        public ushort lastEquipedItem;
        public EPlayerTemperature currentTemperature;
        public List<EPlayerState> states = new List<EPlayerState>();

        protected override void Load()
        {
            BackgroundThreadDispatcher.Run(async () =>
            {
                try
                {
                    var healthData = await AdvancedHealth.DatabaseManager.HealthData.GetAsync(Player.Id);
                    if (healthData != null)
                    {
                        HealthData = new Health(healthData);
                        return;
                    }
                    
                    var cshSettings = AdvancedHealth.Instance.Config.HealthSystemSettings;
                    HealthData = new Health(Player.Id, cshSettings.BaseHealth, cshSettings.HeadHealth, cshSettings.BodyHealth,
                        cshSettings.RightArmHealth, cshSettings.LeftArmHealth, cshSettings.RightLegHealth, cshSettings.LeftLegHealth,
                        false);
                }
                catch (Exception ex)
                {
                    AdvancedHealth.Logger.Error("Failed to get player health data.", ex);
                }
            });
        }

        protected override void Unload()
        {
            if (HealthData == null)
                return;
            var healthData = HealthData.ToHealthData();
            BackgroundThreadDispatcher.Run(async () =>
            {
                try
                {
                    await AdvancedHealth.DatabaseManager.HealthData.UpdateAsync(Player.Id, healthData);
                }
                catch (Exception ex)
                {
                    AdvancedHealth.Logger.Error("Failed to get player health data.", ex);
                }
            });
        }

        public void TryAddState(EPlayerState state, bool shouldUpdate = true)
        {
            try
            {
                if (state == EPlayerState.TEMPERATURE_NONE)
                    return;

                if (states.Contains(state))
                    return;
                
                states.Add(state);
                if (shouldUpdate)
                    RefreshStateUI();
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.Error(e.ToString());
            }
        }

        public void TryRemoveState(EPlayerState state, bool shouldUpdate = true)
        {
            try
            {
                if (!states.Contains(state))
                    return;
                
                states.RemoveAt(states.FindIndex(x => x == state));
                if (shouldUpdate)
                    RefreshStateUI();
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error("Failed to remove state.", ex);
            }
        }
        
        private void RefreshStateUI()
        {
            if (HealthData == null)
                return;
            
            short effectID = (short)AdvancedHealth.Instance.Config.EffectId;
            for (int i = 1; i < 18; i++)
            {
                EPlayerState state = (EPlayerState)i;
                if (!states.Contains(state))
                {
                    EffectManager.sendUIEffectVisibility(effectID, TranspConnection, true, $"Status#{i}", false);
                    continue;
                }
                EffectManager.sendUIEffectVisibility(effectID, TranspConnection, true, $"Status#{i}", true);
            }
        }

        public void Drag(UnturnedPlayer target)
        {
            AdvancedHealthComponent targetComp = ComponentManager.Get(target);
            var targetHealth = targetComp.HealthData;
            if (targetHealth == null)
                return;
            if (targetHealth.IsInjured || !targetHealth.IsInjured || targetComp.dragState != EDragState.None || dragState != EDragState.None)
                return;

            dragPartnerId = target.CSteamID;
            targetComp.dragPartnerId = Player.CSteamID;
            dragState = EDragState.Dragger;
            targetComp.dragState = EDragState.Dragged;
        }
        
        public void UnDrag(bool receivedFromPartner = false)
        {
            dragState = EDragState.None;
            dragPartnerId = CSteamID.Nil;

            if (receivedFromPartner)
                return;
            UnturnedPlayer partner = UnturnedPlayer.FromCSteamID(dragPartnerId);
            if (partner == null)
                return;
            AdvancedHealthComponent partnerComp = ComponentManager.Get(partner);
            if (partnerComp == null)
                return;
            partnerComp.UnDrag(true);
        }
        
        public void Revive(bool receivedFromPartner = false)
        {
            var chsettings = AdvancedHealth.Instance.Config.HealthSystemSettings;

            if (!Mathf.Approximately(Player.Player.movement.pluginSpeedMultiplier, chsettings.Movement.DefaultWalkSpeed))
                Player.Player.movement.sendPluginSpeedMultiplier(chsettings.Movement.DefaultWalkSpeed);
            Player.Player.movement.sendPluginJumpMultiplier(1f);
            allowDamage = false;
            hasHeavyBleeding = false;

            if (HealthData != null)
            {
                HealthData.SetBaseHealth(chsettings.BaseHealth);
                HealthData.SetHeadHealth(chsettings.HeadHealth);
                HealthData.SetBodyHealth(chsettings.BodyHealth);
                HealthData.SetLeftArmHealth(chsettings.LeftArmHealth);
                HealthData.SetRightArmHealth(chsettings.RightArmHealth);
                HealthData.SetLeftLegHealth(chsettings.LeftLegHealth);
                HealthData.SetRightLegHealth(chsettings.RightLegHealth);
                HealthData.SetInjured(false);
            }

            Player.Broken = false;
            Player.Bleeding = false;
            Player.Hunger = 0;
            Player.Thirst = 0;
            Player.Infection = 0;
            Player.Heal(100);

            EffectManager.sendUIEffectVisibility((short)AdvancedHealth.Instance.Config.EffectId, TranspConnection, true, "RevivePanel", false);
            Player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
        }
        
        public void BleedOut(bool receivedFromPartner = false)
        {
            allowDamage = true;
            Player.Player.life.askDamage(100, Player.Position.normalized, EDeathCause.BLEEDING, ELimb.SKULL, CSteamID.Nil, out _);
            var chsettings = AdvancedHealth.Instance.Config.HealthSystemSettings;
            if (!Mathf.Approximately(Player.Player.movement.pluginSpeedMultiplier, chsettings.Movement.DefaultWalkSpeed))
                Player.Player.movement.sendPluginSpeedMultiplier(chsettings.Movement.DefaultWalkSpeed);
            Player.Player.movement.sendPluginJumpMultiplier(1f);

            HealthData?.SetInjured(false);

            EffectManager.sendUIEffectVisibility((short)AdvancedHealth.Instance.Config.EffectId, TranspConnection, true, "RevivePanel", false);
            Player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
        }

        private void Update()
        {
            if (dragState == EDragState.Dragger && dragPartnerId != CSteamID.Nil)
            {
                UnturnedPlayer partner = UnturnedPlayer.FromCSteamID(dragPartnerId);

                if (partner != null)
                    if (Vector3.Distance(partner.Position, Player.Position) > 3)
                        partner.Player.teleportToPlayer(Player.Player);
            }

            if (HealthData == null)
                return;

            #region Injured

            if (HealthData.IsInjured)
            {
                Player.Bleeding = false;

                int secs = (int)(HealthData.DeathDate - DateTime.Now).TotalSeconds;
                EffectManager.sendUIEffectText((short)AdvancedHealth.Instance.Config.EffectId, TranspConnection, true, "tb_message",
                    AdvancedHealth.Instance.Localize("ui_bleeding", secs.ToString()));
                if (HealthData.DeathDate < DateTime.Now)
                {
                    BleedOut();
                    return;
                }
            }

            #endregion

            #region Regeneration
            
            if (Player.Dead)
                return;

            bool canRegenerate =
                Player.Player.life.food >= AdvancedHealth.Instance.Config.HealthSystemSettings.Regen.HealthRegenMinFood &&
                Player.Player.life.water >=
                AdvancedHealth.Instance.Config.HealthSystemSettings.Regen.HealthRegenMinWater &&
                Player.Player.life.virus >= AdvancedHealth.Instance.Config.HealthSystemSettings.Regen.HealthRegenMinVirus;

            // Head
            if (_nextHeadHealDate <= DateTime.Now)
            {
                if (canRegenerate)
                    HealthData.SetHeadHealth(HealthData.HeadHealth + 1);

                _nextHeadHealDate =
                    DateTime.Now.AddSeconds(AdvancedHealth.Instance.Config.HealthSystemSettings.Regen.HeadRegenTicks);
            }

            // Body
            if (_nextBodyHealDate <= DateTime.Now)
            {
                if (canRegenerate)
                    HealthData.SetBodyHealth(HealthData.BodyHealth + 1);

                _nextBodyHealDate =
                    DateTime.Now.AddSeconds(AdvancedHealth.Instance.Config.HealthSystemSettings.Regen.BodyRegenTicks);
            }

            // Arm
            if (_nextArmHealDate <= DateTime.Now)
            {
                if (canRegenerate)
                {
                    HealthData.SetLeftArmHealth(HealthData.LeftArmHealth + 1);
                    HealthData.SetRightArmHealth(HealthData.RightArmHealth + 1);
                }

                _nextArmHealDate =
                    DateTime.Now.AddSeconds(AdvancedHealth.Instance.Config.HealthSystemSettings.Regen.ArmRegenTicks);
            }

            // Leg
            if (_nextLegHealDate <= DateTime.Now)
            {
                if (canRegenerate)
                {
                    HealthData.SetLeftLegHealth(HealthData.LeftLegHealth + 1);
                    HealthData.SetRightLegHealth(HealthData.RightLegHealth + 1);
                }

                _nextLegHealDate =
                    DateTime.Now.AddSeconds(AdvancedHealth.Instance.Config.HealthSystemSettings.Regen.LegRegenTicks);
            }

            #endregion
        }
    }
}
