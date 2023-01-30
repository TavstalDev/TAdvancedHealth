using Steamworks;
using System;
using System.Linq;
using System.Collections.Generic;
using Rocket.Unturned.Player;
using Tavstal.TAdvancedHealth.Compatibility;
using Tavstal.TAdvancedHealth.Managers;
using SDG.Unturned;
using SDG.NetTransport;
using Tavstal.TAdvancedHealth.Helpers;
using System.Runtime;

namespace Tavstal.TAdvancedHealth.Modules
{
    public class TAdvancedHealthComponent : UnturnedPlayerComponent
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
                    var config = TAdvancedHealthMain.Instance.Configuration.Instance;

                    if (state == EPlayerStates.NONE_TEMPERATURE)
                    {
                        StatusIcon icon2 = UnturnedHelper.GetStatusIcon(EPlayerStates.WARM);
                        if (icon2 != null)
                        {
                            List<StatusIcon> icons = config.CustomHealtSystemAndComponentSettings.statusIcons.FindAll(x => x.GroupIndex == icon2.GroupIndex && x.Status != state);
                            foreach (StatusIcon ic in icons)
                            {
                                TryRemoveState(ic.Status, false);
                            }
                            RefreshStateUI();
                        }
                        return;
                    }

                    StatusIcon icon = UnturnedHelper.GetStatusIcon(state);

                    if (icon != null)
                    {
                        if (icon.GroupIndex == -1)
                        {
                            states.Add(state);
                            RefreshStateUI();
                        }
                        else
                        {
                            List<StatusIcon> icons = config.CustomHealtSystemAndComponentSettings.statusIcons.FindAll(x => x.GroupIndex == icon.GroupIndex && x.Status != state);
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
                Logger.LogError(e.ToString());
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
                Logger.LogError(e.ToString());
            }
        }

        private void RefreshStateUI()
        {
            PlayerHealth health = TAdvancedHealthMain.Database.GetPlayerHealth(this.Player.Id);
            short effectID = (short)health.HUDEffectID;

            for (int i = 0; i < 12; i++)
            {
                int localuiname = i + 1;

                if (states.Count - 1 >= i)
                {
                    EPlayerStates value = states.ElementAt(i);
                    StatusIcon icon = UnturnedHelper.GetStatusIcon(value);

                    EffectManager.sendUIEffectImageURL(effectID, this.Player.CSteamID, true, "Status#" + localuiname + "_img", icon.IconUrl);
                    StartCoroutine(TAdvancedHealthMain.Instance.DelayedInvoke(0.1f, () => {
                        if (states.Count >= localuiname)
                            EffectManager.sendUIEffectVisibility(effectID, transCon, true, "Status#" + localuiname, true);
                    }));
                }
                else
                {
                    EffectManager.sendUIEffectVisibility(effectID, transCon, true, "Status#" + localuiname, false);
                }
            }
        }
    
        public void Drag(UnturnedPlayer target)
        {
            TAdvancedHealthComponent targetComp = target.GetComponent<TAdvancedHealthComponent>();

            if (TAdvancedHealthMain.Database.GetPlayerHealth(this.Player.Id).isInjured || targetComp.DragState != EDragState.NONE || !TAdvancedHealthMain.Database.GetPlayerHealth(target.Id).isInjured || DragState != EDragState.NONE)
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
                    TAdvancedHealthComponent partnerComp = partner.GetComponent<TAdvancedHealthComponent>();
                    if (partnerComp != null)
                        partnerComp.UnDrag();
                }
            }

            DragState = EDragState.NONE;
            DragPartnerId = CSteamID.Nil;
        }

        public void Revive(bool recievedFromPartner = false)
        {
            var chsettings = TAdvancedHealthMain.Instance.CHSCSettings;

            if (this.Player.Player.movement.pluginSpeedMultiplier != chsettings.DefaultWalkSpeed)
                this.Player.Player.movement.sendPluginSpeedMultiplier(chsettings.DefaultWalkSpeed);
            this.Player.Player.movement.sendPluginJumpMultiplier(1f);
            allowDamage = false;
            hasHeavyBleeding = false;
            
            PlayerHealth health = TAdvancedHealthMain.Database.GetPlayerHealth(this.Player.Id);
            TAdvancedHealthMain.Database.Update(this.Player.Id, new PlayerHealth 
            {
                BaseHealth = chsettings.BaseHealth,
                BodyHealth = chsettings.BodyHealth,
                HeadHealth = chsettings.HeadHealth,
                RightArmHealth = chsettings.RightArmHealth,
                LeftArmHealth = chsettings.LeftArmHealth,
                LeftLegHealth = chsettings.RightLegHealth,
                RightLegHealth = chsettings.LeftLegHealth,
                dieDate = health.dieDate,
                HUDEffectID = health.HUDEffectID,
                isHUDEnabled = health.isHUDEnabled,
                isInjured = false,
                PlayerId = health.PlayerId
            });
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
            var chsettings = TAdvancedHealthMain.Instance.CHSCSettings;
            if (this.Player.Player.movement.pluginSpeedMultiplier != chsettings.DefaultWalkSpeed)
                this.Player.Player.movement.sendPluginSpeedMultiplier(chsettings.DefaultWalkSpeed);
            this.Player.Player.movement.sendPluginJumpMultiplier(1f);
            TAdvancedHealthMain.Database.UpdateInjured(this.Player.Id, false, DateTime.Now);
            EffectManager.sendUIEffectVisibility((short)EffectID, transCon, true, "RevivePanel", false);
            this.Player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
        }
    }
}
