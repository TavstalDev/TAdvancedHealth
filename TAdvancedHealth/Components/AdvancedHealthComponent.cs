using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using Tavstal.TAdvancedHealth.Models;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TAdvancedHealth.Utils.Helpers;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Threading;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tavstal.TAdvancedHealth.Components
{
    public class AdvancedHealthComponent : UnturnedPlayerComponent
    {
        public ITransportConnection TranspConnection => Player.SteamPlayer().transportConnection;
        [FormerlySerializedAs("HasHeavyBleeding")] 
        public bool hasHeavyBleeding;
        public readonly ProgressBarDatas ProgressBarData = new ProgressBarDatas();
        [FormerlySerializedAs("DragState")] 
        public EDragState dragState = EDragState.None;
        [FormerlySerializedAs("DragPartnerId")] 
        public CSteamID dragPartnerId = CSteamID.Nil;
        public Health? HealthData {  get; set; }

        public Dictionary<ushort, DateTime> LastDefibliratorUses { get; set; } = new Dictionary<ushort, DateTime>();
        private DateTime _nextHeadHealDate;
        private DateTime _nextBodyHealDate;
        private DateTime _nextArmHealDate;
        private DateTime _nextLegHealDate;


        [FormerlySerializedAs("AllowDamage")] 
        public bool allowDamage;
        [FormerlySerializedAs("LastEquipedItem")] 
        public ushort lastEquipedItem;
        [FormerlySerializedAs("EffectID")] 
        public ushort effectId;
        [FormerlySerializedAs("States")] 
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
                    HUDStyle style = AdvancedHealth.Instance.Config.HUDStyles.FirstOrDefault(x => x.Enable) ??
                                     AdvancedHealth.Instance.Config.HUDStyles[0];

                    HealthData = new Health(Player.Id, cshSettings.BaseHealth, cshSettings.HeadHealth, cshSettings.BodyHealth,
                        cshSettings.RightArmHealth, cshSettings.LeftArmHealth, cshSettings.RightLegHealth, cshSettings.LeftLegHealth,
                        false, true, style.EffectID);
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

        public void TryAddState(EPlayerState state)
        {
            try
            {
                if (!states.Contains(state))
                {
                    var config = AdvancedHealth.Instance.Config;

                    if (state == EPlayerState.NoneTemperature)
                    {
                        StatusIcon? icon2 = HealthHelper.GetStatusIcon(EPlayerState.Warm);
                        if (icon2 != null)
                        {
                            List<StatusIcon> icons = config.HealthSystemSettings.StatusIcons.FindAll(x => x.GroupIndex == icon2.GroupIndex && x.Status != state);
                            foreach (StatusIcon ic in icons)
                                TryRemoveState(ic.Status, false);
                            RefreshStateUI();
                        }
                        return;
                    }

                    StatusIcon? icon = HealthHelper.GetStatusIcon(state);

                    if (icon != null)
                    {
                        if (icon.GroupIndex == -1)
                        {
                            states.Add(state);
                            RefreshStateUI();
                        }
                        else
                        {
                            List<StatusIcon> icons = config.HealthSystemSettings.StatusIcons.FindAll(x => x.GroupIndex == icon.GroupIndex && x.Status != state);
                            foreach (StatusIcon ic in icons)
                                TryRemoveState(ic.Status, false);

                            states.Add(state);
                            RefreshStateUI();
                        }
                    }
                }
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
                if (states.Contains(state))
                {
                    states.RemoveAt(states.FindIndex(x => x == state));
                    if (shouldUpdate)
                        RefreshStateUI();
                }
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
            
            short effectID = (short)HealthData.HUDEffectID;

            for (int i = 0; i < 12; i++)
            {
                int localuiname = i + 1;

                if (states.Count - 1 >= i)
                {
                    EPlayerState value = states.ElementAt(i);
                    StatusIcon? icon = HealthHelper.GetStatusIcon(value);
                    if (icon == null)
                        continue;

                    EffectManager.sendUIEffectImageURL(effectID, TranspConnection, true, "Status#" + localuiname + "_img", icon.IconUrl);
                    AdvancedHealth.Instance.InvokeAction(0.1f, () => {
                        if (states.Count >= localuiname)
                            EffectManager.sendUIEffectVisibility(effectID, TranspConnection, true, "Status#" + localuiname, true);
                    });
                }
                else
                {
                    EffectManager.sendUIEffectVisibility(effectID, TranspConnection, true, "Status#" + localuiname, false);
                }
            }
        }

        public void Drag(UnturnedPlayer target)
        {
            AdvancedHealthComponent targetComp = target.GetComponent<AdvancedHealthComponent>();
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
            AdvancedHealthComponent partnerComp = partner.GetComponent<AdvancedHealthComponent>();
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

            EffectManager.sendUIEffectVisibility((short)effectId, TranspConnection, true, "RevivePanel", false);
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

            EffectManager.sendUIEffectVisibility((short)effectId, TranspConnection, true, "RevivePanel", false);
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
                EffectManager.sendUIEffectText((short)effectId, TranspConnection, true, "tb_message",
                    AdvancedHealth.Instance.Localize("ui_bleeding", secs.ToString()));
                if (HealthData.DeathDate < DateTime.Now)
                {
                    BleedOut();
                    return;
                }
            }

            #endregion

            #region Regeneration

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
