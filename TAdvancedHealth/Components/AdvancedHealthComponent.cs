using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TAdvancedHealth.Models.Database;
using Tavstal.TAdvancedHealth.Models.Enums;
using Tavstal.TAdvancedHealth.Utils.Helpers;

namespace Tavstal.TAdvancedHealth.Components
{
    public class AdvancedHealthComponent : UnturnedPlayerComponent
    {
        public ITransportConnection transCon => Player.SteamPlayer().transportConnection;
        public bool hasHeavyBleeding = false;
        public ProgressBarDatas progressBarData = new ProgressBarDatas();
        public EDragState DragState = EDragState.NONE;
        public CSteamID DragPartnerId = CSteamID.Nil;

        public Dictionary<ushort, DateTime> lastDefibliratorUses { get; set; } = new Dictionary<ushort, DateTime>();
        public DateTime nextHeadHealDate { get; set; }
        public DateTime nextBodyHealDate { get; set; }
        public DateTime nextArmHealDate { get; set; }
        public DateTime nextLegHealDate { get; set; }


        public bool allowDamage = false;
        public ushort lastEquipedItem = 0;
        public ushort EffectID = 0;
        public List<EPlayerStates> states = new List<EPlayerStates>();

        public void TryAddState(EPlayerStates state)
        {
            try
            {
                if (!states.Contains(state))
                {
                    var config = TAdvancedHealth.Instance.Config;

                    if (state == EPlayerStates.NONE_TEMPERATURE)
                    {
                        StatusIcon icon2 = HealthHelper.GetStatusIcon(EPlayerStates.WARM);
                        if (icon2 != null)
                        {
                            List<StatusIcon> icons = config.HealthSystemSettings.statusIcons.FindAll(x => x.GroupIndex == icon2.GroupIndex && x.Status != state);
                            foreach (StatusIcon ic in icons)
                            {
                                TryRemoveState(ic.Status, false);
                            }
                            RefreshStateUI();
                        }
                        return;
                    }

                    StatusIcon icon = HealthHelper.GetStatusIcon(state);

                    if (icon != null)
                    {
                        if (icon.GroupIndex == -1)
                        {
                            states.Add(state);
                            RefreshStateUI();
                        }
                        else
                        {
                            List<StatusIcon> icons = config.HealthSystemSettings.statusIcons.FindAll(x => x.GroupIndex == icon.GroupIndex && x.Status != state);
                            foreach (StatusIcon ic in icons)
                            {
                                TryRemoveState(ic.Status, false);
                            }

                            states.Add(state);
                            RefreshStateUI();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggerHelper.LogError(e.ToString());
            }
        }

        public void TryRemoveState(EPlayerStates state, bool shouldUpdate = true)
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
            catch (Exception e)
            {
                LoggerHelper.LogError(e.ToString());
            }
        }

        private void RefreshStateUI()
        {
            HealthData health = TAdvancedHealth.Database.GetPlayerHealth(this.Player.Id);
            short effectID = (short)health.HUDEffectID;

            for (int i = 0; i < 12; i++)
            {
                int localuiname = i + 1;

                if (states.Count - 1 >= i)
                {
                    EPlayerStates value = states.ElementAt(i);
                    StatusIcon icon = HealthHelper.GetStatusIcon(value);

                    EffectManager.sendUIEffectImageURL(effectID, this.Player.CSteamID, true, "Status#" + localuiname + "_img", icon.IconUrl);
                    TAdvancedHealth.Instance.InvokeAction(0.1f, () => {
                        if (states.Count >= localuiname)
                            EffectManager.sendUIEffectVisibility(effectID, transCon, true, "Status#" + localuiname, true);
                    });
                }
                else
                {
                    EffectManager.sendUIEffectVisibility(effectID, transCon, true, "Status#" + localuiname, false);
                }
            }
        }
    
        public void Drag(UnturnedPlayer target)
        {
            AdvancedHealthComponent targetComp = target.GetComponent<AdvancedHealthComponent>();

            if (TAdvancedHealth.Database.GetPlayerHealth(this.Player.Id).IsInjured || targetComp.DragState != EDragState.NONE || !TAdvancedHealth.Database.GetPlayerHealth(target.Id).IsInjured || DragState != EDragState.NONE)
                return;

            DragPartnerId = target.CSteamID;
            targetComp.DragPartnerId = this.Player.CSteamID;
            DragState = EDragState.DRAGGER;
            targetComp.DragState = EDragState.DRAGGED;
        }

        public void UnDrag(bool recievedFromPartner = false)
        {
            if (recievedFromPartner)
            {
                UnturnedPlayer partner = UnturnedPlayer.FromCSteamID(DragPartnerId);
                if (partner != null)
                {
                    AdvancedHealthComponent partnerComp = partner.GetComponent<AdvancedHealthComponent>();
                    if (partnerComp != null)
                        partnerComp.UnDrag();
                }
            }

            DragState = EDragState.NONE;
            DragPartnerId = CSteamID.Nil;
        }

        public void Revive(bool recievedFromPartner = false)
        {
            var chsettings = TAdvancedHealth.Instance.CHSCSettings;

            if (this.Player.Player.movement.pluginSpeedMultiplier != chsettings.DefaultWalkSpeed)
                this.Player.Player.movement.sendPluginSpeedMultiplier(chsettings.DefaultWalkSpeed);
            this.Player.Player.movement.sendPluginJumpMultiplier(1f);
            allowDamage = false;
            hasHeavyBleeding = false;
            
            HealthData health = TAdvancedHealth.Database.GetPlayerHealth(this.Player.Id);
            TAdvancedHealth.Database.UpdateHealthAsync(this.Player.Id, new HealthData 
            {
                BaseHealth = chsettings.BaseHealth,
                BodyHealth = chsettings.BodyHealth,
                HeadHealth = chsettings.HeadHealth,
                RightArmHealth = chsettings.RightArmHealth,
                LeftArmHealth = chsettings.LeftArmHealth,
                LeftLegHealth = chsettings.RightLegHealth,
                RightLegHealth = chsettings.LeftLegHealth,
                DeathDate = health.DeathDate,
                HUDEffectID = health.HUDEffectID,
                IsHUDEnabled = health.IsHUDEnabled,
                IsInjured = false,
                PlayerId = health.PlayerId
            }, EDatabaseEvent.ALL);
            this.Player.Broken = false;
            this.Player.Bleeding = false;
            this.Player.Hunger = 0;
            this.Player.Thirst = 0;
            this.Player.Infection = 0;
            this.Player.Heal(100);

            EffectManager.sendUIEffectVisibility((short)EffectID, transCon, true, "RevivePanel", false);
            this.Player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
        }

        public void BleedOut(bool recievedFromPartner = false)
        {
            allowDamage = true;
            this.Player.Player.life.askDamage(100, this.Player.Position.normalized, EDeathCause.BLEEDING, ELimb.SKULL, CSteamID.Nil, out EPlayerKill outKill);
            var chsettings = TAdvancedHealth.Instance.CHSCSettings;
            if (this.Player.Player.movement.pluginSpeedMultiplier != chsettings.DefaultWalkSpeed)
                this.Player.Player.movement.sendPluginSpeedMultiplier(chsettings.DefaultWalkSpeed);
            this.Player.Player.movement.sendPluginJumpMultiplier(1f);
            TAdvancedHealth.Database.UpdateInjured(this.Player.Id, false, DateTime.Now);
            EffectManager.sendUIEffectVisibility((short)EffectID, transCon, true, "RevivePanel", false);
            this.Player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
        }
    }
}
